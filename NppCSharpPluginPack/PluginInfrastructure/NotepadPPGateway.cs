﻿// NPP plugin platform for .Net v0.94.00 by Kasper B. Graversen etc.
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using NppDemo.PluginInfrastructure;

namespace Kbg.NppPluginNET.PluginInfrastructure
{
	public interface INotepadPPGateway
	{
		void FileNew();

		void AddToolbarIcon(int funcItemsIndex, toolbarIcons icon);
		void AddToolbarIcon(int funcItemsIndex, Bitmap icon);
		string GetNppPath();
		string GetPluginConfigPath();
		string GetCurrentFilePath();
		unsafe string GetFilePath(IntPtr bufferId);
		void SetCurrentLanguage(LangType language);
		bool OpenFile(string path);
		bool SaveCurrentFile();
		
		void ShowDockingForm(System.Windows.Forms.Form form);
		void HideDockingForm(System.Windows.Forms.Form form);

		Color GetDefaultForegroundColor();
		Color GetDefaultBackgroundColor();
		System.Windows.Media.Color GetDefaultForeColor();
		System.Windows.Media.Color GetDefaultBackColor();
		bool IsDarkModeEnabled();
		unsafe System.Windows.Media.Color[] GetDarkModeColors();
		string GetConfigDirectory();
		int[] GetNppVersion();
		string[] GetOpenFileNames();
		void SetStatusBarSection(string message, StatusBarSection section);
		/// <summary>
		/// Register a modeless form (i.e., a form that doesn't block the parent application until closed)<br></br>
		/// with Notepad++ using NPPM_MODELESSDIALOG<br></br>
		/// If you don't do this, Notepad++ may intercept some keystrokes in unintended ways.
		/// </summary>
		/// <param name="formHandle">the Handle attribute of a Windows form</param>
		void AddModelessDialog(IntPtr formHandle);
		/// <summary>
		/// unregister a modeless form that was registered with AddModelessDialog.<br></br>
		/// This MUST be called in the Dispose method of the form, BEFORE the components of the form are disposed.
		/// </summary>
		/// <param name="formHandle">the Handle attribute of a Windows form</param>
		void RemoveModelessDialog(IntPtr formHandle);
        /// <summary>
		/// Introduced in Notepad++ 8.5.6.<br></br>
        /// NPPM_ALLOCATEINDICATOR: allocate one or more unused indicator IDs,
        /// which can then be assigned styles and used to style regions of text.<br></br>
        /// returns false and sets indicators to null if numberOfIndicators is less than 1, or if the requested number of indicators could not be allocated.<br></br>
        /// Otherwise, returns true, and sets indicators to an array of numberOfIndicators indicator IDs.<br></br>
        /// See https://www.scintilla.org/ScintillaDoc.html#Indicators for more info on the indicator API.
        /// </summary>
        /// <param name="numberOfIndicators">number of consecutive indicator IDs to allocate</param>
        /// <returns></returns>
        bool AllocateIndicators(int numberOfIndicators, out int[] indicators);

    }

	/// <summary>
	/// This class holds helpers for sending messages defined in the Msgs_h.cs file. It is at the moment
	/// incomplete. Please help fill in the blanks.
	/// </summary>
	public class NotepadPPGateway : INotepadPPGateway
	{
		private const int Unused = 0;

		public void FileNew()
		{
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) NppMsg.NPPM_MENUCOMMAND, Unused, NppMenuCmd.IDM_FILE_NEW);
		}

		public void AddToolbarIcon(int funcItemsIndex, toolbarIcons icon)
		{
			IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(icon));
			try {
				Marshal.StructureToPtr(icon, pTbIcons, false);
				_ = Win32.SendMessage(
					PluginBase.nppData._nppHandle,
					(uint) NppMsg.NPPM_ADDTOOLBARICON,
					PluginBase._funcItems.Items[funcItemsIndex]._cmdID,
					pTbIcons);
			} finally {
				Marshal.FreeHGlobal(pTbIcons);
			}
		}

		public void AddToolbarIcon(int funcItemsIndex, Bitmap icon)
		{
			var tbi = new toolbarIcons();
			tbi.hToolbarBmp = icon.GetHbitmap();
			AddToolbarIcon(funcItemsIndex, tbi);
		}

		/// <summary>
		/// Gets the path of the current document.
		/// </summary>
		public string GetCurrentFilePath()
		{
			var path = new StringBuilder(2000);
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETFULLCURRENTPATH, 0, path);
			return path.ToString();
		}

		/// <summary>
		/// This method incapsulates a common pattern in the Notepad++ API: when
		/// you need to retrieve a string, you can first query the buffer size.
		/// This method queries the necessary buffer size, allocates the temporary
		/// memory, then returns the string retrieved through that buffer.
		/// </summary>
		/// <param name="message">Message ID of the data string to query.</param>
		/// <returns>String returned by Notepad++.</returns>
		public string GetString(NppMsg message)
		{
			int len = Win32.SendMessage(
					PluginBase.nppData._nppHandle,
					(uint) message, Unused, Unused).ToInt32()
				+ 1;
			var res = new StringBuilder(len);
			_ = Win32.SendMessage(
				PluginBase.nppData._nppHandle, (uint) message, len, res);
			return res.ToString();
		}

		/// <returns>The path to the Notepad++ executable.</returns>
		public string GetNppPath()
			=> GetString(NppMsg.NPPM_GETNPPDIRECTORY);

		/// <returns>The path to the Config folder for plugins.</returns>
		public string GetPluginConfigPath()
			=> GetString(NppMsg.NPPM_GETPLUGINSCONFIGDIR);

		/// <summary>
		/// Open a file for editing in Notepad++, pretty much like using the app's
		/// File - Open menu.
		/// </summary>
		/// <param name="path">The path to the file to open.</param>
		/// <returns>True on success.</returns>
		public bool OpenFile(string path)
			=> Win32.SendMessage(
				PluginBase.nppData._nppHandle, (uint) NppMsg.NPPM_DOOPEN, Unused, path).ToInt32()
			!= 0;

		/// <summary>
		/// Gets the path of the current document.
		/// </summary>
		public unsafe string GetFilePath(IntPtr bufferId)
		{
			var path = new StringBuilder(2000);
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) NppMsg.NPPM_GETFULLPATHFROMBUFFERID, bufferId, path);
			return path.ToString();
		}

		public void SetCurrentLanguage(LangType language)
		{
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) NppMsg.NPPM_SETCURRENTLANGTYPE, Unused, (int) language);
		}

		/// <summary>
		/// open a standard save file dialog to save the current file<br></br>
		/// Returns true if the file was saved
		/// </summary>
		public bool SaveCurrentFile()
		{
			IntPtr result = Win32.SendMessage(PluginBase.nppData._nppHandle,
					(uint)(NppMsg.NPPM_SAVECURRENTFILEAS),
					0, 0);
			return result.ToInt32() == 1;
		}

		public void HideDockingForm(System.Windows.Forms.Form form)
		{
			Win32.SendMessage(PluginBase.nppData._nppHandle,
					(uint)(NppMsg.NPPM_DMMHIDE),
					0, form.Handle);
		}

		public void ShowDockingForm(System.Windows.Forms.Form form)
		{
			Win32.SendMessage(PluginBase.nppData._nppHandle,
					(uint)(NppMsg.NPPM_DMMSHOW),
					0, form.Handle);
		}

		public Color GetDefaultForegroundColor()
		{
			var rawColor = (int)Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETEDITORDEFAULTFOREGROUNDCOLOR, 0, 0);
			return Color.FromArgb(rawColor & 0xff, (rawColor >> 8) & 0xff, (rawColor >> 16) & 0xff);
		}

		public Color GetDefaultBackgroundColor()
		{
			var rawColor = (int)Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETEDITORDEFAULTBACKGROUNDCOLOR, 0, 0);
			return Color.FromArgb(rawColor & 0xff, (rawColor >> 8) & 0xff, (rawColor >> 16) & 0xff);
		}

		public System.Windows.Media.Color GetDefaultForeColor()
		{
			var rawColor = Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETEDITORDEFAULTFOREGROUNDCOLOR, 0, 0);
			return colorRefToColor(rawColor);
		}

		public System.Windows.Media.Color GetDefaultBackColor()
		{
			var rawColor = Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETEDITORDEFAULTBACKGROUNDCOLOR, 0, 0);
			return colorRefToColor(rawColor);
		}

		public bool IsDarkModeEnabled()
		{
			return Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ISDARKMODEENABLED, 0, 0).ToInt32() != 0;
		}

		public unsafe System.Windows.Media.Color[] GetDarkModeColors()
		{
			// TODO: When Notepad++ is updated, this size may need updating!
			const int NB_DARK_COLOR_REFS = 12;
			var colorRefs = new int[NB_DARK_COLOR_REFS];
			fixed (int* colorRefsPtr = colorRefs)
			{
				IntPtr success = Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETDARKMODECOLORS, NB_DARK_COLOR_REFS * sizeof(int), (IntPtr)colorRefsPtr);
				return colorRefs.Select(cr => colorRefToColor((IntPtr)cr)).ToArray();
			}
		}

		private static System.Windows.Media.Color colorRefToColor(IntPtr colorRef)
		{
			int rawColor = (int)colorRef;
			return System.Windows.Media.Color.FromRgb((byte)(rawColor & 0xff), (byte)((rawColor >> 8) & 0xff), (byte)((rawColor >> 16) & 0xff));
		}

		/// <summary>
		/// Figure out default N++ config file path<br></br>
		/// Path is usually -> .\Users\<username>\AppData\Roaming\Notepad++\plugins\config\
		/// </summary>
		public string GetConfigDirectory()
        {
			var sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
			return sbIniFilePath.ToString();
		}

		/// <summary>
		/// 3-int array: {major, minor, bugfix}<br></br>
		/// Thus GetNppVersion() would return {8, 5, 0} for version 8.5.0
		/// and {7, 7, 1} for version 7.7.1
		/// </summary>
		/// <returns></returns>
		public int[] GetNppVersion()
		{
			int version = Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETNPPVERSION, 0, 0).ToInt32();
			int major = version >> 16;
			int minor = Math.DivRem(version & 0xffff, 10, out int bugfix);
			if (minor == 0)
				(bugfix, minor) = (minor, bugfix);
			return new int[] { major, minor, bugfix };
        }

        public string[] GetOpenFileNames()
        {
            int nbFile = (int)Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETNBOPENFILES, 0, 0);

            using (ClikeStringArray cStrArray = new ClikeStringArray(nbFile, Win32.MAX_PATH))
            {
                if (Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETOPENFILENAMES, cStrArray.NativePointer, nbFile) != IntPtr.Zero)
                    return cStrArray.ManagedStringsUnicode.ToArray();
            }
            return null;
        }

		public void AddModelessDialog(IntPtr formHandle)
		{
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_MODELESSDIALOG, IntPtr.Zero, formHandle);
		}

        public void RemoveModelessDialog(IntPtr formHandle)
        {
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_MODELESSDIALOG, new IntPtr(1), formHandle);
        }

		/// <summary>
		/// the status bar is the bar at the bottom with the document type, EOL type, current position, line, etc.<br></br>
		/// Set the message for one of the sections of that bar.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="section"></param>
		public void SetStatusBarSection(string message, StatusBarSection section)
		{
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETSTATUSBAR, (int)section, message);
		}

        public unsafe bool AllocateIndicators(int numberOfIndicators, out int[] indicators)
		{
			indicators = null;
			if (numberOfIndicators < 1)
				return false;
			indicators = new int[numberOfIndicators];
			fixed (int * indicatorsPtr = indicators)
			{
				IntPtr success = Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ALLOCATEINDICATOR, (IntPtr)numberOfIndicators, (IntPtr)indicatorsPtr);
				for (int ii = 1; ii < numberOfIndicators; ii++)
					indicators[ii] = indicators[ii - 1] + 1;
				return success != IntPtr.Zero;
			}
		}

    }

	/// <summary>
	/// This class holds helpers for sending messages defined in the Resource_h.cs file. It is at the moment
	/// incomplete. Please help fill in the blanks.
	/// </summary>
	class NppResource
	{
		private const int Unused = 0;

		public void ClearIndicator()
		{
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) Resource.NPPM_INTERNAL_CLEARINDICATOR, Unused, Unused);
		}
	}
}
