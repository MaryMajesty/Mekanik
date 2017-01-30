using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Scrollbar : Entity
	{
		private double _MaxValue;
		public double MaxValue
		{
			get { return this._MaxValue; }

			set
			{
				if (this._MaxValue != value)
				{
					this._MaxValue = value;
					this.UpdateBar();
				}
			}
		}
		public double ValueRange = 50;
		private double _Height = 200;
		public double Height
		{
			get { return this._Height; }

			set
			{
				if (this._Height != value)
				{
					this._Height = value;
					this.SetPos(this.Pos + this.Value / this.MaxValue * value);
					this.UpdateBar();
				}
			}
		}
		public double Width = 8;
		public double Value;
		public VertexArray BarGraphic;
		public MouseArea BarArea;
		public Vector StartPosition;
		public bool Vertical = true;

		public double Pos
		{
			get { return this.Vertical ? this.StartPosition.Y : this.StartPosition.X; }
		}

		public Scrollbar()
		{
			this.Interfacial = true;
			this.Graphics.Add(this.BarGraphic = new VertexArray(VertexArrayType.Polygon));
		}

		public override void OnInitialization()
		{
			this.StartPosition = this.Position;
			this.AddMouseArea(this.BarArea = new MouseArea(new Rectangle(0, 1)) { Draggable = true });
			this.MaxValue = 100;
		}

		public override void OnDrag(Vector _position)
		{
			this.Value = Meth.Limit(0, ((this.Vertical ? _position.Y : _position.X) - this.Pos) / this.Height * this.MaxValue, this.MaxValue - this.ValueRange);
			this.SetPos(this.Pos + this.Value / this.MaxValue * this.Height);
		}

		public void SetPos(double _pos)
		{
			if (this.Vertical)
				this.Y = _pos;
			else
				this.X = _pos;
		}

		public void UpdateBar()
		{
			this.BarGraphic.Vertices.Clear();

			double l = this.ValueRange / this.MaxValue * this.Height;

			for (int i = 0; i <= 5; i++)
				this.BarGraphic.Add(new Vector(this.Width / 2, this.Width / 2) + Vector.FromAngle(Meth.Tau / 2 - (this.Vertical ? 0 : Meth.Tau / 4) + i / 5.0 * Meth.Tau / 2) * this.Width / 2);

			for (int i = 0; i <= 5; i++)
				this.BarGraphic.Add((this.Vertical ? new Vector(this.Width / 2, l - this.Width / 2) : new Vector(l - this.Width / 2, this.Width / 2)) + Vector.FromAngle(i / 5.0 * Meth.Tau / 2 - (this.Vertical ? 0 : Meth.Tau / 4)) * this.Width / 2);

			this.BarArea.Shape = this.BarGraphic;
		}

		public override void Update()
		{
			this.BarGraphic.Color = this.IsDragged ? Color.White * 0.7 : (this.BarArea.IsHovered ? Color.White * 0.6 : Color.White * 0.5);
		}
	}
}