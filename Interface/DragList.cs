using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class DragList<T> : Alignable where T : DragItem
	{
		public double Spacing = 16;
		public bool AllowNewItems = true;
		public Func<int, T> OnItemAdd = index => { throw new Exception("You haven't implemented yet what should happen when an item gets added, you silly."); };
		public Action<T> OnItemRemove = item => { };
		public Action OnOrderChange = () => { };
		public VertexArray Plus = new VertexArray(VertexArrayType.Lines) { Color = Color.Black, Visible = false };
		public MouseArea PlusArea;
		public double PlusProgress;
		public double Width = 200;
		public double MaxHeight = 200;
		public bool FixedHeight;
		private int _MousePos;

		public double Height
		{
			get { return this.FixedHeight ? this.MaxHeight : this.Items.Sum(item => item.GetHeight()) + this.Spacing * (this.Items.Count + 1); }
		}

		public Bunch<T> Items
		{
			get { return this.Children.GetTypes<T>().Where(item => !item.IsDead).OrderBy(item => item._ListIndex); }
		}

		public DragList()
		{
			this.Interfacial = true;
		}

		internal override double _GetRectWidth() => this.Width;
		internal override double _GetRectHeight() => this.Height;

		public void SetItems(params T[] _items)
		{
			this.Clear();

			int index = 0;
			double y = this.Spacing;
			foreach (T item in _items)
			{
				item.Y = y;
				item._ListIndex = index;

				y += item.GetHeight() + this.Spacing;
				index++;

				this.Children.Add(item);
			}
		}

		public override void OnInitialization()
		{
			this.Plus.Add(new Vector(this.Spacing / -3, 0));
			this.Plus.Add(new Vector(this.Spacing / 3, 0));
			this.Plus.Add(new Vector(0, this.Spacing / -3));
			this.Plus.Add(new Vector(0, this.Spacing / 3));
			this.Graphics.Add(this.Plus);

			this.AddMouseArea(this.PlusArea = new MouseArea(new Rectangle(0, 1))
				{
					OnClick = key =>
						{
							T[] items = this.Items.ToArray();
							for (int i = this._MousePos; i < items.Length; i++)
								items[i]._ListIndex++;
							
							T item = this.OnItemAdd(this._MousePos);

							this.Children.Add(item);

							item._ListIndex = this._MousePos;
							if (items.Length == 0)
								item.Y = 0;
							else
							{
								if (this._MousePos < items.Length)
									item.Y = items[this._MousePos].Y - this.Spacing / 2 - item.GetHeight() / 2;
								else
									item.Y = items[this._MousePos - 1].Y + items[this._MousePos - 1].GetHeight() + this.Spacing / 2 - item.GetHeight() / 2;
							}

							this.OnOrderChange();
						}
				});
		}

		public void RemoveItem(T _item)
		{
			T[] items = this.Items.ToArray();
			for (int i = _item._ListIndex + 1; i < items.Length; i++)
				this.Items[i]._ListIndex--;

			_item.Kill();

			this.OnItemRemove(_item);
			this.OnOrderChange();
		}

		public override void Update()
		{
			double y = this.Spacing;
			Bunch<double> pos = new Bunch<double>();
			Bunch<double> threshes = new Bunch<double>();
			Bunch<double> middles = new Bunch<double>();
			foreach (T item in this.Items)
			{
				pos.Add(y);
				threshes.Add(y + item.GetHeight() + this.Spacing / 2);
				middles.Add(y + item.GetHeight() / 2);
				y += item.GetHeight() + this.Spacing;
			}

			double lheight = 0;

			if (this.Items.Count > 0)
			{
				middles.Add(y + this.Items.Last.GetHeight() / 2);

				T d = this.Items.First(item => item._BeingDragged);
				if (d != null)
				{
					int index = d._ListIndex;
					double p = d.Y + d.GetHeight() / 2;

					if (p < threshes[0])
						index = 0;
					else if (p > threshes.Last)
						index = threshes.Count - 1;
					else
					{
						for (int i = 0; i < threshes.Count; i++)
						{
							if (p < threshes[i])
							{
								index = i;
								break;
							}
						}
					}

					T[] items = this.Items.ToArray();

					int dif = index - d._ListIndex;
					if (dif > 0)
					{
						for (int i = d._ListIndex + 1; i <= index; i++)
							items[i]._ListIndex--;
					}
					else if (dif < 0)
					{
						for (int i = index; i < d._ListIndex; i++)
							items[i]._ListIndex++;
					}

					d._ListIndex = index;

					if (dif != 0)
						this.OnOrderChange();
				}

				foreach (T item in this.Items)
				{
					if (!item._BeingDragged)
					{
						item.Y += (pos[item._ListIndex] - item.Y) / 2;
						if (Meth.Abs(item.Y - pos[item._ListIndex]) < 1)
						{
							item.Y = pos[item._ListIndex];
							item._NeedsUpdate = false;
						}
					}
					else
						item.Y += item.LocalMousePosition.Y - item._DragStart.Y;
					
					item.Y = Meth.Limit(0, item.Y, this.Height - item.GetHeight());
					item.Z = item._BeingDragged ? 1 : 0;
				}

				lheight = this.Items.Last.GetHeight();
			}
			else
			{
				pos.Add(0);
				middles.Add(double.MaxValue);
			}

			if (this.LocalMousePosition.X >= 0 && this.LocalMousePosition.X < this.Width && this.LocalMousePosition.Y >= 0 && this.LocalMousePosition.Y < pos.Last + lheight + this.Spacing && !this.Items.Any(item => item._BeingDragged))
			{
				this._MousePos = threshes.IndexOf((this.Spacing / 2 + threshes).Last(t => t < middles.First(m => m > this.LocalMousePosition.Y))) + 1;
				Vector v = new Vector(this.Width / 2, (this.Spacing / 2 + threshes)[this._MousePos]);
				if (this.Plus.Position != v)
				{
					this.Plus.Position = v;
					this.PlusArea.Shape = new Rectangle(this.Plus.Position - this.Spacing / 2, this.Spacing);
				}
				this.Plus.Visible = true;
			}
			else
				this.Plus.Visible = false;

			this.Plus.Color = this.PlusArea.IsHovered ? Color.White * 0.5 : Color.Black;
		}
	}

	public class DragItem : Entity
	{
		internal Vector _DragStart;
		internal bool _BeingDragged;
		internal int _ListIndex;
		internal bool _NeedsUpdate;
		public virtual double GetHeight() { return 20; }

		public void StartDrag()
		{
			this._DragStart = this.LocalMousePosition;
			this._BeingDragged = true;
		}

		public void StopDrag() => this._BeingDragged = false;
	}
}