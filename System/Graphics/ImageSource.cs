using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Mekanik
{
	public partial class ImageSource
	{
		#region Properties

		internal int _TextureId;
		public int TextureId
		{
			get { return this._TextureId; }
		}
		public byte[] Bytes
		{
			get { return GameBase.ImageToBytes(this); }
		}
		public byte[] BytesRgb
		{
			get { return GameBase.ImageToBytesRgb(this); }
		}
		public Color this[int _x, int _y]
		{
			get { return this.Pixels[_x, _y]; }

			set
			{
				GL.BindTexture(TextureTarget.Texture2D, this._TextureId);
				GL.TexSubImage2D(TextureTarget.Texture2D, 0, _x, _y, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, value.Bytes);
			}
		}
		public int Width
		{
			get { return this.Size.X; }
		}
		public int Height
		{
			get { return this.Size.Y; }
		}
		private Point _Size;
		public Point Size
		{
			get { return this._Size; }
		}
		public byte[] PixelBytes
		{
			get
			{
				byte[] @out = new byte[this.Width * this.Height * 4];
				GL.BindTexture(TextureTarget.Texture2D, this._TextureId);
				GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, @out);

				return @out;
			}
		}
		public Color[,] Pixels
		{
			get
			{
				byte[] bs = this.PixelBytes;
				Color[,] @out = new Color[this.Width, this.Height];
				for (int i = 0; i < this.Width * this.Height; i++)
				{
					Point p = new Point(i % this.Width, Meth.Down(i / this.Width));
					@out[p.X, p.Y] = new Color(bs.SubArray(i * 4, 4));
				}
				return @out;
			}
		}
		public byte[] PixelBytesGbra
		{
			get
			{
				byte[] bs = this.PixelBytes;
				byte[] @out = new byte[bs.Length];
				for (int i = 0; i < bs.Length; i += 4)
				{
					@out[i] = bs[i + 2];
					@out[i + 1] = bs[i + 1];
					@out[i + 2] = bs[i + 0];
					@out[i + 3] = bs[i + 3];
				}
				return @out;
			}
		}
		public byte[] PixelBytesGbr
		{
			get
			{
				byte[] bs = this.PixelBytes;
				byte[] @out = new byte[bs.Length / 4 * 3];
				for (int i = 0; i < bs.Length; i += 4)
				{
					int x = i / 4 * 3;
					@out[x] = bs[i + 2];
					@out[x + 1] = bs[i + 1];
					@out[x + 2] = bs[i + 0];
				}
				return @out;
			}
		}

		#endregion

		#region Constructors

		public ImageSource()
		{
			this._TextureId = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, this._TextureId);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 0);
		}

		protected ImageSource(ImageSource _base)
		{
			this._TextureId = _base._TextureId;
			this._Size = _base._Size;
		}

		public ImageSource(int _width, int _height, IntPtr _rgbadata) : this() { this.SetData(_width, _height, _rgbadata); }

		public ImageSource(int _width, int _height) : this() { this._SetSize(_width, _height); }

		public ImageSource(Point _size) : this(_size.X, _size.Y) { }

		public ImageSource(int _textureid, Point _size)
		{
			this._TextureId = _textureid;
			this._Size = _size;
		}

		public ImageSource(Color[,] _pixels)
			: this()
		{
			this._Size.X = _pixels.GetLength(0);
			this._Size.Y = _pixels.GetLength(1);

			byte[] bs = new byte[this.Width * this.Height * 4];
			for (int x = 0; x < this.Width; x++)
			{
				for (int y = 0; y < this.Height; y++)
				{
					for (int i = 0; i < 4; i++)
						bs[y * this.Width * 4 + x * 4 + i] = _pixels[x, y].Bytes[i];
				}
			}

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, this.Width, this.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, bs);
		}

		public ImageSource(byte[] _bytes) : this(GameBase.LoadImageSource(_bytes)) { }

		public ImageSource(string _path) : this(File.ReadBytes(_path)) { }

		#endregion

		internal void _SetSize(int _width, int _height)
		{
			this._Size = new Point(_width, _height);

			GL.BindTexture(TextureTarget.Texture2D, this._TextureId);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _width, _height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
		}

		public void SetData(int _width, int _height, IntPtr _rgbadata)
		{
			this._Size = new Point(_width, _height);

			GL.BindTexture(TextureTarget.Texture2D, this._TextureId);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _width, _height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, _rgbadata);
		}

		public ImageSource[,] Split(int _xcount, int _ycount)
		{
			ImageSource[,] @out = new ImageSource[_xcount, _ycount];

			int w = this.Size.X / _xcount;
			int h = this.Size.Y / _ycount;

			for (int x = 0; x < _xcount; x++)
			{
				for (int y = 0; y < _ycount; y++)
					@out[x, y] = new ImageSource(this.Pixels.SubArray(x * w, y * h, w, h));
			}

			return @out;
		}

		public ImageSource[,] Split(Point _amount) => this.Split(_amount.X, _amount.Y);

		public ImageSource[] Split(int _amount, bool _horizontally = true)
		{
			ImageSource[,] imgs = _horizontally ? this.Split(_amount, 1) : this.Split(1, _amount);
			ImageSource[] @out = new ImageSource[_amount];
			for (int i = 0; i < _amount; i++)
				@out[i] = _horizontally ? imgs[i, 0] : imgs[0, i];
			return @out;
		}

		public void Dispose() => GL.DeleteTextures(1, new int[] { this._TextureId });

		public void Save(string _path) => File.Write(_path, this.Bytes);
		public void SaveRgb(string _path) => File.Write(_path, this.BytesRgb);

		public ImageSource Extend()
		{
			Renderer r = new Renderer(this.Size + 2);
			r.Draw(new Image(this) { Position = 1 });
			r.Dispose();
			return r.ImageSource;
		}

		//public Bunch<Color> GetPalette()
		//{
		//	Bunch<Color> @out = new Bunch<Color>();
		//	for (int i = 0; i < this.PixelBytes.Length; i += 4)
		//	{
		//		Color c = new Color(this.PixelBytes.SubArray(i, 4));
		//		if (!@out.Contains(c))
		//			@out.Add(c);
		//	}
		//	return @out;
		//}

		public ImageSource Clone()
		{
			Renderer r = new Renderer(this.Size);
			r.Draw(new Image(this));
			r.Dispose();
			return r.ImageSource;
		}
	}
}