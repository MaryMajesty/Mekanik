using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	class EntitySelector : Entity
	{
		public Bunch<EntityType> Entities;
		public Point Size = new Point(12, 4);
		public Bunch<CollapseGroup> Groups = new Bunch<CollapseGroup>();
		public EntityEditor EntityEditor;

		public LevelEditor Editor
		{
			get { return (LevelEditor)this.Parents[2]; }
		}

		public EntitySelector(Bunch<EntityType> _entities, LevelEditor _editor)
		{
			this.Entities = _entities;
			_editor.EntityEditor = this.EntityEditor = new EntityEditor();
		}

		public override void OnInitialization()
		{
			this.Children.Add(this.EntityEditor);

			Dictionary<string, Bunch<EntityTile>> groups = new Dictionary<string, Bunch<EntityTile>>();
			foreach (EntityType t in this.Entities.Where(item => item.Name != "Decoration"))
			{
				if (!groups.ContainsKey(t.Group))
					groups[t.Group] = new Bunch<EntityTile>();
				groups[t.Group].Add(new EntityTileNormal(t) { Types = this.Entities });
			}

			foreach (Areaset a in this.Parent.Areasets.Select(item => item.Value).Where(item => item.Decorations.Count > 0))
			{
				groups["Decoration " + a.Name] = new Bunch<EntityTile>();
				foreach (KeyValuePair<string, ImageSource> decoration in a.Decorations)
					groups["Decoration " + a.Name].Add(new EntityTileDecoration(a, decoration.Key) { Types = this.Entities });
			}

			foreach (KeyValuePair<string, Bunch<EntityTile>> g in groups.OrderBy(item => item.Key.GetAlphabetIndex()))
			{
				int height = Meth.Up(g.Value.Count / (double)this.Size.X);
				CollapseGroup c = new CollapseGroup(FontBase.Consolas, g.Key, new EntityGroup(g.Value, this.Editor.TileEditor, this.EntityEditor, new Point(this.Size.X, height))) { InnerHeight = height * this.Parent.TileSize.Y * 2, Width = this.Size.X * this.Parent.TileSize.X * 2 };
				this.Groups.Add(c);
				this.Children.Add(c);
			}
		}

		public override void Update()
		{
			double y = 0;
			foreach (CollapseGroup g in this.Groups)
			{
				g.Y = y;
				y += g.Height;
			}

			this.EntityEditor.Y = y;
		}
	}
}