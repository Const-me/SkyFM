using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Net.NetworkInformation;

namespace SkyFM.agent.MSS
{
	/// <summary>Interface for shoutcast stream being received from the network.</summary>
	interface iNetworkStream
	{
		/// <summary>Parsed icecast HTTP headers.</summary>
		IcecastHeaders icyHeaders { get; }
		
		/// <summary>Open the stream</summary>
		/// <param name="streamUri">Source URI</param>
		/// <param name="cancelToken"></param>
		/// <param name="reportProgress">Routine to report while pending. Can be null.</param>
		/// <returns>False when cancelled, true when completed, exception in all other cases incl. timeout.</returns>
		Task<bool> open( string streamUri, CancellationToken cancelToken, Action reportProgress );

		/// <summary>Close the stream</summary>
		void close();

		/// <summary>Read the response stream.</summary>
		/// <remarks>May throw any kind of exceptions, e.g. throws ObjectDisposedException is the stream has been closed by another thread.</remarks>
		Task<int> Read( byte[] buffer, int offset, int count );

		/// <summary>The type of the network interface used by this stream.</summary>
		NetworkInterfaceType networkType { get; }

		/// <summary>Additional information about the type of the network interface used by this stream.</summary>
		NetworkInterfaceSubType networkSubtype { get; }
	}

	interface iMetadataConsumer
	{
		/// <summary>Called when metadata piece is read from the stream.</summary>
		/// <remarks>The length is guaranteed to be > 0</remarks>
		/// <param name="metadata"></param>
		/// <param name="length">How many bytes does the buffer contain.</param>
		void onMetadata( byte[] metadata, int length );
	}

	/// <summary>MediaStreamSource uses this one to obtain audio samples.</summary>
	interface iSamplesSource
	{
		/// <summary>Call this one to open a new stream URI.</summary>
		/// <param name="streamUri">Stream URI to play.</param>
		/// <param name="consumer">The consumer to keep calling reportGetSampleProgress( 0 ) until succeeded. May be null.</param>
		/// <returns>Nothing. Will throw exception if failed.</returns>
		Task openMedia( string streamUri, iSamplesConsumer consumer );

		/// <summary>After openMedia completed successfully, this method will return the media information to report to the OS.</summary>
		/// <param name="mediaStreamAttributes">A collection of attributes describing features of the entire media source.</param>
		/// <param name="availableMediaStreams">A description of each audio and video stream contained within the content.</param>
		void openMediaResult( out IDictionary<MediaSourceAttributesKeys, string> mediaStreamAttributes, out IEnumerable<MediaStreamDescription> availableMediaStreams );

		/// <summary>Close the stream</summary>
		/// <returns>false if previously closed.</returns>
		bool closeMedia();

		/// <summary>Get the audio sample from the stream.</summary>
		/// <remarks>This method either completes synchronously, or calls reportGetSampleProgress if download buffer is empty.</remarks>
		/// <returns>The description of the media stream that this sample came from.</returns>
		Task<MediaStreamSample> getSample( iSamplesConsumer consumer );

		iMetadataConsumer metadataConsumer { get; }
		iNetworkStream http { get; }
		CircularBuffer buffer { get; }
		StreamFormat streamFormat { get; }
		BufferDownloader downloader { get; }
		iByteStream mainSoundStream { get; }

		void reportBufferFullness();
		void reportErrorStatus( string err );
	}

	/// <summary>SamplesSource uses this one to report getSample progress.</summary>
	interface iSamplesConsumer
	{
		/// <summary>SamplesSource calls this method to report the sample progress while the stream is buffering.</summary>
		/// <param name="bufferingProgress"></param>
		void reportGetSampleProgress( double bufferingProgress );
	}

	/// <summary>This interface helps various media source components to locate each other.</summary>
	interface iLocator
	{
		iSamplesSource samplesSource { get; }
		iNetworkStream http { get; }
		CircularBuffer buffer { get; }
		StreamFormat streamFormat { get; }
		BufferDownloader downloader { get; }
		iByteStream mainSoundStream { get; }
		int idChannel { get; }
		int bytesPerSecond { get; }
		iAudioTrack currentTrack { get; }
	}
}