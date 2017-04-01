using GalaSoft.MvvmLight;
using System.Threading.Tasks;
using System;
using SkyFM.Model;

namespace SkyFM.ViewModel
{
	/// <summary>This class contains properties that a View can data bind to.
	/// <para>Use the <strong>inpc</strong> snippet to add bindable properties to this ViewModel.</para>
	/// <para>You can also use Blend to data bind with the tool's support.</para>
	/// <seealso href="http://www.galasoft.ch/mvvm/getstarted"/>
	/// </summary>
	public class LoginViewModel: DelayLoadedViewModel
	{
		/// <summary>Initializes a new instance of the LoginViewModel class.</summary>
		public LoginViewModel()
		{
			if( CredentialsCache.instance.hasLoginPass )
			{
				this.valLogin = CredentialsCache.instance.login;
				this.valPassword = CredentialsCache.instance.password;
			}
			else
			{
/* #if DEBUG
				this.valLogin = "soonts@gmail.com";
				this.valPassword = "listen2dasky";
#endif */
			}
		}

		bool m_bSkipLogin = false;

		protected override async Task loadAsyncImpl()
		{
			if( !m_bSkipLogin )
			{
				if( String.IsNullOrEmpty( valLogin ) )
					throw new Exception( STR.login_error_user );

				if( String.IsNullOrEmpty( valPassword ) )
					throw new Exception( STR.login_error_password );

				await CredentialsCache.instance.authenticate( valLogin, valPassword );
			}

			await JsonCache.instance.update();

			// Save m_bSkipLogin value
			AppSettings.loginPreference = !m_bSkipLogin;
			JsonCache.instance.writeStreamListForQuality( AppSettings.qualityKey );

			ViewModelLocator.ClearLogin();
			ViewModelLocator.HomeStatic.initialize();
			ViewModelLocator.navigationService.NavigateTo( Global.pageUri<HomePage>( true ) );
		}

		public void viewLoaded()
		{
			// this.loadAsync();
		}

		public string valLogin { get; set; }
		public string valPassword { get; set; }

		public void actLoginButton()
		{
			m_bSkipLogin = false;
			doLogin();
		}

		public void actSkipLoginButton()
		{
			m_bSkipLogin = true;
			doLogin();
		}

		async Task doLogin()
		{
			try
			{
				await this.loadAsync( true );
			}
			catch( System.Exception ex )
			{
				msgbox.error( ex.userFriendlyMessage() );
				return;
			}
		}
	}

	public partial class ViewModelLocator
	{
		private static LoginViewModel _loginPropertyName;

		/// <summary>Gets the Home property.</summary>
		public static LoginViewModel LoginStatic
		{
			get
			{
				if( _loginPropertyName == null )
					CreateLogin();
				return _loginPropertyName;
			}
		}

		/// <summary>Gets the Home property.</summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes." )]
		public LoginViewModel Login { get { return LoginStatic; } }

		/// <summary>Provides a deterministic way to create the Home property.</summary>
		public static void CreateLogin()
		{
			if( _loginPropertyName == null )
				_loginPropertyName = new LoginViewModel();
		}

		/// <summary>Provides a deterministic way to delete the Home property.</summary>
		public static void ClearLogin()
		{
			if( null == _loginPropertyName ) return;
			_loginPropertyName.Cleanup();
			_loginPropertyName = null;
		}
	}
}