using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class WeightedBunch<T>
	{
		private Bunch<Tuple<T, double>> _Bunch = new Bunch<Tuple<T, double>>();

		public void Add(T _item, double _weight) => this._Bunch.Add(new Tuple<T, double>(_item, _weight));

		public double TotalWeight
		{
			get { return this._Bunch.Sum(item => item.Item2); }
		}
		public T RandomItem
		{
			get
			{
				double n = Meth.Random * this.TotalWeight;
				foreach (Tuple<T, double> item in this._Bunch)
				{
					if (n > item.Item2)
						n -= item.Item2;
					else
						return item.Item1;
				}
				throw new Exception();
			}
		}
	}
}