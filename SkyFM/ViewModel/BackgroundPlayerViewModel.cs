using System;
using GalaSoft.MvvmLight;
using Microsoft.Phone.BackgroundAudio;
using System.Windows.Threading;
using SkyFM.Model;
using SkyFM.shared;
using System.ComponentModel;
using System.Windows;
using System.Text;

namespace SkyFM.ViewModel
{
	/// <summary>A view model for the background player.</summary>
	/// <remarks>Due to the crappy Microsoft's API, this one is a mess :-(
	/// It needs to merge different data sources to gather the required data about the player state.</remarks>
	public class BackgroundPlayerViewModel: INotifyPropertyChanged
	{
		private BackgroundPlayerViewModel()
		{
			if( Global.inFormDesigner )
			{
				m_isPending = true;
				m_title = "Small Green Vest";
				m_artist = "Jane K. West";
				m_cbTotalNetworkBytes = 123456789;
				bufferFullness = 88.92137856987326591832456;
				bufferFullnessText = STR.play_buffer_fmt.FormatWith( bufferFullness / 100 );
				produceStatusMessage();
			}
		}
		public static readonly BackgroundPlayerViewModel instance = new BackgroundPlayerViewModel();
		public void ensureInitialized() { }

		public static bool agentShouldReloadStreams = false;

		DispatcherTimer m_timer;
		static BackgroundAudioPlayer player { get { return BackgroundAudioPlayer.Instance; } }

		int m_subs = 0;

		public void subsribe()
		{
			// Update at once
			m_playerState = player.safePlayerState();
			timer_Tick( null, null );

			m_subs++;
			if( m_subs > 1 )
				return;

			// PlayStateChanged event
			player.PlayStateChanged += new EventHandler( player_PlayStateChanged );

			// Timer to poll properties
			m_timer = new DispatcherTimer();
			m_timer.Interval = SkyFM.shared.TrackTag.statusUpdateInterval;
			m_timer.Tick += this.timer_Tick;
			m_timer.Start();
		}

		public void unsubscribe()
		{
			m_subs--;
			if( m_subs > 0 )
				return;
			if( m_subs < 0 )
			{
				Logger.warning( "subscription counter became negative" );
				m_subs = 0;
				return;
			}

			// PSC
			player.PlayStateChanged -= player_PlayStateChanged;

			// Timer
			if( null != m_timer )
			{
				m_timer.Tick -= this.timer_Tick;
				m_timer.Stop();
				m_timer = null;
			}
		}

		public void Play( int channel )
		{
			if( null != player.Track )
				player.Track = null;

			string listenKey = CredentialsCache.instance.listenKey;

			shared.NowPlaying.write( channel, listenKey, agentShouldReloadStreams, AppSettings.powerSaverMode );
			player.Play();
			// player.Volume = 1.0d;

			isPending = true;
			// Logger.info( "Play called: {0}", newTrack.Tag );
			Logger.info( "Play called" );

			timer_Tick( this, null );
			player_PlayStateChanged( this, null );

			AppSettings.incrementPlayCounter( channel );
		}

		public void Stop()
		{
			Logger.info( "Stop called" );
			// player.Stop();
			player.Track = null;

			timer_Tick( this, null );
			player_PlayStateChanged( this, null );
		}

		bool m_bFirstPlay = true;

		void timer_Tick( object sender, EventArgs e )
		{
			pollForErrors();

			SkyFM.SharedUtils.startOnGuiThread( () =>
			{
				// Poll status in the track tag
				var audioTrack = player.safeTrack();
				if( null == audioTrack )
					return;
				var tag = audioTrack.parseTag();
				if( null == tag )
					return;
				updateStatusFromFields( tag );

				// Save A & T
				m_artist = audioTrack.Artist;
				m_title = audioTrack.Title;

				produceStatusMessage();

				// TODO [low]: find a better way
				raisePlayingChanged();

				if( m_bFirstPlay )
				{
					if( this.isReallyPlaying() )
					{
						MarketplaceReview.promptReview();
						m_bFirstPlay = false;
					}
				}
			} );
		}

		string m_artist, m_title;
		PlayState m_playerState;

		void player_PlayStateChanged( object sender, EventArgs e )
		{
			pollForErrors();

			bool wasPlaying = isPlaying();
			m_playerState = player.safePlayerState();
			bool nowPlaying = isPlaying();
			Logger.info( "player_PlayStateChanged: {0}", m_playerState.ToString() );
			SkyFM.SharedUtils.startOnGuiThread( () =>
			{
				produceStatusMessage();

				if( wasPlaying != nowPlaying )
					raisePlayingChanged();

				if( !nowPlaying )
					this.isPending = false;
			} );
		}

		void produceStatusMessage()
		{
			string s = null;
			do
			{
				if( !String.IsNullOrEmpty( m_error ) )
				{
					s = STR.play_status_failed_fmt.FormatWith( m_error );
					break;
				}
				s = m_playerState.ToString();

				if( !isPlaying() )
					break;

				if( m_isPending )
				{
					s = STR.play_status_buffering;
					break;
				}

				StringBuilder sb = new StringBuilder( 128 );
				if( !String.IsNullOrEmpty( m_artist ) )
					sb.AppendFormat( "{0} – ", m_artist );	//< en-dash
				sb.Append( m_title );
				s = sb.ToString();
			}
			while( false );

			if( s == statusLine )
				return;
			statusLine = s;
			raise( s_statusLineChanged );
		}

		static readonly PropertyChangedEventArgs s_statusLineChanged = new PropertyChangedEventArgs( "statusLine" );
		public string statusLine { get; private set; }

		#region Status in the track tag

		static readonly PropertyChangedEventArgs s_channelNameChanged = new PropertyChangedEventArgs( "channelName" );
		public string channelName { get { return JsonCache.instance.channelNameForId( idChannel ); } }

		int m_idChannel;
		public int idChannel
		{
			get { return m_idChannel; }
			private set
			{
				if( value == m_idChannel )
					return;
				m_idChannel = value;
				raise( s_channelNameChanged );
			}
		}

		static readonly PropertyChangedEventArgs s_bufferFullnessChanged = new PropertyChangedEventArgs( "bufferFullness" );
		public double bufferFullness { get; private set; }

		static readonly PropertyChangedEventArgs s_bufferFullnessTextChanged = new PropertyChangedEventArgs( "bufferFullnessText" );
		public string bufferFullnessText { get; private set; }

		long m_cbTotalNetworkBytes;
		static readonly PropertyChangedEventArgs s_downloadedBytesChanged = new PropertyChangedEventArgs( "downloadedBytes" );
		public string downloadedBytes { get { return formatBytes( m_cbTotalNetworkBytes ); } }

		static readonly PropertyChangedEventArgs s_isPendingChanged = new PropertyChangedEventArgs( "isPending" );
		bool m_isPending = false;
		public bool isPending
		{
			get { return m_isPending; }
			private set 
			{
				if( value == m_isPending )
					return;
				m_isPending = value;
				raise( s_isPendingChanged );
			}
		}
		string m_error = null;

		void updateStatusFromFields( TagFields fields )
		{
			this.bufferFullness = fields.bufferPercentage * 100;
			this.bufferFullnessText = STR.play_buffer_fmt.FormatWith( fields.bufferPercentage );
			m_cbTotalNetworkBytes = fields.networkBytes;
			isPending = fields.isPending;
			idChannel = fields.idChannel;
			raise( s_bufferFullnessChanged );
			raise( s_bufferFullnessTextChanged );
			raise( s_downloadedBytesChanged );
			m_error = fields.error;
		}

		static string formatBytes( double cb )
		{
			if( cb <= 1E6 )
				return String.Format( STR.play_traffic_kilo_fmt, ( cb * 1E-3 ).ToString( 3 ) );
			if( cb <= 1E9 )
				return String.Format( STR.play_traffic_mega_fmt, ( cb * 1E-6 ).ToString( 4 ) );
			return String.Format( STR.play_traffic_giga_fmt, ( cb * 1E-9 ).ToString( 5 ) );
		}
#endregion

#region misc utils
		bool pollForErrors()
		{
			// check for errors
			string errorString = SkyFM.shared.BackgroundErrorNotifier.getError();
			if( errorString != null )
			{
				Deployment.Current.Dispatcher.BeginInvoke( () =>
				{
					MessageBox.Show( errorString, STR.play_errormessage_title, MessageBoxButton.OK );
				} );
				return true;
			}
			return false;
		}

		void raise( PropertyChangedEventArgs a )
		{
			PropertyChangedEventHandler eh = this.PropertyChanged;
			if( null != eh )
				eh( this, a );
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public event Action evtIsPlayingChanged;

		void raisePlayingChanged()
		{
			var eh = this.evtIsPlayingChanged;
			if( null != eh )
				eh();
		}

		/// <summary>True if the player is either playing or attempting to</summary>
		public bool isPlaying()
		{
			var ps = m_playerState;
			return ps == PlayState.Playing || ps == PlayState.Paused;
		}

		/// <summary>True if the player is playing</summary>
		bool isReallyPlaying()
		{
			var ps = m_playerState;
			if( PlayState.Playing != ps )
				return false;
			if( m_isPending )
				return false;
			return true;
		}
#endregion

		public static bool preInitialize()
		{
			if( Global.inFormDesigner )
				return false;

			// http://stackoverflow.com/a/7270780/126995
			var playerState = player.safePlayerState();
			if( playerState != PlayState.Unknown )
				return false;
			player.Stop();
			return true;
		}
	}
}