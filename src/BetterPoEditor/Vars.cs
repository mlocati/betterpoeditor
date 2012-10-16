using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BePoE
{
	public enum ColorPlace : int
	{
		ForeUnsel = 0,
		BackUnsel = 1,
		ForeSel = 2,
		BackSel = 3,
	}
	public enum ColorEnviro : int
	{
		Untranslated = 0,
		Translated = 1,
		Removed = 2,
	}

	[Serializable]
	public struct ColorEnviroPlace
	{
		public ColorEnviro Enviro;
		public ColorPlace Place;
		public ColorEnviroPlace(ColorEnviro enviro, ColorPlace place)
		{
			this.Enviro = enviro;
			this.Place = place;
		}
		public static ColorEnviroPlace Get(ColorEnviro enviro, ColorPlace place)
		{
			return new ColorEnviroPlace(enviro, place);
		}
	}

	public class Vars : ApplicationSettingsBase
	{
		private static bool? _osIsCaseSensitive = null;
		public static bool OsIsCaseSensitive
		{
			get
			{
				if (!Vars._osIsCaseSensitive.HasValue)
				{
					string tempFolder = Path.GetTempPath();
					string testFileNameLC, testFileNameUC;
					for (int i = 0; ; i++)
					{
						string lc = string.Format("tst{0}", i);
						testFileNameLC = Path.Combine(tempFolder, lc);
						testFileNameUC = Path.Combine(tempFolder, lc.ToUpper());
						if (!(File.Exists(testFileNameLC) || File.Exists(testFileNameUC) || Directory.Exists(testFileNameLC) || Directory.Exists(testFileNameUC)))
							break;
					}
					bool created = false;
					try
					{
						using (FileStream fs = new FileStream(testFileNameLC, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
						{
							created = true;
							fs.WriteByte(32);
							fs.Flush();
						}
						Vars._osIsCaseSensitive = !File.Exists(testFileNameUC);
					}
					finally
					{
						if (created)
						{
							try
							{ File.Delete(testFileNameLC); }
							catch
							{ }
						}
					}
				}
				return Vars._osIsCaseSensitive.Value;
			}
		}

		public static bool IsPosix
		{
			get
			{
				const int PLATFORMID_MACOSX = 6;
				switch (Environment.OSVersion.Platform)
				{
					case (PlatformID)PLATFORMID_MACOSX:
					case PlatformID.Unix:
						return true;
					default:
						switch ((int)Environment.OSVersion.Platform)
						{
							case 128:
								return true;
							default:
								return false;
						}
				}
			}
		}
		[UserScopedSetting]
		public string[] LastFiles
		{
			get
			{
				string[] ris = this["LastFiles"] as string[];
				return (ris == null) ? new string[] { } : ris;
			}
			set
			{
				this["LastFiles"] = (value == null) ? new string[] { } : value;
			}
		}
		public List<string> RemoveLastFile(string fileName, bool save)
		{
			List<string> list = new List<string>(this.LastFiles);

			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (fileName.Equals(list[i], Vars.OsIsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
					list.RemoveAt(i);
			}
			if (save)
			{
				this.LastFiles = list.ToArray();
				this.Save();
			}
			return list;
		}
		public void AddLastFile(string fileName)
		{
			List<string> list = this.RemoveLastFile(fileName, false);
			list.Insert(0, fileName);
			this.LastFiles = list.ToArray();
			this.Save();
		}
		public void RemoveLastFile(string fileName)
		{
			this.RemoveLastFile(fileName, true);
		}

		[UserScopedSetting]
		public string[] FlattenColors
		{
			get
			{
				return this["FlattenColors"] as string[];
			}
			set
			{
				this["FlattenColors"] = value;
			}
		}

		[UserScopedSetting]
		public Font SourceFont
		{
			get
			{
				if ((this["SourceFont"] as Font) == null)
					this["SourceFont"] = Vars.GetStandardSourceFont();
				return (Font)this["SourceFont"];
			}
			set
			{
				this["SourceFont"] = value;
			}
		}
		public static Font GetStandardTranslatedFont()
		{
			try
			{ return new Font("Courier New", 10f); }
			catch
			{
				return new Font(FontFamily.GenericMonospace, 10f);
			}
		}

		[UserScopedSetting]
		public Font TranslatedFont
		{
			get
			{
				if ((this["TranslatedFont"] as Font) == null)
					this["TranslatedFont"] = Vars.GetStandardTranslatedFont();
				return (Font)this["TranslatedFont"];
			}
			set
			{
				this["TranslatedFont"] = value;
			}
		}
		public static Font GetStandardSourceFont()
		{
			try
			{ return new Font("Courier New", 10f); }
			catch
			{
				return new Font(FontFamily.GenericMonospace, 10f);
			}
		}

		[UserScopedSetting, DefaultSettingValue("")]
		public string ViewerPath
		{
			get
			{
				string s = this["ViewerPath"] as string;
				return (s == null) ? "" : s;
			}
			set
			{
				this["ViewerPath"] = (value == null) ? "" : value;
			}
		}

		[UserScopedSetting, DefaultSettingValue("")]
		public string ViewerParameters
		{
			get
			{
				string s = this["ViewerParameters"] as string;
				return (s == null) ? "" : s;
			}
			set
			{
				this["ViewerParameters"] = (value == null) ? "" : value;
			}
		}

		[UserScopedSetting]
		public bool CompileOnSave
		{
			get
			{
				object o = this["CompileOnSave"];
				return ((o == null) || (o.GetType() != typeof(bool))) ? false : (bool)o;
			}
			set
			{
				this["CompileOnSave"] = value;
			}
		}
		[UserScopedSetting, DefaultSettingValue("")]
		public string MOCompilerPath
		{
			get
			{
				string s = this["MOCompilerPath"] as string;
				return (s == null) ? "" : s;
			}
			set
			{
				this["MOCompilerPath"] = (value == null) ? "" : value;
			}
		}
		[UserScopedSetting, DefaultSettingValue("")]
		public string MOCompilerParameters
		{
			get
			{
				string s = this["MOCompilerParameters"] as string;
				return (s == null) ? "" : s;
			}
			set
			{
				this["MOCompilerParameters"] = (value == null) ? "" : value;
			}
		}

		[UserScopedSetting, DefaultSettingValue("2000")]
		public int MaxSearchResults
		{
			get
			{
				object o = this["MaxSearchResults"];
				if (o is int)
				{
					int i = (int)o;
					if (i > 0)
						return i;
				}
				return 2000;
			}
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("MaxSearchResults");
				this["MaxSearchResults"] = value;
			}
		}


		private Dictionary<ColorEnviroPlace, Color> _colors = null;
		public Dictionary<ColorEnviroPlace, Color> Colors
		{
			get
			{
				if (this._colors == null)
				{
					Dictionary<ColorEnviroPlace, Color> ris = null;
					string[] flattenColors = this.FlattenColors;
					if (flattenColors != null)
					{
						ris = new Dictionary<ColorEnviroPlace, Color>();
						Regex rx = new Regex(@"^(?<enviro>[0-9]+)\|(?<place>[0-9]+)\:(?<r>[0-9]+)\,(?<g>[0-9]+)\,(?<b>[0-9]+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
						foreach (string flattenColor in flattenColors)
						{
							Match match = rx.Match(flattenColor);
							if (match.Success)
							{
								ColorEnviroPlace key = ColorEnviroPlace.Get((ColorEnviro)int.Parse(match.Groups["enviro"].Value), (ColorPlace)int.Parse(match.Groups["place"].Value));
								if (!ris.ContainsKey(key))
									ris.Add(key, Color.FromArgb(int.Parse(match.Groups["r"].Value), int.Parse(match.Groups["g"].Value), int.Parse(match.Groups["b"].Value)));
							}
						}
						foreach (ColorPlace place in Enum.GetValues(typeof(ColorPlace)))
						{
							foreach (ColorEnviro enviro in Enum.GetValues(typeof(ColorEnviro)))
							{
								if (!ris.ContainsKey(ColorEnviroPlace.Get(enviro, place)))
								{
									ris = null;
									break;
								}
							}
							if (ris == null)
								break;
						}
					}
					if (ris == null)
					{
						ris = new Dictionary<ColorEnviroPlace, Color>(Vars.DefaultColors.Count);
						foreach (KeyValuePair<ColorEnviroPlace, Color> kv in Vars.DefaultColors)
						{
							ris.Add(kv.Key, kv.Value);
						}
					}
					this._colors = ris;
				}
				return this._colors;
			}
		}

		public override void Save()
		{
			List<string> colors = new List<string>(this.Colors.Count);
			foreach (KeyValuePair<ColorEnviroPlace, Color> kv in this.Colors)
				colors.Add(string.Format("{0}|{1}:{2},{3},{4}", (int)kv.Key.Enviro, (int)kv.Key.Place, kv.Value.R, kv.Value.G, kv.Value.B));
			this["FlattenColors"] = colors.ToArray();
			base.Save();
		}

		private static Dictionary<ColorEnviroPlace, Color> _defaultColors = null;
		public static Dictionary<ColorEnviroPlace, Color> DefaultColors
		{
			get
			{
				if (Vars._defaultColors == null)
				{
					Dictionary<ColorEnviroPlace, Color> C = new Dictionary<ColorEnviroPlace, Color>();

					foreach (ColorPlace place in Enum.GetValues(typeof(ColorPlace)))
					{
						foreach (ColorEnviro enviro in Enum.GetValues(typeof(ColorEnviro)))
						{
							switch (enviro)
							{
								case ColorEnviro.Untranslated:
									switch (place)
									{
										case ColorPlace.ForeUnsel:
										case ColorPlace.BackSel:
											C.Add(ColorEnviroPlace.Get(enviro, place), Color.FromArgb(92, 0, 0));
											break;
										case ColorPlace.BackUnsel:
										case ColorPlace.ForeSel:
											C.Add(ColorEnviroPlace.Get(enviro, place), Color.FromArgb(255, 240, 240));
											break;
										default:
											throw new Exception();
									}
									break;
								case ColorEnviro.Translated:
									switch (place)
									{
										case ColorPlace.ForeUnsel:
										case ColorPlace.BackSel:
											C.Add(ColorEnviroPlace.Get(enviro, place), Color.FromArgb(0, 92, 0));
											break;
										case ColorPlace.BackUnsel:
										case ColorPlace.ForeSel:
											C.Add(ColorEnviroPlace.Get(enviro, place), Color.FromArgb(240, 255, 240));
											break;
										default:
											throw new Exception();
									}
									break;
								case ColorEnviro.Removed:
									switch (place)
									{
										case ColorPlace.ForeUnsel:
										case ColorPlace.BackSel:
											C.Add(ColorEnviroPlace.Get(enviro, place), Color.FromArgb(92, 92, 92));
											break;
										case ColorPlace.BackUnsel:
										case ColorPlace.ForeSel:
											C.Add(ColorEnviroPlace.Get(enviro, place), Color.FromArgb(240, 240, 240));
											break;
										default:
											throw new Exception();
									}
									break;
								default:
									throw new Exception();
							}
						}
					}
					Vars._defaultColors = C;
				}
				return Vars._defaultColors;
			}
		}
	}
}
