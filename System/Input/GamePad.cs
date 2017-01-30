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

		public bool IsConnected
		{
			get { return GP.GetState(this.Id).IsConnected; }
		}
		public string Name
		{
			get { return GP.GetName(this.Id); }
		}
		public Dictionary<string, bool> Inputs0D
		{
			get
			{
				OpenTK.Input.GamePadState s = GP.GetState(this.Id);

				Dictionary<string, bool> @out = new Dictionary<string, bool>();

				@out["GamePad Left"] = s.DPad.IsLeft;
				@out["GamePad Right"] = s.DPad.IsRight;
				@out["GamePad Up"] = s.DPad.IsUp;
				@out["GamePad Down"] = s.DPad.IsDown;

				@out["GamePad A"] = s.Buttons.A == OpenTK.Input.ButtonState.Pressed;
				@out["GamePad B"] = s.Buttons.B == OpenTK.Input.ButtonState.Pressed;
				@out["GamePad X"] = s.Buttons.X == OpenTK.Input.ButtonState.Pressed;
				@out["GamePad Y"] = s.Buttons.Y == OpenTK.Input.ButtonState.Pressed;

				@out["GamePad L"] = s.Buttons.LeftShoulder == OpenTK.Input.ButtonState.Pressed;
				@out["GamePad R"] = s.Buttons.LeftShoulder == OpenTK.Input.ButtonState.Pressed;

				@out["GamePad LS"] = s.Buttons.LeftStick == OpenTK.Input.ButtonState.Pressed;
				@out["GamePad RS"] = s.Buttons.RightStick == OpenTK.Input.ButtonState.Pressed;

				return @out;
			}
		}
		public Dictionary<string, double> Inputs1D
		{
			get
			{
				OpenTK.Input.GamePadState s = GP.GetState(this.Id);

				Dictionary<string, double> @out = new Dictionary<string, double>();
				@out["GamePad Trigger Left"] = s.Triggers.Left;
				@out["GamePad Trigger Right"] = s.Triggers.Right;
				return @out;
			}
		}
		public Dictionary<string, Vector> Inputs2D
		{
			get
			{
				OpenTK.Input.GamePadState s = GP.GetState(this.Id);

				Dictionary<string, Vector> @out = new Dictionary<string, Vector>();
				@out["GamePad ThumbStick Left"] = new Vector(s.ThumbSticks.Left.X, s.ThumbSticks.Left.Y);
				@out["GamePad ThumbStick Right"] = new Vector(s.ThumbSticks.Left.X, s.ThumbSticks.Left.Y);
				return @out;
			}
		}

		public GamePad(int _id)
		{
			this.Id = _id;
		}
		
		public static Bunch<GamePad> GetConnectedGamePads()
		{
			Bunch<GamePad> @out = new Bunch<GamePad>();

			int i = 0;
			while (true)
			{
				GamePad g = new GamePad(i);
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