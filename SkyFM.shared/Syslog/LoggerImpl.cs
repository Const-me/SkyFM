using System;
using System.Text;

#if SYSLOG
namespace SkyFM.shared.Syslog
{
	/// <summary>This class implements a SysLog client.</summary>
	/// <remarks>Developed and tested on WP 7.1 Mango, it might also work on other .NET platforms.</remarks>
	/// <seealso href="http://en.wikipedia.org/wiki/Syslog"/>
	class LoggerImpl
	{
		// WP7's network stack only works OK with the Internet, it doesn't like LAN host names such as "Const-PC".
		// 192.168.1.11 is the IP address of my developer's PC - this way it works OK.
		const string syslogServerHost = "192.168.1.11";

		// I'm using SysRose Syslog Desktop, http://www.sysrose.com/, it's default listen port is 514.
		// You can use any other SysLog server that listens for UDP datagrams.
		const ushort syslogServerPort = 514;

		// The socket
		readonly UdpSocket m_socket;

		// The "host" value in the syslog messages
		const string strSelfAddress = "wp7";

		public LoggerImpl()
		{
			m_socket = new UdpSocket( syslogServerHost, syslogServerPort );
		}

		const int maxDatagram = 1460;

		/// <summary>Log a message to the server</summary>
		/// <param name="lvl">Log level</param>
		/// <param name="fac">Log facility</param>
		/// <param name="message">The message to log</param>
		public void log( eLogLevel lvl, eLogFacility fac, StringBuilder message )
		{
			int priority = (int)fac * 8 + (int)lvl;

			string hdr = String.Format( "<{0}>{1} {2} ",
				priority, DateTime.Now.ToString( "MMM dd HH:mm:ss" ), strSelfAddress );

			// string msg = String.Format( "<{0}>{1} {2} {3}",
			//	priority, DateTime.Now.ToString( "MMM dd HH:mm:ss" ), strSelfAddress, message );
			message.Insert( 0, hdr );

			byte[] payload = Encoding.UTF8.GetBytes( message.ToString() );

			// UDP is very simple to use, it's basically fire & forget, no need to setup session, keep handles, send TTL, etc..
			// The downside is payload size is limited :-(
			if( payload.Length > maxDatagram )
				Array.Resize( ref payload, maxDatagram );

			m_socket.send( payload );
			LocalLogger.write( payload );
		}
	}
}
#endif