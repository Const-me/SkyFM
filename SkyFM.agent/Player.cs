using System;
using System.Threading.Tasks;
using Microsoft.Phone.BackgroundAudio;
using SkyFM.shared;
using Microsoft.Phone.Shell;
using System.Linq;

namespace SkyFM.agent
{
	/// <summary>Shoutcast audio player for background audio.</summary>
	public class Player: PlayerAgentAsync
	{
		// bool m_bUserInitiatedStop = false;

		static bool s_bFirstAction = true;

		protected override async Task userActionAsync( BackgroundAudioPlayer player, AudioTrack track, UserAction action, object param )
		{
			// if( action == UserAction.Stop || action == UserAction.Pause )
			//	m_bUserInitiatedStop = true;

			if( s_bFirstAction )
			{
				// http://stackoverflow.com/a/7270780/126995
				s_bFirstAction = false;
				if( action == UserAction.Stop )
				{
					Logger.info( "First STOP user action has been skipped" );
					return;
				}
			};

			switch( action )
			{
				case UserAction.Play:
					TracksSource.instance.playCommand();
					player.Play();
					break;
				case UserAction.Stop:
					player.Track = null;
					break;
				case UserAction.Pause:
					if( PlayState.Unknown != player.safePlayerState() )
						player.Stop();
					else
						player.Track = null;
					break;
				case UserAction.FastForward:
					break;
				case UserAction.Rewind:
					break;
				case UserAction.Seek:
					break;
				case UserAction.SkipNext:
					break;
				case UserAction.SkipPrevious:
					break;
			}
			await TaskEx.Yield();
		}

		protected override async Task playStateChangedAsync( BackgroundAudioPlayer player, AudioTrack unused, PlayState playState )
		{
			switch( playState )
			{
				case PlayState.TrackEnded:
					{
						string nextStm = TracksSource.instance.getNextStream();
						if( null == nextStm )
						{
							Logger.info( "No more streams, stopping" );
							player.Stop();
							return;
						}
						var track = new AudioTrack( null, null, null, null, null, null, EnabledPlayerControls.Pause );
						Logger.info( "TrackEnded => supplying new one" );
						player.Track = track;
						return;
					}
				case PlayState.TrackReady:
					player.Volume = 1;
					player.Play();
					return;
				case PlayState.Stopped:
					{
						/* do
						{
							if( m_bUserInitiatedStop )
							{
								m_bUserInitiatedStop = false;
								// Player stopped doe to used command.
								break;
							}
							// The player stopped itself.

							string closeReason = Streamer.streamCloseReason;
							if( null == closeReason )
								break;	// Most likely, player stopped due to used command, but this particular command was not received by userActionAsync due to some problems in one of Microsoft's development team.

							// The player stopped itself due to an error..
							Logger.warning( "Detected stop due to an error {0}", closeReason );

							string nextStm = TracksSource.instance.getNextStream();
							if( null == nextStm )
							{
								// ..and there are no suitable streams left.
								Logger.info( "No more streams, stopping" );
								break;
							}

							// ..but there are still streams worse trying.
							var track = new AudioTrack( null, null, null, null, null, null, EnabledPlayerControls.Pause );
							Logger.info( "TrackEnded => supplying new one" );
							player.Track = track;
							return;
						}
						while( false ); */

						clearLiveTile();
						return;
					}
/*				case PlayState.Shutdown:
					break;
				case PlayState.Unknown:
					break;
				case PlayState.Paused:
					break;
				case PlayState.Playing:
					break;
				case PlayState.BufferingStarted:
					break;
				case PlayState.BufferingStopped:
					break;
				case PlayState.Rewinding:
					break;
				case PlayState.FastForwarding:
					break; */
			}
			await TaskEx.Yield();
		}

		void clearLiveTile()
		{
			SharedUtils.startOnGuiThread( () =>
			{
				StandardTileData data = new StandardTileData()
				{
					BackBackgroundImage = new Uri( "doesntexist.png", UriKind.RelativeOrAbsolute ),
					Title = "internet radio",	// TODO: read WMAppManifest.xml instead, XPath "/Deployment/Tokens/TemplateType5/Title/text()".
				};
				ShellTile.ActiveTiles.First().Update( data );
			} );
		}

		public static void streamFailed( string failReason, bool shouldTryNext )
		{
			Logger.warning( "{0}", failReason );
			if( !shouldTryNext )
				return;
			/* var instance = getInstance() as Player;
			if( null == instance ) return;
			instance.streamFailedImpl( failReason ); */
			streamFailedImpl( failReason );
		}

		private static void streamFailedImpl( string closeReason )
		{
			// The player stopped itself due to an error..
			Logger.warning( "Detected stop due to an error {0}", closeReason );
			var player = BackgroundAudioPlayer.Instance;

			string nextStm = TracksSource.instance.getNextStream();
			if( null == nextStm )
			{
				// ..and there are no suitable streams left.
				Logger.warning( "No more streams, stopping" );
				BackgroundErrorNotifier.addError( closeReason );
				return;
			}

			// ..but there are still streams worse trying.
			var track = new AudioTrack( null, null, null, null, null, null, EnabledPlayerControls.Pause );
			Logger.info( "Trying next track.." );
			player.Track = track;
		}
	}
}