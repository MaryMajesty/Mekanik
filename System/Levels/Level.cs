using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Meka;

namespace Mekanik
{
	public class Level : Entity
	{
		public LevelSource Source;
		public Bunch<Image> Layers = new Bunch<Image>();
		internal Bunch<SavedEntity> _SavedEntities;
		internal object _Properties;
		internal RegionBase _RegionBase;

		private Dictionary<string, Entity> _Entrances = new Dictionary<string, Entity>();

		public Point Size
		{
			get { return this.Source.Size; }
		}
		public int Width
		{
			get { return this.Size.X; }
		}
		public int Height
		{
			get { return this.Size.Y; }
		}

		public Level(LevelSource _source)
		{
			this.Physical = true;
			this.Source = _source;
		}

		public Level(GameBase _game, string _path) : this(new LevelSource(_game, _path)) { }

		public void SpawnPlayer(Entity _player, string _entrance) => _player.Position = this.Position + this._Entrances[_entrance].Position;

		public override void OnInitialization()
		{
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

					if (this._SavedEntities != null)
					{
						Bunch<SavedEntity> ss = this._SavedEntities.Where(item => item.Identifier == e.Identifier);
						if (ss.Count > 0)
							ss[0].Apply(e);
					}

					this.Children.Add(e);

					if (e is LevelConnection)
						this._Entrances[e.Identifier] = e;
				}

				z++;
			}
			
			for (int i = 0; i < this.Source.LayerImages.Count; i++)
			{
				Image img = new Image(this.Source.LayerImages[i]) { Z = i * 0.5 - this.Source._MainLayerZ };
				this.Graphics.Add(img);
				this.Layers.Add(img);
			}
			
			this.Static = true;
		}
	}
}