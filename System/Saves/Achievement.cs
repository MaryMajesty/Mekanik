using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka;

namespace Mekanik
{
	public enum AchievementType
	{
		Event,
		Counter,
		Collection
	}

	public abstract class Achievement
	{
		public AchievementType Type;
		public string Name;
		public string Description;
		public ImageSource Icon;

		public int RequirementCount;
		public Bunch<string> RequirementCollection;

		internal MekaItem _DefaultValue
		{
			get
			{
				if (this.Type == AchievementType.Event)
					return new MekaItem("Event", "False");
				else if (this.Type == AchievementType.Counter)
					return new MekaItem("Counter", "0");
				else
					return new MekaItem("Collection", new List<MekaItem>());
			}
		}

		public class Event : Achievement
		{
			public Event(string _name, string _description, ImageSource _icon)
			{
				this.Type = AchievementType.Event;
				this.Name = _name;
				this.Description = _description;
				this.Icon = _icon;
			}
		}

		public class Counter : Achievement
		{
			public Counter(string _name, string _description, ImageSource _icon, int _requirementcount)
			{
				this.Type = AchievementType.Counter;
				this.Name = _name;
				this.Description = _description;
				this.Icon = _icon;
				this.RequirementCount = _requirementcount;
			}
		}

		public class Collection : Achievement
		{
			public Collection(string _name, string _description, ImageSource _icon, params string[] _requirementcollection)
			{
				this.Type = AchievementType.Collection;
				this.Name = _name;
				this.Description = _description;
				this.Icon = _icon;
				this.RequirementCollection = _requirementcollection;
			}
		}
	}
}