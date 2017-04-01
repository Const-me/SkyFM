using GalaSoft.MvvmLight;
using SkyFM.Model;

namespace SkyFM.ViewModel
{
	/// <summary>This class contains static references to all the view models in the application and provides an entry point for the bindings.
	/// <para>See http://www.galasoft.ch/mvvm
	/// </para></summary>
	public partial class ViewModelLocator
	{
		static NavigationService s_navigationService = null;
		public static INavigationService navigationService { get { return s_navigationService; } }

		public ViewModelLocator()
		{
			if( null == s_navigationService )
				s_navigationService = new NavigationService();
		}

		public static ViewModelLocator instance { get { return App.Current.Resources[ "Locator" ] as ViewModelLocator; } }

		/// <summary>Cleans up all the resources.</summary>
		public static void Cleanup()
		{
		}
	}
}