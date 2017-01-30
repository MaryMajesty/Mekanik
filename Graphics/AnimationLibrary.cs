using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class AnimationLibrary
	{
		public Dictionary<string, AnimationSource> Sources = new Dictionary<string, AnimationSource>();

		public AnimationLibrary(Dictionary<string, AnimationSource> _sources)
		{
			foreach (KeyValuePair<string, AnimationSource> s in _sources)
				Sources[s.Key] = s.Value;
		}

		public AnimationLibrary(params string[] _files)
		{
			foreach (string s in _files)
				Sources[File.GetName(s)] = new AnimationSource(s);
		}

		public void Add(string _path)
		{
			this.Sources[File.GetName(_path)] = new AnimationSource(_path);
		}
	}
}