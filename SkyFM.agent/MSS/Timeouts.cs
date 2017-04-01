using System;

namespace SkyFM.agent.MSS
{
	internal static class Timeouts
	{
		// public static readonly TimeSpan httpResponseTimeout = TimeSpan.FromSeconds( 4 );
		static readonly TimeSpan httpMinResponseTimeout = TimeSpan.FromSeconds( 6 );

		// Before version 1.6, this value was always 4 seconds. Now it's the max ( 6 seconds, timeoutForSingleRead )
		public static TimeSpan httpResponseTimeout { get
		{
			return s_httpResponseTimeout;
		} }

		static Timeouts()
		{
			setBitrate( 96000 / 8 );
		}

		// Before version 1.6, this value was 1.333, a user complained the radio can't be played on a slow connection.
		// Let's see what will happen next..
		const double timeoutsExtraFactor = 1.5;

		static double s_ticksPerByte;
		static TimeSpan s_singleRead;
		static TimeSpan s_httpResponseTimeout;

		const int cbServerPackets = 65536;

		public static void setBitrate( int bytesPerSecond )
		{
			s_ticksPerByte = (double)TimeSpan.TicksPerSecond * timeoutsExtraFactor / (double)bytesPerSecond;
			s_singleRead = timeoutForBufferRead( cbServerPackets );
			s_httpResponseTimeout = s_singleRead;
			if( s_httpResponseTimeout < httpMinResponseTimeout )
				s_httpResponseTimeout = httpMinResponseTimeout;
		}

		public static TimeSpan timeoutForBufferRead( int bytesToDownload )
		{
			double ticks = s_ticksPerByte * bytesToDownload * timeoutsExtraFactor;
			TimeSpan res = TimeSpan.FromTicks( (long)ticks );
			if( res > s_singleRead )
				return res;
			return s_singleRead;
		}

		public static TimeSpan timeoutForSingleRead { get { return s_singleRead; } }
	}
}