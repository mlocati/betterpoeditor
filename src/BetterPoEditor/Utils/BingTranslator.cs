using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace BePoE.Utils
{
	static class BingTranslator
	{
		private class AccessToken
		{
			private string _tokenType;
			private string _accessToken;
			private DateTime _expiresOn;
			private string _scope;
			public AccessToken(string jsonString)
			{
				Jayrock.Json.JsonObject json = (Jayrock.Json.JsonObject)Jayrock.Json.Conversion.JsonConvert.Import(jsonString);
				this._tokenType = (string)json["token_type"];
				this._accessToken = (string)json["access_token"];
				this._expiresOn = DateTime.Now.AddSeconds(int.Parse((string)json["expires_in"]));
				this._scope = (string)json["scope"];
			}
			public bool Expired
			{
				get
				{
					return this._expiresOn <= DateTime.Now;
				}
			}
			public string Bearer
			{
				get
				{
					return this._accessToken;
				}
			}
		}
		private class ErrorResponseException : Exception
		{
			public readonly string Code;
			public readonly string Description;
			private ErrorResponseException(string code, string description)
				: base(description)
			{
				this.Code = code;
				this.Description = description;
			}
			public static ErrorResponseException Parse(string jsonString)
			{
				try
				{
					Jayrock.Json.JsonObject json = (Jayrock.Json.JsonObject)Jayrock.Json.Conversion.JsonConvert.Import(jsonString);
					string code = (string)json["error"]; //invalid_client
					string description = (string)json["error_description"]; //ACS50012: Authentication failed. \r\nTrace ID: 5f36fac3-afe4-4e54-95ff-6f9d55be786e\r\nTimestamp: 2012-10-18 20:06:12Z
					if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(description))
					{
						return null;
					}
					else
					{
						return new ErrorResponseException(code, description);
					}
				}
				catch
				{
					return null;
				}
			}
			public bool IsAuthenticationFailed
			{
				get
				{
					return this.Description.StartsWith("ACS50012:");
				}
			}
		}
		private static Dictionary<string, AccessToken> _loadedTokens = new Dictionary<string, AccessToken>();
		private static AccessToken Token
		{
			get
			{
				return BingTranslator.GetAccessToken(Program.Vars.BingTranslatorClientID, Program.Vars.BingTranslatorClientSecret);
			}
		}
		public static void CheckAccessParameters(string clientID, string clientSecret)
		{
			BingTranslator.GetAccessToken(clientID, clientSecret);
		}
		private static AccessToken GetAccessToken(string clientID, string clientSecret)
		{
			if (string.IsNullOrEmpty(clientID) && string.IsNullOrEmpty(clientSecret))
			{
				throw new Exception("To enable Bing translation you have to specify the Client ID and the Client Secret in the options.");
			}
			if (string.IsNullOrEmpty(clientID) || string.IsNullOrEmpty(clientSecret))
			{
				throw new Exception("To enable Bing translation you have to specify both the Client ID and the Client Secret in the options.");
			}
			string key = string.Format("{0}\x01{1}", clientID, clientSecret);
			if (BingTranslator._loadedTokens.ContainsKey(key) && !BingTranslator._loadedTokens[key].Expired)
			{
				return BingTranslator._loadedTokens[key];
			}
			else
			{
				Dictionary<string, string> fields = new Dictionary<string, string>();
				fields.Add("grant_type", "client_credentials");
				fields.Add("client_id", clientID);
				fields.Add("client_secret", clientSecret);
				fields.Add("scope", "http://api.microsofttranslator.com");
				StringBuilder f = null;
				foreach (KeyValuePair<string, string> kv in fields)
				{
					if (f == null)
					{
						f = new StringBuilder();
					}
					else
					{
						f.Append('&');
					}
					f.Append(HttpUtility.UrlEncode(kv.Key)).Append('=').Append(HttpUtility.UrlEncode(kv.Value));
				}
				WebRequest request = WebRequest.Create("https://datamarket.accesscontrol.windows.net/v2/OAuth2-13");
				request.ContentType = "application/x-www-form-urlencoded";
				request.Method = WebRequestMethods.Http.Post;
				byte[] bytes = System.Text.Encoding.ASCII.GetBytes(f.ToString());
				request.ContentLength = bytes.Length;
				using (Stream requestStream = request.GetRequestStream())
				{
					requestStream.Write(bytes, 0, bytes.Length);
				}
				WebResponse response = null;
				try
				{
					Exception error;
					try
					{
						response = request.GetResponse();
						error = null;
					}
					catch (WebException x)
					{
						response = x.Response;
						error = x;
					}
					string rs;
					using (Stream responseStream = response.GetResponseStream())
					{
						using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8, false))
						{
							rs = reader.ReadToEnd();
						}
					}
					if (error == null)
					{
						AccessToken result = new AccessToken(rs);
						BingTranslator._loadedTokens.Add(key, result);
						return result;
					}
					else
					{
						ErrorResponseException er = ErrorResponseException.Parse(rs);
						throw (er == null) ? error : er;
					}
				}
				catch
				{
					throw;
				}
				finally
				{
					if (response != null)
					{
						try
						{ response.Close(); }
						catch
						{ }
						response = null;
					}
				}
			}
		}

		public static string Translate(string langFrom, string langTo, string text)
		{
			Dictionary<string, string> fields = new Dictionary<string, string>();
			string authorizationHeader = string.Format("Bearer {0}", BingTranslator.Token.Bearer);
			fields.Add("text", text);
			fields.Add("from", langFrom);
			fields.Add("to", langTo);
			StringBuilder f = null;
			foreach (KeyValuePair<string, string> kv in fields)
			{
				if (f == null)
				{
					f = new StringBuilder("http://api.microsofttranslator.com/V2/Ajax.svc/Translate?");
				}
				else
				{
					f.Append('&');
				}
				f.Append(HttpUtility.UrlEncode(kv.Key)).Append('=').Append(HttpUtility.UrlEncode(kv.Value));
			}
			WebRequest request = WebRequest.Create(f.ToString());
			request.Method = WebRequestMethods.Http.Get;
			request.Headers.Add("Authorization", authorizationHeader);
			WebResponse response = null;
			try
			{
				Exception error;
				try
				{
					response = request.GetResponse();
					error = null;
				}
				catch (WebException x)
				{
					response = x.Response;
					error = x;
				}
				string rs;
				using (Stream responseStream = response.GetResponseStream())
				{
					using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8, false))
					{
						rs = reader.ReadToEnd();
					}
				}
				if (error == null)
				{
					return (string)Jayrock.Json.Conversion.JsonConvert.Import(rs);
				}
				else
				{
					throw error;
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				if (response != null)
				{
					try
					{ response.Close(); }
					catch
					{ }
					response = null;
				}
				}
		}
	}
}
