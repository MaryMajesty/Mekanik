using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Mekanik
{
	public class RegionBank
	{
		private GameBase _Game;
		private Dictionary<string, RegionBase> _Regions = new Dictionary<string, RegionBase>();
		private LevelBase _PreviousLevel;
		private string _PreviousName;

		public RegionBank(GameBase _game)
		{
			this._Game = _game;
		}

		public void LoadRegion(string _name)
		{
			if (!this._Regions.ContainsKey(_name))
			{
				//this._Levels[_name] = new LevelSource(this._Game.Location + this._Game.LevelFolder + "\\" + _name + ".meka", this._Game);
				this._Regions[_name] = null;
				Thread t = new Thread(() =>
					{
						Renderer.EnableMultithreading();
						this._Regions[_name] = new RegionBase(this._Game, this._Game.Path + "\\" + this._Game.LevelFolder + "\\" + _name);
					});
				t.Start();
			}
		}

		public void UnloadLevel(string _name)
		{
			if (this._Regions.ContainsKey(_name))
			{
				Thread t = new Thread(() =>
					{
						while (!this.IsLevelLoaded(_name))
							Thread.Sleep(1);
						this._Regions.Remove(_name);
					});
				t.Start();
			}
		}

		public bool IsLevelLoaded(string _name) => this._Regions.ContainsKey(_name) && this._Regions[_name] != null;

		public RegionBase GetRegion(string _name)
		{
			if (this._Regions.ContainsKey(_name))
			{
				while (this._Regions[_name] == null)
					Thread.Sleep(1);
				return this._Regions[_name];
			}
			else
				return this._Regions[_name] = new RegionBase(this._Game, this._Game.Path + "\\" + this._Game.LevelFolder + "\\" + _name);
		}

		internal void _LoadSave(LevelBase _level)
		{
			//if (!this._Game._TestStarted)
			//{
			//	string n = this._Regions.Where(item => item.Value == _level.Source).ToArray()[0].Key;
			//	if (this._Game.CurrentSaveFile._Levels.ContainsKey(n))
			//		_level._SavedEntities = this._Game.CurrentSaveFile._Levels[n];
			//}
		}

		internal void _StartLevel(LevelBase _level)
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