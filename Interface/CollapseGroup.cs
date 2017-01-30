using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class CollapseGroup : Entity
	{
		public double Width = 100;
		public double InnerHeight = 100;
		public double BarHeight = 16;
		public VertexArray Bar;
		public VertexArray Triangle;
		public Text Text;
		public double Progress;
		public MouseArea BarArea;
		public bool Opened;
		public Entity ChildArea;
		public Bunch<Entity> Contents;

		public double Height
		{
			get { return this.BarHeight + this.InnerHeight * this.Progress; }
		}

		public CollapseGroup(FontBase _font, string _name, params Entity[] _children)
		{
			this.Interfacial = true;
			this.Contents = _children;

			this.Graphics.Add(this.Text = new Text(_font) { Content = _name, Origin = new Vector(0, 0.5), Z = 1 });
		}

		public override void OnInitialization()
		{
			this.Bar = new VertexArray(VertexArrayType.Polygon) { Color = Color.White * 0.8 };
			this.Bar.Add(0, new Vector(Width, 0), new Vector(Width, BarHeight), new Vector(0, BarHeight));
			this.Graphics.Add(this.Bar);

			this.Triangle = new VertexArray(VertexArrayType.LinesStrip) { Z = 1, Color = Color.Black };
			for (int i = 0; i <= 3; i++)
				this.Triangle.Add(Vector.FromAngle(Meth.Tau / 4 + i / 3.0 * Meth.Tau) * 5);
			this.Graphics.Add(this.Triangle);

			this.AddMouseArea(this.BarArea = new MouseArea(this.Bar) { OnClick = key => this.Opened = !this.Opened });

			this.Children.Add(this.ChildArea = new Entity());
			this.ChildArea.Children.Add(this.Contents);
		}

		public override void Update()
		{
			bool rebind = (this.Progress != 0 && this.Progress != 1);

			this.Progress += ((this.Opened ? 1 : 0) - this.Progress) / 3;
			if (Meth.Abs(this.Progress - (this.Opened ? 1 : 0)) < 0.01)
				this.Progress = this.Opened ? 1 : 0;

			this.Text.Position.Y = BarHeight / 2 + this.InnerHeight * this.Progress;
			this.Bar.Position.Y = this.InnerHeight * this.Progress;
			this.Triangle.Position = new Vector(this.Width - this.BarHeight / 2, this.BarHeight / 2 + this.InnerHeight * this.Progress);

			this.ChildArea.Scale = new Vector(1, this.Progress);

			if (rebind)
				this.BarArea.Shape = this.Bar;

			this.Triangle.Scale = new Vector(1, (this.Progress * 2 - 1) * -1);
		}
	}
}