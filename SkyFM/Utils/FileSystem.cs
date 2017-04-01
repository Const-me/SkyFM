using System;
using System.IO;
using Newtonsoft.Json;

namespace SkyFM
{
	using impl = SkyFM.shared.FileSystem;
	using System.IO.IsolatedStorage;

	class FileSystem
	{
		public static bool readFile( string fileName, Action<Stream> actRead )
		{
			return impl.readFile( fileName, actRead );
		}

		public static void writeFile( string fileName, Action<Stream> actWrite )
		{
			impl.writeFile( fileName, actWrite );
		}

		public static bool deleteFile( string fileName )
		{
			using( IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication() )
			{
				if( !storage.FileExists( fileName ) )
					return false;
				storage.DeleteFile( fileName );
				return true;
			}
		}

		public static bool readObject<tObject>( string fileName, out tObject dest ) where tObject: class
		{
			tObject res = null;
			impl.readText( fileName, textStream =>
			{
				JsonSerializer serializer = JsonSerializer.Create( null );
				res = serializer.Deserialize<tObject>( new JsonTextReader( textStream ) );
			} );
			dest = res;
			return res != null;
		}

		public static void writeObject<tObject>( tObject src, string fileName ) where tObject: class
		{
			impl.writeText( fileName, textStream =>
			{
				JsonSerializer serializer = JsonSerializer.Create( null );
				serializer.Serialize( textStream, src );
			} );
		}
	}
}