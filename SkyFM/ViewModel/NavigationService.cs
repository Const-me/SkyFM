using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Windows;
using Microsoft.Phone.Tasks;
using System.Diagnostics;
using System.Linq;

namespace SkyFM.ViewModel
{
	public interface INavigationService
	{
		event NavigatingCancelEventHandler Navigating;
		void NavigateTo( Uri pageUri );
		void NavigateToWeb( Uri pageUri );
		void GoBack();
		void GoBackTwice();
		void BackOrForward( Uri pageUri );
	}

	public class NavigationService: INavigationService
	{
		private PhoneApplicationFrame _mainFrame;

		public event NavigatingCancelEventHandler Navigating;

		public void NavigateTo( Uri pageUri )
		{
			if( EnsureMainFrame() )
				_mainFrame.Navigate( pageUri );
		}

		public void GoBack()
		{
			if( EnsureMainFrame() && _mainFrame.CanGoBack )
				_mainFrame.GoBack();
		}

		public void GoBackTwice()
		{
			if( EnsureMainFrame() && _mainFrame.CanGoBack )
			{
				_mainFrame.RemoveBackEntry();
				if( _mainFrame.CanGoBack )
					_mainFrame.GoBack();
			}
		}

		public void BackOrForward( Uri pageUri )
		{
			if( !EnsureMainFrame() ) return;

			do
			{
				JournalEntry bs = _mainFrame.BackStack.FirstOrDefault();
				if( null == bs )
					break;
				// if( bs.Source.GetLeftPart( UriPartial.Path ).ToLower() != pageUri.GetLeftPart( UriPartial.Path ).ToLower() )
				if( bs.Source != pageUri )
					break;
				GoBack();
				return;
			}
			while( false );

			// TODO [medium]: redesign the hardcoded URI construction
			string us = pageUri.ToString();
			us += "?removePrevious=1";
			pageUri = new Uri( us, UriKind.Relative );
			NavigateTo( pageUri );
		}

		private bool EnsureMainFrame()
		{
			if( _mainFrame != null )
				return true;

			_mainFrame = Application.Current.RootVisual as PhoneApplicationFrame;

			if( _mainFrame != null )
			{
				// Could be null if the app runs inside a design tool
				_mainFrame.Navigating += ( s, e ) =>
				{
					if( Navigating != null )
						Navigating( s, e );
				};
				return true;
			}
			return false;
		}

		private WebBrowserTask m_WebBrowserTask = null;

		public void NavigateToWeb( Uri pageUri )
		{
			if( null == m_WebBrowserTask )
				m_WebBrowserTask = new WebBrowserTask();
			m_WebBrowserTask.Uri = pageUri;

			// See http://nikovrdoljak.wordpress.com/tag/webbrowsertask/ for the reason why..
			try
			{
				m_WebBrowserTask.Show();
			}
			catch( System.Exception ex )
			{
				Debug.WriteLine( "{0}", ex.Message );
			}
		}
	}
}