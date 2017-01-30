using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class OverlapArea
	{
		public double X;
		public double Y;
		public Vector Position
		{
			get { return new Vector(this.X, this.Y); }

			set
			{
				this.X = value.X;
				this.Y = value.Y;
			}
		}
		public double Width;
		public double Height;
		public Vector Size
		{
			get { return new Vector(this.Width, this.Height); }

			set
			{
				this.Width = value.X;
				this.Height = value.Y;
			}
		}
		public Rect Rect
		{
			get { return new Rect(this.Position, this.Size); }

			set
			{
				this.Position = value.Position;
				this.Size = value.Size;
			}
		}
		internal Bunch<OverlapArea> _OverlappedAreas;
		public Bunch<OverlapArea> OverlappedAreas
		{
			get { return this._OverlappedAreas.Clone(); }
		}
		internal Entity _Parent;
		public Entity Parent
		{
			get { return this._Parent; }
		}

		public OverlapArea(double _x, double _y, double _width, double _height)
		{
			this.X = _x;
			this.Y = _y;
			this.Width = _width;
			this.Height = _height;
		}

		public OverlapArea(Vector _position, Vector _size) : this(_position.X, _position.Y, _size.X, _size.Y) { }
	}
}