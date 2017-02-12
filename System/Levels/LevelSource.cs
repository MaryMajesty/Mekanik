using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Meka;
using Zero;

namespace Mekanik
{
	public class LevelSource : ILoadable
	{
		public List<MekaItem> _Properties;
		public Bunch<LayerSource> Layers = new Bunch<LayerSource>();
		public LayerSource MainLayer;
		public Bunch<ImageSource> LayerImages = new Bunch<ImageSource>();
		public Bunch<Collider> Colliders = new Bunch<Collider>();
		internal GameBase _Game;
		public bool[,] Collisions;
		public string Title;
		public string Author;
		public Point Size;
		public double CollisionTime;
		public Bunch<Entity> Entities = new Bunch<Entity>();
		internal int _MainLayerZ;
		public Script OnLoad;
		public Script OnEnter;
		public Script OnExit;
		private int _CurrentLayer;
		private int _CurrentCollisionLayer;
		private Point _CurrentTile;
		private Renderer _CurrentRenderer;

		public bool FinishedLoading
		{
			get { return this._CurrentLayer == this.Layers.Count; }
		}

		public LevelSource(GameBase _game, MekaItem _item, bool _delayload = false)
		{
			this._Game = _game;

			this.Title = _item["Info"]["Title"].Content;
			this.Author = _item["Info"]["Author"].Content;
			this.Size = new Point(_item["Info"]["Width"].Content.To<int>(), _item["Info"]["Height"].Content.To<int>());
			this.OnLoad = Script.Parse(_item["Info"]["OnLoad"].Content);
			this.OnEnter = Script.Parse(_item["Info"]["OnEnter"].Content);
			this.OnExit = Script.Parse(_item["Info"]["OnExit"].Content);

			this._Properties = _item["Properties"].Children;

			Bunch<string> areasets = _item["Areasets"].Children.Select(item => item.Content).ToBunch();
			
			foreach (MekaItem layer in _item["Layers"].Children)
			{
				LayerSource l = new LayerSource(layer, areasets);
				this.Layers.Add(l);
				if (l.Main)
					this.MainLayer = l;
			}



			int width = this.Size.X * _game.TileCollisionResolution.X;
			int height = this.Size.Y * _game.TileCollisionResolution.Y;

			this.Collisions = new bool[width, height];

			for (int x = 0; x < this.Size.X; x++)
			{
				for (int y = 0; y < this.Size.Y; y++)
				{
					for (int px = 0; px < _game.TileCollisionResolution.X; px++)
					{
						for (int py = 0; py < _game.TileCollisionResolution.Y; py++)
							this.Collisions[x * _game.TileCollisionResolution.X + px, y * _game.TileCollisionResolution.Y + py] = _game.Areasets[this.MainLayer.Tiles[x, y].Item1].Cols[this.MainLayer.Tiles[x, y].Item2][px, py];
					}
				}
			}

			
			
			int z = 0;
			foreach (LayerSource layer in this.Layers)
			{
				if (layer.Main)
					this._MainLayerZ = z;
				z++;
			}
			z = 0;

			if (!_delayload)
			{
				while (!this.FinishedLoading)
					this.LoadStep();
			}
		}

		public void LoadStep()
		{
			LayerSource l = this.Layers[this._CurrentLayer];

			if (this._CurrentRenderer == null)
				this._CurrentRenderer = new Renderer(this.Size * this._Game.TileSize);
			
			if (this._Game.Areasets[l.Tiles[this._CurrentTile.X, this._CurrentTile.Y].Item1].Cols[l.Tiles[this._CurrentTile.X, this._CurrentTile.Y].Item2].Any(item => item) == (this._CurrentCollisionLayer == 1))
				this._CurrentRenderer.Draw(new Image(this._Game.Areasets[l.Tiles[this._CurrentTile.X, this._CurrentTile.Y].Item1].Tiles[l.Tiles[this._CurrentTile.X, this._CurrentTile.Y].Item2]) { BlendMode = BlendMode.None, Position = this._CurrentTile * this._Game.TileSize });

			this._CurrentTile.X++;
			if (this._CurrentTile.X == this.Size.X)
			{
				this._CurrentTile.X = 0;
				this._CurrentTile.Y++;
				if (this._CurrentTile.Y == this.Size.Y)
				{
					this._CurrentTile.Y = 0;
					this._CurrentCollisionLayer++;

					this.LayerImages.Add(this._CurrentRenderer.ImageSource);
					this._CurrentRenderer.DisposeWithoutImage();
					this._CurrentRenderer = null;

					if (this._CurrentCollisionLayer == 2)
					{
						this._CurrentCollisionLayer = 0;
						this._CurrentLayer++;
					}
				}
			}
		}

		public LevelSource(GameBase _game, string _path, bool _delayload = false) : this(_game, MekaItem.LoadFromFile(_path)) { }

		public void Save(string _path)
		{
			Bunch<string> _areasets = new Bunch<string>();

			foreach (LayerSource l in this.Layers)
			{
				foreach (string areaset in l._GetAreasets())
				{
					if (!_areasets.Contains(areaset))
						_areasets.Add(areaset);
				}
			}

			MekaItem file = new MekaItem("File", new List<MekaItem>());

			MekaItem info = new MekaItem("Info", new List<MekaItem>());
			info.Children.Add(new MekaItem("Title", this.Title));
			info.Children.Add(new MekaItem("Author", this.Author));
			info.Children.Add(new MekaItem("Width", this.Size.X.ToString()));
			info.Children.Add(new MekaItem("Height", this.Size.Y.ToString()));
			info.Children.Add(new MekaItem("OnLoad", this.OnLoad.SourceCode));
			info.Children.Add(new MekaItem("OnEnter", this.OnEnter.SourceCode));
			info.Children.Add(new MekaItem("OnExit", this.OnEnter.SourceCode));
			file.Children.Add(info);

			file.Children.Add(new MekaItem("Properties", this._Properties));

			file.Children.Add(new MekaItem("Areasets", _areasets.Select(item => new MekaItem("Areaset", item))));

			MekaItem layers = new MekaItem("Layers", new List<MekaItem>());
			foreach (LayerSource l in this.Layers)
			{
				MekaItem item = l._Export(_areasets);
				if (this.MainLayer == l)
					item.Children.Add(new MekaItem("Main"));
				layers.Children.Add(item);
			}
			file.Children.Add(layers);

			file.SaveToFile(_path);
		}
	}
}