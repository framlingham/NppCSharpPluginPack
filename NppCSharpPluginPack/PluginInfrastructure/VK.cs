﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NppDemo.PluginInfrastructure
{
	/// <summary>
	/// The virtual key codes used in Windows, as defined in the Win32 API. From
	/// https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
	/// Is using https://github.com/microsoft/CsWin32 better?
	/// </summary>
	public enum VK
	{
		LBUTTON = 0x01, // Left mouse button
		RBUTTON = 0x02, // Right mouse button
		CANCEL = 0x03, // Control-break processing
		MBUTTON = 0x04, // Middle mouse button
		XBUTTON1 = 0x05, // X1 mouse button
		XBUTTON2 = 0x06, // X2 mouse button

		// 0x07: Reserved

		BACK = 0x08, // Backspace key
		TAB = 0x09, // Tab key

		// 0x0A-0B: Reserved

		CLEAR = 0x0C, // Clear key
		RETURN = 0x0D, // Enter key

		// 0x0E-0F: Unassigned

		SHIFT = 0x10, // Shift key
		CONTROL = 0x11, // Ctrl key
		MENU = 0x12, // Alt key
		PAUSE = 0x13, // Pause key
		CAPITAL = 0x14, // Caps lock key
		KANA = 0x15, // IME Kana mode
		HANGUL = 0x15, // IME Hangul mode
		IME_ON = 0x16, // IME On
		JUNJA = 0x17, // IME Junja mode
		FINAL = 0x18, // IME final mode
		HANJA = 0x19, // IME Hanja mode
		KANJI = 0x19, // IME Kanji mode
		IME_OFF = 0x1A, // IME Off
		ESCAPE = 0x1B, // Esc key
		CONVERT = 0x1C, // IME convert
		NONCONVERT = 0x1D, // IME nonconvert
		ACCEPT = 0x1E, // IME accept
		MODECHANGE = 0x1F, // IME mode change request
		SPACE = 0x20, // Spacebar key
		PRIOR = 0x21, // Page up key
		NEXT = 0x22, // Page down key
		END = 0x23, // End key
		HOME = 0x24, // Home key
		LEFT = 0x25, // Left arrow key
		UP = 0x26, // Up arrow key
		RIGHT = 0x27, // Right arrow key
		DOWN = 0x28, // Down arrow key
		SELECT = 0x29, // Select key
		PRINT = 0x2A, // Print key
		EXECUTE = 0x2B, // Execute key
		SNAPSHOT = 0x2C, // Print screen key
		INSERT = 0x2D, // Insert key
		DELETE = 0x2E, // Delete key
		HELP = 0x2F, // Help key
		N0 = 0x30, // 0 key
		N1 = 0x31, // 1 key
		N2 = 0x32, // 2 key
		N3 = 0x33, // 3 key
		N4 = 0x34, // 4 key
		N5 = 0x35, // 5 key
		N6 = 0x36, // 6 key
		N7 = 0x37, // 7 key
		N8 = 0x38, // 8 key
		N9 = 0x39, // 9 key

		// 0x3A-40: Undefined

		A = 0x41, // A key
		B = 0x42, // B key
		C = 0x43, // C key
		D = 0x44, // D key
		E = 0x45, // E key
		F = 0x46, // F key
		G = 0x47, // G key
		H = 0x48, // H key
		I = 0x49, // I key
		J = 0x4A, // J key
		K = 0x4B, // K key
		L = 0x4C, // L key
		M = 0x4D, // M key
		N = 0x4E, // N key
		O = 0x4F, // O key
		P = 0x50, // P key
		Q = 0x51, // Q key
		R = 0x52, // R key
		S = 0x53, // S key
		T = 0x54, // T key
		U = 0x55, // U key
		V = 0x56, // V key
		W = 0x57, // W key
		X = 0x58, // X key
		Y = 0x59, // Y key
		Z = 0x5A, // Z key
		LWIN = 0x5B, // Left Windows logo key
		RWIN = 0x5C, // Right Windows logo key
		APPS = 0x5D, // Application key

		// 0x5E: Reserved

		SLEEP = 0x5F, // Computer Sleep key
		NUMPAD0 = 0x60, // Numeric keypad 0 key
		NUMPAD1 = 0x61, // Numeric keypad 1 key
		NUMPAD2 = 0x62, // Numeric keypad 2 key
		NUMPAD3 = 0x63, // Numeric keypad 3 key
		NUMPAD4 = 0x64, // Numeric keypad 4 key
		NUMPAD5 = 0x65, // Numeric keypad 5 key
		NUMPAD6 = 0x66, // Numeric keypad 6 key
		NUMPAD7 = 0x67, // Numeric keypad 7 key
		NUMPAD8 = 0x68, // Numeric keypad 8 key
		NUMPAD9 = 0x69, // Numeric keypad 9 key
		MULTIPLY = 0x6A, // Multiply key
		ADD = 0x6B, // Add key
		SEPARATOR = 0x6C, // Separator key
		SUBTRACT = 0x6D, // Subtract key
		DECIMAL = 0x6E, // Decimal key
		DIVIDE = 0x6F, // Divide key
		F1 = 0x70, // F1 key
		F2 = 0x71, // F2 key
		F3 = 0x72, // F3 key
		F4 = 0x73, // F4 key
		F5 = 0x74, // F5 key
		F6 = 0x75, // F6 key
		F7 = 0x76, // F7 key
		F8 = 0x77, // F8 key
		F9 = 0x78, // F9 key
		F10 = 0x79, // F10 key
		F11 = 0x7A, // F11 key
		F12 = 0x7B, // F12 key
		F13 = 0x7C, // F13 key
		F14 = 0x7D, // F14 key
		F15 = 0x7E, // F15 key
		F16 = 0x7F, // F16 key
		F17 = 0x80, // F17 key
		F18 = 0x81, // F18 key
		F19 = 0x82, // F19 key
		F20 = 0x83, // F20 key
		F21 = 0x84, // F21 key
		F22 = 0x85, // F22 key
		F23 = 0x86, // F23 key
		F24 = 0x87, // F24 key

		// 0x88-8F: Reserved

		NUMLOCK = 0x90, // Num lock key
		SCROLL = 0x91, // Scroll lock key

		// 0x92-96: OEM specific
		// 0x97-9F: Unassigned

		LSHIFT = 0xA0, // Left Shift key
		RSHIFT = 0xA1, // Right Shift key
		LCONTROL = 0xA2, // Left Ctrl key
		RCONTROL = 0xA3, // Right Ctrl key
		LMENU = 0xA4, // Left Alt key
		RMENU = 0xA5, // Right Alt key
		BROWSER_BACK = 0xA6, // Browser Back key
		BROWSER_FORWARD = 0xA7, // Browser Forward key
		BROWSER_REFRESH = 0xA8, // Browser Refresh key
		BROWSER_STOP = 0xA9, // Browser Stop key
		BROWSER_SEARCH = 0xAA, // Browser Search key
		BROWSER_FAVORITES = 0xAB, // Browser Favorites key
		BROWSER_HOME = 0xAC, // Browser Start and Home key
		VOLUME_MUTE = 0xAD, // Volume Mute key
		VOLUME_DOWN = 0xAE, // Volume Down key
		VOLUME_UP = 0xAF, // Volume Up key
		MEDIA_NEXT_TRACK = 0xB0, // Next Track key
		MEDIA_PREV_TRACK = 0xB1, // Previous Track key
		MEDIA_STOP = 0xB2, // Stop Media key
		MEDIA_PLAY_PAUSE = 0xB3, // Play/Pause Media key
		LAUNCH_MAIL = 0xB4, // Start Mail key
		LAUNCH_MEDIA_SELECT = 0xB5, // Select Media key
		LAUNCH_APP1 = 0xB6, // Start Application 1 key
		LAUNCH_APP2 = 0xB7, // Start Application 2 key

		// 0xB8-B9: Reserved

		OEM_1 = 0xBA, // It can vary by keyboard. For the US ANSI keyboard , the Semiсolon and Colon key
		OEM_PLUS = 0xBB, // For any country/region, the Equals and Plus key
		OEM_COMMA = 0xBC, // For any country/region, the Comma and Less Than key
		OEM_MINUS = 0xBD, // For any country/region, the Dash and Underscore key
		OEM_PERIOD = 0xBE, // For any country/region, the Period and Greater Than key
		OEM_2 = 0xBF, // It can vary by keyboard. For the US ANSI keyboard, the Forward Slash and Question Mark key
		OEM_3 = 0xC0, // It can vary by keyboard. For the US ANSI keyboard, the Grave Accent and Tilde key

		// 0xC1-DA: Reserved

		OEM_4 = 0xDB, // It can vary by keyboard. For the US ANSI keyboard, the Left Brace key
		OEM_5 = 0xDC, // It can vary by keyboard. For the US ANSI keyboard, the Backslash and Pipe key
		OEM_6 = 0xDD, // It can vary by keyboard. For the US ANSI keyboard, the Right Brace key
		OEM_7 = 0xDE, // It can vary by keyboard. For the US ANSI keyboard, the Apostrophe and Double Quotation Mark key
		OEM_8 = 0xDF, // It can vary by keyboard. For the Canadian CSA keyboard, the Right Ctrl key

		// 0xE0: Reserved
		// 0xE1: OEM specific

		OEM_102 = 0xE2, // It can vary by keyboard. For the European ISO keyboard, the Backslash and Pipe key

		// 0xE3-E4: OEM specific

		PROCESSKEY = 0xE5, // IME PROCESS key

		// 0xE6: OEM specific

		PACKET = 0xE7, // Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP

		// 0xE8: Unassigned
		// 0xE9-F5: OEM specific

		ATTN = 0xF6, // Attn key
		CRSEL = 0xF7, // CrSel key
		EXSEL = 0xF8, // ExSel key
		EREOF = 0xF9, // Erase EOF key
		PLAY = 0xFA, // Play key
		ZOOM = 0xFB, // Zoom key
		NONAME = 0xFC, // Reserved
		PA1 = 0xFD, // PA1 key
		OEM_CLEAR = 0xFE, // Clear key
	}
}
