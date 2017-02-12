using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class LoadingSequence : Entity
	{
		private Action _OnEnd;
		private Bunch<LoadingScreen> _Screens;
		private LoadingScreen _Current;
		private Color _StartBackground;

		public LoadingSequence(Action _onend, params LoadingScreen[] _screens)
		{
			this._OnEnd = _onend;
			this._Screens = _screens;
		}

		public override void OnInitialization()
		{
			this._StartBackground = this.Parent.Background;

			//if (this._Screens.Count > 0)
			//{
			//	this.Children.Add(this._Current = this._Screens[0]);
			//	this._Screens.RemoveAt(0);
			//}
		}

		public override void Update()
		{
			if (this._Current == null || this._Current.IsDead)
			{
				if (this._Screens.Count > 0)
				{
					this.Children.Add(this._Current = this._Screens[0]);
					this._Screens.RemoveAt(0);
				}
				else
				{
					this.Parent.Background = this._StartBackground;
					this._OnEnd();
					this.Kill();
				}
			}
		}
	}
}