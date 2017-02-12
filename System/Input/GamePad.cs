using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GP = OpenTK.Input.GamePad;

namespace Mekanik
{
	public class GamePad
	{
		public int Id;
		internal Dictionary<string, Analog> _Controls;

		public bool IsConnected
		{
			get { return GP.GetState(this.Id).IsConnected; }
		}
		public string Name
		{
			get { return GP.GetName(this.Id); }
		}
		public Dictionary<Key, bool> Inputs0D
		{
			get
			{
				OpenTK.Input.GamePadState s = GP.GetState(this.Id);

				Dictionary<Key, bool> @out = new Dictionary<Key, bool>();

				@out[Key.GamePadLeft] = s.DPad.IsLeft;
				@out[Key.GamePadRight] = s.DPad.IsRight;
				@out[Key.GamePadUp] = s.DPad.IsUp;
				@out[Key.GamePadDown] = s.DPad.IsDown;
				
				@out[Key.GamePadA] = s.Buttons.A == OpenTK.Input.ButtonState.Pressed;
				@out[Key.GamePadB] = s.Buttons.B == OpenTK.Input.ButtonState.Pressed;
				@out[Key.GamePadX] = s.Buttons.X == OpenTK.Input.ButtonState.Pressed;
				@out[Key.GamePadY] = s.Buttons.Y == OpenTK.Input.ButtonState.Pressed;

				@out[Key.GamePadStart] = s.Buttons.Start == OpenTK.Input.ButtonState.Pressed;
				@out[Key.GamePadSelect] = s.Buttons.Back == OpenTK.Input.ButtonState.Pressed;

				@out[Key.GamePadLeftShoulder] = s.Buttons.LeftShoulder == OpenTK.Input.ButtonState.Pressed;
				@out[Key.GamePadRightShoulder] = s.Buttons.LeftShoulder == OpenTK.Input.ButtonState.Pressed;

				@out[Key.GamePadLeftStick] = s.Buttons.LeftStick == OpenTK.Input.ButtonState.Pressed;
				@out[Key.GamePadRightStick] = s.Buttons.RightStick == OpenTK.Input.ButtonState.Pressed;

				return @out;
			}
		}
		public Dictionary<GamePadTrigger, double> Inputs1D
		{
			get
			{
				OpenTK.Input.GamePadState s = GP.GetState(this.Id);

				Dictionary<GamePadTrigger, double> @out = new Dictionary<GamePadTrigger, double>();
				@out[GamePadTrigger.Left] = s.Triggers.Left;
				@out[GamePadTrigger.Right] = s.Triggers.Right;
				return @out;
			}
		}
		public Dictionary<GamePadStick, Vector> Inputs2D
		{
			get
			{
				OpenTK.Input.GamePadState s = GP.GetState(this.Id);

				Dictionary<GamePadStick, Vector> @out = new Dictionary<GamePadStick, Vector>();
				@out[GamePadStick.Left] = new Vector(s.ThumbSticks.Left.X, s.ThumbSticks.Left.Y * -1);
				@out[GamePadStick.Right] = new Vector(s.ThumbSticks.Right.X, s.ThumbSticks.Right.Y * -1);
				return @out;
			}
		}

		internal GamePad(GameBase _game, int _id)
		{
			this.Id = _id;

			if (!_game._GamePadControlsInstances.ContainsKey(_id))
				_game._GamePadControlsInstances[_id] = _game._GamePadControls.ToDictionary(item => item.Key, item => item.Value._Clone());
			this._Controls = _game._GamePadControlsInstances[_id];
			foreach (KeyValuePair<string, Analog> ans in this._Controls)
				ans.Value._GamePad = this;
		}
		
		public static Bunch<GamePad> GetConnectedGamePads(GameBase _game)
		{
			Bunch<GamePad> @out = new Bunch<GamePad>();

			int i = 0;
			while (true)
			{
				GamePad g = new GamePad(_game, i);
				if (g.IsConnected)
				{
					@out.Add(g);
					i++;
				}
				else
					break;
			}

			return @out;
		}
	}
}