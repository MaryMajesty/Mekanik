using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;

namespace Mekanik
{
	public class Sound
	{
		internal int _SourceId;
		private SoundSource _Source;
		internal bool _ToBePlayed;

		private double _Pitch = 1;
		public double Pitch
		{
			get { return this._Pitch; }
			
			set
			{
				this._Pitch = value;
				AL.Source(this._SourceId, ALSourcef.Pitch, (float)value);
			}
		}
		private bool _Looping;
		public bool Looping
		{
			get { return this._Looping; }

			set
			{
				this._Looping = value;
				AL.Source(this._SourceId, ALSourceb.Looping, value);
			}
		}
		public double Offset
		{
			get
			{
				float f;
				AL.GetSource(this._SourceId, ALSourcef.SecOffset, out f);
				return f;
			}

			set { AL.Source(this._SourceId, ALSourcef.SecOffset, (float)value); }
		}
		public bool IsFinished
		{
			get { return AL.GetSourceState(this._SourceId) == ALSourceState.Stopped; }
		}
		private double _Volume = 1;
		internal double _VolumeDistance = 1;
		internal double _VolumeGame = 1;
		public double Volume
		{
			get { return this._Volume; }

			set
			{
				this._Volume = Meth.Limit(0, value, 1);
				this._UpdateVolume();
			}
		}
		internal double _Direction
		{
			set
			{
				if (!this._DirectionOverwritten)
				{
					double angle = Meth.Tau / 4 * 3;
					angle += Meth.Tau / 4 * Meth.Limit(-1, value, 1);
					AL.Source(this._SourceId, ALSource3f.Position, (float)Meth.Cos(angle), 0, (float)Meth.Sin(angle));
				}
			}
		}
		private bool _DirectionOverwritten;
		private double _DirectionValue;
		public double Direction
		{
			get { return this._DirectionValue; }

			set
			{
				this._DirectionValue = value;
				this._DirectionOverwritten = true;

				double angle = Meth.Tau / 4 * 3;
				angle += Meth.Tau / 4 * Meth.Limit(-1, value, 1);
				AL.Source(this._SourceId, ALSource3f.Position, (float)Meth.Cos(angle), 0, (float)Meth.Sin(angle));
			}
		}

		public Sound(SoundSource _source)
		{
			this._SourceId = AL.GenSource();
			this._Source = _source;
			
			AL.BindBufferToSource(this._SourceId, _source._BufferId);
		}

		public void Play()
		{
			AL.SourcePlay(this._SourceId);
		}

		public void Stop()
		{
			AL.SourceStop(this._SourceId);
		}

		internal void _UpdateVolume() => AL.Source(this._SourceId, ALSourcef.Gain, (float)(this._Volume * this._VolumeDistance * this._VolumeGame));

		public void Dispose()
		{
			AL.DeleteSource(this._SourceId);
		}
	}
}