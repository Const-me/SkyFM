using System;
using System.Text;
using System.Net;

namespace SkyFM.agent.MSS
{
	/// <summary>Defines the format of waveform-audio data.</summary>
	/// <seealso href="http://msdn.microsoft.com/en-us/library/hh180779(v=vs.95).aspx" />
	internal class WAVEFORMATEX
	{
		protected ushort wFormatTag;
		protected ushort nChannels;
		protected uint nSamplesPerSec;
		protected uint nAvgBytesPerSec;
		protected ushort nBlockAlign;
		protected ushort wBitsPerSample;
		protected ushort cbSize;

		/// <summary>Flip byte order of WORD variable.</summary>
		protected static ushort toBigEndian( ushort val )
		{
			unchecked
			{
				return (ushort)IPAddress.HostToNetworkOrder( (short)val );
			}
		}

		/// <summary>Flip byte order of DWORD variable.</summary>
		protected static uint toBigEndian( uint val )
		{
			unchecked
			{
				return (uint)IPAddress.HostToNetworkOrder( (int)val );
			}
		}

		/// <summary>Print the fields in hexadecimal.</summary>
		public virtual void PrintHex( StringBuilder sb )
		{
			sb.AppendFormat( "{0:X4}", toBigEndian( this.wFormatTag ) );
			sb.AppendFormat( "{0:X4}", toBigEndian( this.nChannels ) );
			sb.AppendFormat( "{0:X8}", toBigEndian( this.nSamplesPerSec ) );
			sb.AppendFormat( "{0:X8}", toBigEndian( this.nAvgBytesPerSec ) );
			sb.AppendFormat( "{0:X4}", toBigEndian( this.nBlockAlign ) );
			sb.AppendFormat( "{0:X4}", toBigEndian( this.wBitsPerSample ) );
			sb.AppendFormat( "{0:X4}", toBigEndian( this.cbSize ) );
		}
	}
}