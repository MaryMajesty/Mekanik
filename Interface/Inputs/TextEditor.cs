using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class TextEditor : TextBased
	{
		public string AllowedChars;
		public int CursorPosition;
		public int SelectionLength;
		public bool MultiLine;
		public bool TabAllowed;
		public MouseArea MouseArea;
		public Vector FixedSize;
		public Vector MinSize;
		public Bunch<TextSelectPattern> SelectPatterns = new Bunch<TextSelectPattern>(new TextSelectPattern.Word("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789"));
		private bool _PressedShift;
		private Tuple<int, int> _SelectStart;
		private int _DoubleClickPos;
		private int _DoubleClickTime;
		private bool _DoubleClicked;
		
		public int CurrentLine
		{
			get { return GetCharPos(this.CursorPosition).Y; }
		}
		public string[] Lines
		{
			get { return this.Content.ToLines(); }
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

		public TextEditor(string _value) : base(_value)
		{
			this._Text = new RichText(FontBase.Consolas);
			this.Edge = new Vector(3, 2);
			this.Interfacial = true;

			this.Content = _value;
		}

		internal override double _GetRectWidth() => Meth.Max((this.FixedSize != 0) ? this.FixedSize.X : this._Text.Width + this.Edge.X * 2, this.MinSize.X);
		internal override double _GetRectHeight() => Meth.Max((this.FixedSize != 0) ? this.FixedSize.Y : this._Text.Height + this.Edge.Y * 2, this.MinSize.Y);

		protected override bool _IsEdited() => this.IsFocused;

		public override void OnInitialization()
		{
			this.Children.Add(this._Text);
			this.AddMouseArea(this.MouseArea = new MouseArea(new Rectangle(0, 1)) { OnClick = this.OnClick, ClickableBy = new Bunch<Key>(Key.MouseLeft, Key.MouseUp, Key.MouseDown) });
			this.UpdateMouseArea();
		}

		protected virtual void _OnCursorChange() { }

		public Action OnEnter = () => { };
		protected virtual void _OnEnter() { this.OnEnter(); }

		public Func<string, bool> BeforeInput = input => { return true; };
		protected virtual bool _BeforeInput(string _input) { return this.BeforeInput(_input); }

		public Action<string> AfterInput = input => { };
		protected virtual void _AfterInput(string _input) { this.AfterInput(_input); }

		public Func<bool> BeforeBackspace = () => { return true; };
		protected virtual bool _BeforeBackspace() { return this.BeforeBackspace(); }

		public Action AfterBackspace = () => { };
		protected virtual void _AfterBackspace() { this.AfterBackspace(); }

		public Action AfterContentChange = () => { };
		public Action AfterContentChangeByUser = () => { };
		public Action OnTab = () => { };

		public int GetLeft() { return Meth.Min(this.CursorPosition, this.CursorPosition + this.SelectionLength); }
		public int GetRight() { return Meth.Max(this.CursorPosition, this.CursorPosition + this.SelectionLength); }

		public override void OnFocusLoss() => this.SelectionLength = 0;

		protected override void _OnContentChange(string _old, string _new)
		{
			if (this.CursorPosition > this.Content.Length)
				this.CursorPosition = this.Content.Length;
			if (this.MouseArea != null)
				this.UpdateMouseArea();

			this.AfterContentChange();
		}

		protected override void _OnCharSizeChange()
		{
			if (this._Initialized)
				this.UpdateMouseArea();
		}

		public override void OnTextInput(string _text)
		{
			if (_text[0] >= 32)
			{
				this.EnterText(_text);
				this.AfterContentChangeByUser();
			}
		}

		public void EnterText(string _text)
		{
			if (this.AllowedChars == null || _text.ToBunch().TrueForAll(item => this.AllowedChars.Contains(item)))
			{
				if (this._BeforeInput(_text))
				{
					if (this.SelectionLength != 0)
						this.DeleteText();
					this.Content = this.Content.Substring(0, this.CursorPosition) + _text + this.Content.Substring(this.CursorPosition);
					this.CursorPosition += _text.Where(item => item != '\r').ToArray().Length;
					this._OnCursorChange();
					this._AfterInput(_text);
					this.AfterContentChange();
				}
			}
		}

		public void DeleteText()
		{
			this._Text.Content = this.Content.Substring(0, this.GetLeft()) + this.Content.Substring(this.GetRight());
			this.CursorPosition = this.GetLeft();
			this.SelectionLength = 0;
			this._OnCursorChange();
			this._OnContentChange("", "");
		}

		public override void Update()
		{
			if (this.MouseArea.IsClicked)
			{
				if (!this._DoubleClicked)
				{
					int start = this._SelectStart.Item1;
					int end = this.GetCharPosition(this.LocalMousePosition);
					if (this.CursorPosition != end)
					{
						this.CursorPosition = end;
						this._OnCursorChange();
					}
					this.SelectionLength = start - end;
				}
				else
				{
					int start = this._SelectStart.Item1;
					int end = this._SelectStart.Item1 + this._SelectStart.Item2;
					bool b = false;

					Tuple<int, int> t = this.GetDoubleClickSelection(this.GetCharPosition(this.LocalMousePosition));
					if (t.Item1 < start)
					{
						start = t.Item1;
						b = true;
					}
					if (t.Item1 + t.Item2 > end)
						end = t.Item1 + t.Item2;
					
					this.CursorPosition = b ? start : end;
					this.SelectionLength = b ? (end - start) : (start - end);
				}
			}
			else
				this._DoubleClicked = false;

			if (this._DoubleClickTime > 0)
				this._DoubleClickTime--;
		}

		public override void OnKeyRelease(Key _key)
		{
			if (_key == Key.LShift)
				this._PressedShift = true;
		}

		public override void OnKeyPress(Key _key)
		{
			if (_key == Key.Left || _key == Key.Right)
			{
				int o = (_key == Key.Left) ? -1 : 1;
				if (Parent.IsKeyPressed(Key.LControl))
					o = (o == 1) ? this.Content.Length - this.CursorPosition : -this.CursorPosition;

				if (!Parent.IsKeyPressed(Key.LShift))
				{
					if ((o > 0 && this.CursorPosition <= this.Content.Length - o) || (o < 0 && this.CursorPosition >= -o))
					{
						this.CursorPosition += o;
						this.SelectionLength = 0;
						this._OnCursorChange();
					}
				}
				else
				{
					if (this._PressedShift)
					{
						this.SelectionLength = 0;
						this._PressedShift = false;
					}
					if ((o > 0 && this.CursorPosition <= this.Content.Length - o) || (o < 0 && this.CursorPosition >= -o))
					{
						this.CursorPosition += o;
						this.SelectionLength -= o;
						this._OnCursorChange();
					}
				}
			}
			else if (_key == Key.Up || _key == Key.Down)
			{
				int o = (_key == Key.Up) ? -1 : 1;
				if ((o == -1 && this.CurrentLine > 0) || (o == 1 && this.CurrentLine < this.Lines.Length - 1))
				{
					int p = this.CursorPosition + this.SelectionLength;
					this.CursorPosition = this.GetCharPosition(this.GetCursorPosition() + new Vector(0, o * this.Font.CharSize));
					if (Parent.IsKeyPressed(Key.LShift))
						this.SelectionLength = p - this.CursorPosition;
					this._OnCursorChange();
				}
			}
			else if (_key == Key.BackSpace)
			{
				if (this._BeforeBackspace())
				{
					if (this.SelectionLength == 0 && this.CursorPosition > 0)
						this.SelectionLength = -1;
					this.DeleteText();
					this._AfterBackspace();
					this.AfterContentChangeByUser();
				}
			}
			else if (_key == Key.Delete)
			{
				if (this.SelectionLength == 0 && this.CursorPosition < this.Content.Length)
					this.SelectionLength = 1;
				this.DeleteText();
				this.AfterContentChangeByUser();
			}
			else if (_key == Key.Enter)
			{
				if (this.MultiLine)
				{
					this.EnterText("\n");
					this.AfterContentChangeByUser();
				}
				else
					this._OnEnter();
			}
			else if (Parent.IsKeyPressed(Key.LControl))
			{
				if ((_key == Key.C || _key == Key.X) && this.SelectionLength != 0)
					this.Parent.SetClipboard(this.Content.Substring(this.GetLeft(), this.GetRight() - this.GetLeft()));

				if (_key == Key.X)
				{
					this.DeleteText();
					this.AfterContentChangeByUser();
				}

				if (_key == Key.V)
				{
					this.EnterText((string)this.Parent.GetClipboard());
					this.AfterContentChangeByUser();
				}

				if (_key == Key.A)
				{
					this.CursorPosition = 0;
					this.SelectionLength = this.Content.Length;
					this._OnCursorChange();
				}
			}
			else if (_key == Key.Tab)
			{
				if (this.TabAllowed)
				{
					this.EnterText("\t");
					this.AfterContentChangeByUser();
				}
				else
					this.OnTab();
			}
		}

		public Vector GetCursorPosition()
		{
			int x = 0;
			int y = 0;
			for (int i = 0; i < this.CursorPosition; i++)
			{
				if (this.Content[i] == '\n')
				{
					x = 0;
					y++;
				}
				else
					x++;
			}

			return this.GetCursorPosition(new Point(x, y));
		}

		public Vector GetCursorPosition(Point _char)
		{
			string l = this.Lines[_char.Y];
			return this.Edge + new Vector(this._Text.GetSize(new string(l.Substring(0, _char.X).Select(item => (item == ' ') ? '_' : item).ToArray())).X, _char.Y * this.Font.CharSize) + (_char.X == 0 ? 0 : new Vector(-2, 0));
		}

		public int GetCharPosition(Vector _pos)
		{
			string c = this.Content;
			
			int y = (int)Meth.Limit(0, Meth.Down((_pos.Y - this.Edge.Y) / this.Font.CharSize), this.Lines.Length - 1);
			string[] lines = c.Split('\n');
			string line = lines[y];

			int pos = 0;
			bool b = false;
			for (int i = 0; i < line.Length; i++)
			{
				this._Text._Text.Content = line.Substring(0, i);
				double first = this._Text.Width;
				this._Text._Text.Content = line.Substring(0, i + 1);
				double second = this._Text.Width;

				if (_pos.X - this.Edge.X < first + (second - first) / 2 - 3)
				{
					pos = i;
					b = true;
					break;
				}
			}

			if (!b)
				pos = line.Length;

			int p = 0;
			for (int i = 0; i < y; i++)
				p += lines[i].Length + 1;
			p += pos;
			this._Text._Text.Content = c;
			return p;
		}

		public void UpdateMouseArea() { this.MouseArea.Shape = new Rectangle(0, this.Size); }

		public void OnClick(Key _key)
		{
			if (_key == Key.MouseLeft)
			{
				this.CursorPosition = this.GetCharPosition(this.LocalMousePosition);
				this.SelectionLength = 0;
				this._OnCursorChange();
				this._SelectStart = new Tuple<int, int>(this.CursorPosition, 0);

				if (this._DoubleClickPos == this.CursorPosition && this._DoubleClickTime > 0)
				{
					Tuple<int, int> t = this.GetDoubleClickSelection(this.CursorPosition);
					this.CursorPosition = t.Item1 + t.Item2;
					this.SelectionLength = -t.Item2;
					this._SelectStart = t;

					this._DoubleClickTime = 0;
					this._DoubleClicked = true;
				}
				else
				{
					this._DoubleClickPos = this.CursorPosition;
					this._DoubleClickTime = 15;
				}
			}
			else if (_key == Key.MouseDown)
				this.Y -= this.CharSize * 3;
			else if (_key == Key.MouseUp)
				this.Y += this.CharSize * 3;
		}

		public Tuple<int, int> GetDoubleClickSelection(int _pos)
		{
			Tuple<int, int> @out = new Tuple<int, int>(_pos, (_pos == this.Content.Length) ? 0 : 1);

			foreach (TextSelectPattern p in this.SelectPatterns)
			{
				Tuple<int, int> t = p.GetRange(this.Content, _pos);
				if (t.Item2 > @out.Item2)
					@out = t;
			}

			return @out;
		}

		public Point GetCharPos(int _pos) { return this.Content.GetCharPos(_pos); }
		
		public double GetHeight(int _lines) { return this.Font.CharSize * _lines + this.Offset.Y * 2; }

		public int[] GetSelectedLines()
		{
			int start = this.GetCharPos(this.GetLeft()).Y;
			int end = this.GetCharPos(this.GetRight()).Y;
			List<int> @out = new List<int>();
			for (int i = start; i <= end; i++)
				@out.Add(i);
			return @out.ToArray();
		}
	}
}