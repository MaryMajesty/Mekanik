using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class TabList : Entity
	{
		public Bunch<TabInfo> TabInfos = new Bunch<TabInfo>();
		public TabGroup Group;
		public int CurTab = 2;
		public Vector InnerSize;
		public bool AlignRight;
		public bool Collapsed;
		public VertexArray CollapseArrow = new VertexArray(VertexArrayType.Polygon);
		public MouseArea CollapseArea;
		public double CollapseArrowProgress;
		public double CollapseProgress;
		public VertexArray Inline;
		public VertexArray Outline;

		public string CurName
		{
			get { return this.TabInfos[this.CurTab].Name; }
		}
		public double Border
		{
			//get { return Font.Consolas.GetLineSpacing(14) / 2; }
			get { return FontBase.Consolas.CharSize / 2; }
		}
		public Vector OuterSize
		{
			get { return this.InnerSize + Border + new Vector(0, this.TabOffset); }

			set
			{
				Vector s = value - (Border + new Vector(0, this.TabOffset));
				if (this.InnerSize != s)
				{
					this.InnerSize = s;
					this._UpdateGraphics();
				}
			}
		}
		public Vector TotalSize
		{
			get { return this.OuterSize * this.CollapseProgress; }
		}
		public double TabOffset
		{
			get { return (new Label("HAIL HELIX")).Height; }
		}

		public TabList(params TabInfo[] _tabinfos)
		{
			this.Interfacial = true;
			this.TabInfos = _tabinfos;
		}

		public override void OnInitialization()
		{
			this.CollapseProgress = this.Collapsed ? 0 : 1;

			double d = Border / 2;
			double x = d;

			for (int i = 0; i < this.TabInfos.Count; i++)
			{
				Tab t = new Tab(this.TabInfos[i].Name) { Position = new Vector(x, 0.5) };
				this.Children.Add(t);
				x += t.Width;
			}

			if (this.AlignRight)
			{
				foreach (Entity c in this.Children)
					c.X += (this.OuterSize.X - x - d);
			}

			this.CollapseArrow.Add(new Vector(0, -1), new Vector(-1, 0), new Vector(-1, 0), new Vector(0, 1), new Vector(0, 0.5), new Vector(1, 0.5), new Vector(1, -0.5), new Vector(0, -0.5));
			this.CollapseArrow.Scale = this.TabOffset / 2 * 0.8;
			this.CollapseArrow.Position = new Vector(this.AlignRight ? this.TabOffset / 2 : (this.OuterSize.X - this.TabOffset / 2), this.TabOffset / 2);
			this.CollapseArrow.Rotation = this.AlignRight ? Meth.Tau / 2 : 0;
			this.Graphics.Add(this.CollapseArrow);

			this.AddMouseArea(this.CollapseArea = new MouseArea(this.CollapseArrow) { OnClick = key => this.Collapsed = !this.Collapsed });

			this._UpdateGraphics();

			this.Children.Add(this.Group = this.TabInfos[0].Entity);
			this.SelectTab(this.Children.GetTypes<Tab>()[0]);
		}

		private void _UpdateTabs()
		{
			double d = Border / 2;
			double x = d;

			foreach (Tab t in this.Children.GetTypes<Tab>())
			{
				t.Position = new Vector(x * this.CollapseProgress, 0.5);
				t.Scale = new Vector(this.CollapseProgress, 1);
				x += t.Width;
			}

			if (this.AlignRight)
			{
				foreach (Entity c in this.Children)
				{
					c.X += (this.OuterSize.X - x - d);
					c.X += (this.OuterSize.X - c.X) * (1 - this.CollapseProgress);
				}
			}

			
			this.Group.Scale = new Vector(this.CollapseProgress, 1);
			this.Inline.Scale = new Vector(this.CollapseProgress, 1);
			this.Outline.Scale = new Vector(this.CollapseProgress, 1);

			if (this.AlignRight)
			{
				this.Group.X = this.OuterSize.X - this.TotalSize.X + this.Border / 2;
				this.Inline.Position.X = this.OuterSize.X - this.TotalSize.X;
				this.Outline.Position.X = this.OuterSize.X - this.TotalSize.X;
			}

			this.CollapseArrow.Position = new Vector(this.AlignRight ? (this.TabOffset / 2 + (this.OuterSize.X - this.TabOffset) * (1 - this.CollapseProgress)) : (this.TotalSize.X - this.TabOffset / 2 + this.TabOffset * (1 - this.CollapseProgress)), this.TabOffset / 2);
			this.CollapseArrow.Rotation = this.AlignRight ? (Meth.Tau / 2 - Meth.Tau / 2 * (1 - this.CollapseProgress)) : (Meth.Tau / 2 * (1 - this.CollapseProgress));
			this.CollapseArea.Shape = this.CollapseArrow;
		}

		public override void Update()
		{
			this.CollapseArrowProgress += ((this.CollapseArea.IsHovered ? 1 : 0.5) - this.CollapseArrowProgress) * 0.4;
			this.CollapseArrow.Color = Color.Black ^ (byte)(255 * this.CollapseArrowProgress);

			if (this.CollapseProgress != (this.Collapsed ? 0 : 1))
			{
				if (this.CollapseProgress < 0.01)
					this.CollapseProgress = 0;
				if (this.CollapseProgress > 0.99)
					this.CollapseProgress = 1;

				this.CollapseProgress += ((this.Collapsed ? 0 : 1) - this.CollapseProgress) * 0.4;

				this._UpdateTabs();
			}
		}

		private void _UpdateGraphics()
		{
			this.Graphics.Clear();
			this.Graphics.Add(this.CollapseArrow);
			
			double d = Border / 2;

			Bunch<Vector> vs = new Bunch<Vector>();
			vs.Add(0);
			vs.Add(new Vector(0, d + this.InnerSize.Y));
			vs.Add(new Vector(d, d + this.InnerSize.Y + d));
			vs.Add(new Vector(d + this.InnerSize.X, d + this.InnerSize.Y + d));
			vs.Add(new Vector(d + this.InnerSize.X + d, d + this.InnerSize.Y));
			vs.Add(new Vector(d + this.InnerSize.X + d, d));
			vs.Add(new Vector(d + this.InnerSize.X, 0));
			vs.Add(0);

			if (this.AlignRight)
				vs = vs.Select(item => new Vector(this.OuterSize.X - item.X, item.Y));

			this.Graphics.Add(this.Inline = new VertexArray(VertexArrayType.Polygon) { Color = Color.White, Vertices = vs.Select(item => new Vertex(item, Color.White)), Position = new Vector(0, this.TabOffset), Z = 1 });
			this.Graphics.Add(this.Outline = new VertexArray(VertexArrayType.LinesStrip) { Color = Color.Black, Vertices = vs.Select(item => new Vertex(item, Color.White)), Position = new Vector(0, this.TabOffset), Z = 1 });
		}

		public void SelectTab(Tab _tab)
		{
			this.Children.Remove(this.Group);

			this.CurTab = this.Children.GetTypes<Tab>().IndexOf(_tab);

			for (int i = 0; i < this.TabInfos.Count; i++)
			{
				Tab t = this.Children.GetTypes<Tab>()[i];
				if (i == CurTab)
					t.Z = 1;
				else
					t.Z = 1 / (i + 2.0);
			}

			TabGroup g = this.TabInfos[this.CurTab].Entity;
			//g.Position = new Vector(0, this.ContentOffset) + Font.Consolas.GetLineSpacing(14) / 4;
			g.Position = new Vector(0, this.TabOffset) + this.Border / 2;
			g.Z = 2;
			this.Children.Add(this.Group = g);
		}

		public bool IsSelected(Tab _tab) { return this.Children.GetTypes<Tab>().IndexOf(_tab) == this.CurTab; }
	}

	public class TabInfo : Entity
	{
		public string Name;
		public TabGroup Entity;

		public TabInfo(string _name, params Entity[] _entities)
		{
			this.Name = _name;
			this.Entity = new TabGroup(_entities);
		}
	}

	public class TabGroup : Entity
	{
		public Bunch<Entity> Entities;

		public TabGroup(Bunch<Entity> _entities) { this.Entities = _entities; }

		public override void OnInitialization() { this.Children.Add(this.Entities); }
	}

	public class Tab : Entity
	{
		public string Content;
		public Label Label;
		public MouseArea MouseArea;
		public double Cutoff;
		public double OffsetLength = 5;

		public double Width
		{
			get { return this.Label.Width + this.Cutoff; }
		}
		public bool IsSelected
		{
			get { return ((TabList)this.Parents[0]).IsSelected(this); }
		}

		public Tab(string _content)
		{
			this.Interfacial = true;
			this.Content = _content;

			//this.Cutoff = Font.Consolas.GetLineSpacing(14) / 4;
			this.Cutoff = FontBase.Consolas.CharSize / 4;
			this.Label = new Label(this.Content) { Z = 0.000001, Position = new Vector(this.Cutoff * 0.5, 0) };

			Bunch<Vector> vs = new Bunch<Vector>();

			double w = this.Label.Width + this.Cutoff * 3;

			vs.Add(0);
			vs.Add(new Vector(0, this.Label.Height - this.Cutoff));
			vs.Add(new Vector(this.Cutoff, this.Label.Height));
			vs.Add(new Vector(w - this.Cutoff, this.Label.Height));
			vs.Add(new Vector(w, this.Label.Height - this.Cutoff));
			vs.Add(new Vector(w, 0));

			this.Graphics.Add(new VertexArray(VertexArrayType.Polygon) { Color = Color.White, Position = new Vector(-this.Cutoff, 0), Vertices = vs.Select(item => new Vertex(new Vector(item.X, this.Label.Height - item.Y), Color.White)) });
			this.Graphics.Add(new VertexArray(VertexArrayType.LinesStrip) { Color = Color.Black, Position = new Vector(-this.Cutoff, 0), Vertices = vs.Select(item => new Vertex(new Vector(item.X, this.Label.Height - item.Y), Color.White)) });
		}

		public override void OnInitialization()
		{
			this.Children.Add(this.Label);
			this.AddMouseArea(this.MouseArea = new MouseArea(new Rectangle(-this.Cutoff, 0, this.Width + this.Cutoff * 2, this.Label.Height)) { OnClick = key => ((TabList)this.Parents[0]).SelectTab(this) });
		}

		public override void Update()
		{
			this.Offset.X = 0.01;
			this.Offset.Y += ((this.MouseArea.IsHovered && (this.LocalMousePosition.Y + this.Offset.Y < this.Label.Height) ? 0 : (this.IsSelected ? 0 : 1)) * OffsetLength - this.Offset.Y) / 3;
			if (this.Offset.Y < this.OffsetLength / 10)
				this.Offset.Y = 0;
			if (this.Offset.Y > this.OffsetLength / 10 * 9)
				this.Offset.Y = this.OffsetLength;
		}
	}
}