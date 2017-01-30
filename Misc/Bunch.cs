using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Mekanik
{
	public class Bunch<T> : IEnumerable<T>
	{
		//public T[] Objects;
		private List<T> _Objects;
		internal Action<T> _OnAdd = _ => { };
		internal Action<T> _OnRemove = _ => { };
		//private int _HashCode = Meth.RandomInt;

		public int Count
		{
			get { return this._Objects.Count; }
		}
		public int LastIndex
		{
			get { return this._Objects.Count - 1; }
		}
		public T Last
		{
			get { return this[this.LastIndex]; }
		}
		public T Random
		{
			get { return this[Meth.Down(Meth.Random * this.Count)]; }
		}

		public T this[int _index]
		{
			get { return this._Objects[_index]; }
			set { this._Objects[_index] = value; }
		}

		public Bunch() { this._Objects = new List<T>(); }
		public Bunch(params T[] _items) { this._Objects = new List<T>(_items); }
		public Bunch(IEnumerable<T> _items) { this._Objects = new List<T>(_items); }

		public IEnumerator<T> GetEnumerator()
		{
			foreach (T obj in this._Objects)
				yield return obj;
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		//public override int GetHashCode() { return this._HashCode; }

		public void Add(params T[] _objects)
		{
			//T[] nobjects = new T[Objects.Length + _objects.Length];
			//for (int i = 0; i < Objects.Length; i++)
			//	nobjects[i] = Objects[i];
			//for (int i = 0; i < _objects.Length; i++)
			//{
			//	this._OnAdd(_objects[i]);
			//	nobjects[Objects.Length + i] = _objects[i];
			//}

			foreach (T obj in _objects)
			{
				this._OnAdd(obj);
				this._Objects.Add(obj);
			}
			//Objects = nobjects;

			//this._HashCode = Meth.RandomInt;
		}

		public void Add<TType>(Bunch<TType> _bunch) where TType : T
		{
			foreach (TType obj in _bunch)
			{
				this._OnAdd(obj);
				this.Add(obj);
			}
		}

		public void RemoveAt(int _index)
		{
			this._OnRemove(this[_index]);
			this._Objects.RemoveAt(_index);
		}

		internal void _Remove(T _object) => this._Objects.Remove(_object);

		public void Remove(Bunch<T> _objects)
		{
			foreach (T obj in _objects)
			{
				this._OnRemove(obj);
				this._Objects.Remove(obj);
			}
		}

		public void Remove(params T[] _objects) => this.Remove(_objects.ToBunch());

		public bool Contains(T _object) => this.Any(item => item.Equals(_object));

		public bool Contains(Bunch<T> _objects)
		{
			foreach (T obj in _objects)
			{
				if (this.Contains(obj))
					return true;
			}
			return false;
		}

		public int IndexOf(T _object) => this._Objects.IndexOf(_object);
		public int LastIndexOf(T _object) => this._Objects.LastIndexOf(_object);

		public T First(Func<T, bool> _function)
		{
			if (this.Any(_function))
				return this._Objects.First(_function);
			else
				return default(T);
		}

		//public Bunch<int> IndexesOf(T _object)
		//{
			//Bunch<int> @out = new Bunch<int>();
			//for (int i = 0; i < Objects.Length; i++)
			//{
			//	if (Objects[i].Equals(_object))
			//		@out.Add(i);
			//}
			//return @out;
		//}

		//public T WhereFirst(Func<T, bool> _function)
		//{
		//	for (int i = 0; i < this.Count; i++)
		//	{
		//		if (_function.Invoke(this[i]))
		//			return this[i];
		//	}
		//	return default(T);
		//}

		public Bunch<T> Where(Func<T, bool> _function) => new Bunch<T>(this._Objects.Where(item => _function(item)));

		public void MoveToTopAt(int _index)
		{
			T obj = this._Objects[_index];
			this._Objects.Remove(obj);
			this._Objects.Insert(0, obj);
			//Bunch<T> nthis = new Bunch<T>();
			//for (int i = 0; i < this.Count; i++)
			//{
			//	if (i != _index)
			//		nthis.Add(Objects[i]);
			//}
			//nthis.Add(Objects[_index]);
			//this.Objects = nthis.Objects;
		}

		public void MoveToTop(T _object) => MoveToTopAt(IndexOf(_object));

		public void MoveToBottomAt(int _index)
		{
			T obj = this._Objects[_index];
			this._Objects.Remove(obj);
			this._Objects.Add(obj);
			//Bunch<T> nthis = new Bunch<T>();
			//nthis.Add(Objects[_index]);
			//for (int i = 0; i < this.Count; i++)
			//{
			//	if (i != _index)
			//		nthis.Add(Objects[i]);
			//}
			//this.Objects = nthis.Objects;
		}

		public void Insert(int _index, T _object) => this._Objects.Insert(_index, _object);

		public void MoveToBottom(T _object) => MoveToBottomAt(IndexOf(_object));

		public Bunch<TType> GetTypes<TType>() where TType : T  
		{
			return new Bunch<TType>(this._Objects.Where(item => item.GetType() == typeof(TType) || item.GetType().IsSubclassOf(typeof(TType))).Select(item => (TType)item));
			//Bunch<TType> @out = new Bunch<TType>();
			//foreach (object obj in Objects)
			//{
			//	if (obj.GetType() == typeof(TType) || obj.GetType().IsSubclassOf(typeof(TType)))
			//		@out.Add((TType)obj);
			//}
			//return @out;
		}

		public Bunch<T> SubBunch(int _start)
		{
			Bunch<T> @out = new Bunch<T>();
			for (int i = _start; i < this.Count; i++)
				@out.Add(this[i]);
			return @out;
		}

		public Bunch<T> SubBunch(int _start, int _length)
		{
			Bunch<T> @out = new Bunch<T>();
			for (int i = _start; i < _start + _length; i++)
				@out.Add(this[i]);
			return @out;
		}

		public void Clear() => this._Objects.Clear();

		public Bunch<T> OrderBy(Func<T, double> _func) => new Bunch<T>() { ((IEnumerable<T>)this).OrderBy(item => _func(item)).ToArray() };
		public Bunch<T> OrderByDescending(Func<T, double> _func) => new Bunch<T>() { ((IEnumerable<T>)this).OrderBy(item => _func(item)).Reverse().ToArray() };

		public Bunch<TResult> Select<TResult>(Func<T, TResult> _func) => new Bunch<TResult>() { ((IEnumerable<T>)this).Select<T, TResult>(item => _func(item)).ToArray() };

		public Bunch<T> Clone()
		{
			Bunch<T> @out = new Bunch<T>();
			foreach (T obj in this)
				@out.Add(obj);
			return @out;
		}

		public Bunch<T> Reverse() => new Bunch<T>() { ((IEnumerable<T>)this).Reverse().ToArray() };

		public Bunch<T> MoveTo(T _item, int _index)
		{
			Bunch<T> @out = this.Clone();
			@out.Remove(_item);
			return @out.SubBunch(0, _index) + _item + @out.SubBunch(_index);
		}

		public bool TrueForAll(Func<T, bool> _func) => this._Objects.TrueForAll(item => _func(item));

		public void Shuffle()
		{
			for (int i = 0; i < this._Objects.Count; i++)
			{
				int n = Meth.Down(Meth.Random * this._Objects.Count);
				T o = this[n];
				this[n] = this[i];
				this[i] = o;
			}
		}

		//public int IndexOf(T _item)
		//{
		//	for (int i = 0; i < this.Count; i++)
		//	{
		//		if (this[i].Equals(_item))
		//			return i;
		//	}
		//	return -1;
		//}

		public Bunch<T> Merge(Bunch<T> _bunch)
		{
			Bunch<T> @out = this.Clone();
			@out.Add(_bunch.Where(item => !@out.Contains(item)));
			return @out;
		}

		public double Max(Func<T, double> _func)
		{
			if (this.Count == 0)
				return 0;
			else
				return this.Max<T>(item => _func(item));
		}

		//public TOut Sum<TOut>(Func<T, TOut> _func)
		//{
		//	MethodInfo plus = _func.GetType().GetMethod("op_Addition");
		//	if (plus == null)
		//		throw new Exception("The type has to have a + method!");

		//	TOut sum = default(TOut);
		//	foreach (T t in this)
		//		sum = (TOut)plus.Invoke(sum, new object[] { sum, _func(t) });

		//	return sum;
		//}

		//public static bool operator ==(Bunch<T> _one, string _two)
		//{
		//	if (typeof(T) != typeof(string))
		//		return false;

		//	string[] parts = _two.Split(',');
		//	for (int i = 1; i < parts.Length; i++)
		//	{
		//		if (parts[i][0] == ' ')
		//			parts[i] = parts[i].Substring(1);
		//	}

		//	if (_one.Count != parts.Length)
		//		return false;

		//	for (int i = 0; i < parts.Length; i++)
		//	{
		//		object o = _one[i];
		//		if (parts[i] != (string)o)
		//			return false;
		//	}

		//	return true;
		//}
		//public static bool operator !=(Bunch<T> _one, string _two) { return !(_one == _two); }

		public static Bunch<T> operator +(Bunch<T> _one, T _two)
		{
			Bunch<T> @out = _one.Clone();
			@out.Add(_two);
			return @out;
		}
		public static Bunch<T> operator +(T _one, Bunch<T> _two)
		{
			Bunch<T> @out = new Bunch<T>() { _one };
			@out.Add(_two);
			return @out;
		}
		public static Bunch<T> operator +(Bunch<T> _one, Bunch<T> _two)
		{
			Bunch<T> @out = _one;
			_one.Add(_two);
			return @out;
		}

		public static Bunch<T> Parse(string _string)
		{
			return _string.Split('|').Select(item => item.To<T>()).ToBunch<T>();
		}

		public static implicit operator T[](Bunch<T> @this) { return @this.ToArray(); }
		public static implicit operator Bunch<T>(T[] @this) { return new Bunch<T>(@this); }
		public static implicit operator Bunch<T>(T @this) { return new Bunch<T>(@this); }
	}
}