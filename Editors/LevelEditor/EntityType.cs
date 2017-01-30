using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Mekanik
{
	class EntityType
	{
		public string Name;
		public string Group = "Ungrouped";
		public ImageSource Icon;
		public Vector Origin;
		public Vector? Lock;
		public Bunch<EntityProperty> Properties = new Bunch<EntityProperty>();
		public bool Event;
		public bool Entrance;

		public EntityType(Type _type)
		{
			this.Name = _type.Name;
			if (this.Name != "Decoration")
				this.Icon = GameBase.LoadImageSource(File.ReadBytes((_type.GetCustomAttribute<IconAttribute>(true)).Path));

			Bunch<LockAttribute> atlock = _type.GetCustomAttributes<LockAttribute>().ToBunch();
			if (atlock.Count > 0)
				this.Lock = atlock[0].Pos;

			Bunch<OriginAttribute> atorigin = _type.GetCustomAttributes<OriginAttribute>().ToBunch();
			if (atorigin.Count > 0)
				this.Origin = atorigin[0].Origin;

			Bunch<GroupAttribute> atgroup = _type.GetCustomAttributes<GroupAttribute>().ToBunch();
			if (atgroup.Count > 0)
				this.Group = atgroup[0].Group;

			foreach (FieldInfo f in _type.GetFields().Where(item => item.GetCustomAttributes<EditableAttribute>().Count() > 0))
				this.Properties.Add(new EntityProperty(f));
			
			this.Properties = this.Properties.First(item => item.Name == "Identifier") + this.Properties.Where(item => item.Name != "Identifier");

			this.Event = _type.GetCustomAttributes<EventAttribute>().ToArray().Length > 0;
			this.Entrance = _type.IsSubclassOf(typeof(LevelConnection));

			//Bunch<Attribute> atidentifier = _type.GetCustomAttributes(typeof(IdentifierAttribute)).ToBunch();
			//if (atidentifier.Count > 0)
			//	this.Properties.First(item => item.Name == "Identifier").Default = ((IdentifierAttribute)atidentifier[0]).Default;
		}
	}
}