using System;
using System.Windows.Controls;
// using Microsoft.Advertising.Mobile.UI;
using SkyFM;
// using MC.Phone.Ads.Provider;

namespace XamlExtrensions
{
	/// <summary>This is a base class for a page that contains MS banner advertisements.</summary>
	/// <remarks>The XAML should have a grid 480x80px, named "adsPanel".</remarks>
	public class PageWithAds: Page
	{
		/* Grid adsPanel = null;
		AdControl adControl;

		bool bInitialized = false;
		void InitializeIfNeeded()
		{
			if( bInitialized ) return;
			bInitialized = true;

			adsPanel = this.FindName( "adsPanel" ) as Grid;
			if( null == adsPanel )
			{
				Logger.warning( "XAML for PageWithAds should contain a Grid named \"adsPanel\"." );
				return;
			}

			adControl = new AdControl( "38092a17-72ff-47cc-b26e-fdabf036dd8f", "10116282", true )
			{
				Width = 480,
				Height = 80
			};
			adControl.ErrorOccurred += adControl_ErrorOccurred;
			adControl.AdRefreshed += adControl_AdRefreshed;
		}

		bool shouldDisplayAds()
		{
			return null != adsPanel && null != adControl;
		}

		void adControl_ErrorOccurred( object sender, Microsoft.Advertising.AdErrorEventArgs e )
		{
			e.Error.log( "ErrorCode = {0}", e.ErrorCode );
			if( null != this.adsPanel )
				this.adsPanel.Visibility = System.Windows.Visibility.Collapsed;
		}

		void adControl_AdRefreshed( object sender, EventArgs e )
		{
			Logger.info( "The AdControl has received a new ad." );
			if( null != this.adsPanel )
				this.adsPanel.Visibility = System.Windows.Visibility.Visible;
		}

		protected override void OnNavigatedTo( System.Windows.Navigation.NavigationEventArgs e )
		{
			InitializeIfNeeded();

			base.OnNavigatedTo( e );

			do
			{
				if( !shouldDisplayAds() )
					break;
				if( adsPanel.Children.Contains( adControl ) )
					break;
				adsPanel.Children.Add( adControl );
			}
			while( false );
		}

		protected override void OnNavigatedFrom( System.Windows.Navigation.NavigationEventArgs e )
		{
			base.OnNavigatedFrom( e );

			do
			{
				// Remove AdControl from page layout.
				// If this code is removed, an exception will be thrown if the user navigates away from this page while AdControl is fetching an ad navigates
				// back to this page. This is problem is easily reproduced in the Windows Phone Emulator.
				// Just navigate to the page, and immediately navigate away (using the phone's Windows button, for example) before an ad is shown, hit the back button, and watch the fireworks =P

				if( !shouldDisplayAds() )
					break;
				if( !adsPanel.Children.Contains( adControl ) )
					break;
				adsPanel.Children.Remove( adControl );
			}
			while( false );
		} */

		protected override void OnNavigatedTo( System.Windows.Navigation.NavigationEventArgs e )
		{
			InitializeIfNeeded();

			base.OnNavigatedTo( e );
		}

		// AdsControl adsControl = null;

		bool bInitialized = false;
		void InitializeIfNeeded()
		{
			if( bInitialized ) return;
			bInitialized = true;
/*
			adsControl = this.FindName( "adsControl" ) as AdsControl;
			if( null == adsControl )
			{
				Logger.warning( "XAML for PageWithAds should contain an AdsControl named \"adsControl\"." );
				return;
			}

 			adsControl.adControl.AdError += adControl_AdError;
			adsControl.adControl.NoAd += adControl_NoAd;
			adsControl.adControl.NewAd += adControl_NewAd;
*/
		}

/*
		void adControl_NoAd( object sender, AdEventArgs e )
		{
			Logger.warning( "{0}", e.AdProviderName );
			if( null != this.adsControl )
				this.adsControl.Visibility = System.Windows.Visibility.Collapsed;
		}

		void adControl_NewAd( object sender, AdEventArgs e )
		{
			Logger.info( "The AdControl has received a new ad." );
			if( null != this.adsControl )
				this.adsControl.Visibility = System.Windows.Visibility.Visible;
		}

		void adControl_AdError( object sender, AdErrorEventArgs e )
		{
			e.Error.log( "AdError from {0}", e.AdProviderName );
			if( null != this.adsControl )
				this.adsControl.Visibility = System.Windows.Visibility.Collapsed;
		}
*/
	}
}