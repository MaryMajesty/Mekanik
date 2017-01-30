using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	[AttributeUsage(AttributeTargets.Class)]
	public class IconAttribute : Attribute
	{
		public string Path;

		public IconAttribute(string _path)
		{
			this.Path = _path;
		}
	}
}