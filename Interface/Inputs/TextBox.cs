using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class TextBox : TextEditor
	{
		public Action OnFocus;
		public Action OnDefocus;
		public VertexArray Selection;
		public VertexArray Cursor;
		public int BlinkFrames;
		public RectangleRounded Background;

		public Color SelectColor
		{
			get { return this.Cursor.Color; }

			set
			{
				this.Cursor.Color = value;
				this.Selection.Color = value ^ 85;
			}
		}

		public TextBox(string _value)
			: base(_value)
		{
			this.Graphics.Add(this.Background = new RectangleRounded(0, this.Size, 5) { Z = -1, Color = Color.White * 0.9, Quality = 2 });
			this.Graphics.Add(this.Selection = new VertexArray(VertexArrayType.Quads) { Color = Color.Black ^ 85 });

			this.Graphics.Add(this.Cursor = new VertexArray(VertexArrayType.Lines) { Vertices = new Bunch<Vertex>(new Vector(0), new Vector(0, this._Text.Font.CharSize)), Color = Color.Black ^ 170 });
		}

		protected override void _OnEnter()
		{
			this.OnEnter();
			this.LoseFocus();
		}

		public sealed override void OnFocusGain()
		{
			this.BlinkFrames = 0;
			if (this.OnFocus != null)
				this.OnFocus();
		}

		public sealed override void OnFocusLoss()
		{
			if (this.OnDefocus != null)
				this.OnDefocus();
			this.SelectionLength = 0;
		}

		protected override void _OnCursorChange() => this.BlinkFrames = 0;

		//protected override void _AfterInput(string _input)
		//{
		//	this.BlinkFrames = 0;
		//	this.AfterInput(_input);
		//}

		protected override void _OnContentChange(string _old, string _new)
		{
			base._OnContentChange(_old, _new);
			this.BlinkFrames = 0;

			if (this.Background != null)
				this.Background.Size = this.Size;
		}

		public override void Update()
		{
			base.Update();

			Action<Vector, Vector> add = (v1, v2) =>
				{
					this.Selection.Add(v1 + new Vector(0, 1));
					this.Selection.Add(v2 + new Vector(0, 1));
					//this.Selection.Add(v2 + new Vector(0, this.Font.GetLineSpacing(this.CharSize) + 1));
					//this.Selection.Add(v1 + new Vector(0, this.Font.GetLineSpacing(this.CharSize) + 1));
					this.Selection.Add(v2 + new Vector(0, this.Font.CharSize + 1));
					this.Selection.Add(v1 + new Vector(0, this.Font.CharSize + 1));
				};

			this.Selection.Vertices.Clear();
			Point start = this.GetCharPos(this.GetLeft());
			Point end = this.GetCharPos(this.GetRight());
			if (start.Y == end.Y)
				add(GetCursorPosition(start), GetCursorPosition(end));
			else
			{
				add(GetCursorPosition(start), GetCursorPosition(new Point(Lines[start.Y].Length, start.Y)));
				for (int i = start.Y + 1; i < end.Y; i++)
					add(GetCursorPosition(new Point(0, i)), GetCursorPosition(new Point(Lines[i].Length, i)));
				add(GetCursorPosition(new Point(0, end.Y)), GetCursorPosition(end));
			}

			this.Cursor.Position = this.GetCursorPosition();
			this.Cursor.Visible = this.BlinkFrames < 25 && this.IsFocused;

			this.BlinkFrames = (this.BlinkFrames + 1) % 50;
		}
	}
}