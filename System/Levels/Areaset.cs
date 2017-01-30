using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka;

namespace Mekanik
{
	public class Areaset
	{
		public string Name;
		public Bunch<ImageSource> Tiles = new Bunch<ImageSource>();
		public Bunch<bool[,]> Cols = new Bunch<bool[,]>();
		public Dictionary<string, ImageSource> Decorations = new Dictionary<string, ImageSource>();

		public Areaset(MekaItem _file, Point _tilesize, Point _tilecollisionresolution)
		{
			if (_file["Info"]["Tilesize"].To<Point>() != _tilesize || _file["Info"]["Colsize"].To<Point>() != _tilecollisionresolution)
				throw new Exception("Tilesize and/or Colsize don't match up with this game's.");

			ImageSource tileset = GameBase.LoadImageSource(_file["Tileset"].Data);
			ImageSource[,] tiles = tileset.Split(tileset.Size / _tilesize);
			ImageSource colset = GameBase.LoadImageSource(_file["Colset"].Data);

			int pixelcount = Meth.Up(_tilecollisionresolution.X * _tilecollisionresolution.Y / 4);

			for (int y = 0; y < tiles.GetLength(1); y++)
			{
				for (int x = 0; x < tiles.GetLength(0); x++)
				{
					this.Tiles.Add(tiles[x, y]);

					Bunch<Color> cs = new Bunch<Color>();
					for (int i = 0; i < pixelcount; i++)
						cs.Add(colset[x * pixelcount + i, y]);

					Bunch<byte> bs = new Bunch<byte>();
					foreach (Color c in cs)
						bs.Add(c.Bytes);

					bs = bs.SubBunch(0, _tilecollisionresolution.X * _tilecollisionresolution.Y);
					bool[,] cols = new bool[_tilecollisionresolution.X, _tilecollisionresolution.Y];
					for (int i = 0; i < bs.Count; i++)
						cols[i % _tilecollisionresolution.X, Meth.Down(i / _tilecollisionresolution.X)] = bs[i] == 1;
					this.Cols.Add(cols);
				}
			}
		}

		public Areaset(string _path, Point _tilesize, Point _tilecollisionresolution)
			: this(MekaItem.LoadFromFile(_path + "\\Areaset.meka"), _tilesize, _tilecollisionresolution)
		{
			this.Name = _path.Substring(_path.LastIndexOf('\\') + 1);
			
			if (File.Exists(_path + "\\Decorations"))
			{
				foreach (File f in File.GetAllFiles(_path + "\\Decorations"))
					this.Decorations[f.Name] = GameBase.LoadImageSource(f.Bytes);
			}
		}
	}
}