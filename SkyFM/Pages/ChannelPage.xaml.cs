using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using SkyFM.ViewModel;

namespace SkyFM
{
	/// <summary>Description for ChannelPage.</summary>
	public partial class ChannelPage: XamlExtrensions.PageWithAds
	{
		/// <summary>Initializes a new instance of the ChannelPage class.</summary>
		public ChannelPage()
		{
			InitializeComponent();

			var chan = this.DataContext as ChannelViewModel;
			if( null == chan )
				return;
			ChannelPageColorer.color( this, chan.idChannel );
		}

		protected override void OnNavigatedTo( NavigationEventArgs e )
		{
			var hvm = ViewModelLocator.instance.Home;
			if( null == hvm.selectedChannel )
			{
				// Came here by back button, the app already tombstoned, and the player did not play: need to restore channel from the state
				hvm.initialize();
				int idChan = this.LoadState<int>( "idChannel" );
				hvm.selectChannelById( idChan );
				this.DataContext = hvm.selectedChannel;
			}
			base.OnNavigatedTo( e );
		}
	}
}