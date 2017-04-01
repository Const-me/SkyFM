using System;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.Phone.BackgroundAudio;

namespace SkyFM.agent
{
	/// <summary>A background agent that performs per-track streaming for playback</summary>
	public class Streamer: StreamingAgentAsync, iStreamer
	{
		int m_idChannel, m_bytesPerSecond;
		CurrentTrack m_track;

		protected override async Task<MediaStreamSource> beginStreamingAsync( AudioTrack track )
		{
			var src = TracksSource.instance;
			m_idChannel = src.idChannel;
			m_bytesPerSecond = src.bytesPerSecond;
			m_track = new CurrentTrack( track );

			string uri = src.getCurrentStream();
			var res = new SkyFM.agent.MSS.Source( uri, this );
			await TaskEx.Yield();
			return res;
		}

		void iStreamer.streamClosed( string failReason, bool shouldTryNext )
		{
			Logger.info( "reason = {0}", failReason );
			this.NotifyComplete();
			Player.streamFailed( failReason, shouldTryNext );
		}

		int iStreamer.idChannel { get { return m_idChannel; } }
		int iStreamer.bytesPerSecond { get { return m_bytesPerSecond; } }
		iAudioTrack iStreamer.currentTrack { get { return m_track; } }
	}
}