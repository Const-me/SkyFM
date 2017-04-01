using System;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Text;
using Microsoft.Phone.Info;

namespace SkyFM
{
	public enum eLogLevel: byte
	{
		Emergency = 0,
		Alert = 1,
		Critical = 2,
		Error = 3,
		Warning = 4,
		Notice = 5,
		Information = 6,
		Debug = 7,
	}

	public enum eLogFacility: byte
	{
		Kernel = 0,
		User = 1,
		Mail = 2,
		Daemon = 3,
		Auth = 4,
		Syslog = 5,
		Lpr = 6,
		News = 7,
		UUCP = 8,
		Cron = 9,
		Local0 = 10,
		Local1 = 11,
		Local2 = 12,
		Local3 = 13,
		Local4 = 14,
		Local5 = 15,
		Local6 = 16,
		Local7 = 17,
	}

	public static class Logger
	{
#if SYSLOG
		static readonly SkyFM.shared.Syslog.LoggerImpl m_impl = new SkyFM.shared.Syslog.LoggerImpl();
		static bool isAgent = true;

		/// <summary>Call this method from the App constructor.</summary>
		/// <remarks>
		/// <para>Unless this method is called, the logger will assume it's running in the context of a background agent.</para>
		/// <para>Unfortunately, WP7 has no API to determine this programmatically.</para>
		/// </remarks>
		public static void specifyUiProcess()
		{
			isAgent = false;
		}

		/// <summary>Messages with the level that exceeds this constant will be silently discarded.</summary>
		const eLogLevel s_maxLevelToReport = 
			// eLogLevel.Debug;
			eLogLevel.Information;

		/// <summary>True to repeat every message to the debug output.</summary>
		const bool s_bLitterDebugOutput = false;

		static void log( eLogLevel lvl, StackFrame frame, string msg, params object[] args )
		{
			if( lvl > s_maxLevelToReport )
				return;

			if( null == msg )
				msg = String.Empty;
			StringBuilder sb = new StringBuilder( msg.Length + 64 );

			// Process & thread
			sb.Append( isAgent ? "Agent" : "UI" );
			sb.AppendFormat( " {0:X}", Thread.CurrentThread.ManagedThreadId );

			if( null != frame )
			{
				// Class + method, from the stack frame
				MethodBase objMethod = frame.GetMethod();

				string type = objMethod.DeclaringType.FullName;
				string method = objMethod.Name;

				do
				{
					// Clean up some Async CTP garbage
					int indPlus = type.LastIndexOf( '+' );
					if( indPlus < 0 )
						break;
					if( method != "MoveNext" )
						break;

					int iBegin = indPlus + 1;
					if( iBegin + 1 >= type.Length )
						break;

					if( type[ iBegin ] == '<' )
						iBegin++;
					int iEnd = type.IndexOf( '>', iBegin );
					if( iEnd < 0 )
						break;

					method = type.Substring( iBegin, iEnd - iBegin );
					type = type.Substring( 0, indPlus );
				}
				while( false );

				sb.AppendFormat( ": {0}.{1}", type, method );
			}

			if( !String.IsNullOrEmpty( msg ) )
			{
				// The message, if any
				sb.Append( ": ");
				sb.AppendFormat( msg, args );
			}

			{
				// RAM usage
				long curr = DeviceStatus.ApplicationCurrentMemoryUsage;
				long max = DeviceStatus.ApplicationMemoryUsageLimit;
				double mbRam = (double)curr / ( 1024 * 1024 );
				sb.AppendFormat( " ({0:G3} MB", mbRam );
				if( max > 0 )
					sb.AppendFormat( ", {0:P1}", (double)curr / (double)max );
				sb.Append( ')' );
			}

			if( s_bLitterDebugOutput && Debugger.IsAttached )
			{
				string logMessage = sb.ToString();
				Debug.WriteLine( logMessage );
			}

			m_impl.log( lvl, eLogFacility.User, sb );
		}

		/// <summary>Log caller method name, level = debug</summary>
		public static void trace()
		{
			var frame = new StackTrace().GetFrame( 1 );
			log( eLogLevel.Debug, frame, null );
		}
		/// <summary>Log a message, level = debug</summary>
		public static void trace( string msg, params object[] args )
		{
			var frame = new StackTrace().GetFrame( 1 );
			log( eLogLevel.Debug, frame, msg, args );
		}

		/// <summary>Log a message, level = info</summary>
		public static void info( string msg, params object[] args )
		{
			var frame = new StackTrace().GetFrame( 1 );
			log( eLogLevel.Information, frame, msg, args );
		}

		/// <summary>Log a message, level = warning</summary>
		public static void warning( string msg, params object[] args )
		{
			var frame = new StackTrace().GetFrame( 1 );
			log( eLogLevel.Warning, frame, msg, args );
		}
		/// <summary>Log exception, level = warning</summary>
		public static void logWarning( this Exception ex )
		{
			var frame = new StackTrace().GetFrame( 1 );
			log( eLogLevel.Warning, frame, "{0}", ex.ToString() );
		}
		/// <summary>Log error + extra message, level = warning</summary>
		public static void logWarning( this Exception ex, string msg, params object[] args )
		{
			var frame = new StackTrace().GetFrame( 1 );
			StringBuilder sb = new StringBuilder( 512 );
			sb.AppendFormat( msg, args );
			sb.AppendLine();
			sb.Append( ex.ToString() );
			log( eLogLevel.Warning, frame, "{0}", sb.ToString() );
		}

		/// <summary>Log exception, level = error</summary>
		/// <param name="ex"></param>
		public static void log( this Exception ex )
		{
			var frame = new StackTrace().GetFrame( 1 );
			log( eLogLevel.Error, frame, "{0}", ex.ToString() );
		}
		/// <summary>Log exception + extra message, level = error</summary>
		public static void log( this Exception ex, string msg, params object[] args )
		{
			var frame = new StackTrace().GetFrame( 1 );
			StringBuilder sb = new StringBuilder( 512 );
			sb.AppendFormat( msg, args );
			sb.AppendLine();
			sb.Append( ex.ToString() );
			log( eLogLevel.Error, frame, "{0}", sb.ToString() );
		}

#else
		public static void specifyUiProcess() { }

		public static void trace() { }
		public static void trace( string msg, params object[] args ) { }

		public static void info( string msg, params object[] args ) { }

		public static void warning( string msg, params object[] args ) { }
		public static void logWarning( this Exception ex ) { }
		public static void logWarning( this Exception ex, string msg, params object[] args ) { }

		public static void log( this Exception ex ) { }
		public static void log( this Exception ex, string msg, params object[] args ) { }
#endif
	}
}