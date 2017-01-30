using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Image : Sizable
	{
		//public Point CutSize;
		//public Point CutStart;
		//public bool Repeated;
		//public Point Quality = new Point(1, 1);
		//public bool FlippedHorizontally;

		public ImageSource Source
		{
			get { return this.Texture; }

			set
			{
				this.Texture = value;
				this.Size = this.Texture.Size;
				//this.CutSize = this.Size;
			}
		}

		public Image(string _path)
		{
			this.Texture = GameBase.LoadImageSource(File.ReadBytes(_path));
			this.Color = Color.White;

			this.Size = this.Texture.Size;
			//this.CutSize = this.Size;

			this.LockToGrid = true;
		}

		public Image(ImageSource _source)
		{
			this.Texture = _source;
			this.Color = Color.White;

			this.Size = this.Texture.Size;
			//this.CutSize = this.Size;

			this.LockToGrid = true;
		}

		protected internal override VertexArray _ToVertexArray()
		{
			return new VertexArray(VertexArrayType.Quads) { Vertices = new Bunch<Vertex>(new Vertex(0, 0), new Vertex(new Vector(this.Width, 0), new Vector(this.Texture.Width, 0)), new Vertex(this.Size, this.Texture.Size), new Vertex(new Vector(0, this.Height), new Vector(0, this.Texture.Height))) };
		}

		public Image Extend()
		{
			ImageSource src = this.Source.Extend();
			Vector p = this.Source.Size * this.Origin;
			return new Image(src) { Origin = (p + 1) / src.Size };
		}

		//public void Dispose()
		//{
		//	this.Texture._Image.Dispose();
		//	this.Texture._Texture.Dispose();
		//}
	}
};