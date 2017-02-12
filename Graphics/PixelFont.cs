using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka;

namespace Mekanik
{
	public class PixelFont : FontBase
	{
		public Point CharSize;
		public Dictionary<char, ImageSource> Chars = new Dictionary<char, ImageSource>();
		private char? _DefaultChar;
		private Renderer _Renderer;

		public PixelFont(string _path)
		{
			MekaItem file = MekaItem.LoadFromFile(_path);

			MekaItem chars = file["Characters"].Children[0];

			ImageSource src = new ImageSource(chars.Data);
			ImageSource[,] cs = src.Split(chars["Count"].To<Point>());

			foreach (ImageSource c in cs)
			{
				for (int x = 0; x < c.Width; x++)
				{
					for (int y = 0; y < c.Height; y++)
					{
						if (c[x, y].A > 0 && c[x, y] != Color.White)
							c[x, y] = Color.White;
					}
				}
			}

			{
				int y = 0;
				foreach (MekaItem line in file["Characters"].Children.Where(item => item.Name == "Line"))
				{
					for (int x = 0; x < line.Content.Length; x++)
						this.Chars[line.Content[x]] = cs[x, y];
					y++;
				}
			}

			if (file.Contains("Character Size"))
				this.CharSize = file["Character Size"].To<Point>();
			else
				this.CharSize = this.Chars.ToArray()[0].Value.Size;

			if (file.Contains("Default"))
				this._DefaultChar = file["Default"].Content[0];
		}

		private string[] _SplitText(string _content, int _tablength, string _symboltab, char? _symbolspace)
		{
			string[] lines = _content.Split('\n');

			string t = "";
			if (_symboltab != null)
				t += _symboltab;
			while (t.Length < _tablength)
				t += " ";

			for (int i = 0; i < lines.Length; i++)
			{
				string n = "";
				foreach (char c in lines[i])
				{
					if (c == '\t')
						n += t;
					else if (c == ' ' && _symbolspace.HasValue)
						n += _symbolspace.Value;
					else if (this.Chars.ContainsKey(c))
						n += c;
					else if (c != ' ' && this._DefaultChar.HasValue)
						n += this._DefaultChar;
					else
						n += " ";
				}
				lines[i] = n;
			}

			return lines;
		}

		public override ImageSource GetImage(string _content, Color _color, int _tablength = 4, string _symboltab = null, char? _symbolspace = default(char?))
		{
			Point size = this.GetSize(_content);

			if (this._Renderer == null)
				this._Renderer = new Renderer(size);
			else if (this._Renderer.Resolution != size)
				this._Renderer.Resolution = size;
			
			this._Renderer.Clear();

			Bunch<string> lines = this._SplitText(_content, _tablength, _symboltab, _symbolspace);

			for (int y = 0; y < lines.Count; y++)
			{
				for (int x = 0; x < lines[y].Length; x++)
				{
					if (lines[y][x] != ' ')
						this._Renderer.Draw(new Image(this.Chars[lines[y][x]]) { Position = new Vector(x, y) * this.CharSize, Color = _color, BlendMode = BlendMode.None });
				}
			}

			return this._Renderer.ImageSource.Clone();
		}

		public override Point GetSize(string _content, int _tablength = 4)
		{
			string[] lines = this._SplitText(_content, _tablength, null, null);
			return new Point(lines.Max(item => item.Length), lines.Length) * this.CharSize;
		}
	}
}