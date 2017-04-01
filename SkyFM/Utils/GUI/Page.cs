using System;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using SkyFM;

namespace SkyFM.ViewModel
{
	interface iPageState
	{
		void SaveState( string key, object value );
		T LoadState<T>( string key );
	}

	interface iViewModelNavigation
	{
		void OnNavigatedTo( iPageState pageState, NavigationEventArgs e );
		void OnNavigatedFrom( iPageState pageState, NavigationEventArgs e );
	}
}

namespace XamlExtrensions
{
	public class Page: PhoneApplicationPage, SkyFM.ViewModel.iPageState
	{
		protected override void OnNavigatedTo( NavigationEventArgs e )
		{
			base.OnNavigatedTo( e );

			// Check for "removePrevious=1" param in the page URI
			this.removePrevPageIfRequested();

			// If the VM implements the interface for receiving navigation events, call that.
			var nav = this.DataContext as SkyFM.ViewModel.iViewModelNavigation;
			if( null != nav )
				nav.OnNavigatedTo( this, e );

			// Check for the crash log of the prev.launch.
			Global.checkCrashLog();
		}

		protected override void OnNavigatedFrom( NavigationEventArgs e )
		{
			base.OnNavigatedFrom( e );
			// If the VM implements the interface for receiving navigation events, call that.
			var nav = this.DataContext as SkyFM.ViewModel.iViewModelNavigation;
			if( null != nav )
				nav.OnNavigatedFrom( this, e );
		}

		public void SaveState( string key, object value )
		{
			if( State.ContainsKey( key ) )
				State.Remove( key );
			State.Add( key, value );
		}

		public T LoadState<T>( string key )
		{
			if( State.ContainsKey( key ) )
				return (T)State[ key ];
			return default( T );
		}
	}
}