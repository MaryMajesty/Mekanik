using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class RectangleRounded : Sizable
	{
		public Vector Edge;
		public int Quality = 5;

		public RectangleRounded(Vector _position, Vector _size, Vector _edge)
		{
			this.Position = _position;
			this.Size = _size;
			this.Edge = _edge;
		}

		public RectangleRounded(double _x, double _y, double _width, double _height, double _edgewidth, double _edgeheight)
			: this(new Vector(_x, _y), new Vector(_width, _height), new Vector(_edgewidth, _edgeheight)) { }

		protected internal override VertexArray _ToVertexArray()
		{
			VertexArray @out = new VertexArray(VertexArrayType.Polygon);

			Vector s = this.Size - this.Edge * 2;
			Bunch<Vector> ps = new Bunch<Vector>(this.Edge, this.Edge + s.OnlyX, this.Edge + s, this.Edge + s.OnlyY);
			for (int n = 0; n < 4; n++)
			{
				for (int i = 0; i <= this.Quality; i++)
					@out.Add(ps[(n + 2) % 4] + Vector.FromAngle(i / (double)this.Quality * Meth.Tau / 4 + Meth.Tau / 4 * n) * this.Edge);
			}

			return @out;
		}
	}
}