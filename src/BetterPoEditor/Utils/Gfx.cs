using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;

namespace BePoE.Utils
{
	internal static class Gfx
	{
		private static bool _appIconAlreadyRetrieved = false;
		private static Icon _appIcon = null;
		public static Icon AppIcon
		{
			get
			{
				if (!Gfx._appIconAlreadyRetrieved)
				{
					try
					{ Gfx._appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
					catch
					{ Gfx._appIcon = null; }
					finally
					{ Gfx._appIconAlreadyRetrieved = true; }
				}
				return Gfx._appIcon;
			}
		}
		public static void SetFormIcon(Form form)
		{
			try
			{
				if ((form != null) && (Gfx.AppIcon != null))
					form.Icon = Gfx.AppIcon;
			}
			catch
			{ }
		}
	}
}
