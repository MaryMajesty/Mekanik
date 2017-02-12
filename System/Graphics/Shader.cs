using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Mekanik
{
	public class Shader
	{
		public static Shader Outline
		{
			get
			{
				Shader @out = Shader.FromRaw("Mekanik.Outline",
@"uniform float Selected;
uniform vec4 Color0;
uniform vec4 Color1;

void main()
{
	vec2 pos = gl_TexCoord[0].xy;
    vec4 pixel = texture2D(Texture, pos);
	
    if (Selected == 1.0)
    {
	 	if (pixel.a == 0.0)
	    {
	    	for (int x = -1; x <= 1; x++)
	    	{
	    		for (int y = -1; y <= 1; y++)
	    		{
					if (texture2D(Texture, pos + vec2(float(x) / Size.x, float(y) / Size.y)).a > 0.0)
					{
						if (mod(floor(pos.x * Size.x) + floor(pos.y * Size.y), 2.0) < 0.5)
							pixel = Color0;
						else
							pixel = Color1;
					}
	    		}
	    	}
	    }
    }
	
    gl_FragColor = gl_Color * pixel;
}");
				@out["Color0"] = Color.White;
				@out["Color1"] = Color.Black;
				return @out;
			}
		}

		public static Shader TextOutline
		{
			get
			{
				Shader @out = Shader.FromRaw("Mekanik.TextOutline",
@"uniform float Radius;
uniform float Alpha;
uniform vec4 Color;

void main()
{
	vec2 pos = gl_TexCoord[0].xy;
    vec4 pixel = texture2D(Texture, pos);
	
	float dis = 1.0;
	
	if (pixel.a < 1.0)
	{
	    for (float x = -Radius; x <= Radius; x++)
	    {
	    	for (float y = -Radius; y <= Radius; y++)
	    	{
				float a = texture2D(Texture, pos + vec2(x / Size.x, y / Size.y)).a;
				float d = sqrt(x * x + y * y + (1.0 - a)) / Radius;
				if (d <= 1.0 && d < dis && a > 0.0)
	    			dis = d;
	    	}
	    }
	}
	
	vec4 c = vec4(Color.r, Color.g, Color.b, pow(Color.a - dis, 0.4));
	gl_FragColor = gl_Color * ((c * (1.0 - pixel.a) + pixel * pixel.a) * Alpha + c * (1.0 - Alpha));
}");
				@out["Radius"] = 3;
				@out["Alpha"] = 1;
				@out["Color"] = Color.White;
				return @out;
			}
		}

//		public static Shader TextOutlineBlack
//		{
//			get
//			{
//				Shader @out = Shader.FromRaw("Mekanik.TextOutlineBlack",
//@"uniform float Radius;
//uniform float Alpha;

//void main()
//{
//	vec2 pos = gl_TexCoord[0].xy;
//    vec4 pixel = texture2D(Texture, pos);
	
//	float dis = 1.0;
	
//	if (pixel.a < 1.0)
//	{
//	    for (float x = -Radius; x <= Radius; x++)
//	    {
//	    	for (float y = -Radius; y <= Radius; y++)
//	    	{
//				float a = texture2D(Texture, pos + vec2(x / Size.x, y / Size.y)).a;
//				float d = sqrt(x * x + y * y + (1.0 - a)) / Radius;
//				if (d <= 1.0 && d < dis && a > 0.0)
//	    			dis = d;
//	    	}
//	    }
//	}
	
//	vec4 white = vec4(1.0, 1.0, 1.0, pow(1.0 - dis, 0.4));
//	gl_FragColor = gl_Color * ((white * (1.0 - pixel.a) + pixel * pixel.a) * Alpha + white * (1.0 - Alpha));
//}");
//				@out["Radius"] = 5;
//				@out["Alpha"] = 1;
//				return @out;
//			}
//		}

		public static Shader Grid
		{
			get
			{
				Shader @out = Shader.FromRaw("Mekanik.Grid",
@"uniform vec4 Color;
uniform vec2 TileSize;

void main()
{
	vec2 pos = gl_TexCoord[0].xy;
    vec4 pixel = texture2D(Texture, pos);
	
	if (mod(pos.x * Size.x, TileSize.x) < 1.0 || mod(pos.y * Size.y, TileSize.y) < 1.0)
		gl_FragColor = gl_Color * (Color * (1.0 - pixel.a) + pixel * pixel.a);
	else
		gl_FragColor = gl_Color * pixel;
}");
				@out["Color"] = Color.Black;
				@out["TileSize"] = new Vector(16, 16);

				return @out;
			}
		}

		public object this[string _name]
		{
			set
			{
				if (!this._Parameters.ContainsKey(_name))
					this._InitNeeded[_name] = true;
				this._Parameters[_name] = value;
			}
		}
		
		internal int _ShaderId;
		internal int _ProgramId;
		private Dictionary<string, bool> _InitNeeded = new Dictionary<string, bool>();
		private Dictionary<string, object> _Parameters = new Dictionary<string, object>();
		private Dictionary<string, int> _TextureUnits = new Dictionary<string, int>();
		private int _TextureUnitCount = 1;

		public Shader(string _path) : this(File.GetName(_path), File.ReadText(_path), _iscode: true) { }

		private Shader(string _name, string _code, bool _iscode)
		{
			this._ShaderId = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(this._ShaderId, "uniform sampler2D Texture; uniform vec2 Size; " + _code);
			GL.CompileShader(this._ShaderId);

			string s = GL.GetShaderInfoLog(this._ShaderId);
			if (s != "")
				throw new Exception("Shader: " + _name + "\n\n" + s);

			this._ProgramId = GL.CreateProgram();
			GL.AttachShader(this._ProgramId, this._ShaderId);
			GL.LinkProgram(this._ProgramId);

			string p = GL.GetProgramInfoLog(this._ProgramId);

			if (p != "")
				throw new Exception("Shader: " + _name + "\n\n" + p);
		}

		public static Shader FromRaw(string _name, string _code) => new Shader(_name, _code, _iscode: true);

		internal void _SetUp(ImageSource _texture)
		{
			GL.UseProgram(this._ProgramId);
			
			if (_texture != null)
			{
				this["Texture"] = _texture;
				this["Size"] = _texture.Size;
			}
			
			foreach (KeyValuePair<string, object> p in this._Parameters)
			{
				int l = GL.GetUniformLocation(this._ProgramId, p.Key);

				if (this._InitNeeded[p.Key])
				{
					this._InitNeeded[p.Key] = false;

					if (p.Value is ImageSource)
					{
						if (p.Key == "Texture")
							GL.Uniform1(l, 0);
						else
						{
							GL.Uniform1(l, this._TextureUnitCount);
							this._TextureUnits[p.Key] = this._TextureUnitCount;
							this._TextureUnitCount++;
						}
					}
				}
				
				if (p.Value is double)
					GL.Uniform1(l, (float)((double)p.Value));
				else if (p.Value is int)
					GL.Uniform1(l, (float)((int)p.Value));
				else if (p.Value is bool)
					GL.Uniform1(l, (float)((bool)p.Value ? 1 : 0));
				else if (p.Value is Color)
					GL.Uniform4(l, ((Color)p.Value).ToOpenTK());
				else if (p.Value is Vector)
					GL.Uniform2(l, ((Vector)p.Value).ToOpenTK());
				else if (p.Value is Point)
					GL.Uniform2(l, ((Vector)((Point)p.Value)).ToOpenTK());
				else if (p.Value is ImageSource)
				{
					if (p.Key != "Texture")
					{
						GL.ActiveTexture((TextureUnit)(this._TextureUnits[p.Key] + (int)TextureUnit.Texture0));
						GL.BindTexture(TextureTarget.Texture2D, ((ImageSource)p.Value)._TextureId);
					}
				}
				else
					throw new Exception("Shader parameter type " + p.Value.GetType().Name + " is not supported yet.");
			}
		}

		public void Dispose()
		{
			GL.DeleteProgram(this._ProgramId);
		}
	}
}