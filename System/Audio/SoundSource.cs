using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;
using System.IO;

namespace Mekanik
{
	public class SoundSource
	{
		private static bool _AudioContextCreated;

		private static void _CreateAudioContext()
		{
			IntPtr d = Alc.OpenDevice(OpenTK.Audio.AudioContext.DefaultDevice);
			OpenTK.ContextHandle c = Alc.CreateContext(d, new int[0]);
			Alc.MakeContextCurrent(c);

			AL.Listener(ALListener3f.Position, 0, 0, 0);
			AL.Listener(ALListener3f.Velocity, 0, 0, 0);
			float[] vs = new float[] { 0, 0, -1, 0, 1, 0 };
			AL.Listener(ALListenerfv.Orientation, ref vs);
		}

		internal int _BufferId;
		private double _Length;
		public double Length
		{
			get { return this._Length; }
		}

		public SoundSource(string _path)
		{
			if (!SoundSource._AudioContextCreated)
			{
				SoundSource._AudioContextCreated = true;
				SoundSource._CreateAudioContext();
			}

			this._BufferId = AL.GenBuffer();
			
			string ext = File.GetExtension(_path).ToLower();
			if (ext == "wav")
				this._LoadWav(File.ReadBytes(_path));
			else if (ext == "ogg")
				this._LoadOggPages(_path);
			else
				throw new Exception("Unsupported file format.");
		}

		public SoundSource(int _channels, int _samplerate, int _bitspersample, byte[] _data) { this._LoadData(_channels, _samplerate, _bitspersample, _data); }

		//public static SoundSource[] SplitStereo(string _path)
		//{
		//	Tuple<int, int, int, byte[]> info = SoundSource._GetInfo(File.ReadBytes(_path));

		//	byte[][] bs = new byte[2][];
		//	bs[0] = new byte[info.Item4.Length / 2];
		//	bs[1] = new byte[info.Item4.Length / 2];

		//	int bytes = (info.Item3 / 8);
		//	for (int i = 0; i < info.Item4.Length; i += bytes * 2)
		//	{
		//		for (int x = 0; x < bytes; x++)
		//		{
		//			bs[0][i / 2 + x] = info.Item4[i + x];
		//			bs[1][i / 2 + x] = info.Item4[i + bytes + x];
		//		}
		//	}

		//	throw new Exception();
		//	return new SoundSource[] { new SoundSource(1, info.Item2, info.Item3, bs[0]), new SoundSource(1, info.Item2, info.Item3, bs[1]) };
		//}

		private static Tuple<int, int, int, byte[]> _GetInfo(byte[] _bytes)
		{
			int channels = (int)Beth.FromEndian(_bytes.SubArray(22, 2));
			int samplerate = (int)Beth.FromEndian(_bytes.SubArray(24, 4));
			int bitspersample = (int)Beth.FromEndian(_bytes.SubArray(34, 2));

			byte[] data = _bytes.SubArray(44, (int)Beth.FromEndian(_bytes.SubArray(40, 4)));

			//throw new Exception();

			return new Tuple<int, int, int, byte[]>(channels, samplerate, bitspersample, data);
		}

		private void _LoadWav(byte[] _bytes)
		{
			Tuple<int, int, int, byte[]> t = SoundSource._GetInfo(_bytes);
			this._LoadData(t.Item1, t.Item2, t.Item3, t.Item4);
		}

		private void _LoadData(int _channels, int _samplerate, int _bitspersample, byte[] _data)
		{
			ALFormat format;
			if (_channels == 1)
			{
				if (_bitspersample == 8)
					format = ALFormat.Mono8;
				else if (_bitspersample == 16)
					format = ALFormat.Mono16;
				else
					throw new Exception("Unsupported sample rate.");
			}
			else if (_channels == 2)
			{
				if (_bitspersample == 8)
					format = ALFormat.Stereo8;
				else if (_bitspersample == 16)
					format = ALFormat.Stereo16;
				else
					throw new Exception("Unsupported sample rate.");
			}
			else
				throw new Exception("More than two audio channels are not supported.");

			this._Length = _data.Length / (_bitspersample / 8) / (double)_samplerate;

			AL.BufferData(this._BufferId, format, _data, _data.Length, _samplerate);
		}

		private void _LoadOggPages(string _path)
		{
			FileStream stream = new FileStream(_path, FileMode.Open);

			long pos = 0;
			long end = stream.Length;

			Bunch<OggPage> ps = new Bunch<OggPage>();

			while (pos < end)
			{
				byte[] header = new byte[27];
				stream.Read(header, 0, 27);

				OggPage p = new OggPage();
				p.Granule = Beth.FromEndian(header.SubArray(6, 8));
				p.StreamId = (int)Beth.FromEndian(header.SubArray(14, 4));
				p.StreamPosition = (int)Beth.FromEndian(header.SubArray(18, 4));

				int ss = header[26];
				p.Segments = new byte[ss];
				pos += 27;

				int si = 0;
				while (ss > 0)
				{
					ss--;
					p.Segments[si] = ((byte)stream.ReadByte());
					si++;
					pos++;
				}

				p.SegmentsPositionInFile = pos;

				foreach (byte s in p.Segments)
				{
					pos += s;
					stream.Position += s;
				}

				ps.Add(p);
			}

			stream.Close();

			byte[][] bs1 = ps[0].ReadSegments(_path);
			byte[][] bs2 = ps[1].ReadSegments(_path);

			throw new Exception();
		}

		struct OggPage
		{
			public long Granule;
			public int StreamId;
			public int StreamPosition;
			public byte[] Segments;
			public long SegmentsPositionInFile;

			public byte[][] ReadSegments(string _path)
			{
				byte[][] @out = new byte[this.Segments.Length][];

				using (FileStream s = new FileStream(_path, FileMode.Open))
				{
					s.Position += this.SegmentsPositionInFile;

					for (int i = 0; i < this.Segments.Length; i++)
					{
						@out[i] = new byte[this.Segments[i]];
						s.Read(@out[i], 0, this.Segments[i]);
					}
				}

				return @out;
			}
		}
	}
}