using System;
using System.Collections.Generic;
using SkyFM.shared;

namespace SkyFM.agent
{
	internal class StreamsList
	{
		Dictionary<int, string[]> m_streams;
		int m_bitrate;

		private StreamsList()
		{
			m_streams = StreamsCache.Read( out m_bitrate );
		}

		public static StreamsList load()
		{
			return new StreamsList();
		}

		/// <summary>Return designated bytes per second in the streams.</summary>
		public int bytesPerSecond { get { return m_bitrate >> 3; } }

		/// <summary>Return stream list for specific channel.</summary>
		public string[] this[ int idChannel ]
		{ get {
			string[] res;
			if( m_streams.TryGetValue( idChannel, out res ) )
				return res;
			throw new Exception( "Channel id={0} was not found in the streams list".FormatWith( idChannel ) );
		} }
	}
}