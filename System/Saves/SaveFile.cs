using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Meka;

namespace Mekanik
{
	public class SaveFile
	{
		internal GameBase _Game;
		internal int _Id;
		public readonly Dictionary<string, MekaItem> Achievements = new Dictionary<string, MekaItem>();
		internal Dictionary<string, Bunch<SavedEntity>> _Levels = new Dictionary<string, Bunch<SavedEntity>>();
		public DateTime LastPlayed;
		public int PlayTime;
		public Bunch<MekaItem> CustomInfo = new Bunch<MekaItem>();

		internal string _PositionLevel;
		public string PositionLevel
		{
			get { return this._PositionLevel; }
		}
		internal string _PositionEntrance;
		public string PositionEntrance
		{
			get { return this._PositionEntrance; }
		}

		internal SaveFile(GameBase _game)
		{
			this._Game = _game;

			foreach (Achievement a in _game._Achievements)
				this.Achievements[a.Name] = a._DefaultValue;
		}

		internal SaveFile(GameBase _game, string _path)
			: this(_game)
		{
			MekaItem file = MekaItem.LoadFromFile(_path);

			this.PlayTime = file["Play Time"].Content.To<int>();
			this.LastPlayed = file["Last Played"].Content.To<DateTime>();

			this.CustomInfo = file["Custom Info"];

			foreach (MekaItem level in file["Levels"].Children)
			{
				this._Levels[level.Name] = new Bunch<SavedEntity>();
				foreach (MekaItem entity in level.Children)
					this._Levels[level.Name].Add(new SavedEntity(entity));
			}

			foreach (MekaItem achievement in file["Achievements"].Children)
				this.Achievements[achievement.Name] = achievement.Children[0];

			string n = File.GetName(_path);
			n = n.Substring(n.IndexOf('#') + 1);
			this._Id = n.To<int>();
		}

		internal void _SaveLevel(LevelBase _level, string _name)
		{
			Bunch<SavedEntity> ss = new Bunch<SavedEntity>();

			foreach (Entity entity in _level.Children.Where(item => item.Identifier != "[none]"))
			{
				Bunch<FieldInfo> fs = entity.GetType().GetFields().Where(item => item.GetCustomAttributes<SavableAttribute>().ToArray().Length > 0).ToArray();
				if (fs.Count > 0)
					ss.Add(new SavedEntity(fs, entity));
			}
			
			this._Levels[_name] = ss;
		}

		internal void _Save()
		{
			string path = this._Game.Path + "\\Internal\\Saves\\Save File #" + this._Id.ToString() + ".meka";

			MekaItem file = new MekaItem("Save File #" + this._Id.ToString(), new Bunch<MekaItem>());

			file.Children.Add(new MekaItem("Play Time", this.PlayTime.ToString()));
			file.Children.Add(new MekaItem("Last Played", DateTime.Now.ToString()));
			file.Children.Add(new MekaItem("Custom Info", this.CustomInfo));

			MekaItem levels = new MekaItem("Levels", new List<MekaItem>());
			foreach (KeyValuePair<string, Bunch<SavedEntity>> level in this._Levels)
			{
				MekaItem l = new MekaItem(level.Key, new List<MekaItem>());
				foreach (SavedEntity e in level.Value)
					l.Children.Add(new MekaItem(e.Identifier, e.Properties));
				levels.Children.Add(l);
			}
			file.Children.Add(levels);

			MekaItem achievements = new MekaItem("Achievements", new List<MekaItem>());
			foreach (KeyValuePair<string, MekaItem> achievement in this.Achievements)
				achievements.Children.Add(new MekaItem(achievement.Key, new List<MekaItem>() { achievement.Value }));
			file.Children.Add(achievements);

			file.SaveToFile(path);
		}
	}
}