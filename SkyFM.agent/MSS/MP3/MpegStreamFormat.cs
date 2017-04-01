using System;

namespace SkyFM.agent.MSS
{
	/// <summary>This class implements MP3 stream format.</summary>
	/// <seealso href="http://www.datavoyage.com/mpgscript/mpeghdr.htm" />
	class MpegStreamFormat: StreamFormat
	{
		public MpegStreamFormat( iLocator locator ) : base( locator ) { }

		Mpeg.eChannels channels;
		ushort samplingRate;
		int frameSize, averageBitrate;

		static eSetupFrameResult tryReadSampleHeader( iByteStream src,
			out Mpeg.eChannels channels, out ushort samplingRate, out int frameSize, out int averageBitrate )
		{
			channels = Mpeg.eChannels.Stereo;
			samplingRate = 0;
			frameSize = 0;
			averageBitrate = 0;

			int val = src.ReadByte();
			if( val < 0 )
				return eSetupFrameResult.NoData;
			if( val != 0xFF )	// Frame sync
				return eSetupFrameResult.Fail;

			val = src.ReadByte();
			if( val < 0 )
				return eSetupFrameResult.NoData;

			if( ( val & 0xE0 ) != 0xE0 )	// Frame sync
				return eSetupFrameResult.Fail;

			byte indVersion = shiftMask( val, 3, 3 ); // MPEG Audio version ID
			if( 1 == indVersion )
				return eSetupFrameResult.Fail;

			byte indLayer = shiftMask( val, 1, 3 ); // Layer description
			if( 0 == indLayer )
				return eSetupFrameResult.Fail;
			// ignore protection

			val = src.ReadByte();
			if( val < 0 )
				return eSetupFrameResult.NoData;

			byte bitrateIndex = shiftMask( val, 4, 0xF ); // Bitrate index
			if( 0xF == bitrateIndex || 0 == bitrateIndex )
				return eSetupFrameResult.Fail;

			byte samplingRateIndex = shiftMask( val, 2, 3 ); // Sampling rate frequency index
			if( 3 == samplingRateIndex )
				return eSetupFrameResult.Fail;
			bool padding = ( 0 != shiftMask( val, 1, 1 ) ); // Padding bit

			val = src.ReadByte();
			if( val < 0 )
				return eSetupFrameResult.NoData;
			channels = (Mpeg.eChannels)shiftMask( val, 6, 3 ); // Channel Mode


			Mpeg.eVersion version = (Mpeg.eVersion)indVersion;
			byte layer = Mpeg.lookupLayer( indLayer );

			averageBitrate = Mpeg.calcBitRate( version, layer, bitrateIndex );
			samplingRate = Mpeg.lookupSampleRate( version, samplingRateIndex );
			frameSize = Mpeg.calcFrameSize( version, layer, averageBitrate, samplingRate, padding );

			return eSetupFrameResult.OK;
		}

		protected override eSetupFrameResult trySetupNextFrame( iByteStream src, bool deepSearch )
		{
			// Try to parse frame from the current position
			StreamFormat.eSetupFrameResult res = tryReadSampleHeader( src, out this.channels, out this.samplingRate, out this.frameSize, out this.averageBitrate );
			if( !deepSearch || res != StreamFormat.eSetupFrameResult.OK )
				return res;

			// Skip the frame contents to the next header
			if( !src.trySkip( this.frameSize - 4 ) )  // when tryReadSampleHeader returned OK, it has read all 4 header bytes.
				return eSetupFrameResult.NoData;

			// Try to parse the next frame header
			Mpeg.eChannels nextChannels;
			ushort nextSamplingRate;
			int nextFrameSize;
			int nextAverageBitrate;
			res = tryReadSampleHeader( src, out nextChannels, out nextSamplingRate, out nextFrameSize, out nextAverageBitrate );
			return res;
		}

		protected override WAVEFORMATEX constructCodecPrivateData()
		{
			return new MPEGLAYER3WAVEFORMAT( this.channels, this.samplingRate, this.averageBitrate, this.frameSize );
		}

		protected override long getSamplePlacement( out int offset, out int count )
		{
			offset = 0;
			count = frameSize;
			return audioDurationFromBufferSize( averageBitrate, frameSize );
		}
	}
}