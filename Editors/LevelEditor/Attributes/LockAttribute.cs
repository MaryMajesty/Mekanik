using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	[AttributeUsage(AttributeTargets.Class)]
	public class LockAttribute : Attribute
	{
		public Vector Pos;
		public LockAttribute(Vector _pos) { this.Pos = _pos; }
		public LockAttribute(double _x, double _y) { this.Pos = new Vector(_x, _y); }
	}
}