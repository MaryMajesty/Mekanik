using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using Meka;

namespace Mekanik
{
	public class Server
	{
		private GameBase _Game;
		private TcpListener _Listener;
		public Bunch<User> Users = new Bunch<User>();
		private Bunch<Tuple<int, DateTime>> _Pings = new Bunch<Tuple<int, DateTime>>();

		internal bool _Started;
		public bool Started
		{
			get { return this._Started; }
		}
		internal string _Ip = "[Unknown]";
		public string Ip
		{
			get { return this._Ip; }
		}
		internal int _Port;
		public int Port
		{
			get { return this._Port; }
		}
		public bool IsConnected
		{
			get { return this.Users.Count > 0; }
		}

		internal Server(GameBase _game)
		{
			this._Game = _game;
		}

		public async void Start(int _port)
		{
			this._Game.LocalPlayerIds = new Bunch<int>();
			for (int i = 0; i < this._Game.LocalPlayerCount; i++)
				this._Game.LocalPlayerIds.Add(i);

			this._Port = _port;
			this._Started = true;

			this._Listener = new TcpListener(System.Net.IPAddress.Any, _port);
			this._Listener.Start();

			Thread t = new Thread(this._Receive);
			t.Start();

			while (this._Game.IsRunning)
			{
				TcpClient c = await this._Listener.AcceptTcpClientAsync();
				
				User player = new User(c);
				Interlocked.Exchange<Bunch<User>>(ref this.Users, this.Users + player);
			}
		}

		private int _GetNextId()
		{
			int @out = 0;
			while (this.Users.Any(item => item.Ids.Any(id => id == @out)) || this._Game.LocalPlayerIds.Any(item => item == @out))
				@out++;
			return @out;
		}

		internal void _Send(MekaItem _item)
		{
			foreach (User u in this.Users.Where(item => item._Started))
				u.Send(_item);
		}

		public int GetPingId()
		{
			int id = Meth.RandomInt;
			Interlocked.Exchange<Bunch<Tuple<int, DateTime>>>(ref this._Pings, this._Pings + new Tuple<int, DateTime>(id, DateTime.Now));
			return id;
		}

		private void _Receive()
		{
			this._Game._Americanize();

			try
			{
				System.Net.WebClient c = new System.Net.WebClient();
				this._Ip = c.DownloadString("http://icanhazip.com");
				this._Ip = this._Ip.Substring(0, this._Ip.Length - 1);
			}
			catch { }

			//int failed = 0;
			Bunch<byte> bytes = new Bunch<byte>();
			while (this._Game.IsRunning)
			{
				foreach (User p in this.Users.Where(item => item._CanReceive))
				{
					p._Receive();
					//byte[] bs = p._Data;
					//MekaItem data = null;
					//try { data = Todes.Data.Read(bs); bytes += bs; }
					//catch
					//{
					//	failed++;
					//	this._Game.Title = failed.ToString();
					//	File.Write("Part 0", bytes.ToBunch());
					//	File.Write("Part 1", bs.ToBunch());
					//}

					while (p._ByteReader.CanRead)
					{
						MekaItem data = p._ByteReader.ReadCustom<MekaItem, Meka.Hidden.MekaItemParser>();

						if (data.Name == "Sync")
						{
							//foreach (MekaItem key in data["Keys"].Children)
							//	this._Game._PressKey(key["Name"].Content, key["ID"].To<int>(), key["Pressed"].To<bool>(), false, false);

							if (data.Contains("Custom"))
							{
								Dictionary<string, MekaItem> syncs = new Dictionary<string, MekaItem>();
								foreach (MekaItem sync in data["Custom"].Children)
									syncs[sync.Name] = sync;

								if (syncs.Count > 0)
									this._Game._Syncs.Add(new Tuple<User, Dictionary<string, MekaItem>>(p, syncs));
							}

							foreach (MekaItem message in data["Messages"].Children)
								this._Game.OnMessageFromClient(message);

							if (data.Contains("Pong"))
							{
								TimeSpan dif = DateTime.Now - this._Pings.First(item => item.Item1 == data["Pong"].Content.To<int>()).Item2;
								Interlocked.Exchange<Bunch<double>>(ref p._Latencies, p._Latencies + dif.TotalMilliseconds);
								if (p._Latencies.Count > 10)
									p._Latencies.RemoveAt(0);
							}
						}
						else if (data.Name == "Start")
						{
							int ids = data["Local Users"].To<int>();

							Bunch<int> nids = new Bunch<int>();
							for (int i = 0; i < ids; i++)
							{
								int id = this._GetNextId();
								this._Game._UserIds.Add(id);
								this._Game.OnlinePlayerIds.Add(id);
								p.Ids.Add(id);
							}

							MekaItem start = new MekaItem("Start", new List<MekaItem>()
								{
									new MekaItem("Server IDs", string.Join(", ", this._Game.LocalPlayerIds.Select(item => item.ToString()))),
									new MekaItem("Player IDs", string.Join(", ", p.Ids.Select(item => item.ToString())))
								});

							Bunch<int> uids = this._Game.LocalPlayerIds.Clone();
							foreach (User user in this.Users.Where(item => item != p))
								uids.Add(user.Ids);
							start.Children.Add(new MekaItem("User IDs", string.Join(", ", uids.Select(item => item.ToString()))));

							start.Children.Add(new MekaItem("Info", this._Game.OnUserConnect(p)));

							p.Send(start);

							p._Started = true;

							this._Game._ClientConnects.Add(p);
						}
						//else if (data.Name == "Key")
						//	this._Game._PressKey(data["Name"].Content, data["ID"].To<int>(), data["Pressed"].To<bool>(), false, false);

						Thread.Sleep(1);
					}

					//this._Game.Title = p._ByteReader._Bytes.Count.ToString();
				}

				Thread.Sleep(1);
			}
		}

		public bool HasSent(string _name)
		{
			return this.Users.Any(item => item.HasSent(_name));
		}

		public void Disconnect()
		{
			foreach (User u in this.Users)
				u.Disconnect();
		}
	}
}