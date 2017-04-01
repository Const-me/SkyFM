using System;
using System.Collections.Generic;
using System.Net;

namespace SkyFM
{
	static class WebExceptionStatuses
	{
		static Dictionary<WebExceptionStatus, string> s_webExceptionStatuses = new Dictionary<WebExceptionStatus, string>()
		{
			{ WebExceptionStatus.ConnectFailure, "The remote service could not be contacted at the transport level." },
			{ WebExceptionStatus.ConnectionClosed, "The connection was closed prematurely" },
			{ WebExceptionStatus.KeepAliveFailure, "The server closed a connection made with the Keep-alive header set." },
			{ WebExceptionStatus.NameResolutionFailure, "The name service could not resolve the host name." },
			{ WebExceptionStatus.ProtocolError, "The response received from the server was complete but indicated an error at the protocol level." },
			{ WebExceptionStatus.ReceiveFailure, "A complete response was not received from the remote server." },
			{ WebExceptionStatus.RequestCanceled, "The request was canceled." },
			{ WebExceptionStatus.SecureChannelFailure, "An error occurred in a secure channel link." },
			{ WebExceptionStatus.SendFailure, "A complete request could not be sent to the remote server." },
			{ WebExceptionStatus.ServerProtocolViolation, "The server response was not a valid HTTP response." },
			{ WebExceptionStatus.Success, "No error was encountered." },
			{ WebExceptionStatus.Timeout, "No response was received within the time-out set for the request." },
			{ WebExceptionStatus.TrustFailure, "A server certificate could not be validated." },
			{ WebExceptionStatus.MessageLengthLimitExceeded, "A message was received that exceeded the specified limit when sending a request or receiving a response from the server." },
			{ WebExceptionStatus.Pending, "An internal asynchronous request is pending." },
			{ WebExceptionStatus.PipelineFailure, "This value supports the .NET Framework infrastructure and is not intended to be used directly in your code." },
			{ WebExceptionStatus.ProxyNameResolutionFailure, "The name resolver service could not resolve the proxy host name." },
			{ WebExceptionStatus.UnknownError, "An exception of unknown type has occurred." },
		};

		public static string descriptionOrNull( this WebExceptionStatus s )
		{
			string res;
			if( s_webExceptionStatuses.TryGetValue( s, out res ) )
				return res;
			return null;
		}
	}
}