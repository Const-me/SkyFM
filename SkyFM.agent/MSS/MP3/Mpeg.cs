using System;
using System.Collections.Generic;

namespace SkyFM.agent.MSS
{
	/// <summary>This static class contains utility code & data to parse MP3 container format.</summary>
	internal static class Mpeg
	{
		public enum eVersion: byte
		{
			ver25 = 0,
			ver2 = 2,
			ver1 = 3,
		};

		private static readonly byte[] s_layers = new byte[ 4 ] { 0xFF, 3, 2, 1 };

		public static byte lookupLayer( byte indLayer )
		{
			return s_layers[ indLayer ];
		}

		private static readonly short[ , ] s_bitrates = new short[ , ]
		{
			{ 0, 32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448 },	// version 1, layer 1
			{ 0, 32, 48, 56, 64,  80,  96,  112, 128, 160, 192, 224, 256, 320, 384 },	// version 1, layer 2
			{ 0, 32, 40, 48, 56,  64,  80,  96,  112, 128, 160, 192, 224, 256, 320 },	// version 1, layer 3
			{ 0, 32, 48, 56, 64,  80,  96,  112, 128, 144, 160, 176, 192, 224, 256 },	// version 2, layer 1
			{ 0, 8,  16, 24, 32,  40,  48,  56,  64,  80,  96,  112, 128, 144, 160 },	// version 2, layers 2 + 3
		};

		/// <summary>Calculates the bit rate of the Mp3 audio from the data in the frame header.</summary>
		/// <param name="version">Mp3 version parsed out of the audio frame header.</param>
		/// <param name="layer">Mp3 layer parsed out of the audio frame header.</param>
		/// <param name="bitRateIndex">Mp3 Bit rate index parsed out of the audio frame header.</param>
		/// <returns>Mp3 bit rate calculated from the provided values, if valid.  Otherwise, -2 is returned.</returns>
		public static int calcBitRate( eVersion version, byte layer, byte bitRateIndex )
		{
			switch( version )
			{
				case eVersion.ver1:
					switch( layer )
					{
						case 1: // MPEG 1 Layer 1
							return s_bitrates[ 0, bitRateIndex ] * 1000;
						case 2: // MPEG 1 Layer 2
							return s_bitrates[ 1, bitRateIndex ] * 1000;
						case 3: // MPEG 1 Layer 3 (MP3)
							return s_bitrates[ 2, bitRateIndex ] * 1000;
						default: // MPEG 1 LAYER ERROR
							throw new ArgumentException( "Invalid MPEG layer #" );
					}
				case eVersion.ver2:
				case eVersion.ver25:
					switch( layer )
					{
						case 1: // MPEG 2 or 2.5 Layer 1
							return s_bitrates[ 3, bitRateIndex ] * 1000;
						case 2: // MPEG 2 or 2.5 Layer 2
						case 3: // MPEG 2 or 2.5 Layer 3
							return s_bitrates[ 4, bitRateIndex ] * 1000;
						default:
							throw new ArgumentException( "Invalid MPEG layer #" );
					}
				default: // VERSION ERROR
					throw new ArgumentException( "Invalid MPEG version." );
			}
		}

		static readonly Dictionary<eVersion, ushort[]> s_samplingRates = new Dictionary<eVersion, ushort[]>
		{
			{ eVersion.ver1, new ushort[] { 44100, 48000, 32000 } },
			{ eVersion.ver2, new ushort[] { 22050, 24000, 16000 } },
			{ eVersion.ver25, new ushort[] { 11025, 12000,  8000 } },
		};

		public static ushort lookupSampleRate( eVersion version, byte samplingRateIndex )
		{
			return s_samplingRates[ version ][ samplingRateIndex ];
		}

		public static int calcFrameSize( eVersion version, byte layer, int bitRate, ushort samplingRate, bool pad )
		{
			int padding = ( pad ) ? 1 : 0;
			switch( layer )
			{
				case 1:
					return ( ( 12 * bitRate / samplingRate ) + padding ) * 4;
				case 2:
				case 3:
					// MPEG2 is a special case here.
					switch( version )
					{
						case eVersion.ver1:
							return ( 144 * bitRate / samplingRate ) + padding;
						case eVersion.ver2:
						case eVersion.ver25:
							return ( 72 * bitRate / samplingRate ) + padding;
						default:
							throw new ArgumentException( "Invalid MPEG version" );
					}
				default:
					throw new ArgumentException( "Invalid MPEG layer #" );
			}
		}

		public enum eChannels: byte
		{
			Stereo = 0,
			JointStereo = 1,
			DualCHannel = 2,
			Mono = 3,
		}
	}
}