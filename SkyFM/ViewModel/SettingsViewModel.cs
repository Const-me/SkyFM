using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using SkyFM.Model;
using System.Windows;
using Microsoft.Phone.Tasks;

namespace SkyFM.ViewModel
{
	/// <summary>This class contains properties that a View can data bind to.
	/// <para>Use the <strong>inpc</strong> snippet to add bindable properties to this ViewModel.</para>
	/// <para>You can also use Blend to data bind with the tool's support.</para>
	/// <seealso href="http://www.galasoft.ch/mvvm/getstarted"/>
	/// </summary>
	public class SettingsViewModel: ViewModelBase
	{
		public class Item
		{
			public readonly string key;
			public readonly bool premium;
			readonly StreamList.Channel.Stream.eFormat format;
			public readonly int bitrate;

			public Item( string k, StreamList.Channel.Stream.eFormat f, int br )
			{
				this.key = k;
				this.format = f;
				this.bitrate = br;
				this.premium = this.key.Contains( "premium" );
			}

			string formatString()
			{
				switch( format )
				{
					case StreamList.Channel.Stream.eFormat.mp3:
						return "MP3";
					case StreamList.Channel.Stream.eFormat.aac:
						return "HE-AAC";
				}
				return "???";
			}
			
			string premiumStatus()
			{
				if( premium )
					return STR.settings_chan_premium;
				return STR.settings_chan_free;
			}

			public override string ToString()
			{
				return String.Format( STR.settings_chan_fmt, formatString(), bitrate, premiumStatus() );
			}
		}

		/// <summary>Initializes a new instance of the SettingsViewModel class.</summary>
		public SettingsViewModel()
		{
#if DEBUG
			if( Global.inFormDesigner )
			{
				var r = DebugData.response();
				JsonCache.initWithDebugData( r );
			}
#endif

			var src = JsonCache.instance.getStreamLists();
			this.items = src.Select( kvp =>
				{
					var stm = kvp.Value.channels.First().streams.First();
					return new Item( kvp.Key, stm.format, stm.bitrate );
				} )
				.OrderBy( i => i.bitrate )
				.ToList();
			string k = AppSettings.qualityKey;
			m_selectedItem = this.items.First( i => i.key == k );
		}

		public List<Item> items { get; private set; }

		Item m_selectedItem;
		public Item selectedItem 
		{
			get { return m_selectedItem; }
			set 
			{
				// Validate
				bool hasPremium = ( true == AppSettings.loginPreference );
				if( value.premium && ( !hasPremium ) )
				{
					var res = MessageBox.Show( STR.setting_signupMessage_text, STR.setting_signupMessage_title, MessageBoxButton.OKCancel );
					if( res == MessageBoxResult.OK )
					{
						WebBrowserTask task = new WebBrowserTask();
						task.Uri = new Uri( "http://www.sky.fm/premium" );
						task.Show();
					}

					Global.delay( () =>
					{
						// Fuckin' WP7.
						base.RaisePropertyChanged( "selectedItem" );
					} );
				}
				else
				{
					AppSettings.qualityKey = value.key;
					m_selectedItem = value;
				}
			}
		}

		public override void Cleanup()
		{
			// Clean own resources if needed
			base.Cleanup();
		}
	}

	public partial class ViewModelLocator
	{
		private static SettingsViewModel _settingsPropertyName;

		/// <summary>Gets the Home property.</summary>
		public static SettingsViewModel SettingsStatic
		{
			get
			{
				if( _settingsPropertyName == null )
					CreateSettings();
				return _settingsPropertyName;
			}
		}

		/// <summary>Gets the Home property.</summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes." )]
		public SettingsViewModel Settings { get { return SettingsStatic; } }

		/// <summary>Provides a deterministic way to create the Home property.</summary>
		public static void CreateSettings()
		{
			if( _settingsPropertyName == null )
				_settingsPropertyName = new SettingsViewModel();
		}

		/// <summary>Provides a deterministic way to delete the Home property.</summary>
		public static void ClearSettings()
		{
			if( null == _settingsPropertyName ) return;
			_settingsPropertyName.Cleanup();
			_settingsPropertyName = null;
		}
	}
}