using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class ColorPattern
	{
		public string Name;
		public bool Range;
		public bool EndLine;
		public Bunch<string> Symbols;
		public Func<string, int> Func;
		internal bool _ColorChanged;

		private Color _Color;
		public Color Color
		{
			get { return this._Color; }

			set
			{
				if (this._Color != value)
				{
					this._Color = value;
					this._ColorChanged = true;
				}
			}
		}

		public ColorPattern(string _name, Color _color, bool _range, bool _endline, params string[] _symbols)
		{
			this.Name = _name;
			this._Color = _color;
			this.Range = _range;
			this.EndLine = _endline;
			this.Symbols = new Bunch<string>(_symbols);
		}

		public ColorPattern(string _name, Color _color, bool _range, params string[] _symbols) : this(_name, _color, _range, false, _symbols) { }

		public ColorPattern(string _name, Color _color, params string[] _symbols) : this(_name, _color, false, _symbols) { }

		public ColorPattern(string _name, Color _color, Func<string, int> _func)
		{
			this.Color = _color;
			this.Func = _func;
		}

		internal int GetLength(string _text)
		{
			if (this.Func != null)
				return this.Func(_text);
			else if (this.Range)
				return _text.StartsWith(this.Symbols[0]) ? this.Symbols[0].Length : 0;
			else
			{
				Bunch<string> ss = this.Symbols.Where(item => _text.StartsWith(item));
				if (ss.Count > 0)
					return ss.OrderByDescending(item => item.Length)[0].Length;
				else
					return 0;
			}
		}

		internal int GetLength2(string _text)
		{
			if (this.EndLine && _text.StartsWith("\n"))
				return 1;
			else if (this.Symbols.Count > 1)
				return _text.StartsWith(this.Symbols[1]) ? this.Symbols[1].Length : 0;
			else
				return 0;
		}
	}
}