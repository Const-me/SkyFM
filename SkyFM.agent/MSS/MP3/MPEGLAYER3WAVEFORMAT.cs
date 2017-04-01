using System;
using System.Text;

namespace SkyFM.agent.MSS
{
	/// <summary>Describes an MPEG Audio Layer-3 (MP3) audio format.</summary>
	internal class MPEGLAYER3WAVEFORMAT: WAVEFORMATEX
	{
		ushort wID;
		uint fdwFlags;
		ushort nBlockSize;
		ushort nFramesPerBlock;
		ushort nCodecDelay;

		public override void PrintHex( StringBuilder sb )
		{
			base.PrintHex( sb );
			sb.AppendFormat( "{0:X4}", toBigEndian( this.wID ) );
			sb.AppendFormat( "{0:X8}", toBigEndian( this.fdwFlags ) );
			sb.AppendFormat( "{0:X4}", toBigEndian( this.nBlockSize ) );
			sb.AppendFormat( "{0:X4}", toBigEndian( this.nFramesPerBlock ) );
			sb.AppendFormat( "{0:X4}", toBigEndian( this.nCodecDelay ) );
		}

		const ushort MPEGLAYER3_ID_MPEG = 1;

		/// <summary>Insert padding as needed to achieve the stated average bitrate.</summary>
		const uint MPEGLAYER3_FLAG_PADDING_ISO = 0;

		/// <summary>Always insert padding. The average bit rate may be higher than stated.</summary>
		const uint MPEGLAYER3_FLAG_PADDING_ON = 1;

		/// <summary>Never insert padding. The average bit rate may be lower than stated.</summary>
		const uint MPEGLAYER3_FLAG_PADDING_OFF = 2;

		public MPEGLAYER3WAVEFORMAT( Mpeg.eChannels ch, ushort samplingRate, int avgBitrate, int frameSize )
		{
			// The documentation @ http://msdn.microsoft.com/en-us/library/hh180779(v=vs.95).aspx has the following sample:
			// 5500 0200 22560000 10270000 0100 0000 0C00 0100 00000000 0A02 0100 0000
			// 22.05kHz stereo 80kbps with 522 bytes per frame, one frame per block

			base.wFormatTag = 0x55;    // 5500
			base.nChannels = (ushort)( ( ch == Mpeg.eChannels.Mono ) ? 1 : 2 );	// 0200
			base.nSamplesPerSec = samplingRate; // 22560000
			base.nAvgBytesPerSec = (uint)( ( avgBitrate + 7 ) / 8 );// 10270000
			base.nBlockAlign = 1;      // 0100
			base.wBitsPerSample = 0;   // 0000: Some compression schemes cannot define a value for wBitsPerSample, so this member can be zero. 
			base.cbSize = 12;          // 0C00

			this.wID = MPEGLAYER3_ID_MPEG; // 0100
			this.fdwFlags = MPEGLAYER3_FLAG_PADDING_ISO; // 00000000
			this.nBlockSize = (ushort)frameSize; // 0A02
			this.nFramesPerBlock = 1;
			this.nCodecDelay = 0;
		}
	}
}