using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Mekanik
{
	class EntityProperty
	{
		public string Name;
		public Type Type;
		public object Default;

		public EntityProperty(FieldInfo _info)
		{
			this.Name = _info.Name;
			this.Type = _info.FieldType;

			Bunch<EditableAttribute> attr = _info.GetCustomAttributes<EditableAttribute>().ToBunch();
			if (attr.Count > 0)
				this.Default = attr[0].Default;
		}
	}
}