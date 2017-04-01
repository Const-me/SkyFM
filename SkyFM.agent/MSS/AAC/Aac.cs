using System;
using System.Collections.Generic;
using System.Linq;

namespace SkyFM.agent.MSS
{
	static class Aac
	{
		// Sauce: http://wiki.multimedia.cx/index.php?title=MPEG-4_Audio#Sampling_Frequencies
		static readonly int[] s_samplingRates = new int[ 13 ] { 96000,  88200, 64000, 48000, 44100, 32000, 24000, 22050, 16000, 12000, 11025, 8000, 7350 };

		public static int lookupSampleRate( byte ind )
		{
			if( ind > 12 )
				throw new ArgumentOutOfRangeException();
			return s_samplingRates[ ind ];
		}

		static readonly byte[] s_channelsIndex = new byte[ 8 ] { 0, 1, 2, 3, 4, 5, 6, 8 };

		public static byte lookupChannelsCount( byte ind )
		{
			if( ind >= 8 )
				throw new ArgumentOutOfRangeException();
			return s_channelsIndex[ ind ];
		}

		const int bitsPerBlock = 6144;
		const int samplesPerBlock = 1024;

		public static int calcBitRate( int sampleRate, byte numberOfChannels )
		{
			return bitsPerBlock / samplesPerBlock * sampleRate * numberOfChannels;
		}
	}
}