using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Meka;

namespace Mekanik
{
	internal class SavedEntity
	{
		public string Identifier;
		public List<MekaItem> Properties;

		public SavedEntity(Bunch<FieldInfo> _fields, Entity _entity)
		{
			this.Identifier = _entity.Identifier;
			this.Properties = PropertySaver.Save(_fields.ToDictionary(item => item.Name, item => item.GetValue(_entity)));
		}

		public SavedEntity(MekaItem _item)
		{
			this.Identifier = _item.Name;
			this.Properties = _item.Children;
		}

		public void Apply(Entity _entity) => PropertyReflector.ApplyProperties(_entity, PropertySaver.Load(PropertyReflector.GetPropertyTypes<SavableAttribute>(_entity.GetType()), this.Properties));
	}
}