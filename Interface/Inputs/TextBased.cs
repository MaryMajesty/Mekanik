using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class TextBased : InputControl
	{
		public RichText _Text;

		public ColorTheme ColorTheme
		{
			get { return this._Text.ColorTheme; }
			set { this._Text.ColorTheme = value; }
		}
		protected virtual void _OnContentChange(string _old, string _new) { }
		public string Content
		{
			get { return this._Text.Content; }

			set
			{
				string l = this.Content;
				string c = new string(value.Where(item => item != '\r').ToArray());
				this._Text.Content = c;
				this._OnContentChange(l, c);
			}
		}
		protected virtual void _OnColorChange() { }
		public Color TextColor
		{
			get { return this._Text.TextColor; }

			set
			{
				this._Text.TextColor = value;
				this._OnColorChange();
			}
		}
		protected virtual void _OnFontChange() { }
		public FontBase Font
		{
			get { return this._Text.Font; }

			set
			{
				this._Text.Font = value;
				this._OnFontChange();
			}
		}
		public Shader Shader
		{
			get { return this._Text.Shader; }
			set { this._Text.Shader = value; }
		}
		//protected virtual void _OnBoldChange() { }
		//public bool Bold
		//{
		//	get { return this._Text.Bold; }
		//	set
		//	{
		//		this._Text.Bold = value;
		//		this._OnBoldChange();
		//	}
		//}
		//protected virtual void _OnItalicChange() { }
		//public bool Italic
		//{
		//	get { return this._Text.Italic; }
		//	set
		//	{
		//		this._Text.Italic = value;
		//		this._OnItalicChange();
		//	}
		//}
		//protected virtual void _OnUnderlinedChange() { }
		//public bool Underlined
		//{
		//	get { return this._Text.Underlined; }
		//	set
		//	{
		//		this._Text.Underlined = value;
		//		this._OnUnderlinedChange();
		//	}
		//}
		protected virtual void _OnCharSizeChange() { }
		public double CharSize
		{
			get { return this._Text.CharSize; }

			set
			{
				this._Text.CharSize = value;
				this._OnCharSizeChange();
			}
		}
		protected virtual void _OnEdgeChange() { }
		public Vector Edge
		{
			get { return this._Text.Position; }

			set
			{
				this._Text.Position = value;
				this._OnEdgeChange();
			}
		}
		public string SymbolTab
		{
			get { return this._Text.SymbolTab; }
			set { this._Text.SymbolTab = value; }
		}
		public char? SymbolSpace
		{
			get { return this._Text.SymbolSpace; }
			set { this._Text.SymbolSpace = value; }
		}

		public TextBased(string _value) { }
		
		public void SetWhitespaceColor(Color _color)
		{
			this.ColorTheme.Patterns.Add(new ColorPattern("Whitespace", _color, "\t", "\n", " "));
			this._Text._NeedsUpdate = true;
		}

		public override object GetValue() => this.Content;
		internal override double _GetRectWidth() => this._Text.Width + this.Edge.X * 2;
		internal override double _GetRectHeight() => this._Text.Height + this.Edge.Y * 2;
	}
}