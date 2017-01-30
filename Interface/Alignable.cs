using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Alignable : Entity
	{
		virtual internal double _GetRectWidth() => 0;
		virtual internal double _GetRectHeight() => 0;

		public Vector RectSize
		{
			get { return new Vector(this._GetRectWidth(), this._GetRectHeight()); }
		}
	}
}