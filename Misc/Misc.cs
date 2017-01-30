using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Reflection;
using Zero;

namespace Mekanik
{
	public static class Misc
	{
		internal static CultureInfo Format = new CultureInfo("en-US");

		public static string String(this object @this) { return System.Convert.ToString(@this, Format); }

		public static int ToInt(this string @this) { return System.Convert.ToInt32(@this); }
		public static double ToDouble(this string @this) { return System.Convert.ToDouble(@this, Format); }

		private static Comp _Mekanik;
		public static Variable Mekanik
		{
			get
			{
				if (Misc._Mekanik == null)
					Misc._Mekanik = Wrapper.WrapAssembly(Assembly.GetExecutingAssembly());
				return new Variable("Mekanik", Misc._Mekanik);
			}
		}
	}
}