using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Phone.BackgroundAudio;
using System.Reflection;

namespace SkyFM
{
	public static class SharedUtils
	{
		public static string FormatWith( this string format, params object[] args )
		{
			if( format == null )
				throw new ArgumentNullException( "format" );
			return String.Format( format, args );
		}

		public static V itemOrDefault<K, V>( this Dictionary<K, V> dict, K k )
		{
			V res;
			if( dict.TryGetValue( k, out res ) )
				return res;
			return default( V );
		}

		public static R itemOrDefault<K, V, R>( this Dictionary<K, V> dict, K k, Func<V, R> conv )
		{
			V val;
			if( dict.TryGetValue( k, out val ) )
				return conv( val );
			return default( R );
		}

		/// <summary>Returns true if the current thread is a GUI thread, otherwise false.</summary>
		/// <returns></returns>
		public static bool checkAccess()
		{
			var disp = Deployment.Current.Dispatcher;
			return disp.CheckAccess();
		}

		/// <summary>If the current thread is a GUI thread, execute the action right away.
		/// If however it's some other thread, schedule the action to be executed by the GUI thread and immediately return.</summary>
		/// <param name="act">The action to execute.</param>
		public static void startOnGuiThread( Action act )
		{
			if( checkAccess() )
				act();
			else
				Deployment.Current.Dispatcher.BeginInvoke( act );
		}

		// http://stackoverflow.com/a/563959/126995
		public static Dictionary<string, string> parseQueryString( string paramsString )
		{
			if( string.IsNullOrEmpty( paramsString ) )
				return null;

			// convert to dictionary
			var dict = new Dictionary<string, string>();

			// remove the leading ?
			if( paramsString.StartsWith( "?" ) )
				paramsString = paramsString.Substring( 1 );

			var length = paramsString.Length;

			for( var i = 0; i < length; i++ )
			{
				var startIndex = i;
				var pivotIndex = -1;

				while( i < length )
				{
					char ch = paramsString[ i ];
					if( ch == '=' )
					{
						if( pivotIndex < 0 )
						{
							pivotIndex = i;
						}
					}
					else if( ch == '&' )
					{
						break;
					}
					i++;
				}

				string name;
				string value;
				if( pivotIndex >= 0 )
				{
					name = paramsString.Substring( startIndex, pivotIndex - startIndex );
					value = paramsString.Substring( pivotIndex + 1, ( i - pivotIndex ) - 1 );
				}
				else
				{
					name = paramsString.Substring( startIndex, i - startIndex );
					value = null;
				}

				dict.Add( HttpUtility.UrlDecode( name ), HttpUtility.UrlDecode( value ) );

				// if string ends with ampersand, add another empty token
				if( ( i == ( length - 1 ) ) && ( paramsString[ i ] == '&' ) )
					dict.Add( null, string.Empty );
			}
			return dict;
		}

		public static string userFriendlyMessage( this Exception ex )
		{
			if( ex is AggregateException )
			{
				var ae = ex as AggregateException;
				return ae.Flatten().InnerExceptions.First().userFriendlyMessage();
			}

			do
			{
				var w = ex as WebException;
				if( null == w ) break;
				// if( w.Status == WebExceptionStatus.ProtocolError )
				{
					var resp = w.Response as HttpWebResponse;
					if( null != resp &&
							resp.StatusCode != HttpStatusCode.OK &&
							Enum.IsDefined( typeof( HttpStatusCode ), resp.StatusCode ) )
						return String.Format( "HTTP status {0} - {1}", (int)resp.StatusCode, resp.StatusDescription );
				}
				// if( w.Status == WebExceptionStatus.UnknownError )
				// 	break; // it just prints "not found" :-(
				string statusText = w.Status.descriptionOrNull();
				if( null != statusText )
					return String.Format( "Communication error. {0}", statusText );
			}
			while( false );

			return ex.Message;
		}

		/// <summary>The BackgroundAudioPlayer.PlayerState sometimes throws COMException (0x803A00D1): the background audio resources are no longer available</summary>
		/// <param name="player"></param>
		/// <returns>Either BackgroundAudioPlayer.PlayerState property, or PlayState.Unknown if the exception was thrown.</returns>
		public static PlayState safePlayerState( this BackgroundAudioPlayer player )
		{
			try
			{
				return player.PlayerState;
			}
			catch( Exception ex )
			{
				ex.logWarning();
				return PlayState.Unknown;
			}
		}

		public static AudioTrack safeTrack( this BackgroundAudioPlayer player )
		{
			try
			{
				return player.Track;
			}
			catch( Exception ex )
			{
				ex.logWarning();
				return null;
			}
		}

		// http://www.jonathanantoine.com/2011/11/07/wp7-how-to-get-the-version-number-of-my-application/
		public static Version getCurrentVersion()
		{
			Version version;

			var assembly = Assembly.GetExecutingAssembly().FullName;
			var fullVersionNumber = assembly.Split( '=' )[ 1 ].Split( ',' )[ 0 ];

			version = new Version( fullVersionNumber );

			return version;
		}
	}
}