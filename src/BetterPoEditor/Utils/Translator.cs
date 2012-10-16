using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net;
using System.Net.Security;
using System.Web;
using Jayrock.Json;
using Jayrock.Json.Conversion;

namespace BePoE.Utils
{
	public static class Translator
	{
		public static string Referer = "about:blank";
		public static string Translate(string fromLangId, string toLangId, string textToTranslate)
		{
			if (string.IsNullOrEmpty(fromLangId))
				throw new ArgumentNullException("fromLangId");
			if (string.IsNullOrEmpty(toLangId))
				throw new ArgumentNullException("toLangId");
			if (string.IsNullOrEmpty(textToTranslate))
				return textToTranslate;
			if (fromLangId.Equals(toLangId, StringComparison.InvariantCultureIgnoreCase))
				return textToTranslate;
			HttpWebResponse ans = null;
			try
			{
				string url = string.Format("http://ajax.googleapis.com/ajax/services/language/translate?langpair={0}%7c{1}&q={2}&v=1.0", HttpUtility.UrlEncode(fromLangId), HttpUtility.UrlEncode(toLangId), HttpUtility.UrlEncodeUnicode(textToTranslate));
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
				req.Method = "GET";
				req.KeepAlive = true;
				req.AllowAutoRedirect = true;
				req.AllowWriteStreamBuffering = false;
				req.AuthenticationLevel = AuthenticationLevel.None;
				req.Referer = Translator.Referer;
				req.UserAgent = "";
				try
				{
					ans = (HttpWebResponse)req.GetResponse();
				}
				catch (WebException e)
				{
					ans = (HttpWebResponse)e.Response;
				}
				Encoding encoding = Encoding.ASCII;
				string contentType = ans.Headers["Content-Type"];
				if (!string.IsNullOrEmpty(contentType))
				{
					Match m = Regex.Match(contentType, @"(^|[\W])charset\s*=\s*(?<cs>[\w\-]+)", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
					if (m.Success)
					{
						try
						{ encoding = Encoding.GetEncoding(m.Groups["cs"].Value.ToLowerInvariant()); }
						catch
						{ }
					}
				}
				byte[] buffer;
				using (Stream stream = ans.GetResponseStream())
				{
					using (BinaryReader br = new BinaryReader(stream))
					{
						if (ans.ContentLength > 0L)
						{
							buffer = br.ReadBytes(Convert.ToInt32(ans.ContentLength));
						}
						else
						{
							List<byte> bufferBuilder = new List<byte>();
							for (; ; )
							{
								byte[] b1 = br.ReadBytes(512);
								if ((b1 == null) || (b1.Length == 0))
									break;
								bufferBuilder.AddRange(b1);
							}
							buffer = bufferBuilder.ToArray();
						}
					}
				}
				string text = encoding.GetString(buffer);
				JsonObject obj = (JsonObject)JsonConvert.Import(text);
				string responseStatus = obj["responseStatus"] as string;
				if ((!string.IsNullOrEmpty(responseStatus)) && (!responseStatus.Equals("200")))
					throw new Exception(string.Format("Response status: {0}", responseStatus));
				JsonObject responseData = obj["responseData"] as JsonObject;
				if (responseData == null)
					throw new Exception("No response.data.");
				string translatedText = responseData["translatedText"] as string;
				if (string.IsNullOrEmpty(translatedText))
					throw new Exception("No translated text.");
				return translatedText;
			}
			catch
			{
				throw;
			}
			finally
			{

				if (ans != null)
				{
					try
					{ ans.Close(); }
					catch
					{ }
					ans = null;
				}
			}
		}
	}
}
