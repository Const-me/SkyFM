using System;
using Microsoft.Phone.BackgroundAudio;
using System.Windows;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text;
using SkyFM.shared;

namespace SkyFM.agent
{
	/// <summary>This class wraps AudioPlayerAgent API into the async-friendly abstract class.</summary>
	/// <remarks>Logging and exception handling are added, as well.</remarks>
	public abstract class PlayerAgentAsync: AudioPlayerAgent
	{
		static PlayerAgentAsync()
		{
			UnhandledExceptionHandler.subscribe();
		}

		public PlayerAgentAsync()
		{
			Logger.info( "constructed" );
		}

		protected override void OnError( BackgroundAudioPlayer player, AudioTrack track, Exception ex, bool isFatal )
		{
			if( isFatal )
			{
				BackgroundErrorNotifier.addError( ex );
				ex.log();
				Abort();
			}
			else
			{
				ex.logWarning();
				try
				{
					// Force the track to stop 
					// http://blogs.msdn.com/b/wpukcoe/archive/2012/02/11/background-audio-in-windows-phone-7-5-part-3.aspx
					player.Track = null;
				}
				catch (System.Exception ex2)
				{
					ex2.logWarning( "Exception while trying to stop da playa" );
				}
				NotifyComplete();
			}
		}

		/// <summary>Called when the play state changes, except for the error state.</summary>
		protected override async void OnPlayStateChanged( BackgroundAudioPlayer player, AudioTrack track, PlayState playState )
		{
			Logger.info( "new playState = {0}", playState.ToString() );

			try
			{
				await this.playStateChangedAsync( player, track, playState ).ConfigureAwait( false );
				NotifyComplete();
			}
			catch (System.Exception ex)
			{
				this.onException( ex );
			}
		}

		/// <summary>Called when the user requests an action using some application-provided UI or the Universal Volume Control (UVC) and the application has requested notification of the action.</summary>
		protected override async void OnUserAction( BackgroundAudioPlayer player, AudioTrack track, UserAction action, object param )
		{
			Logger.info( "action = {0};", action.ToString() );

			try
			{
				await this.userActionAsync( player, track, action, param ).ConfigureAwait( false );
				NotifyComplete();
			}
			catch( System.Exception ex )
			{
				this.onException( ex );
			}
		}

		private void onException( Exception ex )
		{
			if( ex.shouldBeIgnored() )
			{
				ex.logWarning();
				this.NotifyComplete();
				return;
			}
			ex.log();
			BackgroundErrorNotifier.addError( ex );
			this.Abort();
		}

		protected override void OnCancel()
		{
			Logger.trace();
			base.OnCancel();
		}

		/// <summary>Handle OnPlayStateChanged asyncronously.</summary>
		/// <param name="player">The Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.</param>
		/// <param name="track">The track playing at the time that the play state changed.</param>
		/// <param name="playState">The new state of the player.</param>
		protected abstract Task playStateChangedAsync( BackgroundAudioPlayer player, AudioTrack track, PlayState playState );

		/// <summary>Handle OnUserAction asyncronously</summary>
		/// <param name="player">The Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.</param>
		/// <param name="track">The track playing at the time of the user action.</param>
		/// <param name="action">The action that the user has requested.</param>
		/// <param name="param">The data associated with the requested action.</param>
		protected abstract Task userActionAsync( BackgroundAudioPlayer player, AudioTrack track, UserAction action, object param );
	}
}