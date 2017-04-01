using System;
using System.Net;
using System.Collections.Generic;

namespace SkyFM.agent.MSS
{
	internal static class HttpUtils
	{
		/// <summary>Convert HTTP response headers into dictionary.</summary>
		/// <param name="resp"></param>
		/// <returns></returns>
		public static Dictionary<string, string> headersDictionary( this HttpWebResponse resp )
		{
			var headers = resp.Headers;
			string[] allKeys = headers.AllKeys;
			Dictionary<string, string> res= new Dictionary<string, string>( allKeys.Length );
			foreach (string key in allKeys)
			{
				string val = headers[ key ];
				res[ key.ToLowerInvariant() ] = val;
			}
			return res;
		}

		public static bool readKey( this Dictionary<string, string> dic, string key, out string val )
		{
			if( !dic.TryGetValue( key, out val ) )
				return false;
			if( String.IsNullOrEmpty( val ) )
			{
				val = null;
				return false;
			}
			return true;
		}

		public static bool readKey( this Dictionary<string, string> dic, string key, out int ival )
		{
			ival = 0;
			string val;
			if( !dic.TryGetValue( key, out val ) )
				return false;
			if( String.IsNullOrEmpty( val ) )
				return false;
			return int.TryParse( val, out ival );
		}
	}
}