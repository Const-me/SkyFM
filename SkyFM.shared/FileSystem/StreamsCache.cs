using System.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SkyFM.shared
{
	/// <summary>This static class provides access to the stream lists.</summary>
	/// <remarks>The UTF8 plain-text file "streams.lst" contains streams for every channel with a given quality.
	/// If the UI changes the stream quality, the agent must re-read the "streams.lst", this is achieved by passing "&rld=1" when opening a track.</remarks>
	public static class StreamsCache
	{
		static readonly FileSystemFile s_file = new FileSystemFile( "streams.cfg", "5328b32f-0f74-4dc4-a498-56216af40c8f" );

		/// <summary>Read the stream lists from the isolated storage.</summary>
		/// <param name="bitsPerSecond">Bits per second in the streams.</param>
		/// <returns>Stream URIs for each channel.</returns>
		public static Dictionary<int, string[]> Read( out int bitsPerSecond )
		{
			Dictionary<int, string[]> res = null;
			int bitrate = 0;

			s_file.readText( txtReader =>
			{
				string line = txtReader.ReadLine();
				bitrate = int.Parse( line );

				res = new Dictionary<int, string[]>( 64 );

				int? currChannel = null;
				List<string> streams = new List<string>( 8 );
				while( !txtReader.EndOfStream )
				{
					line = txtReader.ReadLine();

					if( String.IsNullOrEmpty( line ) )
					{
						if( currChannel.HasValue && streams.Count > 0 )
							res.Add( currChannel.Value, streams.ToArray() );
						streams.Clear();
						currChannel = null;
						continue;
					}

					if( !currChannel.HasValue )
					{
						currChannel = int.Parse( line );
						continue;
					}
					streams.Add( line );
				}

			} );
			bitsPerSecond = bitrate;
			return res;
		}

		/// <summary>Write streams list to the isolated storage.</summary>
		/// <param name="bitsPerSecond">Bits per second in the streams.</param>
		/// <param name="vals">Stream URIs for each channel.</param>
		public static void Write( int bitsPerSecond, ILookup<int, string> vals )
		{
			s_file.writeText( textStream =>
			{
				textStream.WriteLine( "{0}", bitsPerSecond );
				foreach( var g in vals )
				{
					textStream.WriteLine( "{0}", g.Key );
					foreach( var s in g )
						textStream.WriteLine( "{0}", s );
					textStream.WriteLine();
				}
			} );
		}
	}
}