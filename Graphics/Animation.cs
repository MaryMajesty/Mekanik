using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class Animation : VertexGraphic
	{
		public AnimationLibrary Library;
		public AnimationSource CurrentSource;
		public string CurSource;
		public int CurFrame;
		public double TimePassed;
		private bool _Stopped;
		private bool _StopSmooth;
		public double SpeedModifier = 1;

		public bool IsFinished
		{
			get { return this.TimePassed >= this.CurrentSource.Speed * this.CurrentSource.Sprites.Count; }
		}
		public Point Size
		{
			get { return this.CurrentSource.Sprites[this.CurFrame].Size; }
		}

		public Animation() { }

		public Animation(AnimationLibrary _library) { this.Library = _library; }

		public Animation(string _path)
		{
			this.Library = new AnimationLibrary(_path);
			this.Play(this.Library.Sources.ToArray()[0].Key);
		}

		public void Play(string _animation)
		{
			if (this.CurSource != _animation)
				this.ForcePlay(_animation);
			this._Stopped = false;
			this._StopSmooth = false;
		}

		public void ForcePlay(string _animation)
		{
			this.CurSource = _animation;
			this.CurrentSource = this.Library.Sources[_animation];
			this.TimePassed = 0;
			this._Stopped = false;
			this._StopSmooth = false;
		}

		public void SwitchToAnimation(string _animation)
		{
			this.CurSource = _animation;
			this.CurrentSource = this.Library.Sources[_animation];
		}

		public void Stop()
		{
			this.TimePassed = this.CurrentSource.Speed * this.CurrentSource.Sprites.Count;
			this.CurFrame = this.CurrentSource.Sprites.LastIndex;
			this._Stopped = true;
		}

		public void StopSmooth()
		{
			this._StopSmooth = true;
		}

		public override void Update()
		{
			if (this.CurrentSource != null)
			{
				if (Meth.Sign(this.TimePassed - 1) != Meth.Sign(this.SpeedModifier))
					this._Stopped = false;

				if (!this._Stopped)
				{
					this.TimePassed += this.SpeedModifier;
					if (this.TimePassed > this.CurrentSource.Speed * this.CurrentSource.Sprites.Count || (this.SpeedModifier < 0 && this.TimePassed <= 0))
					{
						if (this.CurrentSource.Repeated)
						{
							if (this.SpeedModifier > 0)
							{
								if (this._StopSmooth)
								{
									this._StopSmooth = false;
									this._Stopped = true;
									this.TimePassed = this.CurrentSource.Speed * this.CurrentSource.Sprites.Count;
								}
								else
									this.TimePassed %= (this.CurrentSource.Speed * this.CurrentSource.Sprites.Count);
							}
							else
							{
								if (this._StopSmooth)
								{
									this._StopSmooth = false;
									this._Stopped = true;
									this.TimePassed = 0;
								}
								else
									this.TimePassed = (this.TimePassed + (this.CurrentSource.Speed * this.CurrentSource.Sprites.Count)) % (this.CurrentSource.Speed * this.CurrentSource.Sprites.Count);
							}
						}
						else
						{
							this.TimePassed = (this.SpeedModifier > 0) ? this.CurrentSource.Speed * this.CurrentSource.Sprites.Count : 0;
							this._Stopped = true;
							this._StopSmooth = false;
						}
					}

					this.CurFrame = Meth.Min(Meth.Down(this.TimePassed / this.CurrentSource.Speed), this.CurrentSource.Sprites.LastIndex);
				}
			}
		}

		protected internal override VertexArray _ToVertexArray()
		{
			if (this.CurrentSource != null)
			{
				this.Texture = this.CurrentSource.Sprites[this.CurFrame];
				Vector off = this.CurrentSource.Offsets[this.CurFrame];

				Vector size = this.CurrentSource.Sprites[this.CurFrame].Size;
				return new VertexArray(VertexArrayType.Quads) { Vertices = new Bunch<Vertex>(new Vector(0, 0) + off, new Vertex(new Vector(size.X, 0) + off, new Vector(size.X, 0)), new Vertex(size + off, size), new Vertex(new Vector(0, size.Y) + off, new Vector(0, size.Y))) };
			}
			else
				return new VertexArray(VertexArrayType.Quads);
		}
	}
}