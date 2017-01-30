using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class NineSlice : VertexGraphic
	{
		private bool _Started;

		//private ImageSource _Source;
		//public ImageSource Source
		//{
		//	get { return this._Source; }

		//	set
		//	{
		//		this._Source = value;
		//		this.Texture = 
		//		//if (this._Started)
		//			//this._Update();
		//	}
		//}
		private Point _Size;
		public Point Size
		{
			get { return this._Size; }

			set
			{
				this._Size = value;
				if (this._Started)
					this._Update();
			}
		}
		private VertexArray _VertexArray;

		public NineSlice(ImageSource _source)
		{
			//this.Source = _source;
			this.Texture = _source;
		}

		private void _Update()
		{
			//PhotoCanvas c = new PhotoCanvas(this.Size.X, this.Size.Y);

			Point s = this.Texture.Size / 3;
			Point mid = this.Size - s * 2;

			this._VertexArray = new VertexArray(VertexArrayType.Quads);

			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					//Image img = new Image(this.Source) { CutStart = s * new Vector(x, y), CutSize = s, Repeated = true };
					//img.Position = new Vector((x == 2) ? (mid.X + s.X) : (x * s.X), (y == 2) ? (mid.Y + s.Y) : (y * s.Y));
					//img.Size = new Vector((x == 1) ? mid.X : s.X, (y == 1) ? mid.Y : s.Y);
					//c.Draw(img);
					Vector pos = new Vector((x == 2) ? (mid.X + s.X) : (x * s.X), (y == 2) ? (mid.Y + s.Y) : (y * s.Y));
					Vector size = new Vector((x == 1) ? mid.X : s.X, (y == 1) ? mid.Y : s.Y);
					for (int xx = 0; xx <= 1; xx++)
					{
						for (int yy = 0; yy <= 1; yy++)
							this._VertexArray.Add(pos + size * new Vector(xx, (xx == 1 ? yy : 1 - yy)), new Vector(x + xx, y + (xx == 1 ? yy : 1 - yy)) * s);
					}
				}
			}

			//this.Texture = c.ImageSource.Clone();
		}

		protected internal override VertexArray _ToVertexArray()
		{
			if (!this._Started)
			{
				this._Started = true;
				this._Update();
			}
			return this._VertexArray;
			//return new VertexArray(VertexArrayType.Quads) { Vertices = new Bunch<Vertex>(new Vertex(0, 0), new Vertex(new Vector(this.Size.X, 0), new Vector(this.Size.X, 0)), new Vertex(this.Size, this.Size), new Vertex(new Vector(0, this.Size.Y), new Vector(0, this.Size.Y))) };
		}
	}
}