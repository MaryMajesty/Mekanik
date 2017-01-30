using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public class MekanikalError : Exception
	{
		public string Type;
		public bool Fatal;
		private string _Source;

		public string Source
		{
			get { return (this.Type == "Unknown") ? this._Source : this.StackTrace; }
		}

		public MekanikalError(string _type, bool _fatal, string _message)
			: base(_message)
		{
			this.Type = _type;
			this.Fatal = _fatal;
		}

		public MekanikalError(Exception _error)
			: base(_error.Message)
		{
			this.Type = "Unknown";
			this.Fatal = true;
			this._Source = _error.StackTrace;
		}
	}
}