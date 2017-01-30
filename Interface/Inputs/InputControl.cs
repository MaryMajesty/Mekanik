using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class InputControl : Alignable
	{
		private static Dictionary<Type, InputType> _Controls;
		private static Dictionary<Type, InputType> Controls
		{
			get
			{
				if (InputControl._Controls == null)
				{
					Dictionary<Type, InputType> @out = new Dictionary<Type, InputType>();
					@out[typeof(bool)] = new InputType(typeof(Checkbox));
					@out[typeof(string)] = new InputType(typeof(TextBox));
					@out[typeof(Zero.Script)] = new InputType(typeof(ScriptBox));

					foreach (Type t in new Bunch<Type>(typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(ushort)))
					{
						int min = t.GetField("MinValue").GetValue(null).ToString().To<int>();
						int max = t.GetField("MaxValue").GetValue(null).ToString().To<int>();
						@out[t] = new InputType(typeof(NumBox), obj => { ((NumBox)obj).MinValue = min; ((NumBox)obj).MaxValue = max; });
					}

					InputControl._Controls = @out;
				}

				return InputControl._Controls;
			}
		}
			// = new Dictionary<Type, Type>() { { typeof(bool), typeof(Checkbox) }, { typeof(string), typeof(TextBox) }, { typeof(int), typeof(NumBox) } };

		public static bool CanInput(Type _type) => InputControl.Controls.ContainsKey(_type);

		public static InputControl GetControl(Type _type, object _value)
		{
			object value = _value;
			if (value == null)
			{
				if (_type == typeof(string))
					value = "";
				else if (_type.IsValueType)
					value = Activator.CreateInstance(_type);
			}

			InputType t = InputControl.Controls[_type];
			InputControl @out = (InputControl)t.Type.GetConstructor(new Type[] { _type }).Invoke(new object[] { value });
			t.Init?.Invoke(@out);
			return @out;
		}



		public bool IsEdited
		{
			get { return this._IsEdited(); }
		}

		protected virtual bool _IsEdited() => false;
		public virtual object GetValue() => null;

		private class InputType
		{
			public Type Type;
			public Action<object> Init;

			public InputType(Type _type, Action<object> _init = null)
			{
				this.Type = _type;
				this.Init = _init;
			}
		}
	}
}