using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Checkbox : InputControl
	{
		public VertexArray VertexArray = new VertexArray(VertexArrayType.Lines) { Color = Color.Black };
		public Text Text;
		public MouseArea MouseArea;
		public bool Checked;
		public double Progress;
		public bool JustClicked;
		public static double Size = 20;
		public bool Enabled = true;
		public Action OnCheck;
		public Action OnUncheck;

		public string Content
		{
			get { return this.Text.Content; }
			set { this.Text.Content = value; }
		}

		public Checkbox(bool _value)
		{
			this.Text = new Text(FontBase.Consolas) { Color = Color.Black, CharSize = 14, Origin = new Vector(0, 0), Position = new Vector(Size, 2) };
			this.Interfacial = true;

			this.Checked = _value;
			this.Progress = _value ? 1 : 0;
		}

		internal override double _GetRectWidth() => Checkbox.Size + this.Text.Width;
		internal override double _GetRectHeight() => Checkbox.Size;
		public override object GetValue() => this.Checked;
		protected override bool _IsEdited() => this.MouseArea.IsHovered;

		public override void OnInitialization()
		{
			this.MouseArea = new MouseArea(new Rectangle(0, new Vector(Size + this.Text.Width + 2, Size)));
			this.MouseArea.OnClick += key =>
				{
					if (this.Enabled)
					{
						this.Checked = !this.Checked;
						this.JustClicked = true;

						if (this.Checked && this.OnCheck != null)
							this.OnCheck();
						if (!this.Checked && this.OnUncheck != null)
							this.OnUncheck();
					}
				};
			this.AddMouseArea(this.MouseArea);

			this.Graphics.Add(this.VertexArray);
			this.Graphics.Add(this.Text);

			this.Progress = this.Checked ? 1 : 0;
		}

		public override void Update()
		{
			if (this.Enabled)
			{
				if (this.Progress < 0.01)
					this.Progress = 0;
				if (this.Progress > 0.99)
					this.Progress = 1;

				if (this.MouseArea.IsHovered)
				{
					if (!this.MouseArea.IsClicked)
						this.Progress -= (this.Progress - (this.Checked ? 0.7 : 0.2)) * 0.4;
					else
						this.Progress -= (this.Progress - (this.Checked ? 1 : 0)) * 0.5;
				}
				else
				{
					this.JustClicked = false;
					this.Progress -= (this.Progress - (this.Checked ? 1 : 0)) * 0.5;
				}
			}
			else
				this.Progress = this.Checked ? 1 : 0;

			this.VertexArray.Vertices.Clear();
			for (int i = 0; i < 4; i++)
			{
				this.VertexArray.Vertices.Add(Size / 2 + 0.5 + Vector.FromAngle(i * Meth.Tau / 4 - Meth.Tau / 8, Size / 3 * Meth.Root(2)));
				this.VertexArray.Vertices.Add(Size / 2 + 0.5 + Vector.FromAngle(i * Meth.Tau / 4 - Meth.Tau / 8, Size / 3 * Meth.Root(2)) + Vector.FromAngle(i * Meth.Tau / 4 + Meth.Tau / 4 + Progress * Meth.Tau / 8, Size / 1.5 * (1 - Meth.Pow(Progress, 1.2)) + Size / 3 * Meth.Root(2) * Progress * 1.05));
				//this.VertexArray.Vertices.Add(new Vertex(Size / 2 + 0.5 + Vector.FromAngle(i * Meth.Tau / 4 + Meth.Tau / 8, Size / 3 * Meth.Root(2) * (1 - Progress)) + Vector.FromAngle(i * Meth.Tau / 4 - Meth.Tau / 8, -1 * Meth.Root(2) * Progress), Color.Black));
			}
		}
	}
}