using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Alignment : Alignable
	{
		public bool Vertical;
		public double Spacing = 0;
		private Bunch<Alignable> _Items;

		public Alignment(params Alignable[] _items)
		{
			this._Items = _items;
		}

		internal override double _GetRectWidth() => this.Vertical ? this._Items.Max(item => item._GetRectWidth()) : this._Items.Sum(item => item._GetRectWidth()) + this.Spacing * Meth.Max(0, this._Items.Count - 1);
		internal override double _GetRectHeight() => this.Vertical ? this._Items.Sum(item => item._GetRectHeight()) + this.Spacing * Meth.Max(0, this._Items.Count - 1) : this._Items.Max(item => item._GetRectHeight());

		public override void OnInitialization() => this.Children.Add(this._Items);

		public override void Update()
		{
			Vector p = 0;
			foreach (Alignable item in this._Items)
			{
				item.Position = p;
				p += item.RectSize * (this.Vertical ? Point.Down : Point.Right) + this.Spacing * (Vector)(this.Vertical ? Point.Down : Point.Right);
			}
		}
	}
}