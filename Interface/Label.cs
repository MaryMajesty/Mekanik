using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Label : TextBased
	{
		public Label(string _value) : base(_value)
		{
			this._Text = new RichText(FontBase.Consolas) { Content = _value, TextColor = Color.Black, CharSize = 14, Position = new Vector(3, 2) };
			//this.AlwaysUpdateIntermediate = false;
		}

		public override void OnInitialization()
		{
			this.Children.Add(this._Text);
		}

		public Vector Size
		{
			get { return this.RectSize; }
		}
		public double Width
		{
			get { return this.Size.X; }
		}
		public double Height
		{
			get { return this.Size.Y; }
		}

		//protected override void _OnContentChange(string _old, string _new)
		//{
		//	if (this.RenderIntermediately && _old != _new)
		//	{
		//		this.UpdateIntermediate = true;
		//		this.IntermediateSize = this.Size;
		//	}
		//}
	}
}