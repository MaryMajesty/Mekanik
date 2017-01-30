using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	class ColorLine
	{
		public Color Color;
		public int Length;

		public ColorLine(Color _color, int _length)
		{
			this.Color = _color;
			this.Length = _length;
		}
	}

	class TextLine
	{
		public string Text;
		public List<ColorLine> ColorLines;

		public TextLine(string _text, List<ColorLine> _colorlines)
		{
			this.Text = _text;
			this.ColorLines = _colorlines;
		}
	}

	public class RichText : Entity
	{
		private ColorTheme _ColorTheme;
		public ColorTheme ColorTheme
		{
			get { return this._ColorTheme; }

			set
			{
				this._ColorTheme = value;
				this.UpdateColors();
			}
		}
		public Text _Text;
		private Color[] _Colors;
		
		private string _Content = "";
		public string Content
		{
			get { return this._Content; }

			set
			{
				this._Content = value;
				this._Text.Content = value;
				this.UpdateColors();
			}
		}
		public FontBase Font
		{
			get { return this._Text.Font; }
			set { this._Text.Font = value; }
		}
		public double CharSize
		{
			get { return this._Text.CharSize; }
			set { this._Text.CharSize = value; }
		}
		private Color _TextColor = Color.Black;
		public Color TextColor
		{
			get { return this._TextColor; }

			set
			{
				this._TextColor = value;
				this.UpdateColors();
			}
		}
		public Vector Size
		{
			get { return this._Text.Size; }
		}
		public double Width
		{
			get { return this.Size.X; }
		}
		public double Height
		{
			get { return this.Size.Y; }
		}
		public Shader Shader
		{
			get { return this._Text.Shader; }
			set { this._Text.Shader = value; }
		}
		private string _SymbolTab;
		public string SymbolTab
		{
			get { return this._SymbolTab; }

			set
			{
				if (this._SymbolTab != value)
				{
					this._SymbolTab = value;
					this._NeedsUpdate = true;
				}
			}
		}
		private char? _SymbolSpace;
		public char? SymbolSpace
		{
			get { return this._SymbolSpace; }

			set
			{
				if (this._SymbolSpace != value)
				{
					this._SymbolSpace = value;
					this._NeedsUpdate = true;
				}
			}
		}

		internal bool _NeedsUpdate;

		public void UpdateColors()
		{
			this._Colors = new Color[this.Content.Length];
			for (int i = 0; i < this._Colors.Length; i++)
				this._Colors[i] = this.TextColor;

			if (this.ColorTheme != null)
				this.ColorTheme.Apply(this);
			
			this._NeedsUpdate = true;
		}

		public RichText(FontBase _font)
		{
			this._Text = new Text(_font) { TextColor = Color.Black, Content = "sup" };
		}

		public override void OnRender()
		{
			if (this.ColorTheme != null && this.ColorTheme._ColorChanged)
			{
				this.ColorTheme._ApplyColorChange();
				this._NeedsUpdate = true;
			}

			if (this._NeedsUpdate)
			{
				this._NeedsUpdate = false;
				this.UpdateText();
			}
		}

		public void UpdateText()
		{
			foreach (Text t in this.Graphics.GetTypes<Text>())
				t.Dispose();
			this.Graphics.Clear();
			
			int l = 0;
			foreach (TextLine tl in this._GetTextLines())
			{
				int p = 0;
				foreach (ColorLine cl in tl.ColorLines)
				{
					string s = this.Content.Substring(0, p);
					this._Text.Content = tl.Text.Substring(0, p)/* + "M"*/;

					double offset = (this._Text.Content == "") ? 0 : this._Text.WidthWithoutBorder;

					this.Graphics.Add(new Text(this._Text.Font)
						{
							CharSize = this._Text.CharSize,
							Position = new Vector(offset/* - sub*/, this._Text.Font.CharSize * l) * this.Scale,
							Content = tl.Text.Substring(p, cl.Length),
							TextColor = cl.Color,
							SymbolTab = this._SymbolTab,
							SymbolSpace = this._SymbolSpace,
							Shader = this.Shader
						});
					p += cl.Length;
				}
				l++;
			}
			this._Text.Content = this.Content;
			//if (this.Parent != null)
			//	this.Parent.Title += "X";
		}

		public void ColorIn(Color _color, int _start, int _length)
		{
			for (int i = _start; i < _start + _length; i++)
				_Colors[i] = _color;
		}

		private List<TextLine> _GetTextLines()
		{
			List<TextLine> @out = new List<TextLine>();
			string[] lines = this.Content.ToLines();

			for (int i = 0; i < lines.Length; i++)
			{
				int p = 0;
				for (int n = 0; n < i; n++)
					p += lines[n].Length + 1;
				string l = lines[i];

				List<ColorLine> cls = new List<ColorLine>();

				int s = 0;
				Color cc = Color.Transparent;
				for (int x = 0; x < l.Length; x++)
				{
					Color c = this._Colors[p + x];
					if (c != cc)
					{
						cls.Add(new ColorLine(cc, x - s));
						s = x;
						cc = c;
					}
				}
				cls.Add(new ColorLine(cc, l.Length - s));
				@out.Add(new TextLine(l, cls));
			}

			return @out;
		}

		public Vector GetSize(string _content) { return this._Text.Font.GetSize(_content); }
	}
}