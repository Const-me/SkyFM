using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Phone.BackgroundAudio;
using SkyFM.shared;

namespace SkyFM.agent.MSS
{
	partial class Source: MediaStreamSource, iSamplesConsumer
	{
		readonly iSamplesSource m_source;
		readonly string streamUri;
		readonly iStreamer m_streamer;

		const bool s_dbgThrowTimeoutsWhilePlaying = false;
		const bool s_dbgThrowTimeoutsOnOpen = false;

		public Source( string uri, iStreamer streamer )
		{
			m_streamer = streamer;
			streamUri = uri;

			// Initialize timeouts with the bitrate.
			Timeouts.setBitrate( streamer.bytesPerSecond );

			// Construct the samples source.
			m_source = new SamplesSource( this );
		}

		protected override async void OpenMediaAsync()
		{
			try
			{
				TracksSource.instance.beforeOpenStream();

				await m_source.openMedia( streamUri, null ).ConfigureAwait( false );
				if( s_dbgThrowTimeoutsOnOpen )
					throw new TimeoutException( "Debug timeout on open" );

				IDictionary<MediaSourceAttributesKeys, string> mediaStreamAttributes;
				IEnumerable<MediaStreamDescription> availableMediaStreams;
				m_source.openMediaResult( out mediaStreamAttributes, out availableMediaStreams );
				base.ReportOpenMediaCompleted( mediaStreamAttributes, availableMediaStreams );
			}
			catch( Exception ex )
			{
				ex.log();
				closeMediaImpl( ex );
				// http://msdn.microsoft.com/en-us/library/system.windows.media.mediastreamsource.openmediaasync(v=vs.95).aspx
				// If the media cannot be opened, the ErrorOccurred method should be called instead
				base.ErrorOccurred( ex.Message );
				return;
			}

			Logger.info( "completed" );
		}

		protected override async void GetSampleAsync( MediaStreamType mediaStreamType )
		{
			try
			{
				MediaStreamSample sample = await m_source.getSample( this ).ConfigureAwait( false );
				if( s_dbgThrowTimeoutsWhilePlaying )
				{
					const long timeoutsInterval = 120000000;	// 12 seconds * ( 1 second / 100 nanoseconds )
					if( sample.Timestamp > timeoutsInterval )
					{
						Logger.warning( "Intentionally throwing a TimeoutException" );
						throw new TimeoutException( "Debug timeout while playing" );
					}
				}

				this.ReportGetSampleCompleted( sample );

				TracksSource.instance.streamPlaySuceeded();

				if( null == sample.Stream )
					Logger.warning( "returned an empty sample" );
			}
			catch( System.Exception ex )
			{
				ex.log();
				closeMediaImpl( ex );
			}
		}

		void iSamplesConsumer.reportGetSampleProgress( double bufferingProgress )
		{
			Logger.trace( "{0:P}", bufferingProgress );
			try
			{
				base.ReportGetSampleProgress( bufferingProgress );
				this.m_source.reportBufferFullness();
			}
			catch (System.Exception ex)
			{
				ex.log();
				throw;
			}
		}

		internal void closeMediaImpl( Exception ex )
		{
			string failReason = "N/A";
			if( null != ex )
				failReason = ex.userFriendlyMessage();

			Logger.info( "reason = {0}", failReason );
			try
			{
				if( !m_source.closeMedia() )
					return;
			}
			catch( System.Exception e2 )
			{
				e2.logWarning( "exception while closing media - ignored" );
			}

			bool shouldTryNext = false;
			if( null != ex )
				if( !ex.shouldBeIgnored() )
					shouldTryNext = true;

			if( null != m_streamer )
				m_streamer.streamClosed( failReason, shouldTryNext );

			// Unless the error is reported, the player will stick in the "Paused" state
			if( null != ex )
				base.ErrorOccurred( failReason );
		}

		protected override void CloseMedia()
		{
			closeMediaImpl( null );
		}

		public void Dispose()
		{
			this.closeMediaImpl( null );
		}

		protected override void GetDiagnosticAsync( MediaStreamSourceDiagnosticKind diagnosticKind )
		{
			throw new NotSupportedException();
		}

		/// <summary>MediaElement calls this method to ask the MediaStreamSource to seek to the nearest randomly accessible point before the specified time.</summary>
		/// <param name="seekToTime">Time as represented by 100 nanosecond increments to seek to. This is typically measured from the beginning of the media file.</param>
		protected override void SeekAsync( long seekToTime )
		{
			Logger.info( "{0}", seekToTime );
			this.ReportSeekCompleted( seekToTime );
		}

		protected override void SwitchMediaStreamAsync( MediaStreamDescription mediaStreamDescription )
		{
			throw new NotSupportedException();
		}
	}
}