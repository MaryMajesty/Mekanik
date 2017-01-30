using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Mekanik
{
	public abstract class Inputs
	{
		internal Entity _Entity;
		public Bunch<InputSchemeRaw> _InputSchemes = new Bunch<InputSchemeRaw>();

		internal void _Initialize()
		{
			foreach (FieldInfo f in this.GetType().GetFields().Where(item => item.FieldType.IsSubclassOf(typeof(InputSchemeRaw))))
				this._InputSchemes.Add((InputSchemeRaw)f.GetValue(this));

			foreach (InputSchemeRaw i in this._InputSchemes)
				i._Entity = this._Entity;
		}

		internal void _Update()
		{
			foreach (InputSchemeRaw i in this._InputSchemes)
				i._Update();
		}
	}
}