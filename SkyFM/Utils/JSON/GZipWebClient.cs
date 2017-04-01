using System;
using System.Net;
using System.Security;
using ICSharpCode.SharpZipLib.GZip;

namespace SkyFM
{
	// http://www.sharpgis.net/post/2011/08/28/GZIP-Compressed-Web-Requests-in-WP7-Take-2.aspx

	public class GZipWebClient: WebClient
	{
		[SecuritySafeCritical]
		public GZipWebClient()
		{
		}

		protected override WebRequest GetWebRequest( Uri address )
		{
			var req = base.GetWebRequest( address );
			req.Headers[ HttpRequestHeader.AcceptEncoding ] = "gzip"; // Set GZIP header
			return req;
		}

		protected override WebResponse GetWebResponse( WebRequest request, IAsyncResult result )
		{
			WebResponse response = base.GetWebResponse( request, result );
			if( response.Headers[ HttpRequestHeader.ContentEncoding ] == "gzip" )
				return new GZipWebResponse( response ); // If gzipped response, uncompress
			else
				return response;
		}
	}

	internal class GZipWebResponse: WebResponse
	{
		WebResponse response;
		internal GZipWebResponse( WebResponse resp )
		{
			response = resp;
		}

		public override System.IO.Stream GetResponseStream()
		{
			return new GZipInputStream( response.GetResponseStream() );
		}

		public override long ContentLength { get { return -1; } }

		public override void Close()
		{
			response.Close();
		}
	}
}