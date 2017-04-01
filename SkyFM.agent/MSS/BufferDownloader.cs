using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Microsoft.Phone.Net.NetworkInformation;

namespace SkyFM.agent.MSS
{
	internal class BufferDownloader
	{
		readonly CircularBuffer buffer;
		readonly iNetworkStream http;
		readonly TimeSpan tsSleepTimeWhenBufferFull;

		public BufferDownloader( iLocator locator )
		{
			this.http = locator.http;
			this.buffer = locator.buffer;

			// Turn on energy saver mode, if needed
			this.tsSleepTimeWhenBufferFull = BufferConfig.tsSleepTimeWhenBufferFull_normal;
			do
			{
				if( !TracksSource.instance.energySaverMode )
					break;	// The energy saver mode was not requested.

				bool using3GNetworkInterface = ( NetworkInterfaceType.MobileBroadbandGsm == locator.http.networkType );
				if( !using3GNetworkInterface )
					break; // We're not on 3G

				if( locator.bytesPerSecond > ( 128 * 1024 / 8 ) )
					break; // The bitrate is too high for that mode

				this.tsSleepTimeWhenBufferFull = BufferConfig.tsSleepTimeWhenBufferFull_energySaver;
			}
			while( false );
		}

		static long s_cbTotalDownloaded = 0;
		// TODO [very low] fix potentially corrupt data if the __int64 value is read and written @ the same time. Very low 'coz the CPU is single core :-)
		public static long totalBytesReceived { get { return s_cbTotalDownloaded; } }

		async Task<int> downloadAsync()
		{
			if( !buffer.hasFreeSpace() )
			{
				Logger.warning( "BufferDownloader::downloadAsync - the buffer has no free space." );
				return 0;
			}
			int offset, count;
			buffer.getFreeBuffer( out offset, out count );

			// Logger.info( "READ: requested {0} bytes", count );

			Task<int> tRead = http.Read( buffer.buff, offset, count );
			tRead = tRead.addTimeout( Timeouts.timeoutForSingleRead );
			await tRead.ConfigureAwait( false );
			int cb = tRead.Result;

			// Logger.info( "READ: received {0} bytes", cb );

			if( cb > 0 )
			{
				buffer.commitNewData( cb );
				s_cbTotalDownloaded += cb;
			}
			return cb;
		}

		CancellationTokenSource cancelSource = null;

		/// <summary>Either complete synchronously (when the downloader was not running), or fire "cancel" and forget.</summary>
		/// <returns>false if the background downloaded hadn't been running.</returns>
		public bool close()
		{
			if( null == taskBackground )
				return false;	// Not even running
			taskBackground.IgnoreExceptions();
			cancelSource.Cancel();
			taskBackground = null;
			return true;
		}

		Task taskBackground = null;

		bool launchBackground()
		{
			if( null != taskBackground )
				return false;
			this.cancelSource = new CancellationTokenSource();
			CancellationToken cancel = cancelSource.Token;
			taskBackground = TaskEx.Run( () => this.runBackgroundAsync( cancel ), cancel );
			return true;
		}

		async Task runBackgroundAsync( CancellationToken cancel )
		{
			try
			{
				await runBackgroundAsyncImpl( cancel ).ConfigureAwait( false );
			}
			catch (System.Exception ex)
			{
				ex.log();
				http.close();
				throw;
			}
			Logger.info( "Background downloader stopped gracefully" );
		}

		async Task runBackgroundAsyncImpl( CancellationToken cancel )
		{
			Logger.trace();
			while( true )
			{
				if( cancel.IsCancellationRequested )
					return;

				if( buffer.hasFreeSpace() )
				{
					int cb = await this.downloadAsync().ConfigureAwait( false );
					var buffWaiter = m_bufferAwaiter;
					if( 0 == cb )
					{
						Exception ex = new Exception( "runBackgroundAsync: connection dropped" );;
						if( null != buffWaiter )
							buffWaiter.Fail( ex );
						throw ex;
					}

					if( null != buffWaiter )
					{
						// The client is waiting for the data to be pre-buffered
						if( buffWaiter.haveBuffer( buffer.cbLength ) )
						{
							// Sufficient data has been downloaded, so playback is going to resume after this haveBuffer() call.
							m_bufferAwaiter = null;
						}
					}
					continue;	//< Continuing to while(true) without a single sleep.
				}
				await TaskEx.Delay( this.tsSleepTimeWhenBufferFull, cancel ).ConfigureAwait( false );
			}
		}

		public bool throwIfBackgroundFail()
		{
			if( null == taskBackground )
				return false;
			if( taskBackground.IsFaulted )
				taskBackground.Wait();
			return true;
		}

		async Task<bool> ensureSyncBufferImpl( int cbRequired, CancellationToken cancel )
		{
			while( buffer.cbLength < cbRequired )
			{
				int cb = await this.downloadAsync().ConfigureAwait( false );
				if( cancel.IsCancellationRequested )
					return false;
				if( 0 == cb )
					throw new Exception( "ensureBuffer: connection dropped" );
			}
			return true;
		}

		static readonly TimeSpan s_tsSyncBufferTimeout = TimeSpan.FromSeconds( 2 );

		public Task<bool> ensureSyncBuffer( int cbRequired, Action actReportProgress, CancellationToken cancel )
		{
			Task<bool> tImpl = ensureSyncBufferImpl( cbRequired, cancel );
			tImpl = tImpl.addTimeoutAndProgress( Timeouts.timeoutForSingleRead, cancel, actReportProgress, http.close );
			return tImpl;
		}

		/// <summary>This small class makes <see cref="waitForBuffer"/> wait for the buffer being downloaded by <see cref="runBackgroundAsync"/>.</summary>
		class BufferAwaiter
		{
			readonly int m_cbAwaited;
			readonly TaskCompletionSource<bool> m_tcs;

			public BufferAwaiter( int cbRequired )
			{
				m_cbAwaited = cbRequired;
				m_tcs = new TaskCompletionSource<bool>();
			}

			public bool haveBuffer( int cbBuffered )
			{
				if( cbBuffered < m_cbAwaited )
					return false;
				m_tcs.SetResult( true );
				return true;
			}

			public void Fail( Exception ex )
			{
				m_tcs.SetException( ex );
			}

			public Task<bool> theTask { get { return m_tcs.Task; } }
		}

		BufferAwaiter m_bufferAwaiter = null;

		async Task<bool> trueTask()
		{
			return true;
		}

		public Task<bool> waitForBuffer( int cbRequired, int expectedBytesPerSecond, iSamplesConsumer consumer, CancellationToken cancel )
		{
			if( null == taskBackground )
				launchBackground();

			/* double timeoutSeconds = (double)cbRequired / (double)expectedBytesPerSecond;
			timeoutSeconds *= 1.333;
			TimeSpan tsTimeout = TimeSpan.FromSeconds( timeoutSeconds ); */
			int cbLeft = cbRequired - this.buffer.cbLength;
			if( cbLeft < 0 )
				return this.trueTask();
			TimeSpan tsTimeout = Timeouts.timeoutForBufferRead( cbLeft );

			Action actReportProgress = () =>
			{
				this.throwIfBackgroundFail();

				double progress = (double)buffer.cbLength / (double)cbRequired;
				progress = Math.Min( progress, 1 );
				consumer.reportGetSampleProgress( progress );
			};

			m_bufferAwaiter = new BufferAwaiter( cbRequired );
			Task<bool> tImpl = m_bufferAwaiter.theTask;
			tImpl = tImpl.addTimeoutAndProgress( tsTimeout, cancel, actReportProgress, http.close );

			actReportProgress();

			return tImpl;
		}
	}
}