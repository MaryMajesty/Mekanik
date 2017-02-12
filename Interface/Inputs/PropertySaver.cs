using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Meka;
using Zero;

namespace Mekanik
{
	public static class PropertySaver
	{
		public static List<MekaItem> Save(Dictionary<string, object> _properties)
		{
			List<MekaItem> @out = new List<MekaItem>();
			foreach (KeyValuePair<string, object> p in _properties)
			{
				if (p.Value == null)
					@out.Add(new MekaItem(p.Key));
				else
				{
					Type type = p.Value.GetType();
					if (type == typeof(ImageSource))
						@out.Add(new MekaItem(p.Key, ((ImageSource)p.Value).Bytes));
					else if (type.IsArray)
					{
						List<MekaItem> items = new List<MekaItem>();
						Type e = type.GetElementType();
						int length = (int)type.GetMethod("GetLength").Invoke(p.Value, new object[] { 0 });
						MethodInfo getvalue = type.GetMethod("GetValue", new Type[] { typeof(int) });

						for (int i = 0; i < length; i++)
						{
							object item = getvalue.Invoke(p.Value, new object[] { i });
							items.Add(new MekaItem("Item", Save(GetProperties(item))));
						}

						@out.Add(new MekaItem(p.Key, items));
					}
					else if (type.IsValueType || type.IsEnum || type == typeof(Script))
						@out.Add(new MekaItem(p.Key, p.Value.ToString()));
					else if (type == typeof(string))
						@out.Add(new MekaItem(p.Key, (string)p.Value));
					else
						@out.Add(new MekaItem(p.Key, Save(GetProperties(p.Value))));
				}
			}
			return @out;
		}

		public static Dictionary<string, object> GetProperties(object _object) => _object.GetType().GetFields().ToDictionary(it => it.Name, it => it.GetValue(_object));

		public static Dictionary<string, object> Load(Dictionary<string, Type> _properties, List<MekaItem> _data)
		{
			Dictionary<string, object> @out = new Dictionary<string, object>();
			foreach (KeyValuePair<string, Type> p in _properties)
			{
				object value = null;
				if (_data.Any(it => it.Name == p.Key))
				{
					MekaItem item = _data.First(it => it.Name == p.Key);

					if (!item.HasContent && !item.HasData && !item.HasChildren)
						value = null;
					else
					{
						if (p.Value == typeof(ImageSource))
							value = GameBase.LoadImageSource(item.Data);
						else if (p.Value.IsArray)
						{
							value = Activator.CreateInstance(p.Value, new object[] { item.Children.Count });
							Type et = p.Value.GetElementType();
							MethodInfo setvalue = p.Value.GetMethod("SetValue", new Type[] { et, typeof(int) });
							for (int i = 0; i < item.Children.Count; i++)
								setvalue.Invoke(value, new object[] { Load(et, item.Children[i].Children), i });
						}
						else if (p.Value == typeof(string))
							value = item.Content;
						else if (p.Value.GetMethods().Any(it => it.Name == "Parse" && it.GetParameters().Length == 1 && it.GetParameters()[0].ParameterType == typeof(string)))
							value = p.Value.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { item.Content });
						else if (p.Value.IsEnum)
							value = Enum.Parse(p.Value, item.Content);
						//value = p.Value.GetEnumValues().GetValue(p.Value.GetEnumNames().ToBunch().IndexOf(item.Content));
						else
							value = Load(p.Value, item.Children);
					}
				}
				else
				{
					if (p.Value.IsValueType)
						value = Activator.CreateInstance(p.Value);
				}

				@out[p.Key] = value;
			}
			return @out;
		}

		public static object Load(Type _type, List<MekaItem> _data)
		{
			object @out = _type.GetConstructor(new Type[0]).Invoke(new object[0]);

			Bunch<FieldInfo> fs = _type.GetFields();

			Dictionary<string, object> ps = Load(_type.GetFields().ToDictionary(item => item.Name, item => item.FieldType), _data);
			foreach (MekaItem item in _data)
			{
				if (fs.Any(f => f.Name == item.Name))
					_type.GetField(item.Name).SetValue(@out, ps[item.Name]);
			}

			return @out;
		}
	}
}