using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Sandbox<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TValue : new()
	{
		private Dictionary<TKey, TValue> _Objects = new Dictionary<TKey, TValue>();

		public TValue this[TKey _key]
		{
			get
			{
				if (this._Objects.ContainsKey(_key))
					return this._Objects[_key];
				else
					return this._Objects[_key] = new TValue();
			}

			set { this._Objects[_key] = value; }
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			foreach (KeyValuePair<TKey, TValue> pair in this._Objects)
				yield return pair;
		}

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
	}
}