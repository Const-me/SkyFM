using System;

namespace SkyFM.agent.MSS
{
	/// <summary>This class implements HE-AAC stream format.</summary>
	/// <seealso href="http://en.wikipedia.org/wiki/High-Efficiency_Advanced_Audio_Coding" />
	/// <seealso href="http://wiki.multimedia.cx/index.php?title=ADTS" />
	class HeAacStreamFormat: StreamFormat
	{
		public HeAacStreamFormat( iLocator locator ) : base( locator ) { }

		byte nChannels;
		int samplingRate;
		int avgBitrate;
		int frameLength;

		static eSetupFrameResult tryReadSampleHeader( iByteStream src,
			out byte channels, out int samplingRate, out int averageBitrate, out int frameLength )
		{
			channels = 0;
			samplingRate = 0;
			averageBitrate = 0;
			frameLength = 0;

			// Byte 1
			int val = src.ReadByte();
			if( val < 0 )
				return eSetupFrameResult.NoData;
			if( val != 0xFF )	// Frame sync
				return eSetupFrameResult.Fail;

			// Byte 2
			val = src.ReadByte();
			if( val < 0 )
				return eSetupFrameResult.NoData;

			if( ( val & 0xF0 ) != 0xF0 )	// Frame sync
				return eSetupFrameResult.Fail;

			byte mpegVersion = shiftMask( val, 3, 1 );	//    MPEG Version: 0 for MPEG-4, 1 for MPEG-2
			byte layer = shiftMask( val, 1, 3 );
			if( 0 != layer )
				return eSetupFrameResult.Fail; // Layer: always 0

			byte noProtection = shiftMask( val, 0, 1 );

			// Byte 3
			val = src.ReadByte();
			if( val < 0 )
				return eSetupFrameResult.NoData;

			byte profile = shiftMask( val, 6, 3 ); //  	 profile, the MPEG-4 Audio Object Type minus 1
			byte indSamplingRate = shiftMask( val, 2, 0xF );
			if( indSamplingRate > 12 )
				return eSetupFrameResult.Fail;
			samplingRate = Aac.lookupSampleRate( indSamplingRate );

			byte indChannelsConfig = shiftMask( val, 1, 1 );

			// Byte 4
			val = src.ReadByte();
			if( val < 0 )
				return eSetupFrameResult.NoData;

			indChannelsConfig = (byte)( ( indChannelsConfig << 2 ) | shiftMask( val, 6, 3 ) );
			if( 0 == indChannelsConfig || indChannelsConfig >= 8 )
				return eSetupFrameResult.Fail;
			channels = Aac.lookupChannelsCount( indChannelsConfig );

			frameLength = shiftMask( val, 0, 3 );

			// Byte 5
			val = src.ReadByte();
			if( val < 0 )
				return eSetupFrameResult.NoData;
			frameLength = ( ( frameLength << 8 ) | val );

			// Byte 6
			val = src.ReadByte();
			if( val < 0 )
				return eSetupFrameResult.NoData;

			frameLength = ( ( frameLength << 3 ) | shiftMask( val, 5, 7 ) );
			// int bufferFullness = ( val & 0x1F );

			// Byte 7
			val = src.ReadByte();
			if( val < 0 )
				return eSetupFrameResult.NoData;
			// bufferFullness = ( ( bufferFullness << 6 ) | shiftMask( val, 2, 0x3F ) );

			averageBitrate = Aac.calcBitRate( samplingRate, channels );

			return eSetupFrameResult.OK;
		}

		protected override eSetupFrameResult trySetupNextFrame( iByteStream src, bool deepSearch )
		{
			eSetupFrameResult res = tryReadSampleHeader( src, out this.nChannels, out this.samplingRate, out this.avgBitrate, out this.frameLength );
			if( !deepSearch || res != StreamFormat.eSetupFrameResult.OK )
				return res;

			// Skip the frame contents to the next header
			if( !src.trySkip( this.frameLength - 7 ) )  // when tryReadSampleHeader returned OK, it has read all 7 header bytes but ignores the checksum
				return eSetupFrameResult.NoData;
			byte u1; int u2, u3, u4;
			res = tryReadSampleHeader( src, out u1, out u2, out u3, out u4 );
			return res;
		}

		protected override WAVEFORMATEX constructCodecPrivateData()
		{
			return new HEAACWAVEFORMAT( nChannels, samplingRate, avgBitrate );
		}

		protected override long getSamplePlacement( out int offset, out int count )
		{
			offset = 0;
			count = this.frameLength;
			return audioDurationFromBufferSize( this.avgBitrate, this.frameLength );
		}
	}
}