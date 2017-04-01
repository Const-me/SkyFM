using System;
using SkyFM.shared;

namespace SkyFM.agent
{
	class TracksSource
	{
		static TracksSource s_instance = new TracksSource();
		public static TracksSource instance { get { return s_instance; } }

		PlayCommand m_playCommand;
		ChannelStreams m_streams;

		public void playCommand()
		{
			m_playCommand = NowPlaying.read();
			m_streams = new ChannelStreams( m_playCommand );
		}

		public string getCurrentStream()
		{
			return m_streams.getCurrentStream();
		}

		public string getNextStream()
		{
			if( !m_streams.nextStream() )
				return null;
			return getCurrentStream();
		}

		/// <summary>Mark current stream as "unable to open"</summary>
		public void beforeOpenStream()
		{
			m_streams.beforeOpenStream();
		}

		/// <summary>Mark current stream as "opened OK".</summary>
		public void streamPlaySuceeded()
		{
			m_streams.streamPlaySuceeded();
		}

		public int idChannel { get { return m_playCommand.idChannel; } }

		public int bytesPerSecond { get { return m_streams.m_bytesPerSecond; } }

		public bool energySaverMode { get { return m_playCommand.energySaver; } }
	}
}