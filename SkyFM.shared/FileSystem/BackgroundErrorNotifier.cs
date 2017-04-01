using System;

namespace SkyFM.shared
{
	/// <summary>This static class allows background process to report an error, and GUI process poll for errors.</summary>
	/// <remarks>The errors expire after 10 minutes.</remarks>
	public static class BackgroundErrorNotifier
	{
		static readonly FileSystemFile s_file = new FileSystemFile( "backgroundError.msg", "679fc3a5-1c90-495e-949c-bf7d2429589}" );
		static readonly TimeSpan s_errorExpiresAfter = TimeSpan.FromMinutes( 10 );

		public static void addError( Exception ex )
		{
			addError( ex.Message );
		}

		public static void addError( string reason )
		{
			s_file.writeText( writer =>
			{
				writer.WriteLine( "{0:s}", DateTime.UtcNow );
				writer.WriteLine( reason );
			} );
		}

		public static string getError()
		{
			string res = null;
			s_file.readText( reader =>
			{
				string dt = reader.ReadLine();
				if( null == dt )
					return;
				DateTime ts = DateTime.ParseExact( dt, "s", null );
				if( ts + s_errorExpiresAfter < DateTime.UtcNow )
					return;
				res = reader.ReadLine();
			}, true );
			return res;
		}
	}
}