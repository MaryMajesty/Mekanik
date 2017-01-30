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
	public class LevelSource
	{
		public List<MekaItem> _Properties;
		//public object Info;
		public Bunch<LayerSource> Layers = new Bunch<LayerSource>();
		public LayerSource MainLayer;
		public Bunch<ImageSource> LayerImages = new Bunch<ImageSource>();
		public Bunch<Collider> Colliders = new Bunch<Collider>();
		internal GameBase _Game;
		public bool[,] Collisions;
		public string Title;
		public string Author;
		public Point Size;
		//public double RenderTime;
		public double CollisionTime;
		public Bunch<Entity> Entities = new Bunch<Entity>();
		internal int _MainLayerZ;
		public Script OnLoad;
		public Script OnEnter;
		public Script OnExit;

		private bool _Loaded;
		public bool Loaded
		{
			get { return this._Loaded; }
		}
		
		public LevelSource(MekaItem _item, GameBase _game)
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
		}

		public LevelSource(string _path, GameBase _game) : this(MekaItem.LoadFromFile(_path), _game) { }

		//public void StepLoad()
		//{

		//}
	}
}