using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	[AttributeUsage(AttributeTargets.Class)]
	public class OriginAttribute : Attribute
	{
		public Vector Origin;

		public OriginAttribute(double _x, double _y)
		{
			this.Origin = new Vector(_x, _y);
		}
	}
}