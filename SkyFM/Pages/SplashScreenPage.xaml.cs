using System.Threading.Tasks;
using System.Windows.Navigation;
using System;
using SkyFM.Model;
using System.Windows;
using Microsoft.Expression.Interactivity.Core;
using SkyFM.ViewModel;
using System.Diagnostics;

namespace SkyFM
{
	public partial class SplashScreenPage: XamlExtrensions.Page
	{
		// Constructor
		public SplashScreenPage()
		{
			InitializeComponent();
		}

		string m_nextPageXaml = null;

		protected override void OnNavigatedTo( NavigationEventArgs e )
		{
			base.OnNavigatedTo( e );
			m_nextPageXaml = this.NavigationContext.QueryString[ "next" ];
			loadImpl();
		}

		void gotoState( string s )
		{
			bool res = ExtendedVisualStateManager.GoToState( this, s, true );
			if( !res )
				Debug.WriteLine( "gotoState failed." );
		}

		async Task loadImpl()
		{
			try
			{
				await JsonCache.instance.update();
				if( CredentialsCache.instance.needsUpdating )
					await CredentialsCache.instance.update();
#if DEBUG
				await TaskEx.Delay( TimeSpan.FromSeconds( 2 ) );
#endif
				Uri next = Global.pageUri( m_nextPageXaml, true );
				ViewModelLocator.navigationService.NavigateTo( next );
			}
			catch (System.Exception ex)
			{
				string msg = String.Format( STR.splash_error_fmt, ex.userFriendlyMessage() );
				onFail( msg );
			}
		}

		async Task onFail( string msg )
		{
			gotoState( stateStopped.Name );
			await TaskEx.Delay( TimeSpan.FromMilliseconds( 250 ) );
			MessageBox.Show( msg );
			await TaskEx.Delay( TimeSpan.FromMilliseconds( 250 ) );
			gotoState( stateNormal.Name );
			loadImpl();
		}
	}
}