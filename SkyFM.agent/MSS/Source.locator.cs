using Microsoft.Phone.BackgroundAudio;

namespace SkyFM.agent.MSS
{
	partial class Source: iLocator
	{
		iSamplesSource iLocator.samplesSource { get { return this.m_source; } }

		iNetworkStream iLocator.http { get { return this.m_source.http; } }

		StreamFormat iLocator.streamFormat { get { return this.m_source.streamFormat; } }

		BufferDownloader iLocator.downloader { get { return this.m_source.downloader; } }
		CircularBuffer iLocator.buffer { get { return this.m_source.buffer; } }
		iByteStream iLocator.mainSoundStream { get { return this.m_source.mainSoundStream; } }

		// int iLocator.idChannel { get { return this.m_streams.idChannel; } }
		// TODO  [low]: for MP3 streams, MpegStreamFormat knows more accurate value then supplied by the UI thread.
		// int iLocator.bytesPerSecond { get { return this.m_streams.m_bytesPerSecond; } }

		int iLocator.idChannel { get { return TracksSource.instance.idChannel; } }
		int iLocator.bytesPerSecond { get { return TracksSource.instance.bytesPerSecond; } }
		iAudioTrack iLocator.currentTrack { get { return m_streamer.currentTrack; } }
	}
}