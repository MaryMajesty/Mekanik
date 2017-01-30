using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Mekanik
{
	public class ArrayEditor : InputControl
	{
		private DragList<ArrayItem> _DragList;
		private Type _Type;
		private Type _ItemType;
		private Bunch<object> _StartItems = new Bunch<object>();

		public ArrayEditor(Type _type, object _value)
		{
			this._Type = _type;
			this._ItemType = _type.GetElementType();

			int length = (int)_type.GetMethod("GetLength").Invoke(_value, new object[] { 0 });
			MethodInfo getvalue = _type.GetMethod("GetValue", new Type[] { typeof(int) });
			for (int i = 0; i < length; i++)
				this._StartItems.Add(getvalue.Invoke(_value, new object[] { i }));
		}

		public override void OnInitialization()
		{
			this._DragList = new DragList<ArrayItem>();
			this.Children.Add(this._DragList);

			this._DragList.OnItemAdd = index => new ArrayItem(this._ItemType, null);
			this._DragList.SetItems(this._StartItems.Select(item => new ArrayItem(this._ItemType, item)));
		}

		internal override double _GetRectWidth() => this._DragList.RectSize.X;
		internal override double _GetRectHeight() => this._DragList.RectSize.Y;

		public override object GetValue()
		{
			object @out = Activator.CreateInstance(this._Type, new object[] { this._DragList.Items.Count });

			MethodInfo setvalue = this._Type.GetMethod("SetValue", new Type[] { this._ItemType, typeof(int) });
			for (int i = 0; i < this._DragList.Items.Count; i++)
				setvalue.Invoke(@out, new object[] { this._DragList.Items[i].GetValue(), i });

			return @out;
		}

		protected override bool _IsEdited() => this._DragList.Items.Any(item => item._InputControl.IsEdited);

		class ArrayItem : DragItem
		{
			public VertexArray Cross = new VertexArray(VertexArrayType.Lines) { Position = new Vector(2, 5) };
			public MouseArea CrossArea;
			public Rectangle DragRectangle;
			public MouseArea DragArea;
			internal InputControl _InputControl;
			private Type _Type;

			public ArrayItem(Type _type, object _value)
			{
				this.Interfacial = true;

				this._Type = _type;

				this.Cross.Add(new Vector(0, 0), new Vector(11, 11), new Vector(0, 11), new Vector(11, 0));
				this.Graphics.Add(this.Cross);
				
				if (InputControl.CanInput(_type))
					this._InputControl = InputControl.GetControl(_type, _value);
				else
					this._InputControl = new PropertyEditor(_type, _value);
				this._InputControl.Position = new Vector(15, 0);
				this._InputControl.Z = 1;
			}

			public override void OnInitialization()
			{
				this.Children.Add(this._InputControl);
				this.AddMouseArea(this.DragArea = new MouseArea(new Rectangle(new Vector(15, 0), this._InputControl.RectSize)) { OnClick = key => this.StartDrag(), OnRelease = key => this.StopDrag() });
				this.Graphics.Add(this.DragRectangle = new Rectangle(new Vector(15, 0), this._InputControl.RectSize));

				this.AddMouseArea(this.CrossArea = new MouseArea(new Rectangle(this.Cross.Position - 1, 13)) { OnClick = key => ((DragList<ArrayItem>)this.Parents[0]).RemoveItem(this) });
			}

			public override void Update()
			{
				this.Cross.Color = this.CrossArea.IsHovered ? Color.White * 0.5 : Color.Black;
				this.DragRectangle.Color = this.DragArea.IsHovered ? Color.White * 0.95 : Color.White;
			}

			public override double GetHeight() => this.IsInitialized ? this._InputControl.RectSize.Y : 1000;

			public object GetValue()
			{
				if (this._InputControl is PropertyEditor)
					return PropertyReflector.GenerateFromProperties(this._Type, ((PropertyEditor)this._InputControl).GetProperties());
				else
					return this._InputControl.GetValue();
			}
		}
	}
}