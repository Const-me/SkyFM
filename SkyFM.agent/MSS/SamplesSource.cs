using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Threading;
using SkyFM.shared;
using System.Collections.Generic;

namespace SkyFM.agent.MSS
{
	internal class SamplesSource: iSamplesSource
	{
		public readonly iLocator locator;

		public iMetadataConsumer metadataConsumer { get; private set; }

		public SamplesSource( iLocator locator )
		{
			this.locator = locator;
			this.metadataConsumer = new MetadataConsumer( locator.currentTrack );
		}

		public CircularBuffer buffer { get; private set; }
		public BufferDownloader downloader { get; private set; }

		public iNetworkStream http { get; private set; }
		public StreamFormat streamFormat { get; private set; }

		CircularBufferReader mainReader;
		MetadataFilter metaMain;
		iByteStream iSamplesSource.mainSoundStream { get { return metaMain; } }

		CancellationTokenSource shouldStop = null;

		CancellationToken cancelToken { get { return shouldStop.Token; } }
		bool IsCancellationRequested { get { 
			if( null == shouldStop )
				return true;
			var token = shouldStop.Token;
			if( null == token )
				return true;
			return token.IsCancellationRequested;
		} }

		bool m_trackPending = true;

		async Task iSamplesSource.openMedia( string trackUri, iSamplesConsumer consumer )
		{
			Logger.info( "uri = {0}", trackUri );
			m_trackPending = true;

			Action actReportProgress = null;
			if( null != consumer )
				actReportProgress = () => consumer.reportGetSampleProgress( 0 );

			this.shouldStop = new CancellationTokenSource();

			// Establish HTTP connection
			this.http = new NetworkStream();
#if false
			// Debug code below
			await TaskEx.Delay( TimeSpan.FromSeconds( 1 ) );
			throw new Exception( "Test - no connectivity" );
#else
			bool bOpened = await this.http.open( trackUri, this.shouldStop.Token, actReportProgress ).ConfigureAwait( false );
			if( !bOpened )
				return;

			// Create various objects to read the data.
			if( null == this.buffer )
				this.buffer = new CircularBuffer();
			else
				this.buffer.Clear();

			this.mainReader = this.buffer.createReader();
			MetadataProducer metaProducer = new MetadataProducer( this.metadataConsumer );
			this.metaMain = new MetadataFilter( mainReader, this.http.icyHeaders.metadataInterval, metaProducer );

			this.downloader = new BufferDownloader( this.locator );
			reportBufferFullness();

			// Buffer small amount of data for syncing
			bool bHaveSyncBuffer = await downloader.ensureSyncBuffer( BufferConfig.cbSyncBuffer, actReportProgress, this.shouldStop.Token ).ConfigureAwait( false );
			if( !bHaveSyncBuffer )
				return;

			this.buffer.syncWithReader( mainReader );

			// Create the format parser and sync the stream
			this.streamFormat = http.icyHeaders.createStreamParser( this.locator );
			bool synced = this.streamFormat.synchronizeStream( true );
			if( !synced )
				throw new Exception( "Unable to synchronize the stream." );
			this.buffer.syncWithReader( mainReader );
			Logger.info( "completed" );
#endif
		}

		void iSamplesSource.openMediaResult( out IDictionary<MediaSourceAttributesKeys, string> mediaStreamAttributes, out IEnumerable<MediaStreamDescription> availableMediaStreams )
		{
			// Report success
			this.streamFormat.fillCodecPrivateData();
			mediaStreamAttributes = this.streamFormat.mediaSourceAttributes;
			availableMediaStreams = this.streamFormat.availableMediaStreams;
		}

		bool iSamplesSource.closeMedia()
		{
			bool res = false;
			m_trackPending = true;
			if( null != this.shouldStop )
			{
				this.shouldStop.Cancel();
				res = true;
			}

			if( null != this.downloader )
			{
				downloader.close();
				this.downloader = null;
				res = true;
			}

			if( null != this.http )
			{
				this.http.close();
				this.http = null;
				res = true;
			}

			this.shouldStop = null;
			this.metaMain = null;
			this.mainReader = null;
			this.streamFormat = null;
			if( null != this.buffer )
				this.buffer.Clear();

			m_trackPending = false;
			return res;
		}

		async Task<MediaStreamSample> iSamplesSource.getSample( iSamplesConsumer consumer )
		{
			if( IsCancellationRequested )
				return this.streamFormat.constructEmptySample();

			downloader.throwIfBackgroundFail();

			// Update buffer to reflect changes by background downloading and SampleStream reading
			this.buffer.syncWithReader( mainReader );

			if( this.buffer.cbLength < BufferConfig.cbMinBuffer )
			{
				Logger.warning( "pausing to download moar data.." );
				m_trackPending = true;
				bool bBufferOk = await downloader.waitForBuffer( BufferConfig.cbPreBuffer, locator.bytesPerSecond, consumer, cancelToken ).ConfigureAwait( false );
				if( !bBufferOk || IsCancellationRequested )
					return this.streamFormat.constructEmptySample();
				m_trackPending = false;
				consumer.reportGetSampleProgress( 1 );
				Logger.info( "Filled the buffer, unpausing" );
				this.buffer.syncWithReader( mainReader );
			}
			else
				reportBufferFullness();

			// Ensure the stream is at the start of the frame
			bool synced = this.streamFormat.synchronizeStream( false );
			if( !synced )
				throw new Exception( "Unable to synchronize the stream." );

			MediaStreamSample sample = this.streamFormat.constructSample();

			m_trackPending = false;
			return sample;
		}

		public void reportBufferFullness()
		{
			byte bf = 0;
			if( null != this.buffer )
				bf = this.buffer.bufferFullness;
			long cb = BufferDownloader.totalBytesReceived;

			locator.currentTrack.sendStatus( this.locator.idChannel, m_trackPending, bf, cb );
		}

		public void reportErrorStatus( string err )
		{
			Logger.warning( "{0}", err );
			byte bf = 0;
			if( null != this.buffer )
				bf = this.buffer.bufferFullness;
			long cb = BufferDownloader.totalBytesReceived;
			locator.currentTrack.sendError( this.locator.idChannel, bf, cb, err );
		}
	}
}