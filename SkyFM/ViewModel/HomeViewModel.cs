using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using SkyFM.Model;

namespace SkyFM.ViewModel
{
	public class HomeViewModel: ViewModelBase
	{
		// public LoginResponse loginResponse { get; private set; }
		// public BatchUpdateResponse batchUpdateResponse { get; private set; }

		public HomeViewModel()
		{
			if( Global.inFormDesigner )
			{
#if DEBUG
				var r = DebugData.response();
				JsonCache.initWithDebugData( r );
				this.initialize();
				this.selectedChannel = this.channels
					.FirstOrDefault( c => c.idChannel == 188 );
#endif
			}
		}

		public bool initialize()
		{
			if( null != this.channels )
				return false;

			var channelFilter = JsonCache.instance.channelFilterForKey( "default" );
			if( null == channelFilter )
				throw new Exception( STR.error_json_no_channels_list );
			var channels = channelFilter.channels;

			IEnumerable<ChannelViewModel> enmList = channels
				.Select( ch => new ChannelViewModel( ch ) );

			Dictionary<int, int> dictPlayCounters = AppSettings.channelPlayCounters;
			if( null != dictPlayCounters )
			{
				enmList = enmList
					.Select( ( vm, i ) =>
					{
						int playedCount = 0;
						dictPlayCounters.TryGetValue( vm.idChannel, out playedCount );
						return new { v = vm, order = i - ( playedCount * 1000 ) };
					} )
					.OrderBy( unm => unm.order )
					.Select( unm => unm.v );
			}

			var list = enmList.ToList();
/* #if DEBUG
			// debug code: move country to the top of the list
			{
				int index = list.IndexOf( list.FirstOrDefault( c => c.title == "Country" ) );
				var item = list[ index ];
				list[ index ] = list[ 0 ];
				list[ 0 ] = item;
			}
#endif */
			this.channels = list;
			RaisePropertyChanged( "channels" );

			BackgroundPlayerViewModel.preInitialize();

			return true;
		}

		public List<ChannelViewModel> channels { get; private set; }

		/// <summary>for the channels list selected item</summary>
		public ChannelViewModel selectedChannelItem
		{
			get { return null; }
			set
			{
				if( this.selectedChannel != value )
				{
					this.selectedChannel = value;
					RaisePropertyChanged( "selectedChannel" );
				}
				Global.delay( () => RaisePropertyChanged( "selectedChannelItem" ) );
				ViewModelLocator.navigationService.NavigateTo( Global.pageUri<ChannelPage>() );
			}
		}

		/// <summary>for the channel page</summary>
		public ChannelViewModel selectedChannel { get; private set; }

		public bool selectChannelById( int id )
		{
			var chan = channels
				.Where( c => c.idChannel == id )
				.FirstOrDefault();
			if( null == chan )
				return false;
			if( selectedChannel != chan )
			{
				selectedChannel = chan;
				RaisePropertyChanged( "selectedChannel" );
			}
			chan.player.ensureInitialized();
			return true;
		}
	}

	public partial class ViewModelLocator
	{
		private static HomeViewModel _homePropertyName;

		/// <summary>Gets the Home property.</summary>
		public static HomeViewModel HomeStatic
		{
			get
			{
				if( _homePropertyName == null )
					CreateHome();
				return _homePropertyName;
			}
		}

		/// <summary>Gets the Home property.</summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes." )]
		public HomeViewModel Home { get { return HomeStatic; } }

		/// <summary>Provides a deterministic way to create the Home property.</summary>
		public static void CreateHome()
		{
			if( _homePropertyName == null )
				_homePropertyName = new HomeViewModel();
		}

		/// <summary>Provides a deterministic way to delete the Home property.</summary>
		public static void ClearHome()
		{
			if( null == _homePropertyName ) return;
			_homePropertyName.Cleanup();
			_homePropertyName = null;
		}
	}
}