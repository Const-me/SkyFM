using System;
using System.Text;
using System.Net;

namespace SkyFM.shared
{
	public static class NowPlaying
	{
		static readonly FileSystemFile s_file = new FileSystemFile( "play.cmd", "65e4dcf0-ec6a-49d1-8c4f-5df0c429fb55" );

		public static void write( int idChannel, string listenKey, bool forceReload, bool energySaver )
		{
			string cmd = PlayCommand.construct( idChannel, listenKey, forceReload, energySaver );
			s_file.writeString( cmd );
		}

		public static PlayCommand read()
		{
			string contents = s_file.readString( false );
			if( String.IsNullOrEmpty( contents ) )
				return null;
			return new PlayCommand( contents );
		}
	}
}