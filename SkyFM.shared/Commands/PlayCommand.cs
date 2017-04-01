using System;
using System.Text;
using System.Net;
using System.Collections.Generic;

namespace SkyFM.shared
{
	public class PlayCommand
	{
		public static string construct( int idChannel, string listenKey, bool forceReload, bool energySaver )
		{
			StringBuilder sb = new StringBuilder( 64 );
			sb.AppendFormat( "ch={0}", idChannel );

			if( !String.IsNullOrEmpty( listenKey ) )
			{
				sb.AppendFormat( "&key={0}", HttpUtility.UrlEncode( listenKey ) );
			}

			if( forceReload )
				sb.Append( "&rld=1" );
			if( energySaver )
				sb.Append( "&es=1" );

			// Example URI
			// "ch=38&key=bd529fa4639a04&rld=1"
			return sb.ToString();
		}

		// public readonly string qualityKey;
		public readonly int idChannel;
		public readonly string listenKey;
		public readonly bool forceReload;
		public readonly bool energySaver;

		public PlayCommand( string contents )
		{
			var args = SharedUtils.parseQueryString( contents );

			this.idChannel = int.Parse( args[ "ch" ] );

			if( args.ContainsKey( "key" ) )
			{
				string val = args[ "key" ];
				if( !String.IsNullOrEmpty( val ) )
					this.listenKey = val;
			}

			if( args.ContainsKey( "rld" ) && "1" == args[ "rld" ] )
				this.forceReload = true;
			else
				this.forceReload = false;

			if( args.ContainsKey( "es" ) && "1" == args[ "es" ] )
				this.energySaver = true;
			else
				this.energySaver = false;
		}
	}
}