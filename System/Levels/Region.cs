using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Region<T> : Entity
	{
		private RegionBase _RegionBase;
		
		public Entity Player;
		public string PlayerLevel;
		public string PlayerEntrance;
		public Level CurrentLevel
		{
			get { return this._RegionBase.CurrentLevel; }
		}

		public Region(RegionBase _base, Action<Level, T> _onlevelload = null, Action<Level, T> _onlevelenter = null, Action<Level, T> _onlevelexit = null, Action<Level, T> _onlevelupdate = null)
		{
			this._RegionBase = _base;

			if (_onlevelload != null)
				this._RegionBase._OnLevelLoad = b => _onlevelload(b, (T)b._Properties);
			if (_onlevelenter != null)
				this._RegionBase._OnLevelEnter = b => _onlevelenter(b, (T)b._Properties);
			if (_onlevelexit != null)
				this._RegionBase._OnLevelExit = b => _onlevelexit(b, (T)b._Properties);
			if (_onlevelupdate != null)
				this._RegionBase._OnLevelUpdate = b => _onlevelupdate(b, (T)b._Properties);

			this._RegionBase._LoadInfo = m => (T)PropertySaver.Load(typeof(T), m);
		}

		public Region(GameBase _game, string _path, bool _usebank = true, Action<Level, T> _onlevelload = null, Action<Level, T> _onlevelenter = null, Action<Level, T> _onlevelexit = null, Action<Level, T> _onlevelupdate = null)
			: this(new RegionBase(_game, _path, _usebank), _onlevelload, _onlevelenter, _onlevelexit, _onlevelupdate) { }

		public override void OnInitialization()
		{
			this.Children.Add(this._RegionBase);
			
			if (this.Player != null)
				this.SpawnPlayer(this.Player, this.PlayerLevel, this.PlayerEntrance);
		}

		public void SpawnPlayer(Entity _player, string _level, string _entrance) => this._RegionBase.SpawnPlayer(_player, _level, _entrance);
	}
}