using System;

namespace Newtonsoft.Json
{
	// http://stackoverflow.com/a/11830387/126995
	public class ByteArrayConvertor: Newtonsoft.Json.JsonConverter
	{
		public override bool CanConvert( Type objectType )
		{
			return objectType == typeof( byte[] );
		}

		// http://stackoverflow.com/a/9995303/126995
		static byte[] StringToByteArrayFastest( string hex )
		{
			if( hex.Length % 2 == 1 )
				throw new Exception( "The binary key cannot have an odd number of digits" );

			byte[] arr = new byte[ hex.Length >> 1 ];

			for( int i = 0; i < hex.Length >> 1; ++i )
			{
				arr[ i ] = (byte)( ( GetHexVal( hex[ i << 1 ] ) << 4 ) + ( GetHexVal( hex[ ( i << 1 ) + 1 ] ) ) );
			}

			return arr;
		}

		static int GetHexVal( char hex )
		{
			int val = (int)hex;
			//For uppercase A-F letters:
			return val - ( val < 58 ? 48 : 55 );
			//For lowercase a-f letters:
			//return val - (val < 58 ? 48 : 87);
			//Or the two combined, but a bit slower:
			//return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
		}

		public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
		{
			string val = reader.Value as string;
			return StringToByteArrayFastest( val );
		}

		public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
		{
			byte[] arr = (byte[])value;
			writer.WriteRaw( BitConverter.ToString( arr ).Replace( "-", "" ) );
		}
	}
}