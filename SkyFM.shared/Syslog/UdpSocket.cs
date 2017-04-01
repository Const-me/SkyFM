using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

#if SYSLOG
namespace SkyFM.shared.Syslog
{
	/// <summary>This class sends UDP packets.</summary>
	class UdpSocket
	{
		readonly Socket m_socket;
		readonly DnsEndPoint m_remoteEndpoint;
		readonly List<SocketAsyncEventArgs> m_args = new List<SocketAsyncEventArgs>( 8 );	//< used as FIFO stack.
		readonly object syncRoot = new object();

		/// <summary>Create an UDP socket for sending data to the specified addresses.</summary>
		/// <param name="host">Remote host name or IP</param>
		/// <param name="port">Remote UDP port #</param>
		public UdpSocket( string host, ushort port )
		{
			m_socket = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
			m_remoteEndpoint = new DnsEndPoint( host, port );
		}

		/// <summary>Obtain a SocketAsyncEventArgs instance.</summary>
		/// <returns></returns>
		SocketAsyncEventArgs pop()
		{
			SocketAsyncEventArgs res = null;
			lock( syncRoot )
			{
				// Try to find the item in the stack.
				if( m_args.Count > 0 )
				{
					int ind = m_args.Count - 1;
					res = m_args[ ind ];
					m_args.RemoveAt( ind );
				}
			}

			if( null == res )
			{
				// No items in the stack, creating a new one.
				res = new SocketAsyncEventArgs();
				res.Completed += socket_Completed;
			}

			res.SocketError = SocketError.Success;
			res.UserToken = this;
			res.RemoteEndPoint = m_remoteEndpoint;
			return res;
		}

		/// <summary>Recycle a SocketAsyncEventArgs.</summary>
		/// <param name="e"></param>
		void push( SocketAsyncEventArgs e )
		{
			e.SocketError = SocketError.Success;
			lock( syncRoot )
			{
				m_args.Add( e );
			}
		}

		void socket_Completed( object sender, SocketAsyncEventArgs e )
		{
			push( e );
		}

		/// <summary>Send a packet to the network.</summary>
		/// <remarks>No completion status is reported to the caller.</remarks>
		/// <param name="data"></param>
		public void send( byte[] data )
		{
			var args = pop();
			args.SetBuffer( data, 0, data.Length );
			m_socket.SendToAsync( args );
		}
	}
}
#endif