using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class ByteGenerator
	{
		private double _Seed;

		public ByteGenerator(double _seed)
		{
			this._Seed = _seed / Meth.Tau + 1337;
		}

		public byte GetByte()
		{
			string s = Meth.Sin(this._Seed).ToString();
			this._Seed = (this._Seed + 1337) % Meth.Tau;

			return (byte)(int.Parse(s.Substring(s.Length - 3)) % 256);
		}

		public double GetDouble()
		{
			int n = GetByte() * 256 + GetByte();
			return n / (double)((256 * 256) - 1);
		}

		//public Bunch<T> Get(int _length)
		//{
		//	Bunch<T> @out = new Bunch<T>();
		//	for (int i = 0; i < _length; i++)
		//		@out.Add(this.Get());
		//	return @out;
		//}
	}
}