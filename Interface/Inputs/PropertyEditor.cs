using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zero;

namespace Mekanik
{
	public class PropertyEditor : InputControl
	{
		private Dictionary<string, Type> _PropertyTypes;
		private Dictionary<string, Func<object>> _Funcs = new Dictionary<string, Func<object>>();
		private Alignment _Alignment;

		public PropertyEditor(Dictionary<string, Type> _props, Dictionary<string, object> _defaults = null)
		{
			this._PropertyTypes = _props;
			this._Load(_defaults);
		}

		public PropertyEditor(Type _type, object _value = null) : this(_type.GetFields().ToDictionary(item => item.Name, item => item.FieldType), (_value == null) ? null : _type.GetFields().ToDictionary(item => item.Name, item => item.GetValue(_value))) { }

		private void _Load(Dictionary<string, object> _defaults = null)
		{
			Bunch<Alignable> als = new Bunch<Alignable>();

			foreach (KeyValuePair<string, Type> prop in this._PropertyTypes)
			{
				bool vert = false;

				object def = null;
				if (_defaults != null)
					def = _defaults[prop.Key];

				if (def == null)
				{
					if (prop.Value.IsValueType)
						def = Activator.CreateInstance(prop.Value);
					else if (prop.Value.IsArray)
						def = Activator.CreateInstance(prop.Value, new object[] { 0 });
					//def = prop.Value.GetConstructor(new Type[0]).Invoke(new object[0]);
				}
				else
				{
					if (def is string && !prop.Value.IsEnum && prop.Value != typeof(string)/* && prop.Value != typeof(Script)*/)
						def = prop.Value.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { def });
				}

				Alignable al = new Label("Input for type " + prop.Value.Name + " not implemented yet.");

				if (InputControl.CanInput(prop.Value))
				{
					InputControl c = InputControl.GetControl(prop.Value, def);

					this._Funcs[prop.Key] = () => c.GetValue();
					al = c;
				}
				else if (prop.Value.IsEnum)
				{
					Bunch<string> enums = prop.Value.GetEnumNames();
					int index = enums.IndexOf(def.ToString());
					SelectList l = new SelectList(enums) { Index = index, TargetIndex = index };

					this._Funcs[prop.Key] = () => prop.Value.GetEnumValues().GetValue(l.TargetIndex);
					al = l;
				}
				else if (prop.Value.IsArray)
				{
					ArrayEditor e = new ArrayEditor(prop.Value, def);

					this._Funcs[prop.Key] = () => e.GetValue();
					al = e;
					vert = true;
				}

				als.Add(new Alignment(new Label(prop.Key + ":"), al) { Vertical = vert });
			}

			this._Alignment = new Alignment(als) { Vertical = true };
		}

		public override void OnInitialization() => this.Children.Add(this._Alignment);

		public Dictionary<string, object> GetProperties()
		{
			Dictionary<string, object> @out = new Dictionary<string, object>();
			foreach (KeyValuePair<string, Func<object>> func in this._Funcs)
				@out[func.Key] = func.Value();
			return @out;
		}

		public void SetProperties(Dictionary<string, object> _properties)
		{
			this._Alignment.Kill();
			this._Load(_properties);
			this.Children.Add(this._Alignment);
		}

		public override object GetValue() => this.GetProperties();

		internal override double _GetRectWidth() => this._Alignment.RectSize.X;
		internal override double _GetRectHeight() => this._Alignment.RectSize.Y;

		protected override bool _IsEdited() => this._Alignment.Children.GetTypes<InputControl>().Any(item => item.IsEdited);
	}
}