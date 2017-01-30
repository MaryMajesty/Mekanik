using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	[AttributeUsage(AttributeTargets.Class)]
	public class GroupAttribute : Attribute
	{
		public string Group;

		public GroupAttribute(string _group)
		{
			this.Group = _group;
		}
	}
}