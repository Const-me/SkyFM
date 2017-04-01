using System;
using System.IO.IsolatedStorage;
using System.Collections.Generic;

namespace SkyFM.Model
{
	internal static class AppSettings
	{
		const string keyPremium = "premium";
		const string keyQuality = "quality";

		/// <summary>null = not selected (user will be taken to the login screen), true = have premium subscription, false = listening for free streams.</summary>
		public static bool? loginPreference 
		{
			get
			{
				bool premium = false;
				if( !IsolatedStorageSettings.ApplicationSettings.TryGetValue( keyPremium, out premium ) )
					return null;
				return premium;
			}
			set
			{
				if( null == value )
				{
					IsolatedStorageSettings.ApplicationSettings.Remove( keyPremium );
					IsolatedStorageSettings.ApplicationSettings.Remove( keyQuality );
				}
				else
					IsolatedStorageSettings.ApplicationSettings[ keyPremium ] = value.Value;
				IsolatedStorageSettings.ApplicationSettings.Save();

				if( value != true )
				{
					CredentialsCache.instance.clear();
				}
			}
		}

		public static string qualityKey
		{
			get
			{
				if( Global.inFormDesigner )
					return "appleapp_premium_medium";
				string res = null;
				if( IsolatedStorageSettings.ApplicationSettings.TryGetValue( keyQuality, out res ) )
					return res;
				if( true == loginPreference )
					return "appleapp_premium_medium";
				return "appleapp";
			}
			set
			{
				IsolatedStorageSettings.ApplicationSettings[ keyQuality ] = value;
				IsolatedStorageSettings.ApplicationSettings.Save();
				JsonCache.instance.writeStreamListForQuality( value );
				// ^^ this will also set SkyFM.ViewModel.PlayerViewModel.agentShouldReloadStreams static variable.
			}
		}

		const string keyPlayCounters = "playCounters";
		static Dictionary<int, int> s_channelPlayCounters = null;

		public static Dictionary<int, int> channelPlayCounters
		{
			get
			{
				if( Global.inFormDesigner )
					return null;

				if( null != s_channelPlayCounters )
					return s_channelPlayCounters;
				Dictionary<int, int> res = null;
				if( IsolatedStorageSettings.ApplicationSettings.TryGetValue( keyPlayCounters, out res ) )
				{
					s_channelPlayCounters = res;
					return res;
				}
				return null;
			}
		}

		public static void incrementPlayCounter( int idChannel )
		{
			Dictionary<int, int> dict = channelPlayCounters;
			if( null == dict )
			{
				dict = new Dictionary<int, int>()
				{
					{idChannel, 1}
				};
				s_channelPlayCounters = dict;
			}
			else
			{
				if( dict.ContainsKey( idChannel ) )
					dict[ idChannel ] = dict[ idChannel ] + 1;
				else
					dict[ idChannel ] = 1;
			}
			IsolatedStorageSettings.ApplicationSettings[ keyPlayCounters ] = dict;
			IsolatedStorageSettings.ApplicationSettings.Save();
		}

		const string keyNoCrashLogMails = "noCrashLogMails";
		public static bool noCrashLogMails
		{
			get
			{
				bool val = false;
				if( !IsolatedStorageSettings.ApplicationSettings.TryGetValue( keyNoCrashLogMails, out val ) )
					return false;
				return val;
			}
			set
			{
				IsolatedStorageSettings.ApplicationSettings[ keyNoCrashLogMails ] = value;
				IsolatedStorageSettings.ApplicationSettings.Save();
			}
		}

		const string keyMarketplaceReview = "marketplaceReview";
		public static MarketplaceReviewUserData marketplaceReview
		{
			get
			{
				MarketplaceReviewUserData val = null;
				if( !IsolatedStorageSettings.ApplicationSettings.TryGetValue( keyMarketplaceReview, out val ) )
					return null;
				return val;
			}
			set
			{
				IsolatedStorageSettings.ApplicationSettings[ keyMarketplaceReview ] = value;
				IsolatedStorageSettings.ApplicationSettings.Save();
			}
		}

		const string keyPowerSaverMode = "powerSaverMode";
		public static bool powerSaverMode
		{
			get
			{
				bool val = false;
				if( !IsolatedStorageSettings.ApplicationSettings.TryGetValue( keyPowerSaverMode, out val ) )
					return false;
				return val;
			}
			set
			{
				IsolatedStorageSettings.ApplicationSettings[ keyPowerSaverMode ] = value;
				IsolatedStorageSettings.ApplicationSettings.Save();
			}
		}
	}
}