using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class NumBox : InputControl
	{
		private int _Value;
		public int Digits = 4;
		public int MinValue = 0;
		public int MaxValue = 9999;
		//private Point _DigitSize;
		//private Dictionary<char, ImageSource> _Chars = new Dictionary<char, ImageSource>();
		internal TextBox _TextBox;

		public int Value
		{
			get { return this._Value; }
			set { this._ChangeValue(value - this._Value); }
		}

		public NumBox(int _value)
		{
			this._Value = _value;

			//Text _text = new Text(Font.Consolas) { Position = 2 };

			//Vector v = _text.GetSize("0123456789") / new Vector(10, 1);
			//this._DigitSize = new Point(Meth.Up(v.X), Meth.Up(v.Y));

			//PhotoCanvas p = new PhotoCanvas(this._DigitSize);
			//foreach (char c in "0123456789")
			//{
			//	p.Clear(Color.Transparent);
			//	_text.Content = c.ToString();
			//	p.Draw(_text);
			//	this._Chars[c] = p.ImageSource;
			//}
		}

		private void _ChangeValue(int _offset)
		{
			this._Value = (int)Meth.Limit(this.MinValue, this._Value + _offset, this.MaxValue);
			this._TextBox.Content = this._Value.ToString("D" + this.Digits.ToString());
		}

		private void _ApplyText() => this._ChangeValue(int.Parse(this._TextBox.Content) - this.Value);

		public override void OnInitialization()
		{
			this._TextBox = new TextBox("") { /*FixedSize = new Vector(this.Digits * this._DigitSize.X + 4, this._DigitSize.Y), */AllowedChars = "0123456789", OnDefocus = this._ApplyText, OnEnter = this._ApplyText };
			this._ChangeValue(0);
			this.Children.Add(this._TextBox);

			this._TextBox.MouseArea.ClickableBy.Add(Key.MouseUp, Key.MouseDown);
			Action<Key> a = this._TextBox.MouseArea.OnClick;
			this._TextBox.MouseArea.OnClick = key =>
				{
					if (key == Key.MouseUp)
						this._ChangeValue(1);
					else if (key == Key.MouseDown)
						this._ChangeValue(-1);
					else
						a(key);
				};

			//this.AddMouseArea(this._MouseArea = new MouseArea(new Rectangle(0, new Vector(this._DigitSize.X * this.Digits, this._DigitSize.Y) + 4)));
		}
		
		internal override double _GetRectWidth() => this._TextBox.Width;
		internal override double _GetRectHeight() => this._TextBox.Height;
		public override object GetValue() => this._Value;
		protected override bool _IsEdited() => this._TextBox.MouseArea.IsHovered || this._TextBox.IsEdited;
	}
}