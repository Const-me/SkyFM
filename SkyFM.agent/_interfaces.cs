using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyFM.agent
{
	/// <summary>MSS uses this interface to update the track's properties.</summary>
	interface iAudioTrack
	{
		/// <summary>Update the track with the player's status</summary>
		bool sendStatus( int channel, bool bPending, byte buff, long cbTotal );

		/// <summary>Notify about the error occured.</summary>
		void sendError( int channel, byte buff, long cbTotal, string err );

		/// <summary>Update the track's artist & title.</summary>
		void updateMetadata( string title, string artist );
	}

	/// <summary>MSS uses this interface to call the creator streaming agent.</summary>
	interface iStreamer
	{
		/// <summary>Notify the AudioStreamingAgent the currently playing stream is being closed, either as a result of CloseMedia call, or network error.</summary>
		void streamClosed( string failReason, bool shouldTryNext );

		/// <summary>Get currently played channel ID</summary>
		/// <remarks>It's sent back in the track tag, which allows e.g. determine currently played channel after de-tombstoning.</remarks>
		int idChannel { get; }

		/// <summary>Get designated bytes per seconds value.</summary>
		/// <remarks>This value is used to calculate network timeouts.</remarks>
		int bytesPerSecond { get; }

		/// <summary>Get the pointer to the interface to update statistic &amp; metadta.</summary>
		iAudioTrack currentTrack { get; }
	}
}