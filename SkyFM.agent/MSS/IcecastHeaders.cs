using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;

namespace SkyFM.agent.MSS
{
	/// <summary>This readonly class holds parsed collection of IceCast-specific HTTP headers.</summary>
	/// <remarks>It also works a class factory to create the correct StreamFormat-derived class based on the stream data.</remarks>
	class IcecastHeaders
	{
		public readonly string contentType, genre, name, notice1, notice2, url;
		public readonly int bitrate, metadataInterval;

		public IcecastHeaders( HttpWebResponse resp )
		{
			contentType = resp.ContentType;

			Dictionary<string, string> headers = resp.headersDictionary();
			headers.readKey( "icy-genre", out genre );
			headers.readKey( "icy-name", out name );
			headers.readKey( "icy-notice1", out notice1 );
			headers.readKey( "icy-notice2", out notice2 );
			headers.readKey( "icy-url", out url );
			headers.readKey( "icy-br", out bitrate );
			headers.readKey( "icy-metaint", out metadataInterval );
		}

		public StreamFormat createStreamParser( iLocator locator )
		{
			if( this.contentType == "audio/aac" || this.contentType == "audio/aacp" )
				return new HeAacStreamFormat( locator );
			if( this.contentType == "audio/mpeg" )
				return new MpegStreamFormat( locator );
			throw new NotSupportedException( "Unsupported content type \"{0}\"".FormatWith( this.contentType ) );
		}
	}
}