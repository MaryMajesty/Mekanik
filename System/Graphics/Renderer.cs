using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Mekanik
{
	public class Renderer
	{
		public Color Background = Color.Transparent;
		private int _FramebufferId;
		private int _TextureId;
		private bool _RenderToWindow;

		private Point _Resolution;
		public Point Resolution
		{
			get { return this._Resolution; }

			set
			{
				if (this._Resolution != value)
				{
					if (this._RenderToWindow)
						this._Resolution = value;
					else
						this._SetResolution(value.X, value.Y);
				}
			}
		}
		public int Width
		{
			get { return this.Resolution.X; }
		}
		public int Height
		{
			get { return this.Resolution.Y; }
		}
		private ImageSource _ImageSource;
		public ImageSource ImageSource
		{
			get
			{
				if (!this._RenderToWindow)
					return this._ImageSource;
				else
				{
					IntPtr data = System.Runtime.InteropServices.Marshal.AllocHGlobal(this.Width * this.Height * 4);
					GL.ReadPixels(0, 0, this.Width, this.Height, PixelFormat.Bgra, PixelType.UnsignedByte, data);

					ImageSource img = new ImageSource(this.Width, this.Height, data);

					Renderer r = new Renderer(this.Width, this.Height);
					r.Draw(new Image(img) { Position = new Vector(0, this.Height), Scale = new Vector(1, -1), BlendMode = BlendMode.None });
					r.DisposeWithoutImage();
					
					return r.ImageSource;
				}
			}
		}
		
		public Renderer(int _resolutionx, int _resolutiony, bool _rendertowindow = false)
		{
			this._RenderToWindow = _rendertowindow;

			if (!_rendertowindow)
			{
				this._FramebufferId = GL.GenFramebuffer();
				this._TextureId = GL.GenTexture();

				this._SetResolution(_resolutionx, _resolutiony);
				this._ImageSource = new ImageSource(this._TextureId, this.Resolution);
			}
			else
				this._Resolution = new Point(_resolutionx, _resolutiony);
		}

		public Renderer(Point _resolution, bool _rendertowindow = false) : this(_resolution.X, _resolution.Y, _rendertowindow) { }

		private void _SetResolution(int _resolutionx, int _resolutiony)
		{
			this._Resolution = new Point(_resolutionx, _resolutiony);

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, this._FramebufferId);
			GL.BindTexture(TextureTarget.Texture2D, this._TextureId);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 0);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _resolutionx, _resolutiony, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, this._TextureId, 0);

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			GL.BindTexture(TextureTarget.Texture2D, 0);

			if (this._ImageSource != null)
				this._ImageSource._SetSize(_resolutionx, _resolutionx);
		}
		
		public void Clear(Color _color) => this.Draw(new Rectangle(0, this.Resolution) { Color = _color, BlendMode = BlendMode.None });

		public void Clear() => this.Clear(this.Background);
		
		public void Draw(Bunch<Graphic> _graphics)
		{
			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.Enable(EnableCap.Texture2D);
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, this._FramebufferId);
			
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Viewport(0, 0, this.Resolution.X, this.Resolution.Y);

			if (this._RenderToWindow)
				GL.Ortho(0, this.Resolution.X, this.Resolution.Y, 0, 0, 4);
			else
				GL.Ortho(0, this.Resolution.X, 0, this.Resolution.Y, 0, 4);
			
			foreach (Graphic g in _graphics)
				Renderer._Draw(g);
			
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		}

		public static void _Draw(Graphic _graphic)
		{
			if (_graphic is VertexGraphic && !_graphic.Disposed)
			{
				GL.Enable(EnableCap.Texture2D);

				VertexArray v = ((VertexGraphic)_graphic).ToVertexArray()._GetPrimitive();

				if (v.Shader != null)
					v.Shader._SetUp(v.Texture);
				else
					GL.UseProgram(0);

				Vector size = 1;
				if (v.Texture != null)
				{
					GL.ActiveTexture(TextureUnit.Texture0);
					GL.BindTexture(TextureTarget.Texture2D, v.Texture._TextureId);
					size = v.Texture.Size;

					if (v.SmoothTexture)
					{
						GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
						GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
					}
					else
					{
						GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
						GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
					}
				}
				else
					GL.BindTexture(TextureTarget.Texture2D, 0);
				
				GL.Enable(EnableCap.Blend);
				if (v.BlendMode == BlendMode.Alpha)
					//GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
					GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
				else if (v.BlendMode == BlendMode.None)
					GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.Zero);
				else if (v.BlendMode == BlendMode.Multiply)
					GL.BlendFunc(BlendingFactorSrc.DstColor, BlendingFactorDest.Zero);
				else
					GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
				
				if (v.Type == VertexArrayType.Triangles)
					GL.Begin(PrimitiveType.Triangles);
				else
					GL.Begin(PrimitiveType.Lines);
				
				foreach (Vertex vx in v.Vertices)
				{
					GL.Color4(vx.Color.R, vx.Color.G, vx.Color.B, vx.Color.A);
					GL.TexCoord2(vx.ImagePosition.X / size.X, vx.ImagePosition.Y / size.Y);
					GL.Vertex2(vx.Position.X, vx.Position.Y);
					//GL.Vertex3(vx.Position.X, vx.Position.Y, vx.Position.Z);
				}

				//if (!(_graphic is Rectangle))
				//	throw new Exception();

				GL.End();
			}
		}

		public static void EnableMultithreading()
		{
			GameWindow w = new GameWindow(10, 10);
			GraphicsContext c = new GraphicsContext(GraphicsMode.Default, w.WindowInfo);
		}

		public void Dispose()
		{
			GL.DeleteFramebuffers(1, new int[] { this._FramebufferId });
			GL.DeleteTextures(1, new int[] { this._TextureId });
		}

		public void DisposeWithoutImage() => GL.DeleteFramebuffers(1, new int[] { this._FramebufferId });
	}
}