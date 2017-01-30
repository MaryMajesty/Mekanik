using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Meka;
using Meka.ByteOperators;

namespace Mekanik
{
	public class User
	{
		internal TcpClient _Client;
		public Bunch<int> Ids;
		public bool IsServer;
		private Bunch<MekaItem> _Messages = new Bunch<MekaItem>();
		internal double _Speed;
		internal Bunch<double> _Latencies = new Bunch<double>() { 0 };
		internal bool _Started;
		internal ByteReader _ByteReader = new ByteReader(new byte[0]);
		
		public string IP
		{
			get { return ((IPEndPoint)this._Client.Client.RemoteEndPoint).Address.ToString(); }
		}
		public int Port
		{
			get { return ((IPEndPoint)this._Client.Client.RemoteEndPoint).Port; }
		}
		internal bool _CanReceive
		{
			get { return this._Client.Available > 0; }
		}
		//internal byte[] _Data
		//{
		//	get
		//	{
		//		byte[] @out = new byte[this._Client.Available];
		//		this._Client.Client.Receive(@out);
		//		return @out;
		//	}
		//}
		public bool IsConnected
		{
			get { return this._Client.Connected; }
		}
		public double Speed
		{
			get { return this._Speed; }
		}
		public double Latency
		{
			get { return this._Latencies.Average(); }
		}
		
		internal User(TcpClient _client, Bunch<int> _ids = null, bool _isserver = false)
		{
			this._Client = _client;
			if (_ids == null)
				_ids = new Bunch<int>();
			this.Ids = _ids;
			this.IsServer = _isserver;

			Thread t = new Thread(this._SendLoop);
			t.Start();
		}

		internal void _Receive()
		{
			byte[] bs = new byte[this._Client.Available];
			this._Client.Client.Receive(bs);
			this._ByteReader.AddBytes(bs);
		}

		//internal void _CollectData()
		//{
		//	byte[] @out = new byte[this._Client.Available];
		//	this._Client.Client.Receive(@out);
		//	this._CollectedData.Add(@out);
		//}

		private void _SendLoop()
		{
			while (this.IsConnected)
			{
				//if (!this._WantingToSend)
				//{
				//	this._Sending = true;
					if (this._Messages.Count > 0)
					{
						MekaItem msg = this._Messages[0];
						byte[] bs = msg.ToBytes();
						
						DateTime start = DateTime.Now;

						this._Client.Client.Send(bs);

						TimeSpan dif = DateTime.Now - start;
						if (dif.TotalMilliseconds != 0)
							this._Speed = bs.Length / 1000.0 / dif.TotalSeconds;

						//this._Messages.RemoveAt(0);
						Interlocked.Exchange<Bunch<MekaItem>>(ref this._Messages, this._Messages.SubBunch(1));
					}
					//this._Sending = false;
				//}
			}
		}
		
		public void Send(MekaItem _item)
		{
			//this._WantingToSend = true;
			//while (this._Sending)
			//	await Task.Delay(1);
			//this._Messages.Add(_item);
			//this._WantingToSend = false;
			Interlocked.Exchange<Bunch<MekaItem>>(ref this._Messages, this._Messages + _item);
		}

		public bool HasSent(string _name) { return !this._Messages.Any(item => item.Name == _name); }

		public void Disconnect() { this._Client.Close(); }
	}
}