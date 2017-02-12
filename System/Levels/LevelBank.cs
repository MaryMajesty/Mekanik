using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Meka;

namespace Mekanik
{
	public class LevelBank
	{
		private GameBase _Game;
		private Dictionary<string, LevelSource> _Levels = new Dictionary<string, LevelSource>();
		private Dictionary<string, Bunch<string>> _Regions = new Dictionary<string, Bunch<string>>();
		private Level _PreviousLevel;
		private string _PreviousName;

		public LevelBank(GameBase _game)
		{
			this._Game = _game;
		}

		internal void _CheckRegions()
		{
			foreach (File f in File.GetAllFiles(this._Game.Path + "\\" + this._Game.LevelFolder))
			{
				if (f.Name == "_Region")
					this._Regions[f.Path] = MekaItem.LoadFromFile(f.Path).Children.Select(item => File.GetFolder(f.Path) + "\\" + item.Name).ToBunch();
			}
		}

		internal RegionBase _GetRegion(string _name)
		{
			if (this._Regions.Any(item => item.Value.Contains(_name)))
				return new RegionBase(this._Game, this._Regions.First(item => item.Value.Contains(_name)).Key);
			else
				return new RegionBase(this._Game, this._Game.LevelFolder + "\\" + _name);
		}

		public void LoadLevel(string _name)
		{
			if (!this._Levels.ContainsKey(_name))
			{
				//this._Levels[_name] = new LevelSource(this._Game.Location + this._Game.LevelFolder + "\\" + _name + ".meka", this._Game);
				this._Levels[_name] = null;
				Thread t = new Thread(() =>
					{
						Renderer.EnableMultithreading();
						this._Levels[_name] = new LevelSource(this._Game, this._Game.Path + "\\" + this._Game.LevelFolder + "\\" + _name);
					});
				t.Start();
			}
		}

		public void UnloadLevel(string _name)
		{
			if (this._Levels.ContainsKey(_name))
				this._Levels.Remove(_name);
		}

		public bool IsLevelLoaded(string _name) => this._Levels.ContainsKey(_name) && this._Levels[_name] != null;

		public LevelSource GetLevel(string _name)
		{
			if (this._Levels.ContainsKey(_name))
			{
				while (this._Levels[_name] == null)
					Thread.Sleep(1);
				return this._Levels[_name];
			}
			else
				return this._Levels[_name] = new LevelSource(this._Game, this._Game.Path + "\\" + this._Game.LevelFolder + "\\" + _name + ".meka");
		}

		internal void _LoadSave(Level _level)
		{
			if (!this._Game._TestStarted)
			{
				string n = this._Levels.Where(item => item.Value == _level.Source).ToArray()[0].Key;
				if (this._Game.CurrentSaveFile._Levels.ContainsKey(n))
					_level._SavedEntities = this._Game.CurrentSaveFile._Levels[n];
			}
		}

		internal void _StartLevel(Level _level)
		{
			//if (!this._Game._TestStarted)
			//{
			//	if (this._PreviousLevel != null)
			//	{
			//		this._Game.CurrentSaveFile._SaveLevel(this._PreviousLevel, this._PreviousName);
			//		this._PreviousLevel = null;
			//	}

			//	string n = this._Regions.Where(item => item.Value == _level.Source).ToArray()[0].Key;

			//	Bunch<string> olds = this._Regions.Keys.ToArray() + n;
			//	Bunch<string> news = n;
			//	foreach (LevelConnection l in _level.Children.Where(item => item is LevelConnection).Select(item => (LevelConnection)item))
			//	{
			//		if (l.DestinationLevel != "" && !news.Contains(l.DestinationLevel))
			//			news.Add(l.DestinationLevel);
			//	}

			//	foreach (string l in news)
			//		this.LoadRegion(l);
			//	foreach (string l in olds.Where(item => !news.Contains(item)))
			//		this.UnloadLevel(l);

			//	//this._Game._SaveBank.Load(_level, n);

			//	this._PreviousLevel = _level;
			//	this._PreviousName = n;

			//	this._Game.CurrentSaveFile._PositionLevel = n;
			//}
		}
	}
}