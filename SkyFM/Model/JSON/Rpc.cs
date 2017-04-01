using System;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;

namespace SkyFM.Model.JSON
{
	/// <summary>This static class keeps JSON endpoints.</summary>
	public static class Endpoints
	{
		public static readonly Uri login = new Uri( @"http://api.audioaddict.com/v1/sky/members/authenticate" );
		public static readonly Uri batchUpdate = new Uri( @"http://api.audioaddict.com/v1/sky/mobile/batch_update?asset_group_key=mobile_icons&stream_set_key=appleapp,appleapp_high,appleapp_low,appleapp_premium,appleapp_premium_high,appleapp_premium_low,appleapp_premium_medium" );
		// public static readonly Uri batchUpdate = new Uri( @"http://api.audioaddict.com/v1/di/mobile/batch_update?asset_group_key=mobile_icons&stream_set_key=appleapp,appleapp_high,appleapp_low,appleapp_premium,appleapp_premium_high,appleapp_premium_low,appleapp_premium_medium" );
		public static readonly Uri trackHistory = new Uri( @"http://api.audioaddict.com/v1/sky/track_history" );
		public const string strProtocol = "http:";
	}

	/// <summary>This class is a proxy for audioaddict's JSON RPC web service.</summary>
	public static class Rpc
	{
		const string s_authorization = "Basic ZXBoZW1lcm9uOmRheWVpcGgwbmVAcHA="; // "ephemeron:dayeiph0ne@pp", regardless on the user name

		public static async Task<LoginResponse> authenticate( string username, string password )
		{
			// s_authorization = "Basic " + Convert.ToBase64String( Encoding.UTF8.GetBytes( username + ":" + password ) );
			WebClient webClient = new GZipWebClient();
			webClient.Headers[ HttpRequestHeader.Authorization ] = s_authorization;
			String data = String.Format( "username={0}&password={1}", HttpUtility.UrlEncode( username ), HttpUtility.UrlEncode( password ) );
			string response = await webClient.UploadStringTaskAsync( Endpoints.login, data );
			return JsonConvert.DeserializeObject<LoginResponse>( response );
		}

		public static async Task<BatchUpdateResponse> batchUpdate()
		{
			WebClient webClient = new GZipWebClient();
			webClient.Headers[ HttpRequestHeader.Authorization ] = s_authorization;

			// Unlike login which is 800 bytes, this response is relatively large (400 kb).
			// Deserializing it to string is a bad idea. That's why it's streamed directly to JsonSerializer by the code below.
			using( Stream binStream = await webClient.OpenReadTaskAsync( Endpoints.batchUpdate ) )
			using( TextReader textStream = new StreamReader( binStream ) )
			{
				// Debug code
				string completeResponse = textStream.ReadToEnd(); throw new NotImplementedException();

				JsonSerializer serializer = JsonSerializer.Create( null );
				return serializer.Deserialize<BatchUpdateResponse>( new JsonTextReader( textStream ) );
			}
		}

		public static async Task<Dictionary<int, TrackHistory>> trackHistory()
		{
			WebClient webClient = new GZipWebClient();
			webClient.Headers[ HttpRequestHeader.Authorization ] = s_authorization;

			using( Stream binStream = await webClient.OpenReadTaskAsync( Endpoints.trackHistory ) )
			using( TextReader textStream = new StreamReader( binStream ) )
			{
				JsonSerializer serializer = JsonSerializer.Create( null );
				serializer.Converters.Add( new DictionaryConverter<int, TrackHistory>() );
				return serializer.Deserialize<Dictionary<int, TrackHistory>>( new JsonTextReader( textStream ) );
			}
		}
	}
}