using System;
using Microsoft.Phone.BackgroundAudio;
using SkyFM.shared;

namespace SkyFM.agent
{
	/// <summary>This class caches stream list for the channel, and implements failover logic.</summary>
	internal class ChannelStreams
	{
		/// <summary>Construct the stream list</summary>
		/// <param name="track">Track with the play command in the tag.</param>
		public ChannelStreams( PlayCommand playCommand )
		{
			m_listenKey = playCommand.listenKey;
			m_idChannel = playCommand.idChannel;

			// Reload the stream lists from the file system
			if( null == s_streams || playCommand.forceReload )
				s_streams = StreamsList.load();

			// Cache streams data to readonly properties.
			m_bytesPerSecond = s_streams.bytesPerSecond;
			m_streams = s_streams[ playCommand.idChannel ];
			m_streamStatuses = new eStreamStatus[ m_streams.Length ];
			m_currentIndex = 0;
			Logger.info( "Loaded {0} streams, first one is {1}", m_streams.Length, m_streams[ 0 ] );
		}

		static StreamsList s_streams = null;

		readonly int m_idChannel;
		public readonly int m_bytesPerSecond;
		readonly string[] m_streams;
		readonly string m_listenKey;

		[Flags]
		enum eStreamStatus: byte
		{
			/// <summary> haven't tried to open this stream</summary>
			unknown = 0,	//< the default, see http://stackoverflow.com/a/529937/126995

			/// <summary>At least one sample returned from SamplesSource.</summary>
			play = 0x1,

			/// <summary>Fatal error: exception from open or before the first getSample is completed.</summary>
			openFailed = 0x11,
		}

		readonly eStreamStatus[] m_streamStatuses;
		int m_currentIndex;

		int nextIndex()
		{
			m_currentIndex--;
			if( m_currentIndex < 0 )
				m_currentIndex = m_streams.Length - 1;
			return m_currentIndex;
		}

		public bool nextStream()
		{
			eStreamStatus statusToSearch = eStreamStatus.openFailed;
			for( int i = 0; i < m_streamStatuses.Length; i++ )
				statusToSearch &= ( m_streamStatuses[ i ] );

			if( statusToSearch == eStreamStatus.openFailed )
				return false;	// all failed.

			for( int i = 0; i < m_streamStatuses.Length; i++ )
			{
				nextIndex();
				if( m_streamStatuses[ m_currentIndex ] == statusToSearch )
					return true;
			}
			throw new Exception( "Internal error" );
		}

		string curStream { get { return String.Format( "stream {0} / {1}", m_currentIndex + 1, m_streams.Length ); } }

		/// <summary>Get current stream URI.</summary>
		public string getCurrentStream()
		{
			Logger.info( "returning {0}", curStream );
			string trackUri = m_streams[ m_currentIndex ];
			if( !String.IsNullOrEmpty( m_listenKey ) )
				trackUri = String.Format( "{0}?{1}", trackUri, m_listenKey );
			return trackUri;
		}

		/// <summary>Mark current stream as "unable to open"</summary>
		public void beforeOpenStream()
		{
			Logger.info( "{0} - beforeOpenStream", curStream );
			m_streamStatuses[ m_currentIndex ] = eStreamStatus.openFailed;
		}
		
		/// <summary>Mark current stream as "opened OK".</summary>
		public void streamPlaySuceeded()
		{
			if( eStreamStatus.play == m_streamStatuses[ m_currentIndex ] )
				return;
			m_streamStatuses[ m_currentIndex ] = eStreamStatus.play;
			Logger.info( "{0} marked as playable.", curStream );
		}

		public int idChannel { get { return m_idChannel; } }
	}
}