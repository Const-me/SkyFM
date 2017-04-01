using System;
using SkyFM.Model;
using System.Windows;
using Microsoft.Phone.Tasks;
using System.Runtime.Serialization;
using Kawagoe.Threading;

namespace SkyFM
{
	/// <summary>This class stores the user's choice about "review this app" nagging..</summary>
	[DataContract]
	public class MarketplaceReviewUserData
	{
		/// <summary>Is set to true if user has pressed "yes".</summary><remarks>In this case, no questions is ever asked.</remarks>
		[DataMember]
		public bool m_reviewed;

		/// <summary>Next .</summary>
		[DataMember]
		public int m_NextDeclineIndex;

		/// <summary>Next time to ask user.</summary>
		[DataMember]
		public DateTime m_nextDateTime;
	}

	internal static class MarketplaceReview
	{
		static TimeSpan[] s_timespansToPostpone = new TimeSpan[]
		{
			TimeSpan.FromDays( 1 ),
			TimeSpan.FromDays( 2 ),	// 3
			TimeSpan.FromDays( 5 ),	// 8
			TimeSpan.FromDays( 12 ),	// 20
			TimeSpan.FromDays( 30 ),	// 50
		};

		static void promptReviewImpl()
		{
			MarketplaceReviewUserData data = AppSettings.marketplaceReview;
			if( null != data )
			{
				if( data.m_reviewed )
					return;	// Either already reviewed, or postponed more than s_timespansToPostpone.Length times.
				if( data.m_nextDateTime > DateTime.UtcNow )
					return;	// It is not the time
			}
			else
				data = new MarketplaceReviewUserData();

			var res = MessageBox.Show( STR.rateapp_message_text, STR.rateapp_message_title, MessageBoxButton.OKCancel );
			if( res == MessageBoxResult.OK )
			{
				// User accepted
				try
				{
					MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
					marketplaceReviewTask.Show();
				}
				catch( Exception )
				{
					return;
				}
				data.m_reviewed = true;
			}
			else
			{
				// User declined
				int ind = data.m_NextDeclineIndex;
				if( ind < 0 || ind >= s_timespansToPostpone.Length )
					data.m_reviewed = true;
				else
				{
					data.m_nextDateTime = DateTime.UtcNow + s_timespansToPostpone[ ind ];
					data.m_NextDeclineIndex++;
				}
			}

			AppSettings.marketplaceReview = data;
		}

		static OneShotDispatcherTimer s_timer;
		static readonly TimeSpan s_tsReviewDelay = TimeSpan.FromSeconds( 4 );

		public static void promptReview()
		{
			if( null != s_timer )
				s_timer.Stop();
			else
			{
				s_timer = new OneShotDispatcherTimer();
				s_timer.Fired += new EventHandler( s_timer_Fired );
			}
			s_timer.Duration = s_tsReviewDelay;
			s_timer.Start();
		}

		static void s_timer_Fired( object sender, EventArgs e )
		{
			s_timer = null;
			SharedUtils.startOnGuiThread( promptReviewImpl );
		}
	}
}