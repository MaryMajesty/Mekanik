using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Slider : Entity
	{
		public MouseArea MouseArea;
		public VertexArray VertexArray = new VertexArray(VertexArrayType.Lines) { Color = Color.Black };
		public Vector Size = new Vector(64, 18);
		public int Sections = 1;
		public double[] Progresses;
		private double _Value;
		public double Progress;
		private bool _SetValue;
		public Text Text = new Text(FontBase.Consolas) { Color = Color.Black, CharSize = 10 };
		public Func<double, string> ValueDisplay = value => ((int)(value * 100)).ToString();

		public double Value
		{
			get { return this._Value / this.Sections; }
			set
			{
				this._Value = value * this.Sections;
				this._SetValue = true;
			}
		}

		public Slider()
		{
			this.Interfacial = true;
		}

		public override void OnInitialization()
		{
			this.Sections = (int)this.Size.X / 4;
			if (!this._SetValue)
				this.Value = 1;
			else
				this.Value = this._Value;
			this.Progresses = new double[Sections];

			this.VertexArray.Position = new Vector(0, Size.Y);
			this.Graphics.Add(this.VertexArray);
			
			this.MouseArea = new MouseArea(new Rectangle(0, this.Size));
			this.AddMouseArea(this.MouseArea);

			this.Graphics.Add(this.Text);
		}

		public override void Update()
		{
			double v = Meth.Limit(0, LocalMousePosition.X / Size.X * Sections, Sections);
			if (this.MouseArea.IsClicked)
				this._Value = v;

			this.Progress += ((this.MouseArea.IsHovered && !this.MouseArea.IsClicked ? 1 : 0) - this.Progress) * 0.3;

			this.Text.Color ^= (byte)((255 - this.Progress * 255) * (this.MouseArea.IsClicked ? 1 : 0));
			this.Text.Origin = new Vector(this.Value, 1);
			this.Text.Position = new Vector(this.Value * this.Size.X, this.Size.Y - this.Value * this.Size.Y - 6 + this.Value * 3);
			this.Text.Content = this.ValueDisplay(this.Value);

			for (int i = 0; i < Sections; i++)
				Progresses[i] += ((this._Value > i ? 1 : 0) - Progresses[i]) * 0.4;

			this.VertexArray.Vertices.Clear();
			this.VertexArray.Add(0);
			this.VertexArray.Add(new Vector(Size.X, 0));
			for (int i = 0; i < Sections; i++)
			{
				double d = Meth.Abs((v - i) / Sections * Size.X);
				double f = (d <= Size.X / 4) ? 1 - Meth.RSmooth(d / (Size.X / 4)) * 0.2 * this.Progress : 1;
				
				VertexArray.Add(new Vector((i + 0.5) / (double)Sections * Size.X, -1));
				VertexArray.Add(new Vector((i + 0.5) / (double)Sections * Size.X, (Progresses[i] + (i > this._Value ? 1 - f : 0)) * i / Sections * -(Size.Y - 2) * f - 3));
			}
		}
	}
}