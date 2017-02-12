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
	public class Client
	{
		private GameBase _Game;
		private TcpClient _Client;
		public User Server;
		//public Bunch<int> UserIds = new Bunch<int>();
		internal string _LastPing;
		
		public bool IsConnected
		{
			get { return this.Server != null && this.Server.IsConnected; }
		}
		
		internal Client(GameBase _game)
		{
			this._Game = _game;
		}
		
		public async void Connect(string _ip, int _port)
		{
			this._Client = new TcpClient(AddressFamily.InterNetwork);
			await this._Client.ConnectAsync(_ip, _port);
			this.Server = new User(this._Client, 0);

			Thread t = new Thread(this._Receive);
			t.Start();

			this._Send(new MekaItem("Start", new List<MekaItem>() { new MekaItem("Local Users", this._Game.LocalPlayerCount.ToString()) }));
		}
		
		internal void _Send(MekaItem _item) { this.Server.Send(_item); }
		
		private void _Receive()
		{
			this._Game._Americanize();

			//int failed = 0;
			while (this._Game.IsRunning)
			{
				if (this.Server._CanReceive)
				{
					//bool f = false;
					//ByteReader r = new ByteReader(this.Server._Data);
					//while (r.CanRead && !f)
					this.Server._Receive();
					while (this.Server._ByteReader.CanRead)
					{
						//MekaItem data = null;
						//try { data = r.ReadCustom<MekaItem>(); }
						//catch { f = true; failed++; this._Game.Title = failed.ToString(); }

						//if (data != null)
						//{
						MekaItem data = this.Server._ByteReader.ReadCustom<MekaItem, Meka.Hidden.MekaItemParser>();

						if (data.Name == "Start")
						{
							this.Server.Ids = data["Server IDs"].Content.Split(", ").Select(item => item.To<int>()).ToBunch();
							this._Game.LocalPlayerIds = data["Player IDs"].Content.Split(", ").Select(item => item.To<int>()).ToBunch();
							this._Game.OnlinePlayerIds = data["User IDs"].Content.Split(", ").Select(item => item.To<int>()).ToBunch();
							this._Game._UserIds.Add(this._Game.OnlinePlayerIds);

							//this._Game.OnServerConnect(data["Info"].Children);
							this._Game._ServerConnect = data["Info"];
						}
						else if (data.Name == "Sync")
						{
							//foreach (MekaItem key in data["Keys"].Children)
							//	this._Game._PressKey(key["Name"].Content, key["ID"].To<int>(), key["Pressed"].To<bool>(), false, false);

							if (data.Contains("Custom"))
							{
								Dictionary<string, MekaItem> syncs = new Dictionary<string, MekaItem>();
								foreach (MekaItem sync in data["Custom"].Children)
									syncs[sync.Name] = sync;

								if (syncs.Count > 0)
									this._Game._Syncs.Add(new Tuple<User, Dictionary<string, MekaItem>>(this.Server, syncs));
							}

							//foreach (MekaItem message in data["Messages"].Children)
							//	this._Game.OnMessageFromServer(message);

							foreach (MekaItem player in data["Players"].Children)
							{
								Entity p = this._Game.RealEntities.First(item => item.PlayerId == player.Name.To<int>());
								p.Motion += player.Content.To<Vector>() - (p.Position + p.Motion);
							}

							if (data.Contains("Ping"))
								this._LastPing = data["Ping"].Content;

							if (data.Contains("Info"))
							{
								this.Server._Speed = data["Info"]["Speed"].Content.To<double>();
								this.Server._Latencies.Add(data["Info"]["Latency"].Content.To<double>());
								if (this.Server._Latencies.Count > 10)
									this.Server._Latencies.RemoveAt(0);
							}
						}
						//else if (data.Name == "Key")
						//	this._Game._PressKey(data["Name"].Content, data["ID"].To<int>(), data["Pressed"].To<bool>(), false, false);

						//this._Game.Title = this.Server._ByteReader._Bytes.Count.ToString();
						//}

						Thread.Sleep(1);
					}
				}

				Thread.Sleep(1);
			}
		}

		public bool HasSent(string _name) { return this.Server.HasSent(_name); }

		public void Disconnect() { this.Server.Disconnect(); }
	}
}