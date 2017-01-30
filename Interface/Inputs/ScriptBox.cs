using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zero;

namespace Mekanik
{
	public class ScriptBox : TextBox
	{
		static Color ColorString = new Color(255, 170, 85);
		static Color ColorChar = new Color(255, 85, 0);
		static Color ColorNumber = new Color(0, 170, 85);
		static Color ColorNoScopeOperator = new Color(85, 170, 255);
		static Color ColorScopeOperator = new Color(255, 85, 255);
		static Color ColorOneLineScopeOperator = new Color(255, 170, 255);
		static Color ColorObjectOperator = new Color(255, 255, 85);
		static Color ColorFlowOperator = new Color(255, 0, 0);
		static Color ColorComment = new Color(85, 170, 85);
		static Color ColorScript = new Color(255, 0, 85);

		public Script Script
		{
			get { return Script.Parse(this.Content); }
		}

		public ScriptBox(Script _script)
			: base(_script.ToString())
		{
			this.TabAllowed = true;
			this.MultiLine = true;
			
			this.TextColor = new Color(220);
			this.SelectColor = new Color(220);
			this.Background.Color = new Color(30);

			this.ColorTheme = new ColorTheme
				(
					new ColorPattern("String", ColorString, true, true, "\"", "\""),
					new ColorPattern("Char", ColorString, true, true, "\'", "\'"),
					new ColorPattern("Comment", ColorComment, true, true, "#", "#"),
					new ColorPattern("Script", ColorScript, true, true, "@\"", "\""),

					new ColorPattern("NoScopeOperator", ColorNoScopeOperator, Helper.NoScopeOperatorSymbols),
					new ColorPattern("ScopeOperator", ColorScopeOperator, Helper.ScopeOperatorSymbols),
					new ColorPattern("OneLineScopeOperator", ColorOneLineScopeOperator, Helper.OneLineScopeOperators),
					new ColorPattern("ObjectOperator", ColorObjectOperator, Helper.ObjectOperators),
					new ColorPattern("FlowOperator", ColorFlowOperator, Helper.FlowOperators),

					new ColorPattern("Number", ColorNumber, text =>
						{
							int @out = 0;
							for (int i = 0; i < text.Length; i++)
							{
								if (Ext.IsDouble(text.Substring(0, i + 1)) || (i + 1 < text.Length && Ext.IsDouble(text.Substring(0, i)) && Ext.IsDouble(text.Substring(0, i + 2))))
									@out++;
								else
									return @out;
							}
							return @out;
						}),
					new ColorPattern("Variable", new Color(220), text =>
						{
							string alpha = "abcdefghijklmnopqrstuvwxyz_";

							if (!alpha.Contains(text[0].ToString().ToLower()[0]))
								return 0;
							else
							{
								alpha += "0123456789";

								int @out = 1;
								for (int i = 1; i < text.Length; i++)
								{
									if (alpha.Contains(text[i].ToString().ToLower()[0]))
										@out++;
									else
										return @out;
								}
								return @out;
							}
						})
				);

			this.SelectPatterns.Add(new TextSelectPattern.Keywords(Helper.Symbols));

			this.AfterInput = input =>
				{
					if (input == "\n")
					{
						int l = this.GetCharPos(this.CursorPosition).Y - 1;
						string line = this.Lines[l];

						int tabs = Helper.GetNextIndents(line);

						for (int i = 0; i < tabs; i++)
							this.EnterText("\t");
					}
				};

			this.BeforeInput = input =>
				{
					if (input == "\t")
					{
						bool shift = this.Parent.IsKeyPressed(Key.LShift);
						if (shift || (!shift && this.GetSelectedLines().Length > 1))
						{
							int[] sl = this.GetSelectedLines();
							string tx = "";
							int tabs = 0;

							bool firsttab = !shift;

							for (int i = 0; i < sl[0]; i++)
								tx += this.Lines[i] + "\n";

							foreach (int i in this.GetSelectedLines())
							{
								string line = this.Lines[i];
								if (shift)
								{
									if (line.StartsWith("\t"))
									{
										tx += line.Substring(1) + "\n";
										tabs--;

										if (i == this.GetCharPos(this.GetLeft()).Y)
											firsttab = true;
									}
									else
										tx += (line.StartsWith("\t") ? line.Substring(1) : line) + "\n";
								}
								else
								{
									tx += "\t" + line + "\n";
									tabs++;
								}
							}

							for (int i = sl[sl.Length - 1] + 1; i < this.Lines.Length; i++)
								tx += this.Lines[i] + "\n";

							int c = this.CursorPosition;

							tx = tx.Substring(0, tx.Length - 1);
							this.Content = tx;

							if (tabs != 0)
							{
								if (this.SelectionLength > 0)
								{
									if (firsttab)
										this.CursorPosition = c + Meth.Sign(tabs);
									this.SelectionLength += tabs - (firsttab ? Meth.Sign(tabs) : 0);
								}
								else if (this.SelectionLength < 0)
								{
									this.CursorPosition = c + tabs;
									this.SelectionLength -= tabs - (firsttab ? Meth.Sign(tabs) : 0);
								}
								else
									this.CursorPosition = c - 1;
							}

							return false;
						}
					}
					return true;
				};

			this.BeforeBackspace = () =>
				{
					if (this.SelectionLength > 0 || this.CursorPosition == 0)
						return true;
					else
					{
						Point p = this.GetCharPos(this.CursorPosition);
						if (p.Y > 0)
						{
							int x = p.X;
							string l = this.Lines[this.CurrentLine];

							string st = l.Substring(0, x);

							if (!st.Any(item => item != '\t'))
							{
								int tabs = st.Count(item => item == '\t');
								int c = this.CursorPosition;
								this.Content = this.Content.Substring(0, this.CursorPosition - tabs - 1) + this.Content.Substring(this.CursorPosition);
								this.CursorPosition = c - tabs - 1;
								return false;
							}
							else
								return true;
						}
						else
							return true;
					}
				};

			//this.OnDefocus = this._TryParse;
		}

		//private void _TryParse()
		//{
		//	//Script s = null;
		//	//Error e = Script.TryParse(this.Content, out s);
		//}

		public override object GetValue()
		{
			return Script.Store(this.Content);
			//return Script.Parse(this.Content);
		}
	}
}