using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.GZip;
using SkyFM.Model.JSON;
using System.Threading.Tasks;

namespace SkyFM.Model
{
	/// <summary>THis class caches user's login, pass and LoginResponse.</summary>
	/// <remarks>Unlike JsonCache, this one contains user's private data, so different format is used. Paranoia is good.</remarks>
	class CredentialsCache
	{
		// 128 bytes = 1024 bits of true randomness coming from atmospheric noise
		static readonly byte[] s_extraEntropy = new byte[ 128 ]
		{
			0x91, 0x1a, 0x39, 0xda, 0x1a, 0xe3, 0x0d, 0xb3, 0xe3, 0x52, 0x6b, 0x77, 0xbe, 0x6f, 0xb1, 0x93,
			0xb1, 0x4a, 0xfe, 0x7d, 0x6b, 0x10, 0xc0, 0x23, 0xba, 0x94, 0xd9, 0x5b, 0x0c, 0x6b, 0x5c, 0xbc,
			0x0f, 0x4d, 0x93, 0x45, 0xdc, 0xc5, 0x6f, 0x0f, 0xb4, 0xff, 0xbc, 0xf3, 0xfc, 0x50, 0x39, 0x9e,
			0x2a, 0xbf, 0x26, 0x60, 0x55, 0xcf, 0x9c, 0x78, 0x2e, 0x79, 0x35, 0x45, 0x8a, 0x2c, 0x16, 0x18,
			0x57, 0x69, 0xae, 0xa2, 0x4d, 0xde, 0xff, 0xc5, 0xa4, 0x3b, 0x98, 0xe0, 0xca, 0x35, 0x8d, 0x1f,
			0x4d, 0xe6, 0x61, 0x76, 0x1f, 0x02, 0x9f, 0xd3, 0x3a, 0xe1, 0x67, 0x69, 0xa1, 0x00, 0x31, 0xcb,
			0x56, 0x98, 0x74, 0x79, 0x5e, 0x83, 0x8f, 0x9b, 0x29, 0x6a, 0x2d, 0xce, 0x48, 0x5a, 0xa8, 0x12,
			0xf0, 0x9d, 0x3a, 0x30, 0x83, 0x47, 0xe0, 0xdf, 0xe6, 0x35, 0x89, 0xfe, 0xc4, 0x97, 0x76, 0x80, 
		};

		const string fileCredsCache = "stations.bin";	// for lulz

		static LoginCache readObject()
		{
			// Read to RAM
			byte[] bytes = null;
			FileSystem.readFile( fileCredsCache, binStream =>
			{
				bytes = binStream.ReadFully();
			} );
			if( null == bytes )
				return null;

			// Decrypt
			bytes = ProtectedData.Unprotect( bytes, s_extraEntropy );

			// Deserialize
			using( MemoryStream sMem = new MemoryStream( bytes, false ) )
			using( GZipInputStream sUnpack = new GZipInputStream(sMem ) )
			using( TextReader textStream = new StreamReader( sUnpack ) )
			{
				JsonSerializer serializer = JsonSerializer.Create( null );
				return serializer.Deserialize<LoginCache>( new JsonTextReader( textStream ) );
			}
		}

		static void writeObject( LoginCache src )
		{
			byte[] bytes;
			// Serialize to RAM
			using( MemoryStream sMem = new MemoryStream( 1024 ) )
			{
				using( GZipOutputStream sPack = new GZipOutputStream( sMem ) )
				using( TextWriter textStream = new StreamWriter( sPack ) )
				{
					JsonSerializer serializer = JsonSerializer.Create( null );
					serializer.Serialize( textStream, src );
				}
				bytes = sMem.ToArray();
			}
			
			// encrypt
			bytes = ProtectedData.Protect( bytes, s_extraEntropy );

			// Write
			FileSystem.writeFile( fileCredsCache, binStream =>
			{
				binStream.Write( bytes, 0, bytes.Length );
			} );
		}

		LoginCache m_cache;

		private CredentialsCache()
		{
			m_cache = readObject();
		}

		static CredentialsCache s_cache;
		public static CredentialsCache instance
		{
			get
			{
				if( null != s_cache )
					return s_cache;
				s_cache = new CredentialsCache();
				return s_cache;
			}
		}

		void saveResponse( string userName, string password, LoginResponse response )
		{
			m_cache = new LoginCache( userName, password, response );
			writeObject( m_cache );
		}

		public bool hasLoginPass { get {
			return m_cache != null && !String.IsNullOrEmpty( m_cache.userName ) && !String.IsNullOrEmpty( m_cache.password );
		} }

		public string login { get {
			return m_cache.userName;
		} }

		public string password { get {
			return m_cache.password;
		} }

		public async Task<bool> authenticate( string u, string p )
		{
			if( hasLoginPass && !String.IsNullOrEmpty( listenKey ) && u == this.login && p == this.password )
				return true;

			LoginResponse loginResp = await Rpc.authenticate( u, p );
			if( null == loginResp )
				return false;
			saveResponse( u, p, loginResp );
			return true;
		}

		public Task update()
		{
			return this.authenticate( this.m_cache.userName, this.m_cache.password );
		}

		static readonly TimeSpan s_tsListenKeyLifetime = TimeSpan.FromMinutes( 30 );

		public string listenKey { get {
			if( null == m_cache || null == m_cache.response )
				return null;
			if( String.IsNullOrEmpty( m_cache.response.listen_key ) )
				return null;
			/* if( m_cache.downloaded_at.newerThen( s_tsListenKeyLifetime ) )
				return m_cache.response.listen_key;
			return null; */
			return m_cache.response.listen_key;
		} }

		public bool clear()
		{
			m_cache = null;
			return FileSystem.deleteFile( fileCredsCache );
		}

		public bool needsUpdating { get {
			bool? p = AppSettings.loginPreference;
			if( p != true ) return false;	// using free streams
			if( null == m_cache || null == m_cache.response )
				return true;
			return m_cache.downloaded_at.olderThen( s_tsListenKeyLifetime );
		} }
	}
}