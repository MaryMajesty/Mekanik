using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class ColorTheme
	{
		public Bunch<ColorPattern> Patterns;

		internal bool _ColorChanged
		{
			get { return this.Patterns.Any(item => item._ColorChanged); }
		}

		public Color this[string _name]
		{
			get { return this.Patterns.First(item => item.Name == _name).Color; }
			set { this.Patterns.First(item => item.Name == _name).Color = value; }
		}

		public ColorTheme(params ColorPattern[] _patterns) { this.Patterns = new Bunch<ColorPattern>(_patterns); }

		public void Apply(RichText _text)
		{
			string inrange = null;
			int rangestart = 0;

			for (int i = 0; i < _text.Content.Length; i++)
			{
				if (inrange == null)
				{
					Bunch<Tuple<ColorPattern, int>> symbols = new Bunch<Tuple<ColorPattern, int>>();
					foreach (ColorPattern pattern in this.Patterns)
						symbols.Add(new Tuple<ColorPattern, int>(pattern, pattern.GetLength(_text.Content.Substring(i))));
					
					if (symbols.Any(item => item.Item2 > 0))
					{
						Tuple<ColorPattern, int> t = symbols.OrderByDescending(item => item.Item2)[0];//this.Patterns.First(item => item.Symbols.Contains(symbol));

						ColorPattern p = t.Item1;
						int l = t.Item2;

						if (p.Range)
						{
							inrange = p.Symbols[0];
							rangestart = i;
						}
						else
							_text.ColorIn(p.Color, i, l);

						i += l - 1;
					}
					//else
					//{
					//	List<ColorPattern> ps = this.Patterns.Where(item => item.Func != null && item.Func(_text.Content.Substring(i))).ToList();
					//	if (ps.Count > 0)
					//	{

					//	}
					//}
				}
				else
				{
					Bunch<Tuple<ColorPattern, int>> ps = new Bunch<Tuple<ColorPattern, int>>();
					foreach (ColorPattern p in this.Patterns.Where(item => item.Range && item.Symbols[0] == inrange))
						ps.Add(new Tuple<ColorPattern, int>(p, p.GetLength2(_text.Content.Substring(i))));

					if (ps.Any(item => item.Item2 > 0))
					{
						_text.ColorIn(ps.OrderByDescending(item => item.Item2)[0].Item1.Color, rangestart, i - rangestart + 1);
						inrange = null;
					}
				}
			}

			if (inrange != null)
			{
				Bunch<ColorPattern> ps = this.Patterns.Where(item => item.Range && item.Symbols[0] == inrange && item.EndLine);
				if (ps.Count > 0)
					_text.ColorIn(ps[0].Color, rangestart, _text.Content.Length - 1 - rangestart + 1);
			}
		}

		internal void _ApplyColorChange()
		{
			foreach (ColorPattern p in this.Patterns)
				p._ColorChanged = false;
		}
	}
}