using System;
using System.IO;
using System.IO.IsolatedStorage;
using SkyFM.shared;

namespace SkyFM
{
	/// <summary>This static class allows both processes to report fatal exception to the developer.</summary>
	public static class ExceptionLogger
	{
		static readonly FileSystemFile s_file = new FileSystemFile( "crash.log", "e5b38d77-f67a-45eb-a0e8-40dd6f07214b" );

		static bool s_bLogged = false;

		/// <summary>Log the exception to the crash.log isolated storage file.</summary>
		/// <param name="ex">The exception to log.</param>
		/// <param name="extraInfo">Extra information to log.</param>
		public static void logToFile( this Exception ex, string extraInfo )
		{
			if( !s_bLogged )
			{
				s_file.writeText( stm =>
				{
					stm.WriteLine( "{0}", extraInfo );
					stm.WriteLine( "{0}", SharedUtils.getCurrentVersion().ToString() );
					stm.WriteLine( ex.ToString() );
				} );
				s_bLogged = true;
			}

			ex.log( "{0}", extraInfo );
		}

		// <summary>Check if the crash.log file exists in the isolated storage. If its there, read and delete.</summary>
		// <remarks>The action is only executed if there's a file.</remarks>
		// <param name="act">Action to execute with the contents of the crash.log file.</param>
		// <returns>True if there was the crash log file.</returns>
		public static bool onPrevCrash( Action<string> act )
		{
			string contents = s_file.readString( true );
			if( !String.IsNullOrEmpty( contents ) )
			{
				if( null != act )
					act( contents );
				return true;
			}
			return false;
		}
	}
}