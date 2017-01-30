using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class FileMenu : Entity
	{
		public Color TextColor = Color.Black;
		public Color BackgroundColor = Color.White;
		public Color EdgeColor = Color.Black;
		public Bunch<Selectable> Selectable = new Bunch<Selectable>();

		public FileMenu(params Selectable[] _selectables)
		{
			this.AttachedToScreen = true;
			this.Selectable = _selectables;
		}

		public override void OnInitialization()
		{
			double x = 0;
			foreach (Selectable selectable in this.Selectable)
			{
				FileTab t = new FileTab(selectable) { Position = new Vector(x, 0) };
				this.Children.Add(t);
				x += t.Label.Width;
			}
		}

		class FileTab : Entity
		{
			public Selectable Selectable;
			public Label Label;
			public MouseArea MouseArea;
			public double Progress;

			public FileTab(Selectable _selectable)
			{
				this.Interfacial = true;
				this.Selectable = _selectable;
			}

			public override void OnInitialization()
			{
				FileMenu m = (FileMenu)this.Parents[0];

				this.Children.Add(this.Label = new Label(this.Selectable.Name) { Position = new Vector(0.5, 0.5), TextColor = m.TextColor });

				//VertexArray v = new VertexArray(VertexArrayType.LinesStrip) { Color = this.Color };
				Bunch<Vector> vs = new Bunch<Vector>();
				vs.Add(new Vector(0, 0));
				vs.Add(new Vector(0, this.Label.Height / 4 * 3));
				vs.Add(new Vector(this.Label.Height / 4, this.Label.Height));
				vs.Add(new Vector(this.Label.Width - this.Label.Height / 4, this.Label.Height));
				vs.Add(new Vector(this.Label.Width, this.Label.Height / 4 * 3));
				vs.Add(new Vector(this.Label.Width, 0));

				VertexArray v;
				this.Graphics.Add(v = new VertexArray(VertexArrayType.Polygon) { Vertices = vs.Select(item => (Vertex)item), Color = m.BackgroundColor });
				this.Graphics.Add(new VertexArray(VertexArrayType.LinesStrip) { Vertices = vs.Select(item => (Vertex)item), Color = m.EdgeColor });

				this.AddMouseArea(this.MouseArea = new MouseArea(v)
					{
						OnClick = key =>
							{
								if (this.Selectable.IsEnabled())
								{
									this.Parent.ReleaseKey(key);
									this.Selectable.Action();
								}
							}
					});

				this.Progress = this.Selectable.IsEnabled() ? 0 : -1;
			}

			public override void Update()
			{
				this.Progress += ((this.Selectable.IsEnabled() ? (this.MouseArea.IsHovered && (Parent.MousePosition.X > 0 && Parent.MousePosition.Y > 0) ? 1 : 0) : -1) - this.Progress) / 3;

				if (this.Progress < -0.9)
					this.Progress = -1;
				if (this.Progress > -0.1 && this.Progress < 0.1)
					this.Progress = 0;
				if (this.Progress > 0.9)
					this.Progress = 1;

				this.Offset = new Vector(0, -4 + this.Progress * 4);
			}
		}
	}
}