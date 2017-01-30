using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Button : Alignable
	{
		public Action OnClick;
		public RectangleRounded Background;
		public Text Text;
		public MouseArea MouseArea;
		public double HoverProgress;

		public string Content
		{
			get { return this.Text.Content; }
			
			set
			{
				this.Text.Content = value;
				//this.Text.Position = this.Text.Size / 2;
				this.Background.Size = this.Text.Size;
				//this.Background.Position = this.Text.Size / 2;
				this.MouseArea.Shape = this.Background;
			}
		}

		public Button(string _content, Action _onclick)
		{
			this.Interfacial = true;

			this.OnClick = _onclick;

			this.Graphics.Add(this.Text = new Text(FontBase.Consolas) { Z = 1, Content = _content/*, Origin = 0.5*/ });
			//this.Text.Position = this.Text.Size / 2;
			this.Graphics.Add(this.Background = new RectangleRounded(0, this.Text.Size, 2) { Color = Color.White * 0.9/*, Position = this.Text.Size / 2, Origin = 0.5*/ });
		}

		public override void OnInitialization()
		{
			this.AddMouseArea(this.MouseArea = new MouseArea(this.Background) { OnClick = key => this.OnClick() });
		}

		public override void Update()
		{
			this.HoverProgress += ((this.MouseArea.IsHovered ? 1 : 0) - this.HoverProgress) * 0.4;
			this.Background.Color = Color.White * (0.9 - this.HoverProgress * 0.1);
		}

		internal override double _GetRectWidth() => this.Text.Width;
		internal override double _GetRectHeight() => this.Text.Height;
	}
}