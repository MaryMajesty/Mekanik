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

		public Entity Player
		{
			get { return this._RegionBase.Player; }
			set { this._RegionBase.Player = value; }
		}
		public LevelBase CurrentLevel
		{
			get { return this._RegionBase.CurrentLevel; }
		}

		public Region(RegionBase _base, Action<LevelBase, T> _onlevelload = null, Action<LevelBase, T> _onlevelenter = null, Action<LevelBase, T> _onlevelexit = null)
		{
			this._RegionBase = _base;

			foreach (KeyValuePair<string, LevelBase> p in _base._Levels)
				p.Value._Properties = (T)PropertySaver.Load(typeof(T), p.Value.Source._Properties);

			if (_onlevelload != null)
				this._RegionBase._OnLevelLoad = b => _onlevelload(b, (T)b._Properties);
			if (_onlevelenter != null)
				this._RegionBase._OnLevelEnter = b => _onlevelenter(b, (T)b._Properties);
			if (_onlevelexit != null)
				this._RegionBase._OnLevelExit = b => _onlevelexit(b, (T)b._Properties);
		}

		public override void OnInitialization()
		{
			this.Children.Add(this._RegionBase);
		}
	}
}