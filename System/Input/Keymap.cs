using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Keymap
	{
		internal Dictionary<string, Bunch<Key>> _Keys = new Dictionary<string, Bunch<Key>>();

		public void SetKeys(string _name, params Key[] _keys) => this._Keys[_name] = new Bunch<Key>(_keys);

		public KeyInfo GetInfo(Key _key)
		{
			foreach (KeyValuePair<string, Bunch<Key>> c in this._Keys)
			{
				if (c.Value.Contains(_key))
					return new KeyInfo(c.Key, c.Value.IndexOf(_key));
			}
			throw new Exception("The key " + _key.ToString() + " doesn't map to anything.");
		}

		public bool Contains(Key _key) { return this._Keys.Any(item => item.Value.Any(k => k == _key)); }
	}
}