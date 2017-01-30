using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using SFML.Graphics;

namespace Mekanik
{
	public class Ring : VertexGraphic
	{
		public double Radius;
		public double HoleRadius;
		public int Quality = 100;
		//public Color OuterColor;
		//public Color InnerColor;

		public Ring(double _radius) { this.Radius = _radius; }

		public Ring(double _holeradius, double _radius)
		{
			this.HoleRadius = _holeradius;
			this.Radius = _radius;
		}

		protected internal override VertexArray _ToVertexArray()
		{
			VertexArray @out = new VertexArray(VertexArrayType.TrianglesStrip);
			for (int i = 0; i <= Quality; i++)
			{
				@out.Add(this.Position + Vector.FromAngle(i / (double)Quality * Meth.Tau) * HoleRadius + this.HoleRadius);
				@out.Add(this.Position + Vector.FromAngle(i / (double)Quality * Meth.Tau) * Radius + this.Radius);
			}
			return @out;
		}

		internal override VertexArray _ToShapeVertexArray()
		{
			VertexArray @out = new VertexArray(VertexArrayType.Polygon);
			for (int i = 0; i <= Quality; i++)
				@out.Add(this.Position + Vector.FromAngle(i / (double)Quality * Meth.Tau) * HoleRadius + HoleRadius);
			for (int i = Quality; i >= 0; i--)
				@out.Add(this.Position + Vector.FromAngle(i / (double)Quality * Meth.Tau) * Radius + Radius);
			return @out;
		}
	}
}