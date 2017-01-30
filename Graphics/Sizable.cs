using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public abstract class Sizable : VertexGraphic
	{
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
	}
}