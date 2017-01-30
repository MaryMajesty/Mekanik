using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	[AttributeUsage(AttributeTargets.Field)]
	public class EditableAttribute : Attribute
	{
		public object Default;

		public EditableAttribute() { }

		public EditableAttribute(object _default)
		{
			this.Default = _default;
		}
	}
}