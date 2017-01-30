using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	class EntityPropertyInstance
	{
		public string Name;
		public Type Type;
		public string Value;

		public EntityPropertyInstance(EntityProperty _property)
		{
			this.Name = _property.Name;
			this.Type = _property.Type;
			if (_property.Default != null)
				this.Value = _property.Default.ToString();
			else
			{
				if (_property.Type.IsValueType)
					this.Value = Activator.CreateInstance(_property.Type).ToString();
				else
					this.Value = "";
			}
		}
	}
}