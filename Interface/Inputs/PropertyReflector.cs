using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Mekanik
{
	public static class PropertyReflector
	{
		public static Dictionary<string, Type> GetPropertyTypes(Type _type) => _type.GetFields().ToDictionary(item => item.Name, item => item.FieldType);
		public static Dictionary<string, Type> GetPropertyTypes<T>(Type _type) where T : Attribute => _type.GetFields().Where(item => item.GetCustomAttributes<T>().ToArray().Length > 0).ToDictionary(item => item.Name, item => item.FieldType);

		public static Dictionary<string, object> GetPropertyValues(object _object) => _object.GetType().GetFields().ToDictionary(item => item.Name, item => item.GetValue(_object));
		public static Dictionary<string, object> GetPropertyValues<T>(object _object) where T : Attribute => _object.GetType().GetFields().Where(item => item.GetCustomAttributes<T>().ToArray().Length > 0).ToDictionary(item => item.Name, item => item.GetValue(_object));
		
		public static void ApplyProperties(object _object, Dictionary<string, object> _properties)
		{
			Type t = _object.GetType();

			foreach (KeyValuePair<string, object> p in _properties)
				t.GetField(p.Key).SetValue(_object, p.Value);
		}

		public static object GenerateFromProperties(Type _type, Dictionary<string, object> _properties)
		{
			object @out = _type.GetConstructor(new Type[0]).Invoke(new object[0]);

			ApplyProperties(@out, _properties);
			//foreach (KeyValuePair<string, object> p in _properties)
			//	_type.GetField(p.Key).SetValue(@out, p.Value);

			return @out;
		}
	}
}