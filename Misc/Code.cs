using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public delegate void ForDelegate(int _x, int _y);

	public static class Code
	{
		public class Clock
		{
			private Dictionary<int, Bunch<Tuple<Action, int>>> _Codes = new Dictionary<int, Bunch<Tuple<Action, int>>>();

			public void Add(int _frames, Action _code, int _interval = 0)
			{
				if (!this._Codes.ContainsKey(_frames))
					this._Codes[_frames] = new Bunch<Tuple<Action, int>>();
				this._Codes[_frames].Add(new Tuple<Action, int>(_code, _interval));
			}

			public void Add(Action _code) { Add(1, _code); }

			public void Tick()
			{
				Bunch<Tuple<Action, int>> adds = new Bunch<Tuple<Action, int>>();
				Dictionary<int, Bunch<Tuple<Action, int>>> ncodes = new Dictionary<int, Bunch<Tuple<Action, int>>>();

				foreach (KeyValuePair<int, Bunch<Tuple<Action, int>>> b in this._Codes)
				{
					if (b.Key == 1)
					{
						foreach (Tuple<Action, int> d in b.Value)
						{
							d.Item1();
							if (d.Item2 > 0)
								adds.Add(d);
						}
					}
					else
						ncodes[b.Key - 1] = b.Value;
				}
				this._Codes = ncodes;

				foreach (Tuple<Action, int> add in adds)
					Add(add.Item2, add.Item1, add.Item2);
			}

			public void Clear()
			{
				this._Codes.Clear();
			}
		}

		public static void For(int _x, int _y, ForDelegate _code)
		{
			for (int x = 0; x < _x; x++)
			{
				for (int y = 0; y < _y; y++)
					_code(x, y);
			}
		}
	}
}