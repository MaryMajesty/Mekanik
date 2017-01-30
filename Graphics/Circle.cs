using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Circle : VertexGraphic
	{
		public double Radius;
		public int Quality = 20;
		public Color? InnerColor;
		public Color? OuterColor;

		public Circle(double _radius)
		{
			this.Radius = _radius;
			this.Origin = 0.5;
		}

		protected internal override VertexArray _ToVertexArray()
		{
			VertexArray v = new VertexArray(VertexArrayType.TrianglesFan);
			v.Add(this.Radius, this.InnerColor.HasValue ? this.InnerColor.Value : Color.White);
			for (int i = 0; i < Quality; i++)
				v.Add(this.Radius + Vector.FromAngle(i / (double)(Quality - 1) * Meth.Tau, this.Radius), this.OuterColor.HasValue ? this.OuterColor.Value : Color.White);
			return v;
		}
	}
}