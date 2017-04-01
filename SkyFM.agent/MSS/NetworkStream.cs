using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using Microsoft.Phone.Net.NetworkInformation;

namespace SkyFM.agent.MSS
{
	/// <summary>This class implements shoutcast stream being received from the network</summary>
	class NetworkStream: iNetworkStream
	{
		const int initialBurst = 960000;
		const bool includeMetadata = true;

		public IcecastHeaders icyHeaders { get; private set; }
		public NetworkInterfaceType networkType { get; private set; }
		public NetworkInterfaceSubType networkSubtype { get; private set; }

		HttpWebResponse m_response;
		System.IO.Stream responseStream;

		public async Task<bool> open( string streamUri, CancellationToken cancelToken, Action reportProgress )
		{
			HttpWebRequest req = CreateRequest( streamUri );
			Task<WebResponse> tResponse = req.GetResponseAsync();
			tResponse = tResponse.addTimeoutAndProgress( Timeouts.httpResponseTimeout, cancelToken, reportProgress, req.Abort );
			m_response = (HttpWebResponse)await tResponse;
			if( null == m_response )
			{
				// Cancelled
				this.icyHeaders = null;
				this.responseStream = null;
				return false;
			}

			this.icyHeaders = new IcecastHeaders( m_response );
			this.responseStream = m_response.GetResponseStream();

			NetworkInterfaceInfo info = WebRequestExtensions.GetCurrentNetworkInterface( req );
			this.networkType = info.InterfaceType;
			this.networkSubtype = info.InterfaceSubtype;

			return true;
		}

		static HttpWebRequest CreateRequest( string uri )
		{
			if( uri == null )
				throw new ArgumentNullException( "uri" );
			HttpWebRequest request = HttpWebRequest.CreateHttp( uri );
			if( includeMetadata )
				request.Headers[ "Icy-Metadata" ] = "1";
			request.Headers[ "Initial-Burst" ] = initialBurst.ToString();

			request.AllowReadStreamBuffering = false;
			return request;
		}

		/* public Task<int> Read( byte[] buffer, int offset, int count )
		{
			return this.responseStream.ReadAsync( buffer, offset, count );
		} */

		/* public async Task<int> Read( byte[] buffer, int offset, int count )
		{
			// Logger.info( "READ: requested {0} bytes", count );
			int cb = await this.responseStream.ReadAsync( buffer, offset, count );
			// Logger.info( "READ: received {0} bytes", cb );
			return cb;
		} */

		public async Task<int> Read( byte[] buffer, int offset, int count )
		{
			// For some mysterious reasons, this extra await eliminates the ~30% chance of TimeoutException happening within first few seconds of stream playback.
			// Please keep it here!
			await TaskEx.Yield();
			int cb = await this.responseStream.ReadAsync( buffer, offset, count ).ConfigureAwait( false );
			return cb;
		}

		/* int nPending = 0;

		public async Task<int> Read( byte[] buffer, int offset, int count )
		{
			nPending++;
			if( nPending > 1 )
			{
				Debugger.Break();
				throw new Exception( "Concurrent reads detected" );
			}

			try
			{
				int res = await this.responseStream.ReadAsync( buffer, offset, count ).ConfigureAwait( false );
				return res;
			}
			finally
			{
				nPending--;
			}
		} */

		/* public Task<int> Read( byte[] buffer, int offset, int count )
		{
			TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
			try
			{
				IAsyncResult ar = this.responseStream.BeginRead( buffer, offset, count, this.readCallback, tcs );
				if( ar.CompletedSynchronously )
					this.completeRead( ar );
			}
			catch (System.Exception ex)
			{
				tcs.SetException( ex );
			}
			return tcs.Task;
		}

		void readCallback( IAsyncResult ar )
		{
			if( ar.CompletedSynchronously )
				return;
			this.completeRead( ar );
		}

		void completeRead( IAsyncResult ar )
		{
			TaskCompletionSource<int> tcs = ar.AsyncState as TaskCompletionSource<int>;
			try
			{
				int res = this.responseStream.EndRead( ar );
				tcs.SetResult( res );
			}
			catch (System.Exception ex)
			{
				tcs.SetException( ex );
			}
		} */

		public void close()
		{
			if( null != this.responseStream )
			{
				this.responseStream.Close();
				this.responseStream = null;
			}

			if( null != m_response )
			{
				m_response.Close();
				m_response.Dispose();
				m_response = null;
			}
			this.icyHeaders = null;
		}
	}
}