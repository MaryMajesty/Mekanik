using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka;

namespace Mekanik
{
	public class Node
	{
		public Client Client;
		public Server Server;

		public bool Connected
		{
			get { return this.Client.IsConnected || this.Server.IsConnected; }
		}
		public bool IsServer
		{
			get { return this.Server.IsConnected; }
		}

		internal Node(GameBase _game)
		{
			this.Client = new Client(_game);
			this.Server = new Server(_game);
		}

		public void StartServer(int _port)
		{
			this.Server.Start(_port);
		}

		public void ConnectToServer(string _ip, int _port)
		{
			this.Client.Connect(_ip, _port);
		}

		public void Send(MekaItem _item)
		{
			if (this.IsServer)
				this.Server._Send(_item);
			else
				this.Client._Send(_item);
		}

		public void Disconnect()
		{
			if (this.Client.IsConnected)
				this.Client.Disconnect();
			if (this.Server.IsConnected)
				this.Server.Disconnect();
		}

		public bool HasSent(string _name)
		{
			if (this.IsServer)
				return this.Server.HasSent(_name);
			else
				return this.Client.HasSent(_name);
		}
	}
}