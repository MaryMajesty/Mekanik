using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Win32;

namespace Mekanik
{
	public class File
	{
		public readonly Bunch<string> Directories;
		public readonly string Path;
		public readonly string Name;
		public readonly string Extension;

		public string FileName
		{
			get { return this.Name + "." + this.Extension; }

			//set
			//{
			//	System.IO.File.Move
			//}
		}
		public string Text
		{
			get { return System.IO.File.ReadAllText(Path); }
		}
		public byte[] Bytes
		{
			get { return System.IO.File.ReadAllBytes(Path); }
		}

		public File(string _path, string _localto = null)
		{
			string p = System.IO.Path.GetFullPath(_path);
			this.Path = p;
			this.Name = System.IO.Path.GetFileNameWithoutExtension(p);
			string ext = System.IO.Path.GetExtension(p);
			this.Extension = (ext == "") ? "" : ext.Substring(1);

			this.Directories = new Bunch<string>();
			if (_localto != null && p.Contains(_localto))
				p = p.Substring(_localto.Length + ((_localto.Last() == '\\') ? 0 : 1));
			string[] ds = p.Split('\\');
			for (int i = 0; i < ds.Length - 1; i++)
				this.Directories.Add(ds[i]);

			while (Directories.Any(item => item[0] == '#'))
				Directories.Remove(Directories.First(item => item[0] == '#'));

			//if (_localto != null)
			//{
			//	this.Directories = new Bunch<string>();
			//	if (p.Contains(_localto))
			//	{
			//		p = p.Substring(_localto.Length);
			//		string[] ds = p.Split('\\');
			//		for (int i = 0; i < ds.Length - 1; i++)
			//			this.Directories.Add(ds[i]);
			//	}
			//}

			//string assembly = Assembly.GetCallingAssembly().Location.Substring(0, Assembly.GetCallingAssembly().Location.Length - System.IO.Path.GetFileName(Assembly.GetCallingAssembly().Location).Length);

			
		}

		public static bool Exists(string _path) { return System.IO.File.Exists(_path) | System.IO.Directory.Exists(_path); }
		public static void Write(string _path, string _text) { System.IO.File.WriteAllText(_path, _text); }
		public static void Write(string _path, byte[] _bytes) { System.IO.File.WriteAllBytes(_path, _bytes); }
		public static void Delete(string _path) { System.IO.File.Delete(_path); }

		public static string GetName(string _path) { return System.IO.Path.GetFileNameWithoutExtension(_path); }
		public static string GetExtension(string _path) { return System.IO.Path.GetExtension(_path).Substring(1); }
		public static string GetFolder(string _path) { return System.IO.Path.GetDirectoryName(_path); }

		public static void CreateFolder(string _path) { System.IO.Directory.CreateDirectory(_path); }

		public static bool IsFile(string _path)
		{
			for (int x = _path.Length - 1; x >= 0; x--)
			{
				if (_path[x] == '.')
					return true;
				if (_path[x] == '\\')
					return false;
			}
			return false;
		}

		public static string ReadText(string _path) { return System.IO.File.ReadAllText(_path); }
		public static byte[] ReadBytes(string _path) { return System.IO.File.ReadAllBytes(_path); }

		public static Bunch<string> GetFolders(string _path) { return new Bunch<string>() { System.IO.Directory.GetDirectories(_path) }; }
		public static Bunch<File> GetFiles(string _path) { return new Bunch<File>() { System.IO.Directory.GetFiles(_path).Select(item => new File(item)).ToArray() }; }

		public static Bunch<File> GetAllFiles(string _path, string _relativeto = null)
		{
			Bunch<File> @out = new Bunch<File>();

			System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(_path);
			foreach (System.IO.FileInfo f in d.GetFiles())
				@out.Add(new File(f.FullName, _relativeto));
			foreach (System.IO.DirectoryInfo dd in d.GetDirectories())
				@out.Add(GetAllFiles(dd.FullName, _relativeto));

			return @out;
		}

		public static void Bind(string _extension, string _applicationname, string _filetype, string _applicationpath, string _iconpath)
		{
			RegistryKey r = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("Classes");

			RegistryKey ext = r.CreateSubKey("." + _extension);
			ext.SetValue("", _applicationname);

			RegistryKey app = r.CreateSubKey(_applicationname);
			app.SetValue("", _filetype);

			RegistryKey icon = app.CreateSubKey("DefaultIcon");
			icon.SetValue("", _iconpath);

			app.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command").SetValue("", _applicationpath + " %1");
		}

		public static string CreateTempFile(byte[] _bytes, string _name = "", string _extension = "")
		{
			if (!File.Exists(GameBase.TempPath))
				File.CreateFolder(GameBase.TempPath);

			if (_extension != "")
				_extension = "." + _extension;

			string n = _name;
			string path = GameBase.TempPath + _name + _extension;

			while (File.Exists(path) || n == "")
				path = GameBase.TempPath + (n = _name + Meth.Abs(Meth.RandomInt).ToString()) + _extension;

			File.Write(path, _bytes);
			return path;
		}
	}
}