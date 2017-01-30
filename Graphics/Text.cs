using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Mekanik
{
	public class Text : VertexGraphic
	{
		
		//public string Content = "";
		
		//public bool ShowCursor;
		//public int CursorPosition;
		//public int TabLength = 4;
		//public int SelectionLength;
		//public char? PasswordChar;

		//private SFML.Graphics.Text _Text = new SFML.Graphics.Text();
		//private SFML.Graphics.Text _Cursor = new SFML.Graphics.Text();
		//private Rectangle _Selection = new Rectangle(0, 0);
		//public Bitmap _Bitmap;
		//private Graphics _Graphics;
		private bool _NeedsUpdate = true;
		public Color TextColor = Color.Black;
		public int TabLength = 4;

		public string SymbolTab;
		public char? SymbolSpace;

		private FontBase _Font;
		public FontBase Font
		{
			get { return this._Font; }

			set
			{
				this._Font = value;
				//this._Text.Font = _Font._Font;
			}
		}
		//private int _CharSize;
		//public int CharSize
		//{
		//	get { return this._CharSize; }
		//	set { this._CharSize = value; }
		//}
		//public int CharSize
		//{
		//	get { return (int)this._Text.CharacterSize; }
		//	set { this._Text.CharacterSize = (uint)value; }
		//}
		public double CharSize
		{
			get { return this._Font.CharSize; }

			set { this._Font.CharSize = value; }
			//{
				//if (this._Font.CharSize != value)
				//	this._Font = this._Font.ChangeCharSize(value);
			//}
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
		//private double _Width
		//{
		//	get { return this._Text.GetLocalBounds().Width; }
		//}
		//public double Width
		//{
		//	get
		//	{
		//		string c = this.Content;
		//		double @out = 0;

		//		foreach (string l in this.Content.Split('\n'))
		//		{
		//			this.Content = "H";
		//			double sub = this._Width;

		//			this.Content = "H" + l + "H";
		//			double d = this._Width - sub * 2;

		//			if (d > @out)
		//				@out = d;
		//		}

		//		this.Content = c;
		//		return @out;
		//	}
		//}
		//public double Height
		//{
		//	get { return this._Font.GetLineSpacing(this.CharSize) * (1 + this.Content.Count(item => item == '\n')); }
		//}
		private bool _NeedsSizeUpdate;
		private Vector _Size;
		public Vector Size
		{
			get
			{
				//SizeF s1 = this._Graphics.MeasureString(this.Content + "H", this.Font._Font);
				//SizeF s2 = this._Graphics.MeasureString(this.Content + "HH", this.Font._Font);

				if (this._NeedsSizeUpdate)
				{
					this._NeedsSizeUpdate = false;

					this._Size = this.Font.GetSize(this.Content, this.TabLength);
					//string c = this.Content ?? "";

					//if (c == "")
					//	this._Size = this.GetSize("X") * new Vector(0.2, 1);
					//else
					//{
					//	string tab = "";
					//	for (int i = 0; i < this.TabLength; i++)
					//		tab += " ";
						
					//	while (c.Contains('\t'))
					//		c = c.Replace("\t", tab);

					//	Bunch<string> lines = c.Split('\n');
					//	double maxwidth = 0;
					//	for (int i = 0; i < lines.Count; i++)
					//	{
					//		string l = lines[i];
					//		if (l.Length > 0 && l[l.Length - 1] == ' ')
					//			l = l.Substring(0, l.Length - 1) + '_';
					//		double width = this._Graphics.MeasureString(l, this.Font._Font).Width;
					//		if (lines[i].Length == 0)
					//			width /= 4;
					//		if (width > maxwidth)
					//			maxwidth = width;
					//	}

					//	SizeF s = new SizeF((float)maxwidth, (float)(this.CharSize * lines.Count));//this._Graphics.MeasureString(heighttext.Substring(0, heighttext.Length - 1), this.Font._Font).Height
					//	this._Size = new Vector(s.Width, s.Height + (this._Graphics.MeasureString("X", this.Font._Font).Height - this.CharSize));
					//}
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

		//public Text()
		//{
		//	this.LockToGrid = true;
		//	this.SmoothTexture = true;
		//}

		public Text(FontBase _font)
		{
			this.Font = _font;

			//this.CharSize = 14;

			//this._Bitmap = new Bitmap(200, 50);
			//this._Graphics = Graphics.FromImage(this._Bitmap);
			this.LockToGrid = true;
			this.SmoothTexture = true;
		}

		//private void _UpdateStyle()
		//{
		//	this._Text.Style =
		//		(Bold ? SFML.Graphics.Text.Styles.Bold : SFML.Graphics.Text.Styles.Regular)
		//		| (Italic ? SFML.Graphics.Text.Styles.Italic : SFML.Graphics.Text.Styles.Regular)
		//		| (Underlined ? SFML.Graphics.Text.Styles.Underlined : SFML.Graphics.Text.Styles.Regular);
		//}

		//public Vector GetSize(string _content)
		//{
		//	string c = this.Content;
		//	this.Content = _content;
		//	Vector @out = this.Size;
		//	if (_content == "" || _content == null)
		//		@out.X = 0;
		//	this.Content = c;
		//	return @out;
		//}

		protected internal override VertexArray _ToVertexArray()
		{
			if (this._NeedsUpdate)
			{
				this._NeedsUpdate = false;

				if (this.Texture != null)
					this.Texture.Dispose();
				this.Texture = this.Font.GetImage(this.Content, this.TextColor);

				//if (this._Bitmap.Size.ToMekanik() != new Point(Meth.Up(this.Width), Meth.Up(this.Height)))
				//{
				//	this._Bitmap.Dispose();
				//	this._Bitmap = new Bitmap(Meth.Max(Meth.Up(this.Width), 1), Meth.Max(Meth.Up(this.Height), 1));
				//	this._Graphics.Dispose();
				//	this._Graphics = Graphics.FromImage(this._Bitmap);
				//}

				//if (this.Texture != null)
				//	this.Texture.Dispose();
				//this.Texture = new ImageSourceBase(this._Bitmap);

				//this._Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
				//this._Graphics.Clear(System.Drawing.Color.Transparent);



				//string c = this.Content ?? "";

				//if (this.SymbolSpace != null)
				//{
				//	while (c.Contains(" "))
				//		c = c.Replace(' ', this.SymbolSpace.Value);
				//}

				//string tab = "";
				//int len = 0;
				//if (this.SymbolTab != null)
				//{
				//	tab = this.SymbolTab;
				//	len = this.SymbolTab.Length;
				//}
				//for (int i = 0; i < this.TabLength - len; i++)
				//	tab += " ";
				//while (c.Contains('\t'))
				//	c = c.Replace("\t", tab);



				//this._Graphics.DrawString(c, this.Font._Font, new SolidBrush(System.Drawing.Color.FromArgb(this.TextColor.A, this.TextColor.R, this.TextColor.G, this.TextColor.B)), 0, 0);

				//System.Drawing.Imaging.BitmapData data = this._Bitmap.LockBits(new System.Drawing.Rectangle(0, 0, this._Bitmap.Width, this._Bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				//GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, this._Bitmap.Width, this._Bitmap.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
				//this._Bitmap.UnlockBits(data);
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
			//this._Bitmap.Dispose();
			//this._Graphics.Dispose();

			if (this.Texture != null)
				this.Texture.Dispose();

			this._Disposed = true;
		}
	}
}