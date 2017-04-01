using System;
using System.ComponentModel;
using System.Windows.Navigation;
using Microsoft.Phone.BackgroundAudio;
using GalaSoft.MvvmLight;
using Kawagoe.Storage;
using SkyFM.Model;
using System.Threading.Tasks;

namespace SkyFM.ViewModel
{
	public class ChannelViewModel: ViewModelBase, iViewModelNavigation
	{
		readonly Uri imageUri;
		public object image 
		{
			get 
			{ 
				if( Global.inFormDesigner )
					return imageUri;
				return ImageCache.Default.Get( imageUri ); 
			}
		}

		public string title { get; private set; }
		public string subtitle { get; private set; }
		public string currentSong { get; private set; }
		public string channelDirector { get; private set; }
		public readonly int idChannel;

		public ChannelViewModel( ChannelFilter.Channel ch )
		{
			this.channel = ch;
			this.imageUri = Global.uriFromString( ch.asset_url );
			this.title = ch.name;
			this.subtitle = ch.description;
			this.channelDirector = ch.channel_director;
			this.idChannel = ch.id;

			TrackHistory hist = JsonCache.instance.historyForChannel( ch.id );
			if( null != hist )
				this.currentSong = hist.track;

			if( Global.inFormDesigner )
				btnPlayStop = STR.play_button_playthis;
		}

		private readonly ChannelFilter.Channel channel;

		void iViewModelNavigation.OnNavigatedTo( iPageState pageState, NavigationEventArgs e )
		{
			player.evtIsPlayingChanged += player_evtIsPlayingChanged;
			player.subsribe();

			do
			{
				if( BackgroundAudioPlayer.Instance.safePlayerState() == PlayState.Playing )
					break;	// Already playing
				if( e.NavigationMode == NavigationMode.Back )
					break;	// Came with "back" button, this can happen after tombstoned, clicked on the song on UVC, then pressed stop.
				this.playThisChannel();
			}
			while( false );

			updatePlayStopButton();
		}

		void iViewModelNavigation.OnNavigatedFrom( iPageState pageState, NavigationEventArgs e )
		{
			pageState.SaveState( "idChannel", this.idChannel );
			player.unsubscribe();
			player.evtIsPlayingChanged -= player_evtIsPlayingChanged;
		}

		public BackgroundPlayerViewModel player { get { return BackgroundPlayerViewModel.instance; } }

		bool m_bStop = false;

		string m_btnPlayStop;
		static readonly PropertyChangedEventArgs s_btnPlayStopChanged = new PropertyChangedEventArgs( "btnPlayStop" );
		public string btnPlayStop
		{
			get { return m_btnPlayStop; }
			private set
			{
				if( value == m_btnPlayStop )
					return;
				m_btnPlayStop = value;
				var eh = base.PropertyChangedHandler;
				if( null != eh )
					eh( this, s_btnPlayStopChanged );
			}
		}

		bool m_btnPlayStopIsEnabled = true;
		static readonly PropertyChangedEventArgs s_btnPlayStopIsEnabledChanged = new PropertyChangedEventArgs( "btnPlayStopIsEnabled" );
		public bool btnPlayStopIsEnabled
		{
			get { return m_btnPlayStopIsEnabled; }
			private set
			{
				if( value == m_btnPlayStopIsEnabled )
					return;
				m_btnPlayStopIsEnabled = value;
				var eh = base.PropertyChangedHandler;
				if( null != eh )
					eh( this, s_btnPlayStopIsEnabledChanged );
			}
		}

		void updatePlayStopButton()
		{
			m_bStop = false;
			if( player.idChannel == this.idChannel )
			{
				// We're on the current channel's page

				if( player.isPlaying() )
				{
					m_bStop = true;
					btnPlayStop = STR.play_button_stop;
				}
				else
					btnPlayStop = STR.play_button_resume;
			}
			else
			{
				// The player is currently playing some other channel
				btnPlayStop = STR.play_button_playthis;
			}
		}

		public void actPlayStop()
		{
			if( m_bStop )
			{
				player.Stop();
				ViewModelLocator.navigationService.BackOrForward( Global.pageUri<HomePage>() );
			}
			else
				this.playThisChannel();

			updatePlayStopButton();
		}

		void player_evtIsPlayingChanged()
		{
			updatePlayStopButton();
		}

		async void playThisChannel()
		{
			player.Play( this.idChannel );
			LiveTile.createTile( this.idChannel, this.imageUri, this.title.ToLower() );
			btnPlayStopIsEnabled = false;
			for( int i=0; i < 25; i++ )
			{
				await TaskEx.Delay( 100 ).ConfigureAwait( false );
				if( player.isPlaying() )
					break;
			}

			SharedUtils.startOnGuiThread( () =>
			{
				btnPlayStopIsEnabled = true;
				updatePlayStopButton();
			} );
		}
	}
}