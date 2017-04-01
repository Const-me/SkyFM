using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using SkyFM.Model;
using SkyFM.ViewModel;

namespace SkyFM
{
	/// <summary>Description for HomePage.</summary>
	public partial class HomePage: XamlExtrensions.PageWithAds
	{
		/// <summary>Initializes a new instance of the HomePage class.</summary>
		public HomePage()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo( NavigationEventArgs e )
		{
			var hvm = SkyFM.ViewModel.ViewModelLocator.instance.Home;
			hvm.initialize();

			var loginMenuItem = (IApplicationBarMenuItem)this.ApplicationBar.MenuItems[ 1 ] ;
			if( true == AppSettings.loginPreference )
				loginMenuItem.Text = STR.appbar_logout;
			else
				loginMenuItem.Text = STR.appbar_login;

			var aboutMenuItem = (IApplicationBarMenuItem)this.ApplicationBar.MenuItems[ 0 ];
			aboutMenuItem.Text = STR.appbar_about;

			var prefsMenuItem = (IApplicationBarMenuItem)this.ApplicationBar.MenuItems[ 2 ];
			prefsMenuItem.Text = STR.appbar_preferences;

			base.OnNavigatedTo( e );
		}

		void menuLoginLogout_Click(object sender, System.EventArgs e)
		{
			AppSettings.loginPreference = null;
			CredentialsCache.instance.clear();
			BackgroundPlayerViewModel.instance.Stop();
			ViewModelLocator.navigationService.NavigateTo( Global.pageUri<LoginPage>( true ) );
		}

		void menuPrefs_Click( object sender, System.EventArgs e )
		{
			ViewModelLocator.navigationService.NavigateTo( Global.pageUri<SettingsPage>( false ) );
		}

		void menuAbout_Click( object sender, System.EventArgs e )
		{
			ViewModelLocator.navigationService.NavigateTo( Global.pageUri<AboutPage>( false ) );
		}
	}
}