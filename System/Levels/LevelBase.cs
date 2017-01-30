using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Meka;

namespace Mekanik
{
	public class LevelBase : Entity
	{
		public LevelSource Source;
		public Bunch<Type> Entities = new Bunch<Type>();
		public int ColliderCount;
		//private Bunch<bool> _Cols = new Bunch<bool>();
		public Bunch<Image> Layers = new Bunch<Image>();
		public double LevelFriction;
		//public Liquid Liquid;
		
		internal Bunch<SavedEntity> _SavedEntities;
		internal object _Properties;

		public Point Size
		{
			get { return this.Source.Size; }
		}
		//internal double _MainLayerZ;
		//public double MainLayerZ
		//{
		//	get { return this._MainLayerZ; }
		//}

		public LevelBase(LevelSource _source, double _friction)
		{
			this.Physical = true;
			this.Source = _source;
			this.LevelFriction = _friction;
		}

		public LevelBase(string _path, GameBase _game, double _friction) : this(new LevelSource(_path, _game), _friction) { }

		public override void OnInitialization()
		{
			//this.Entities = this.Parent.EntityTypes;

			//for (int i = 0; i < this.Source.Layers.Count; i++)
			//{
			//	LayerSource l = this.Source.Layers[i];

			//	for (int n = 0; n <= 1; n++)
			//	{
			//		Renderer r = new Renderer(this.Size * this.Source._Game.Tilesize);

			//		for (int x = 0; x < this.Size.X; x++)
			//		{
			//			for (int y = 0; y < this.Size.Y; y++)
			//			{
			//				if (this.Source._Game.Areasets[l.Tiles[x, y].Item1].Cols[l.Tiles[x, y].Item2].Any(item => item) == (n == 1))
			//					r.Draw(new Image(this.Source._Game.Areasets[l.Tiles[x, y].Item1].Tiles[l.Tiles[x, y].Item2]) { BlendMode = BlendMode.None, Position = new Vector(x, y) * this.Source._Game.Tilesize });
			//			}
			//		}

			//		Image img = new Image(r.ImageSource) { Z = i + n * 0.6 };
			//		this.Layers.Add(img);
			//		this.Graphics.Add(img);
			//	}
			//}

			int z = 0;

			foreach (LayerSource layer in this.Source.Layers)
			{
				foreach (EntityInstance i in layer.Entities)
				{
					Type t = this.Parent.EntityTypes.First(item => item.Name == i.Type);
					Entity e = (Entity)t.GetConstructor(new Type[0]).Invoke(new object[0]);

					foreach (KeyValuePair<string, string> property in i.Properties)
					{
						FieldInfo f = t.GetField(property.Key);
						f.SetValue(e, property.Value.To(f.FieldType));
					}

					e.Position = new Vector(i.X, i.Y);
					e.Z = z + 0.3 + Meth.InfToOne(i.Z) * 0.1 - this.Source._MainLayerZ;

					//if (this._SavedEntities != null)
					//{
					//	Bunch<SavedEntity> ss = this._SavedEntities.Where(item => item.Identifier == e.Identifier);
					//	if (ss.Count > 0)
					//		ss[0].Apply(e);
					//}

					this.Children.Add(e);
				}

				z++;
			}

			Bunch<ImageSource> layers = new Bunch<ImageSource>();

			foreach (LayerSource l in this.Source.Layers)
			{
				//if (l.Main)
				//	this.MainLayer = l;
				//this.Layers.Add(l);

				for (int n = 0; n <= 1; n++)
				{
					Renderer r = new Renderer(this.Size * this.Parent.Tilesize);

					Bunch<Graphic> gs = new Bunch<Graphic>();
					for (int x = 0; x < this.Size.X; x++)
					{
						for (int y = 0; y < this.Size.Y; y++)
						{
							if (this.Parent.Areasets[l.Tiles[x, y].Item1].Cols[l.Tiles[x, y].Item2].Any(item => item) == (n == 1))
								gs.Add(new Image(this.Parent.Areasets[l.Tiles[x, y].Item1].Tiles[l.Tiles[x, y].Item2]) { BlendMode = BlendMode.None, Position = new Vector(x, y) * this.Parent.Tilesize });
						}
					}

					r.Draw(gs);
					layers.Add(r.ImageSource);
				}
			}

			for (int i = 0; i < layers.Count; i++)
			{
				Image img = new Image(layers[i]) { Z = i * 0.5 - this.Source._MainLayerZ };
				this.Graphics.Add(img);
				this.Layers.Add(img);
			}





			//this.Liquid = new Liquid(this.Source.Collisions) { Z = 1000 };
			//this.Liquid.Liquids[5, 8] = 32;
			//this.Liquid.Liquids[6, 8] = 32;
			//this.Liquid.Liquids[7, 8] = 32;
			//this.Liquid.Liquids[8, 8] = 32;
			//this.Liquid.Liquids[9, 8] = 32;
			//this.Liquid.Liquids[10, 8] = 32;
			//this.Liquid.Liquids[11, 8] = 32;
			//this.Liquid.Liquids[12, 8] = 32;
			//this.Children.Add(this.Liquid);



			this.Static = true;

			foreach (Collider c in this.Source.Colliders)
			{
				this.AddCollider(c);
				//this.Graphics.Add(c.Shape);
			}

			this.Friction = this.LevelFriction;
		}
	}
}