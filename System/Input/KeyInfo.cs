using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public struct KeyInfo
	{
		public string Name;
		public int LocalId;

		public KeyInfo(string _name, int _localid)
		{
			this.Name = _name;
			this.LocalId = _localid;
		}
	}
}