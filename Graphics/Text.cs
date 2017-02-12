using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Mekanik
{
	public class Text : VertexGraphic
	{
		private bool _NeedsUpdate = true;

		private int _TabLength = 4;
		public int TabLength
		{
			get { return this._TabLength; }

			set
			{
				this._TabLength = value;
				this._NeedsUpdate = true;
			}
		}
		private string _SymbolTab;
		public string SymbolTab
		{
			get { return this._SymbolTab; }

			set
			{
				this._SymbolTab = value;
				this._NeedsUpdate = true;
			}
		}
		private char? _SymbolSpace;
		public char? SymbolSpace
		{
			get { return this._SymbolSpace; }

			set
			{
				this._SymbolSpace = value;
				this._NeedsUpdate = true;
			}
		}
		private Color _TextColor = Color.Black;
		public Color TextColor
		{
			get { return this._TextColor; }

			set
			{
				this._TextColor = value;
				this._NeedsUpdate = true;
			}
		}
		private FontBase _Font;
		public FontBase Font
		{
			get { return this._Font; }

			set
			{
				this._Font = value;
				this._NeedsUpdate = true;
			}
		}
		public double CharSize
		{
			get { return this._Font.CharSize; }

			set
			{
				this._Font.CharSize = value;
				this._NeedsUpdate = true;
			}
		}
		private string _Content;
		public string Content
		{
			get { return this._Content; }

			set
			{
				if (this._Content != value)
				{
					this._Content = value;
					this._NeedsUpdate = true;
					this._NeedsSizeUpdate = true;
				}
			}
		}
		private bool _NeedsSizeUpdate = true;
		private Vector _Size;
		public Vector Size
		{
			get
			{
				if (this._NeedsSizeUpdate)
				{
					this._NeedsSizeUpdate = false;
					this._Size = this.Font.GetSize(this.Content, this.TabLength);
				}

				return this._Size;
			}
		}
		public double Width
		{
			get { return this.Size.X; }
		}
		public double WidthWithoutBorder
		{
			get { return this.Width - (this.Font.GetSize("X") - (this.Font.GetSize("XX") - this.Font.GetSize("X"))).X; }
		}
		public double Height
		{
			get { return this.Size.Y; }
		}
		
		public Text(FontBase _font)
		{
			this.Font = _font;
			
			this.LockToGrid = true;
			this.SmoothTexture = !(_font is PixelFont);

			this.Texture = new ImageSource(1, 1);
		}

		protected internal override VertexArray _ToVertexArray()
		{
			if (this._NeedsUpdate && this.Visible && this.Color.A > 0 && this.TextColor.A > 0)
			{
				this._NeedsUpdate = false;

				if (this.Texture != null)
					this.Texture.Dispose();
				this.Texture = this.Font.GetImage(this.Content, this.TextColor, this.TabLength, this.SymbolTab, this.SymbolSpace);
			}

			Vector p = this.Origin * this.Size;
			this.Offset = (p % 1) * -1;

			VertexArray @out = new VertexArray(VertexArrayType.Quads);
			@out.Add(new Vertex(0, 0));
			@out.Add(new Vertex(this.Size.OnlyX, this.Size.OnlyX));
			@out.Add(new Vertex(this.Size, this.Size));
			@out.Add(new Vertex(this.Size.OnlyY, this.Size.OnlyY));
			return @out;
		}

		public override void Dispose()
		{
			if (this.Texture != null)
				this.Texture.Dispose();

			this._Disposed = true;
		}
	}
}