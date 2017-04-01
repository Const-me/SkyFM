using System;
using Microsoft.Phone.BackgroundAudio;
using SkyFM.shared;

namespace SkyFM.agent
{
	/// <summary>This class keeps the GUI process up to date by updating the AudioTrack's properties (artist, title, and most importantly the tag).</summary>
	class CurrentTrack: iAudioTrack
	{
		public readonly AudioTrack track;
		public CurrentTrack( AudioTrack track )
		{
			this.track = track;
		}

		DateTime m_nextSend = default( DateTime );

		bool iAudioTrack.sendStatus( int channel, bool bPending, byte buff, long cbTotal )
		{
			// Throttle send frequency
			DateTime dtNow = DateTime.UtcNow;
			if( dtNow < m_nextSend )
				return false;	// Still not the time to send..
			m_nextSend = dtNow + TrackTag.statusUpdateInterval;

			Logger.trace( "buffer = {0:P1}, traffic = {1}", TagFields.percentage( buff ), cbTotal );

			var t = this.track;
			if( null == t ) return true;
			t.updateStatus( channel, bPending, buff, cbTotal, null );
			return true;
		}

		void iAudioTrack.sendError( int channel, byte buff, long cbTotal, string err )
		{
			var t = this.track;
			if( null == t ) return;
			t.updateStatus( channel, false, buff, cbTotal, err );
		}

		void iAudioTrack.updateMetadata( string title, string artist )
		{
			// There's a bug in WP 7.1: when updating title & artist of the track passed to Streamer.beginStreamingAsync, OS is unable to pick up the changes.
			// However, when updating title & artist of the track received from BackgroundAudioPlayer.Instance.Track, it works OK.

			// TODO [low]: delay by 1-2 seconds. The native code part of the pipeline has some buffers, as well.
			var t = BackgroundAudioPlayer.Instance.Track;
			if( null == t ) return;
			t.updateMetadata( title, artist );
		}
	}
}