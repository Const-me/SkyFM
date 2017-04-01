using System;
using System.Threading;
using System.IO.IsolatedStorage;
using System.IO;

#if SYSLOG
namespace SkyFM.shared.Syslog
{
	/// <summary>This static class dumps log messages to isolated storage, for later offline analysis.</summary>
	static class LocalLogger
	{
		/// <summary>The local file name to write.</summary>
		const string strFileName = "SysLog.local.log";

		/// <summary>The name of the names mutes to guare the access to the log file.</summary>
		static readonly Mutex s_mutex = new Mutex( false, "8d058d08-db8e-480b-98c5-847967566df5" );

		/// <summary>Write a message to the log</summary>
		/// <remarks>The message will be followed by the "\r\n" bytes.</remarks>
		/// <param name="payload">The message to write.</param>
		public static void write(byte[] payload)
		{
			// Offload the file system access to a background thread, and rely on native threads scheduler.
			// Unfortunately, I don't know an easy way to await on System.Threading.Mutex primitive, which is the only one that can be used to synchronize processes.
			ThreadPool.QueueUserWorkItem( writeImpl, payload );
		}

		static readonly byte[] s_CrLf = new byte[ 2 ] { (byte)'\r', (byte)'\n' };

		static void writeImpl( object arg )
		{
			byte[] payload = (byte[]) arg;
			s_mutex.WaitOne();
			try
			{
				using( var store = IsolatedStorageFile.GetUserStoreForApplication() )
				using( var file = store.OpenFile( strFileName, FileMode.Append, FileAccess.Write ) )
				{
					file.Write( payload, 0, payload.Length );
					file.Write( s_CrLf, 0, 2 );
				}
			}
			finally
			{
				s_mutex.ReleaseMutex();
			}
		}
	}
}
#endif