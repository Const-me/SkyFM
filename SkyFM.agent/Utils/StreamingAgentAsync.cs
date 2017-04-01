using System;
using Microsoft.Phone.BackgroundAudio;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SkyFM.agent
{
	/// <summary>This class wraps AudioStreamingAgent into the async-friendly abstract class.</summary>
	/// <remarks>Logging and exception handling are added, as well.</remarks>
	public abstract class StreamingAgentAsync: AudioStreamingAgent
	{
		static StreamingAgentAsync()
		{
			UnhandledExceptionHandler.subscribe();
		}

		public StreamingAgentAsync()
		{
			Logger.info( "constructed" );
		}

		AudioTrack m_track;
		protected AudioTrack track { get { return track; } }

		protected override async void OnBeginStreaming( AudioTrack track, AudioStreamer streamer )
		{
			m_track = track;
			Logger.info( "" );
			try
			{
				MediaStreamSource mss = await beginStreamingAsync( track ).ConfigureAwait( false );
				streamer.SetSource( mss );
				Logger.info( "completed" );
			}
			catch (System.Exception ex)
			{
				ex.log();
				base.Abort();
			}
		}

		protected override void OnCancel()
		{
			Logger.info( "" );
			base.OnCancel();
		}

		protected abstract Task<MediaStreamSource> beginStreamingAsync( AudioTrack track );
	}
}