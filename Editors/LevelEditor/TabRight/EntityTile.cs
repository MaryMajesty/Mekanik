using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	internal abstract class EntityTile
	{
		public Bunch<EntityType> Types;
		public ImageSource Icon;
		public abstract EntityIcon GetIcon(Layer _layer);
	}

	internal class EntityTileNormal : EntityTile
	{
		public EntityType Type;

		public EntityTileNormal(EntityType _type)
		{
			this.Type = _type;
			this.Icon = _type.Icon;
		}

		public override EntityIcon GetIcon(Layer _layer) => new EntityIcon(_layer, this.Type, _dragged: true);
	}

	internal class EntityTileDecoration : EntityTile
	{
		public Areaset Areaset;
		public string Name;

		public EntityTileDecoration(Areaset _areaset, string _name)
		{
			this.Areaset = _areaset;
			this.Name = _name;

			this.Icon = _areaset.Decorations[_name];
		}

		public override EntityIcon GetIcon(Layer _layer)
		{
			EntityIcon i = new EntityIcon(_layer, this.Types.First(item => item.Name == "Decoration"), _dragged: true);
			i.Properties.First(item => item.Name == "Areaset").Value = this.Areaset.Name;
			i.Properties.First(item => item.Name == "Name").Value = this.Name;
			return i;
		}
	}
}