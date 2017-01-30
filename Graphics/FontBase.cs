using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Mekanik
{
	public abstract class FontBase
	{
		public readonly string Name;

		private double _CharSize;
		public double CharSize
		{
			get { return this._CharSize; }

			set
			{
				this._ChangeCharSize(value);
				this._CharSize = value;
			}
		}

		protected FontBase() { }

		protected FontBase(string _name, double _charsize)
		{
			this.Name = _name;
			this._CharSize = _charsize;
		}

		protected virtual void _ChangeCharSize(double _charsize)
		{
			throw new Exception("not implemented yet");
		}

		public virtual ImageSource GetImage(string _content, Color _color, int _tablength = 4, string _symboltab = null, char? _symbolspace = null)
		{
			throw new Exception("not implemented yet");
		}

		public virtual Point GetSize(string _content, int _tablength = 4)
		{
			throw new Exception("not implemented yet");
		}

		public static FontBase Consolas
		{
			get { return GameBase.LoadFont("Consolas", 14); }
		}
	}

	//public partial class Font
	//{
	//	private static Dictionary<string, string> _SystemFonts = new Dictionary<string, string>();
	//	private static bool _Loaded;
	//	private static DateTime _LastUpdate;

	//	public static Font ComicSans = new Font(Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\comic.ttf");
	//	//public static Font Consolas = new Font(Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\consola.ttf");
	//	//public static Font ComicSans
	//	//{
	//	//	get { return new Font(Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\comic.ttf"); }
	//	//}

	//	public static Font Consolas
	//	{
	//		get { return new Font("Consolas", 14); }
	//	}

	//	public static void LoadSystemFonts(LoadingCircle _circle)
	//	{
	//		Font._LastUpdate = DateTime.Now;

	//		if (!Font._Loaded)
	//		{
	//			Font._Loaded = true;

	//			double p = 0;
	//			Thread d = new Thread(new ParameterizedThreadStart(_ =>
	//				{
	//					Bunch<File> fs = File.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Fonts)).Where(item => item.Extension == "ttf");
	//					for (int i = 0; i < fs.Count; i++)
	//					{
	//						File f = fs[i];

	//						System.Drawing.Text.PrivateFontCollection c = new System.Drawing.Text.PrivateFontCollection();
	//						c.AddFontFile(f.Path);
	//						if (!Font._SystemFonts.ContainsKey(c.Families[0].Name))
	//							Font._SystemFonts[c.Families[0].Name] = f.Path;
	//						p = i / (double)fs.LastIndex;
	//					}
	//				}));

	//			d.Start();
	//			while (true)
	//			{
	//				if (_circle != null)
	//				{
	//					_circle.Progress = p;
	//					if (p < 1)
	//						_circle.Parent.DoFrame();
	//					if (_circle.Progress == 1)
	//						break;
	//				}
	//				else
	//				{
	//					if (p == 1)
	//						break;
	//				}
	//			}
	//			//if (_game != null)
	//			//	_circle.Progress = 1;

	//			//Bunch<File> fs = File.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Fonts)).Where(item => item.Extension == "ttf");
	//			//for (int i = 0; i < fs.Count; i++)
	//			//{
	//			//	File f = fs[i];

	//			//	System.Drawing.Text.PrivateFontCollection c = new System.Drawing.Text.PrivateFontCollection();
	//			//	c.AddFontFile(f.Path);
	//			//	if (!Font._SystemFonts.ContainsKey(c.Families[0].Name))
	//			//		Font._SystemFonts[c.Families[0].Name] = f.Path;

	//			//	//if (i == fs.LastIndex || (DateTime.Now - Font._LastUpdate).TotalMilliseconds > 1000 / 60.0)
	//			//	//{
	//			//	//	Font._LastUpdate = DateTime.Now;
	//			//	//	_onupdate(i / (double)fs.LastIndex);
	//			//	//}
	//			//}
	//		}
	//	}
	//	public static void LoadSystemFonts() { Font.LoadSystemFonts(null); }

	//	public static Font FromSystem(string _name)
	//	{
	//		if (!Font._Loaded)
	//			LoadSystemFonts();
	//		return new Font(Font._SystemFonts[_name]);
	//	}

	//	//public static void ShowSystemFonts()
	//	//{
	//	//	if (!Font._Loaded)
	//	//		LoadSystemFonts();
	//	//	System.Windows.Forms.MessageBox.Show(string.Join(", ", Font._SystemFonts.Select(item => item.Key)));
	//	//}
	//}
}