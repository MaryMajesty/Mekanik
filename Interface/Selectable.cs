using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Selectable
	{
		public string Name;
		public Action Action;
		public Func<bool> IsEnabled = () => true;

		public Selectable(string _name, Action _action)
		{
			this.Name = _name;
			this.Action = _action;
		}

		public Selectable(string _name, Action _action, Func<bool> _isenabled)
		{
			this.Name = _name;
			this.Action = _action;
			this.IsEnabled = _isenabled;
		}
	}
}