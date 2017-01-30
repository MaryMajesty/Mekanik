using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mekanik
{
	public enum Key : int
	{
		/// <summary>A key outside the known keys.</summary>
		Unknown = 0,

		// Modifiers
		/// <summary>The left shift key.</summary>
		ShiftLeft,
		/// <summary>The left shift key (equivalent to ShiftLeft).</summary>
		LShift = ShiftLeft,
		/// <summary>The right shift key.</summary>
		ShiftRight,
		/// <summary>The right shift key (equivalent to ShiftRight).</summary>
		RShift = ShiftRight,
		/// <summary>The left control key.</summary>
		ControlLeft,
		/// <summary>The left control key (equivalent to ControlLeft).</summary>
		LControl = ControlLeft,
		/// <summary>The right control key.</summary>
		ControlRight,
		/// <summary>The right control key (equivalent to ControlRight).</summary>
		RControl = ControlRight,
		/// <summary>The left alt key.</summary>
		AltLeft,
		/// <summary>The left alt key (equivalent to AltLeft.</summary>
		LAlt = AltLeft,
		/// <summary>The right alt key.</summary>
		AltRight,
		/// <summary>The right alt key (equivalent to AltRight).</summary>
		RAlt = AltRight,
		/// <summary>The left win key.</summary>
		WinLeft,
		/// <summary>The left win key (equivalent to WinLeft).</summary>
		LWin = WinLeft,
		/// <summary>The right win key.</summary>
		WinRight,
		/// <summary>The right win key (equivalent to WinRight).</summary>
		RWin = WinRight,
		/// <summary>The menu key.</summary>
		Menu,

		// Function keys (hopefully enough for most keyboards - mine has 26)
		// <keysymdef.h> on X11 reports up to 35 function keys.
		/// <summary>The F1 key.</summary>
		F1,
		/// <summary>The F2 key.</summary>
		F2,
		/// <summary>The F3 key.</summary>
		F3,
		/// <summary>The F4 key.</summary>
		F4,
		/// <summary>The F5 key.</summary>
		F5,
		/// <summary>The F6 key.</summary>
		F6,
		/// <summary>The F7 key.</summary>
		F7,
		/// <summary>The F8 key.</summary>
		F8,
		/// <summary>The F9 key.</summary>
		F9,
		/// <summary>The F10 key.</summary>
		F10,
		/// <summary>The F11 key.</summary>
		F11,
		/// <summary>The F12 key.</summary>
		F12,
		/// <summary>The F13 key.</summary>
		F13,
		/// <summary>The F14 key.</summary>
		F14,
		/// <summary>The F15 key.</summary>
		F15,
		/// <summary>The F16 key.</summary>
		F16,
		/// <summary>The F17 key.</summary>
		F17,
		/// <summary>The F18 key.</summary>
		F18,
		/// <summary>The F19 key.</summary>
		F19,
		/// <summary>The F20 key.</summary>
		F20,
		/// <summary>The F21 key.</summary>
		F21,
		/// <summary>The F22 key.</summary>
		F22,
		/// <summary>The F23 key.</summary>
		F23,
		/// <summary>The F24 key.</summary>
		F24,
		/// <summary>The F25 key.</summary>
		F25,
		/// <summary>The F26 key.</summary>
		F26,
		/// <summary>The F27 key.</summary>
		F27,
		/// <summary>The F28 key.</summary>
		F28,
		/// <summary>The F29 key.</summary>
		F29,
		/// <summary>The F30 key.</summary>
		F30,
		/// <summary>The F31 key.</summary>
		F31,
		/// <summary>The F32 key.</summary>
		F32,
		/// <summary>The F33 key.</summary>
		F33,
		/// <summary>The F34 key.</summary>
		F34,
		/// <summary>The F35 key.</summary>
		F35,

		// Direction arrows
		/// <summary>The up arrow key.</summary>
		Up,
		/// <summary>The down arrow key.</summary>
		Down,
		/// <summary>The left arrow key.</summary>
		Left,
		/// <summary>The right arrow key.</summary>
		Right,

		/// <summary>The enter key.</summary>
		Enter,
		/// <summary>The escape key.</summary>
		Escape,
		/// <summary>The space key.</summary>
		Space,
		/// <summary>The tab key.</summary>
		Tab,
		/// <summary>The backspace key.</summary>
		BackSpace,
		/// <summary>The backspace key (equivalent to BackSpace).</summary>
		Back = BackSpace,
		/// <summary>The insert key.</summary>
		Insert,
		/// <summary>The delete key.</summary>
		Delete,
		/// <summary>The page up key.</summary>
		PageUp,
		/// <summary>The page down key.</summary>
		PageDown,
		/// <summary>The home key.</summary>
		Home,
		/// <summary>The end key.</summary>
		End,
		/// <summary>The caps lock key.</summary>
		CapsLock,
		/// <summary>The scroll lock key.</summary>
		ScrollLock,
		/// <summary>The print screen key.</summary>
		PrintScreen,
		/// <summary>The pause key.</summary>
		Pause,
		/// <summary>The num lock key.</summary>
		NumLock,

		// Special keys
		/// <summary>The clear key (Numpad5 with NumLock disabled, on typical keyboards).</summary>
		Clear,
		/// <summary>The sleep key.</summary>
		Sleep,
		/*LogOff,
        Help,
        Undo,
        Redo,
        New,
        Open,
        Close,
        Reply,
        Forward,
        Send,
        Spell,
        Save,
        Calculator,
        
        // Folders and applications
        Documents,
        Pictures,
        Music,
        MediaPlayer,
        Mail,
        Browser,
        Messenger,
        
        // Multimedia keys
        Mute,
        PlayPause,
        Stop,
        VolumeUp,
        VolumeDown,
        TrackPrevious,
        TrackNext,*/

		// Numpad keys
		/// <summary>The Numpad 0 key.</summary>
		Numpad0,
		/// <summary>The Numpad 1 key.</summary>
		Numpad1,
		/// <summary>The Numpad 2 key.</summary>
		Numpad2,
		/// <summary>The Numpad 3 key.</summary>
		Numpad3,
		/// <summary>The Numpad 4 key.</summary>
		Numpad4,
		/// <summary>The Numpad 5 key.</summary>
		Numpad5,
		/// <summary>The Numpad 6 key.</summary>
		Numpad6,
		/// <summary>The Numpad 7 key.</summary>
		Numpad7,
		/// <summary>The Numpad 8 key.</summary>
		Numpad8,
		/// <summary>The Numpad 9 key.</summary>
		Numpad9,
		/// <summary>The Numpad divide key.</summary>
		NumpadDivide,
		/// <summary>The Numpad multiply key.</summary>
		NumpadMultiply,
		/// <summary>The Numpad subtract key.</summary>
		NumpadSubtract,
		/// <summary>The Numpad minus key (equivalent to NumpadSubtract).</summary>
		NumpadMinus = NumpadSubtract,
		/// <summary>The Numpad add key.</summary>
		NumpadAdd,
		/// <summary>The Numpad plus key (equivalent to NumpadAdd).</summary>
		NumpadPlus = NumpadAdd,
		/// <summary>The Numpad decimal key.</summary>
		NumpadDecimal,
		/// <summary>The Numpad enter key.</summary>
		NumpadEnter,

		// Letters
		/// <summary>The A key.</summary>
		A,
		/// <summary>The B key.</summary>
		B,
		/// <summary>The C key.</summary>
		C,
		/// <summary>The D key.</summary>
		D,
		/// <summary>The E key.</summary>
		E,
		/// <summary>The F key.</summary>
		F,
		/// <summary>The G key.</summary>
		G,
		/// <summary>The H key.</summary>
		H,
		/// <summary>The I key.</summary>
		I,
		/// <summary>The J key.</summary>
		J,
		/// <summary>The K key.</summary>
		K,
		/// <summary>The L key.</summary>
		L,
		/// <summary>The M key.</summary>
		M,
		/// <summary>The N key.</summary>
		N,
		/// <summary>The O key.</summary>
		O,
		/// <summary>The P key.</summary>
		P,
		/// <summary>The Q key.</summary>
		Q,
		/// <summary>The R key.</summary>
		R,
		/// <summary>The S key.</summary>
		S,
		/// <summary>The T key.</summary>
		T,
		/// <summary>The U key.</summary>
		U,
		/// <summary>The V key.</summary>
		V,
		/// <summary>The W key.</summary>
		W,
		/// <summary>The X key.</summary>
		X,
		/// <summary>The Y key.</summary>
		Y,
		/// <summary>The Z key.</summary>
		Z,

		// Numbers
		/// <summary>The number 0 key.</summary>
		Number0,
		/// <summary>The number 1 key.</summary>
		Number1,
		/// <summary>The number 2 key.</summary>
		Number2,
		/// <summary>The number 3 key.</summary>
		Number3,
		/// <summary>The number 4 key.</summary>
		Number4,
		/// <summary>The number 5 key.</summary>
		Number5,
		/// <summary>The number 6 key.</summary>
		Number6,
		/// <summary>The number 7 key.</summary>
		Number7,
		/// <summary>The number 8 key.</summary>
		Number8,
		/// <summary>The number 9 key.</summary>
		Number9,

		// Symbols
		/// <summary>The tilde key.</summary>
		Tilde,
		/// <summary>The minus key.</summary>
		Minus,
		//Equal,
		/// <summary>The plus key.</summary>
		Plus,
		/// <summary>The left bracket key.</summary>
		BracketLeft,
		/// <summary>The left bracket key (equivalent to BracketLeft).</summary>
		LBracket = BracketLeft,
		/// <summary>The right bracket key.</summary>
		BracketRight,
		/// <summary>The right bracket key (equivalent to BracketRight).</summary>
		RBracket = BracketRight,
		/// <summary>The semicolon key.</summary>
		Semicolon,
		/// <summary>The quote key.</summary>
		Quote,
		/// <summary>The comma key.</summary>
		Comma,
		/// <summary>The period key.</summary>
		Period,
		/// <summary>The slash key.</summary>
		Slash,
		/// <summary>The backslash key.</summary>
		BackSlash,
		/// <summary>Indicates the last available keyboard key.</summary>
		KeyCount,

		MouseLeft,
		MouseRight,
		MouseMiddle,
		MouseDown,
		MouseUp,
		MouseForward,
		MouseBackward
	}

	//public enum Key
	//{
	//	Unknown = -1,
	//	A = 0,
	//	B,
	//	C,
	//	D,
	//	E,
	//	F,
	//	G,
	//	H,
	//	I,
	//	J,
	//	K,
	//	L,
	//	M,
	//	N,
	//	O,
	//	P,
	//	Q,
	//	R,
	//	S,
	//	T,
	//	U,
	//	V,
	//	W,
	//	X,
	//	Y,
	//	Z,
	//	Num0,
	//	Num1,
	//	Num2,
	//	Num3,
	//	Num4,
	//	Num5,
	//	Num6,
	//	Num7,
	//	Num8,
	//	Num9,
	//	Escape,
	//	LControl,
	//	LShift,
	//	LAlt,
	//	LSystem,
	//	RControl,
	//	RShift,
	//	RAlt,
	//	RSystem,
	//	Menu,
	//	LBracket,
	//	RBracket,
	//	SemiColon,
	//	Comma,
	//	Period,
	//	Quote,
	//	Slash,
	//	BackSlash,
	//	Tilde,
	//	Equal,
	//	Dash,
	//	Space,
	//	Enter,
	//	BackSpace,
	//	Tab,
	//	PageUp,
	//	PageDown,
	//	End,
	//	Home,
	//	Insert,
	//	Delete,
	//	Add,
	//	Subtract,
	//	Multiply,
	//	Divide,
	//	Left,
	//	Right,
	//	Up,
	//	Down,
	//	Numpad0,
	//	Numpad1,
	//	Numpad2,
	//	Numpad3,
	//	Numpad4,
	//	Numpad5,
	//	Numpad6,
	//	Numpad7,
	//	Numpad8,
	//	Numpad9,
	//	F1,
	//	F2,
	//	F3,
	//	F4,
	//	F5,
	//	F6,
	//	F7,
	//	F8,
	//	F9,
	//	F10,
	//	F11,
	//	F12,
	//	F13,
	//	F14,
	//	F15,
	//	Pause,
	//	KeyCount,
	//	MouseLeft,
	//	MouseRight,
	//	MouseMiddle,
	//	MouseX1,
	//	MouseX2,
	//	MouseUp,
	//	MouseDown
	//}
}