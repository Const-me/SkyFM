using System;
using SkyFM;

namespace Newtonsoft.Json
{
	public class UnixDateTimeConverter: Newtonsoft.Json.JsonConverter
	{
		public override bool CanConvert( Type objectType )
		{
			return objectType == typeof( DateTime ) || objectType == typeof( DateTime? );
		}

		static readonly DateTime s_epoch = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );

		public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
		{
			switch( reader.TokenType )
			{
				case JsonToken.Null:
					return null;
				case JsonToken.Integer:
					break;
				default:
					throw new Exception( "Unexpected token parsing date. Expected Integer, got {0}.".FormatWith( reader.TokenType ) );
			}

			long ticks = (long)reader.Value;
			DateTime date = s_epoch.AddSeconds( ticks );
			return date;
		}

		public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
		{
			if( null == value )
			{
				writer.WriteNull();
				return;
			}
			if( value is DateTime )
			{
				DateTime val = (DateTime)value;
				if( val.Kind == DateTimeKind.Local )
					val = val.ToUniversalTime();
				long ticks = ( val - s_epoch ).Ticks;
				if( ticks < 0 )
					throw new ArgumentOutOfRangeException( "Unix epoch starts January 1st, 1970" );
				long seconds = ( ticks + ( TimeSpan.TicksPerSecond / 2 ) ) / TimeSpan.TicksPerSecond;
				writer.WriteValue( seconds );
				return;
			}
			throw new Exception( "Expected date object value." );
		}
	}
}