using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BePoE
{
	static class Program
	{
		public const string VERSION = "1.1";
		public static StringBuilder global = new StringBuilder();
		internal static Vars Vars;
		internal static Utils.SpellChecker SpellChecker;
		internal static string InitialFile;
		[STAThread]
		static int Main(string[] args)
		{
			try
			{
				Application.EnableVisualStyles();
			}
			catch (TypeInitializationException)
			{
				Console.WriteLine(string.Format("{0} must me run in a graphical environment.", Application.ProductName));
				return 1;
			}
			Application.SetCompatibleTextRenderingDefault(false);
			Program.InitialFile = ((args == null) || (args.Length == 0)) ? null : args[0];
			string dictionariesFolder = Path.Combine(new FileInfo(Application.ExecutablePath).Directory.FullName, "dictionaries");
			if (!Directory.Exists(dictionariesFolder))
				Directory.CreateDirectory(dictionariesFolder);
			Program.SpellChecker = new Utils.SpellChecker(dictionariesFolder);
			Program.Vars = new Vars();
			if ((Program.Vars.MOCompilerPath.Length == 0) && (Program.Vars.MOCompilerParameters.Length == 0))
			{
				Program.Vars.MOCompilerParameters = "\"%source%\" --output-file=\"%destination%\"";
				if (Vars.IsPosix)
					Program.Vars.MOCompilerPath = "/usr/bin/msgfmt";
				else
					Program.Vars.MOCompilerPath = Path.Combine(Path.Combine(new FileInfo(Application.ExecutablePath).Directory.FullName, "tools"), "msgfmt.exe");
				Program.Vars.CompileOnSave = File.Exists(Program.Vars.MOCompilerPath);
				Program.Vars.Save();
			}
			if ((Program.Vars.ViewerPath.Length == 0) && (Program.Vars.ViewerParameters.Length == 0))
			{
				string programPath;
				if (Vars.IsPosix)
				{
					programPath = "/usr/bin/gedit";
					if (File.Exists(programPath))
					{
						Program.Vars.ViewerPath = programPath;
						Program.Vars.ViewerParameters = "+%line% \"%file%\"";
					}
					else
					{
						programPath = "/usr/bin/kedit";
						if (File.Exists(programPath))
						{
							Program.Vars.ViewerPath = programPath;
							Program.Vars.ViewerParameters = "\"%file%\"";
						}
						else
						{
							Program.Vars.ViewerParameters = "\"%file%\"";
						}
					}
				}
				else
				{
					programPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), string.Format("Notepad++{0}notepad++.exe", Path.DirectorySeparatorChar));
					if (File.Exists(programPath))
					{
						Program.Vars.ViewerPath = programPath;
						Program.Vars.ViewerParameters = "-n%line% \"%file%\"";
					}
					else
					{
						programPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "notepad.exe");
						if (File.Exists(programPath))
							Program.Vars.ViewerPath = programPath;
						Program.Vars.ViewerParameters = "\"%file%\"";
					}
				}
			}
			using (UI.frmMain frm = new UI.frmMain())
			{
				Application.Run(frm);
			}
			return 0;
		}
	}
}