using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using SkyFM.Model;
using System.Globalization;
using System.Diagnostics;

namespace SkyFM
{
	/// <summary>Description for SettingsPage.</summary>
	public partial class SettingsPage: XamlExtrensions.Page
	{
		/// <summary>Initializes a new instance of the SettingsPage class.</summary>
		public SettingsPage()
		{
			InitializeComponent();
			// The default is 72
			// Debug.WriteLine( "{0}", PageTitle.FontSize );

			if( CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower() == "ja" )
			{
				// PageTitle.FontSize = 48;
				PageTitle.FontSize = 44;
			}
		}

		protected override void OnNavigatedTo( NavigationEventArgs e )
		{
			base.OnNavigatedTo( e );

			/* if( AppSettings.powerSaverMode )
				this.radioEnergy.IsChecked = true;
			else
				this.radioStable.IsChecked = true; */
		}

		protected override void OnNavigatingFrom( NavigatingCancelEventArgs e )
		{
			// AppSettings.powerSaverMode = this.radioEnergy.IsChecked.Value;
			base.OnNavigatingFrom( e );
		}
	}
}