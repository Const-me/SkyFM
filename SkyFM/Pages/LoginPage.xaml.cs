using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Tasks;
using System.Windows.Input;
using System;
using SkyFM.ViewModel;

namespace SkyFM
{
	public partial class LoginPage: XamlExtrensions.Page
	{
		// Constructor
		public LoginPage()
		{
			InitializeComponent();
		}

		void tbLogin_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if( e.Key == Key.Enter )
			{
				e.Handled = true;
				if( String.IsNullOrEmpty( this.tbLogin.Text ) )
					return;
				this.tbPassword.Focus();
			}
		}

		void tbPassword_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if( e.Key == Key.Enter )
			{
				e.Handled = true;
				if( String.IsNullOrEmpty( this.tbPassword.Password ) )
					return;
				if( String.IsNullOrEmpty( this.tbLogin.Text ) )
				{
					this.tbLogin.Focus();
					return;
				}
				this.Focus();
				LoginViewModel vm = this.DataContext as LoginViewModel;
				Global.delay( vm.actLoginButton );
			}
		}
	}
}