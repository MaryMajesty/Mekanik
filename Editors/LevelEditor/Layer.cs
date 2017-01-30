using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka;
using Meka.ByteOperators;

namespace Mekanik
{
	class Layer
	{
		private Tuple<string, int>[,] _Tiles;
		private Renderer[] _Canvases = new Renderer[2];
		private Dictionary<string, Areaset> Areasets;
		private Point _TileSize;
		private Tuple<string, int> _Default;
		public bool SizeChangedEditor;
		public bool SizeChangedLayer;
		public Bunch<EntityIcon> Entities = new Bunch<EntityIcon>();
		public bool EntitiesChanged;
		//public bool TilesChanged;

		public Point Size
		{
			get
			{
				if (this._Tiles == null)
					return new Point(0, 0);
				else
					return new Point(this._Tiles.GetLength(0), this._Tiles.GetLength(1));
			}

			set
			{
				this.SizeChangedEditor = true;
				this.SizeChangedLayer = true;

				this._Canvases[0] = new Renderer(value * this._TileSize);
				this._Canvases[1] = new Renderer(value * this._TileSize);

				if (this._Tiles == null)
				{
					this._Tiles = new Tuple<string, int>[value.X, value.Y];
					for (int x = 0; x < value.X; x++)
					{
						for (int y = 0; y < value.Y; y++)
						{
							//if (this[x, y] != this._Default)
								this[x, y] = this._Default;
							//else
							//	this._Canvas.Draw(new Image(this._Sprites[this._Default]) { Position = new Vector(x, y) * this._TileSize, BlendMode = BlendMode.None });
						}
					}
				}
				else
				{
					Tuple<string, int>[,] old = (Tuple<string, int>[,])this._Tiles.Clone();
					Point s = this.Size;

					this._Tiles = new Tuple<string, int>[value.X, value.Y];
					for (int x = 0; x < value.X; x++)
					{
						for (int y = 0; y < value.Y; y++)
						{
							if (x < s.X && y < s.Y)
								this[x, y] = old[x, y];
							else
								this[x, y] = this._Default;
						}
					}
				}
			}
		}
		public ImageSource[] Previews
		{
			//get { return new ImageSource[0]; }
			get { return this._Canvases.Select(item => item.ImageSource).ToArray(); }
		}
		public Bunch<Image> EntityPreview
		{
			get { return this.Entities.Where(item => !item.Type.Event).Select(item => new Image(item.Icon.Source) { Position = item.Position, Origin = item.Type.Origin, Z = item.Type.Name == "Decoration" ? 0 : 1 }).OrderBy(item => item.Z); }
		}

		public Layer(Dictionary<string, Areaset> _areasets, Point _tilesize, Tuple<string, int> _default)
		{
			this.Areasets = _areasets;
			this._TileSize = _tilesize;
			this._Default = _default;
		}

		public Tuple<string, int> this[int _x, int _y]
		{
			get { return this._Tiles[_x, _y]; }

			set
			{
				if (_x >= 0 && _x < this.Size.X && _y >= 0 && _y < this.Size.Y && this._Tiles[_x, _y] != value)
				{
					if (this._Tiles[_x, _y] != null)
						this._Canvases[this.Areasets[this._Tiles[_x, _y].Item1].Cols[this._Tiles[_x, _y].Item2].Any(item => item) ? 1 : 0].Draw(new Image(this.Areasets[this._Default.Item1].Tiles[this._Default.Item2]) { Position = new Vector(_x, _y) * this._TileSize, BlendMode = BlendMode.None });
					this._Tiles[_x, _y] = value;
					this._Canvases[this.Areasets[value.Item1].Cols[value.Item2].Any(item => item) ? 1 : 0].Draw(new Image(this.Areasets[value.Item1].Tiles[value.Item2]) { Position = new Vector(_x, _y) * this._TileSize, BlendMode = BlendMode.None });
				}
			}
		}

		public void LoadFromImage(ImageSource _source, Bunch<string> _areasets)
		{
			this.Size = _source.Size;

			for (int x = 0; x < _source.Size.X; x++)
			{
				for (int y = 0; y < _source.Size.Y; y++)
					this[x, y] = new Tuple<string, int>(_areasets[(int)Beth.FromEndian(_source[x, y].Bytes.Sub(0, 2))], (int)Beth.FromEndian(_source[x, y].Bytes.Sub(2, 2)));
			}
		}

		internal MekaItem _Export(Bunch<string> _areasets)
		{
			MekaItem @out = new MekaItem("Layer", new List<MekaItem>());

			ImageSource src = new ImageSource(this.Size.X, this.Size.Y);
			for (int x = 0; x < this.Size.X; x++)
			{
				for (int y = 0; y < this.Size.Y; y++)
				{
					Bunch<byte> bs = Beth.ToEndian(_areasets.IndexOf(this[x, y].Item1), 2);
					bs.Add(Beth.ToEndian(this[x, y].Item2, 2));
					src[x, y] = new Color(bs);
				}
			}
			@out.Children.Add(new MekaItem("Tiles", src.Bytes));

			@out.Children.Add(new MekaItem("Entities", this.Entities.Select(item => item.Export()).ToList()));

			return @out;
		}

		public Bunch<string> GetAreasets()
		{
			Bunch<string> @out = new Bunch<string>();
			foreach (Tuple<string, int> tile in this._Tiles)
			{
				if (!@out.Contains(tile.Item1))
					@out.Add(tile.Item1);
			}
			return @out;
		}
	}
}