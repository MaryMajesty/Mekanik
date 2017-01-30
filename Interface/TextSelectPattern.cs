using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public abstract class TextSelectPattern
	{
		public abstract Tuple<int, int> GetRange(string _content, int _pos);

		public class Word : TextSelectPattern
		{
			public string Chars;

			public Word(string _chars) { this.Chars = _chars; }

			public override Tuple<int, int> GetRange(string _content, int _pos)
			{
				int l = 0;

				for (int i = Meth.Min(_pos, _content.Length - 1); i >= 0; i--)
				{
					if (this.Chars.Contains(_content[i]))
						_pos = i;
					else
						break;
				}
				
				for (int i = _pos; i < _content.Length; i++)
				{
					if (i == _content.Length)
						l++;
					else
					{
						if (this.Chars.Contains(_content[i]))
							l = i - _pos + 1;
						else
							break;
					}
				}

				return new Tuple<int, int>(_pos, l);
			}
		}

		public class Keywords : TextSelectPattern
		{
			public Bunch<string> Words;

			public Keywords(params string[] _words) { this.Words = _words; }

			public override Tuple<int, int> GetRange(string _content, int _pos)
			{
				Tuple<int, int> @out = new Tuple<int, int>(_pos, 0);

				for (int i = _pos; i >= Meth.Max(0, _pos - this.Words.Max(item => item.Length)); i--)
				{
					foreach (string word in this.Words)
					{
						if (i + word.Length > _pos && _content.Length - i >= word.Length && _content.Substring(i, word.Length) == word && word.Length > @out.Item2)
							@out = new Tuple<int, int>(i, word.Length);
					}
				}

				return @out;
			}
		}
	}
}