using System;
using System.Windows;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SkyFM.agent
{
	/// <summary>This class subscribes for Application.UnhandledException and TaskScheduler.UnobservedTaskException global events.</summary>
	/// <remarks>The background player process has 2 entry points:
	/// <list type="number">
	/// <item>Player static constructor</item>
	/// <item>Streamer static constructor</item>
	/// </list>
	/// Any of them may happen first.</remarks>
	internal static class UnhandledExceptionHandler
	{
		static readonly object syncRoot = new object();
		static bool s_bSubscribed = false;

		public static bool subscribe()
		{
			if( s_bSubscribed )
				return false;
			lock( syncRoot )
			{
				if( s_bSubscribed )
					return false;
				s_bSubscribed = true;

				SharedUtils.startOnGuiThread( () =>
				{
					Application.Current.UnhandledException += Application_UnhandledException;
					TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
				} );
				return true;
			}
		}

		static void Application_UnhandledException( object sender, ApplicationUnhandledExceptionEventArgs e )
		{
			if( Debugger.IsAttached )
				Debugger.Break();

			e.ExceptionObject.logToFile( "Agent: Application_UnhandledException" );
		}

		static void TaskScheduler_UnobservedTaskException( object sender, UnobservedTaskExceptionEventArgs e )
		{
			if( Debugger.IsAttached )
				Debugger.Break();

			e.Exception.logToFile( "Agent: TaskScheduler_UnobservedTaskException" );
			e.SetObserved();
		}
	}
}