using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class SelectList : Alignable
	{
		public Bunch<string> Items;
		public Bunch<Label> Labels;
		public double Index;
		public int TargetIndex;
		public int TargetQuadRot;
		public MouseArea MouseArea;
		public double Expansion;
		public VertexArray Quad = new VertexArray(VertexArrayType.Quads) { Z = 1, Color = Color.Black };
		public bool LastHovered;
		public bool Scrolling;
		public Vector ScrollStart;
		
		public string SelectedItem
		{
			get { return this.Items[this.TargetIndex]; }
		}

		public SelectList(params string[] _items)// : base(_items[0])
		{
			this.Items = _items;
			
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 2; y++)
					this.Quad.Add(new Vector(x * 6 - 3, ((x == 0) ? y : 1 - y) * 6 - 3));
			}
			//this.Quad.Position = new Vector(4, Font.Consolas.GetLineSpacing(14) / 4 * 3);
			this.Quad.Position = new Vector(4, FontBase.Consolas.CharSize / 4 * 3);
			this.Graphics.Add(this.Quad);

			this.Interfacial = true;
		}

		//public override object GetValue() => this.SelectedItem;
		internal override double _GetRectWidth() => this.Labels[this.TargetIndex].Width;
		internal override double _GetRectHeight() => this.Labels[this.TargetIndex].Height;

		public override void OnInitialization()
		{
			this.Children.Add(this.Labels = this.Items.Select(item => new Label(item) { Z = 1, Shader = Shader.TextOutline }));
			
			this.AddMouseArea(this.MouseArea = new MouseArea(new Rectangle(0, 0, 100, 20)) { ClickableBy = new Bunch<Key>(Key.MouseUp, Key.MouseDown), OnClick = Click });
			Parent.Clock.Add(2, () => this.LastHovered = true);
		}

		public void Click(Key _key)
		{
			this.TargetIndex = (int)Meth.Limit(0, (this.TargetIndex + ((_key == Key.MouseDown) ? 1 : -1)), this.Items.LastIndex);
			this.TargetQuadRot += (_key == Key.MouseDown) ? 1 : -1;

			if (!this.Scrolling)
			{
				this.Scrolling = true;
				this.ScrollStart = this.LocalMousePosition;
			}
		}

		public Vector GetPosition(double _rel)
		{
			double r = Meth.Tau / 8 * _rel * this.Expansion;
			Vector v = Vector.FromAngle(r, 12);
			//Vector p = new Vector(0, _rel * Font.Consolas.GetLineSpacing(14) * this.Expansion);
			Vector p = new Vector(0, _rel * FontBase.Consolas.CharSize * this.Expansion);
			return new Vector(v.X * 0.8 + p.X * 0.2, v.Y * 0.6 + p.Y * 0.4);
		}

		public override void Update()
		{
			if (this.Scrolling && (this.LocalMousePosition - this.ScrollStart).Length >= 2)
				this.Scrolling = false;
			//if (this.Scrolling)
			//	Parent.CursorVisible = false;

			this.Index += (this.TargetIndex - this.Index) / 5;
			this.Expansion += ((this.MouseArea.IsHovered ? 1 : 0) - this.Expansion) / 5;

			this.Quad.Rotation += ((this.TargetQuadRot * Meth.Tau / -4) - this.Quad.Rotation) / 5;
			if (Meth.Abs(this.Quad.Rotation - this.TargetQuadRot * Meth.Tau / -4) < 0.1)
				this.Quad.Rotation = this.TargetQuadRot * Meth.Tau / -4;

			this.Quad.Scale = new Vector(1, 2 / 3.0 + this.Expansion / 3.0);
			if (Meth.Abs(this.TargetQuadRot % 2) == 1)
				this.Quad.Scale.Angle += Meth.Tau / 4;

			if (this.LastHovered != this.MouseArea.IsHovered)
			{
				this.LastHovered = this.MouseArea.IsHovered;

				if (!this.MouseArea.IsHovered)
					this.MouseArea.Shape = new Rectangle(this.Labels[this.TargetIndex].Position, this.Labels[this.TargetIndex].Size);
			}

			if (Meth.Abs(this.TargetIndex - this.Index) < 0.05)
				this.Index = this.TargetIndex;

			for (int i = 0; i < this.Labels.Count; i++)
			{
				byte a = (byte)(Meth.Pow(Meth.Limit(0, 3 - Meth.Abs(i - this.Index), 3) / 3.0 * ((i == this.TargetIndex) ? 1 : this.Expansion), 0.4) * 255);
				
				//if (a != 0 && !this.Labels[i].Visible)
				//{
				//	this.Labels[i].Visible = true;
				//	//this.Labels[i].Shader = Shader.TextOutline;
				//	//this.Labels[i].Shader["Color"] = Color.Red;

				//	//this.Labels[i].RenderIntermediately = true;
				//	//this.Labels[i].RenderGraphicsIntermediately = true;
				//	//this.Labels[i].IntermediateSize = this.Labels[i].Size;
				//	//this.Labels[i].IntermediateShader = Shader.TextOutline;
				//}

				//if (this.Labels[i].Visible)
				//{
					double dis = Meth.Limit(0, Meth.Abs(i - this.Index) - 2, 1);
					
					this.Labels[i].Color ^= (byte)(((i == this.TargetIndex) ? 255 : this.Expansion * 255) * (1 - dis));
					//this.Labels[i].Shader["Alpha"] = (a / 255.0);

					this.Labels[i].Z = a / 255.0 * this.Expansion;

					Vector p = this.GetPosition(i - this.Index);
					double r = p.Angle.ToHalfTau() / 4;
					this.Labels[i].Rotation = r;
					this.Labels[i].Position = p;
				//}
			}
		}
	}
}