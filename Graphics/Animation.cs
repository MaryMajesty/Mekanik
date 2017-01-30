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

		public Animation(AnimationLibrary _library)
		{
			this.Library = _library;
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
				if (!this._Stopped)
				{
					this.TimePassed += this.SpeedModifier;
					if (this.TimePassed > this.CurrentSource.Speed * this.CurrentSource.Sprites.Count)
					{
						if (this.CurrentSource.Repeated)
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
							this.TimePassed = this.CurrentSource.Speed * this.CurrentSource.Sprites.Count;
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
				this.Offset = this.CurrentSource.Offsets[this.CurFrame];

				Vector size = this.CurrentSource.Sprites[this.CurFrame].Size;
				return new VertexArray(VertexArrayType.Quads) { Vertices = new Bunch<Vertex>(new Vector(0, 0), new Vertex(new Vector(size.X, 0), new Vector(size.X, 0)), new Vertex(size, size), new Vertex(new Vector(0, size.Y), new Vector(0, size.Y))) };
			}
			else
				return new VertexArray(VertexArrayType.Quads);
		}
	}
}

//namespace Mekanik
//{
//	public class Animation : Primitive
//	{
//		public Dictionary<string, AnimationSource> Animations = new Dictionary<string, AnimationSource>();
//		public double CurFrame;
//		public int CurSprite;
//		public Analog Direction;
//		public double Speed = 1;
//		public double StopValue;
//		public bool UseColliders;
//		public Nullable<bool> ManualBusy;
//		//public Dictionary<string, CollisionDelegate> OnCollisions;
//		private int _PreviousDirection = 1;
//		private string _CurAnimation;
//		private bool _Stopped;
//		private Color _Color = Color.White;

//		public new Color Color
//		{
//			get { return _Color; }
//			set { _Color = value; }
//		}
//		public bool IsFinished
//		{
//			get { return !Animations[_CurAnimation].Repeated && _Stopped; }
//		}
//		public string CurAnimation
//		{
//			get { return _CurAnimation; }
//		}
//		public bool IsBusy
//		{
//			get { return (CurAnimation != null) ? (ManualBusy.HasValue ? ManualBusy.Value : (Animations[CurAnimation].Busy ? (Animations[CurAnimation].ReleaseWhenFinished ? !_Stopped : true) : false)) : false; }
//		}

//		public Animation(Dictionary<string, AnimationSource> _animations)
//		{
//			this.Animations = _animations;
//		}

//		public void Animate(string _animation, Entity _entity)
//		{
//			if (this._CurAnimation != _animation)
//			{
//				//this.OnCollisions = new Dictionary<string, CollisionDelegate>();

//				this.ManualBusy = null;
//				AnimationSource source = this.Animations[_animation];

//				this._CurAnimation = _animation;
//				if (!source.Reversed)
//				{
//					this.CurFrame = 0;
//					this.CurSprite = 0;
//				}
//				else
//				{
//					this.CurFrame = source.Frames.Length * source.Speed;
//					this.CurSprite = source.Frames.Length - 1;
//				}
//				this._Stopped = false;
//			}
//		}

//		public void Stop()
//		{
//			this._Stopped = true;
//			this.CurFrame = 0;
//			this.CurSprite = 0;
//		}

//		public override void Draw(SFML.Graphics.RenderTarget _target, SFML.Graphics.RenderStates _states)
//		{
//			if (_CurAnimation != null)
//			{
//				AnimationSource source = Animations[_CurAnimation];
//				Image img = new Image(source.Frames[CurSprite]);
//				img.Origin = source.Origin;
//				img.Scale = source.Scale * this.Scale * (source.DirectionTiedToInput ? new Vector(_PreviousDirection, 1) : 1);
//				img.Position = this.Position + source.Offset * img.Scale;
//				img.Color = source.Colors[CurSprite] * this.Color;
//				img.Draw(_target, _states);
//			}
//		}

//		public override void Update(Entity _entity)
//		{
//			if (!_Stopped)
//			{
//				if (_CurAnimation != null)
//				{
//					AnimationSource source = Animations[_CurAnimation];

//					if (source.SpeedTiedToInput && Meth.Abs(Direction.Value) < StopValue)
//						Stop();
//					else
//					{
//						if (source.SpeedTiedToInput && Meth.Abs(Direction.Value) >= StopValue)
//							this._Stopped = false;

//						if (source.SpeedTiedToInput)
//						{
//							if (!_Stopped && Direction.IsDown)
//								AddFrames(1 * Speed * (source.DirectionTiedToInput ? Meth.Abs(Direction.Value) : Direction.Value) * Meth.Sign(this.Scale.X));
//						}
//						else
//							AddFrames(-1 * (Convert.ToInt32(source.Reversed) * 2 - 1));

//						_PreviousDirection = (this.Direction == null) ? 1 : ((this.Direction.Value == 0) ? _PreviousDirection : Meth.Sign(this.Direction.Value));
//					}

//					if (this.UseColliders)
//					{
//						//_entity.Colliders.Clear();
//						//foreach (KeyValuePair<string, Bunch<Collider>[]> group in source.Colliders)
//						//{
//						//	foreach (Collider c in group.Value[CurSprite])
//						//	{
//						//		Collider copy = c * this.Scale;
//						//		if (OnCollisions.ContainsKey(group.Key))
//						//			copy.Collided += OnCollisions[group.Key];
//						//		_entity.Colliders.Add(copy);
//						//	}
//						//}
//					}
//				}
//			}
//		}

//		void AddFrames(double _frames)
//		{
//			AnimationSource source = Animations[_CurAnimation];

//			this.CurFrame += _frames;
//			if (source.Repeated)
//			{
//				this.CurFrame = Meth.RMod(this.CurFrame, (source.Speed * source.Frames.Length));
//				this.CurSprite = Meth.Down(this.CurFrame / source.Speed);
//			}
//			else
//			{
//				this.CurFrame = Meth.Limit(0, this.CurFrame, source.Frames.Length * source.Speed);
//				this.CurSprite = Meth.Min(Meth.Down(this.CurFrame / source.Speed), source.Frames.Length - 1);
//				if (!source.Reversed && this.CurFrame == source.Frames.Length * source.Speed)
//					this._Stopped = true;
//				if (source.Reversed && this.CurFrame == 0)
//					this._Stopped = true;
//			}
//		}
//	}
//}