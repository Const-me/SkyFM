using System;
using System.Text;
using System.Collections.Generic;
using System.Net;

namespace SkyFM.shared
{
	/// <summary>This readonly class contains all the fields that could be found in the audio track's "Tag" property.</summary>
	/// <remarks>Additionally, it provides static methods to construct valid "Tag" values.</remarks>
	public class TagFields
	{
		public readonly int idChannel;

		// public readonly string listenKey;
		// public readonly bool forceReload;

		readonly byte bufferFullness;
		public readonly long networkBytes;
		public readonly bool isPending;
		public readonly string error;

		/// <summary>Construct fields from the dictionary</summary>
		/// <param name="args"></param>
		public TagFields( Dictionary<string, string> args )
		{
			this.idChannel = int.Parse( args[ "ch" ] );

			// this.listenKey = args.itemOrDefault( "key" );
			// this.forceReload = args.itemOrDefault( "rld", val => "1" == val );

			this.bufferFullness = args.itemOrDefault( "cb", val => byte.Parse( val ) );
			this.networkBytes = args.itemOrDefault( "net", val => long.Parse( val ) );
			this.error = args.itemOrDefault( "err" );
			this.isPending = args.itemOrDefault( "p", val => "1" == val );
		}

		public static string statusMessage( int idChannel, bool bPending, byte buff, long networkBytes, string errorText )
		{
			// eg: ch=12&cb=123&net=5642561602&err=network_error
			StringBuilder sb = new StringBuilder( 64 );
			sb.AppendFormat( "ch={0}&cb={1}&net={2}",
				idChannel, buff, networkBytes );
			if( bPending )
				sb.Append( "&p=1" );
			if( !String.IsNullOrEmpty( errorText ) )
				sb.AppendFormat( "&err={0}", HttpUtility.UrlEncode( errorText ) );
			return sb.ToString();
		}

		public double bufferPercentage { get {
			return percentage( this.bufferFullness );
		} }

		public static double percentage( byte buff )
		{
			return (double)( buff ) / 255.0;
		}
	}
}