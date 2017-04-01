using System;
using System.Text;

namespace SkyFM.agent.MSS
{
	/// <summary>Contains format data for an AAC or HE-AAC stream</summary>
	class HEAACWAVEFORMAT: WAVEFORMATEX
	{
		ushort wPayloadType;
		ushort wAudioProfileLevelIndication;
		ushort wStructType;

		public override void PrintHex( StringBuilder sb )
		{
			base.PrintHex( sb );
			sb.AppendFormat( "{0:X4}", toBigEndian( this.wPayloadType ) );
			sb.AppendFormat( "{0:X4}", toBigEndian( this.wAudioProfileLevelIndication ) );
			sb.AppendFormat( "{0:X4}", toBigEndian( this.wStructType ) );
		}

		const ushort WAVE_FORMAT_MPEG_HEAAC = 0x1610;

		public HEAACWAVEFORMAT( byte nChannels, int samplingRate, int avgBitrate )
		{
			// Sky.fm example:
			// 101602002256000033810000010000000C000100FE000000
			base.wFormatTag = WAVE_FORMAT_MPEG_HEAAC;
			base.nChannels = nChannels;
			base.nSamplesPerSec = (uint)samplingRate;
			base.nAvgBytesPerSec = (uint)( ( avgBitrate + 7 ) / 8 );
			base.nBlockAlign = 1;      // 0100
			base.wBitsPerSample = 0;   // 0000: Some compression schemes cannot define a value for wBitsPerSample, so this member can be zero. 
			base.cbSize = 12;          // 0C00

			this.wPayloadType = 1;     // The stream contains an adts_sequence, as defined by MPEG-2
			this.wAudioProfileLevelIndication = 0xFE;	// No audio profile specified
			this.wStructType = 0;
		}
	}
}