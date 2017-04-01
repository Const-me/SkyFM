// Copyright 2010 Andreas Saudemont (andreas.saudemont@gmail.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Kawagoe.Threading;

namespace Kawagoe.Storage
{
    /// <summary>Implements an <see cref="ImageCache"/> on top of the isolated storage.</summary>
    public class PersistentImageCache : ImageCache
    {
        /// <summary>The default value of <see cref="PersistentImageCache.ExpirationDelay"/>.</summary>
        public static readonly TimeSpan DefaultExpirationDelay = TimeSpan.FromDays(1);

        /// <summary>The default value of <see cref="PersistentImageCache.MemoryCacheCapacity"/>.</summary>
        public const int DefaultMemoryCacheCapacity = 100;

        private const string ImageDataExtension = "data";
        private const string ImageTimestampExtension = "tstamp";

        private TimeSpan _expirationDelay = DefaultExpirationDelay;

        /// <summary>Initializes a new <see cref="PersistentImageCache"/> instance with the specified name.</summary>
        public PersistentImageCache(string name)
            : base(name)
        {
        }

        /// <summary>The delay after which an image, once downloaded, is consider expired and is deleted from the cache.</summary>
        public TimeSpan ExpirationDelay
        {
            get
            {
                return _expirationDelay;
            }
            set
            {
                if (value.TotalMinutes < 1)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _expirationDelay = value;
                RequestCachePruning();
            }
        }

        /// <summary>Implements <see cref="ImageCache.GetInternal"/>.</summary>
        protected override ImageSource GetInternal(Uri imageUri)
        {
            BitmapImage imageSource = new BitmapImage();

			string imageKey = GetImageKey( imageUri );
			Stream imageDataStream = LoadImageFromMemoryCache( imageKey );
			if( imageDataStream != null )
            {
				if( !SetBitmapSourceStream( imageSource, imageDataStream, imageKey ) )
					return null;
                return imageSource;
            }

			WeakReference imageSourceRef = new WeakReference( imageSource );
			ThreadPool.QueueUserWorkItem( ( state ) =>
			{
				LoadImageSource( imageUri, imageSourceRef );
			} );
            return imageSource;
        }

		WriteableBitmap writeableBitmapFromStream( Stream stm, string key )
		{
			if( null == stm ) return null;
			BitmapImage imageSource = new BitmapImage();
			if( !SetBitmapSourceStream( imageSource, stm, key ) )
				return null;
			return new WriteableBitmap( imageSource );
		}

		public override WriteableBitmap GetWriteable( Uri imageUri )
		{
			string imageKey = GetImageKey( imageUri );
			Stream imageDataStream = LoadImageFromMemoryCache( imageKey );
			if( null != imageDataStream )
				return writeableBitmapFromStream( imageDataStream, imageKey );
			imageDataStream = ReadImageDataFromCache( imageKey );
			if( null != imageDataStream )
				return writeableBitmapFromStream( imageDataStream, imageKey );
			return null;
		}

        private void LoadImageSource(Uri imageUri, WeakReference imageSourceRef)
        {
            BitmapImage imageSource = imageSourceRef.Target as BitmapImage;
            if (imageSource == null)
            {
                return;
            }

            string imageKey = GetImageKey(imageUri);
            Stream imageDataStream = LoadImageFromMemoryCache(imageKey);
            if (imageDataStream == null)
            {
                imageDataStream = ReadImageDataFromCache(imageKey);
            }
            if (imageDataStream != null)
            {
				SetBitmapSourceStream( imageSource, imageDataStream, imageKey );
            }
            else
            {
                RequestImageDownload(imageUri, imageSourceRef);
            }
        }

        /// <summary>Implements <see cref="ImageCache.Clear"/>.</summary>
        public override void Clear()
        {
            lock (_storeLock)
            lock (_memoryCacheLock)
            {
                ClearMemoryCache();
                DeleteAllImagesFromStore();
            }
        }

        #region Image Downloads

        private readonly Dictionary<Uri, ImageRequest> _pendingRequests = new Dictionary<Uri, ImageRequest>();

        private void RequestImageDownload(Uri imageUri, WeakReference imageSourceRef)
        {
            if (imageUri == null || imageSourceRef == null || imageSourceRef.Target == null)
            {
                return;
            }

            lock (_pendingRequests)
            {
                PrunePendingRequests();

                if (_pendingRequests.ContainsKey(imageUri))
                {
                    ImageRequest request = _pendingRequests[imageUri];
                    lock (request)
                    {
                        _pendingRequests[imageUri].SourceRefs.Add(imageSourceRef);
                    }
                }
                else
                {
                    ImageRequest request = new ImageRequest(imageUri);
                    request.Completed += OnImageRequestCompleted;
                    request.SourceRefs.Add(imageSourceRef);
                    _pendingRequests[imageUri] = request;
                    try
                    {
                        request.Start();
                    }
                    catch (Exception)
                    {
                        _pendingRequests.Remove(imageUri);
                    }
                }
            }
        }

        private void OnImageRequestCompleted(object sender, EventArgs e)
        {
            ImageRequest request = sender as ImageRequest;
            if (request == null)
            {
                return;
            }

            lock (_pendingRequests)
            {
                PrunePendingRequests();

                if (!_pendingRequests.ContainsKey(request.ImageUri))
                {
                    return;
                }
                _pendingRequests.Remove(request.ImageUri);

                if (request.ImageData == null || request.ImageData.Length == 0)
                {
                    return;
                }

                string imageKey = GetImageKey(request.ImageUri);
                WriteImageToCache(imageKey, request.ImageData);
                WriteImageToMemoryCache(imageKey, request.ImageData);

                foreach (WeakReference sourceRef in request.SourceRefs)
                {
                    BitmapSource imageSource = sourceRef.Target as BitmapSource;
                    if (imageSource != null)
                    {
						Stream imageDataStream = new MemoryStream( request.ImageData );
						SetBitmapSourceStream( imageSource, imageDataStream, imageKey );
                    }
                }
            }
        }

        private void PrunePendingRequests()
        {
            lock (_pendingRequests)
            {
                List<Uri> obsoleteUris = null;

                foreach (Uri imageUri in _pendingRequests.Keys)
                {
                    ImageRequest request = _pendingRequests[imageUri];
                    bool hasSources = false;
                    foreach (WeakReference sourceRef in request.SourceRefs)
                    {
                        if (sourceRef.Target != null)
                        {
                            hasSources = true;
                            break;
                        }
                    }
                    if (!hasSources)
                    {
                        if (obsoleteUris == null)
                        {
                            obsoleteUris = new List<Uri>();
                        }
                        obsoleteUris.Add(imageUri);
                    }
                }

                if (obsoleteUris != null)
                {
                    foreach (Uri obsoleteUri in obsoleteUris)
                    {
                        ImageRequest request = _pendingRequests[obsoleteUri];
                        _pendingRequests.Remove(obsoleteUri);
                        request.Cancel();
                    }
                }
            }
        }

        private class ImageRequest
        {
            private bool _started = false;
            private HttpWebRequest _webRequest = null;
            private Stream _responseInputStream = null;
            private byte[] _responseBuffer = new byte[4096];
            private MemoryStream _responseDataStream = new MemoryStream();

            public ImageRequest(Uri imageUri)
            {
                ImageUri = imageUri;
                ImageData = null;
                SourceRefs = new List<WeakReference>();
            }

            public Uri ImageUri
            {
                get;
                private set;
            }

            public byte[] ImageData
            {
                get;
                private set;
            }

            public IList<WeakReference> SourceRefs
            {
                get;
                private set;
            }

            public void Start()
            {
                lock (this)
                {
                    if (_started)
                    {
                        return;
                    }
                    _started = true;

                    _webRequest = (HttpWebRequest)HttpWebRequest.Create(ImageUri);
                    _webRequest.BeginGetResponse(OnGotResponse, null);
                }
            }

            public void Cancel()
            {
                lock (this)
                {
                    if (!_started)
                    {
                        return;
                    }
                    HttpWebRequest webRequest = _webRequest;
                    ReleaseResources();
                    if (webRequest != null)
                    {
                        try
                        {
                            webRequest.Abort();
                        }
                        catch (Exception) { }
                    }
                }
            }

            public event EventHandler Completed;

            private void OnGotResponse(IAsyncResult asyncResult)
            {
                lock (this)
                {
                    if (_webRequest == null)
                    {
                        return;
                    }
                    try
                    {
                        HttpWebResponse webResponse = (HttpWebResponse)_webRequest.EndGetResponse(asyncResult);
                        _responseInputStream = webResponse.GetResponseStream();
                        _responseInputStream.BeginRead(_responseBuffer, 0, _responseBuffer.Length, OnReadResponseCompleted, null);
                    }
                    catch (Exception)
                    {
                        NotifyCompletion();
                    }
                }
            }

            private void OnReadResponseCompleted(IAsyncResult asyncResult)
            {
                lock (this)
                {
                    if (_responseInputStream == null)
                    {
                        return;
                    }
                    try
                    {
                        int readCount = _responseInputStream.EndRead(asyncResult);
                        if (readCount > 0)
                        {
                            _responseDataStream.Write(_responseBuffer, 0, readCount);
                            _responseInputStream.BeginRead(_responseBuffer, 0, _responseBuffer.Length, OnReadResponseCompleted, null);
                        }
                        else
                        {
                            if (_responseDataStream.Length > 0)
                            {
                                ImageData = _responseDataStream.ToArray();
                            }
                            NotifyCompletion();
                        }
                    }
                    catch (Exception)
                    {
                        NotifyCompletion();
                    }
                }
            }

            private void NotifyCompletion()
            {
                lock (this)
                {
                    ReleaseResources();

                    ThreadPool.QueueUserWorkItem((state) =>
                    {
                        if (Completed == null)
                        {
                            return;
                        }
                        try
                        {
                            Completed(this, EventArgs.Empty);
                        }
                        catch (Exception) { }
                    });
                }
            }

            private void ReleaseResources()
            {
                lock (this)
                {
                    _responseBuffer = null;
                    _responseDataStream = null;
                    if (_responseInputStream != null)
                    {
                        try { _responseInputStream.Dispose(); }
                        catch (Exception) { }
                        _responseInputStream = null;
                    }
                    _webRequest = null;
                }
            }
        }

        #endregion

        #region Store Access

        private readonly object _storeLock = new object();
        private IsolatedStorageFile _store = null;
        private readonly SHA1 _hasher = new SHA1Managed();

        /// <summary>The name of directory in isolated storage that contains the files of this image cache.</summary>
        private string StoreDirectoryName
        {
            get
            {
                return "ImageCache_" + Name;
            }
        }

        /// <summary>The isolated storage file used by the cache.</summary>
        private IsolatedStorageFile Store
        {
            get
            {
                lock (_storeLock)
                {
                    if (_store == null)
                    {
                        _store = IsolatedStorageFile.GetUserStoreForApplication();
                        if (!_store.DirectoryExists(StoreDirectoryName))
                        {
                            _store.CreateDirectory(StoreDirectoryName);
                        }
                    }
                    return _store;
                }
            }
        }

        private string GetImageKey(Uri imageUri)
        {
            byte[] imageUriBytes = Encoding.UTF8.GetBytes(imageUri.ToString());
            byte[] hash;
            lock (_hasher)
            {
                hash = _hasher.ComputeHash(imageUriBytes);
            }
            return BitConverter.ToString(hash).Replace("-", "");
        }

        private string GetImageFilePath(string imageKey)
        {
            return Path.Combine(StoreDirectoryName, imageKey) + "." + ImageDataExtension;
        }

        private string GetTimestampFilePath(string imageKey)
        {
            return Path.Combine(StoreDirectoryName, imageKey) + "." + ImageTimestampExtension;
        }

        private Stream ReadImageDataFromCache(string imageKey)
        {
            RequestCachePruning();

            MemoryStream dataStream = null;
            try
            {
                string imageFilePath = GetImageFilePath(imageKey);
                lock (_storeLock)
                {
                    if (!Store.FileExists(imageFilePath))
                    {
                        return null;
                    }
                    if (GetImageTimestamp(imageKey).Add(ExpirationDelay) < DateTime.UtcNow)
                    {
                        DeleteImageFromCache(imageKey);
                        return null;
                    }
                    using (IsolatedStorageFileStream fileStream = Store.OpenFile(imageFilePath, FileMode.Open, FileAccess.Read))
                    {
                        if (fileStream.Length > int.MaxValue)
                        {
                            return null;
                        }
                        dataStream = new MemoryStream((int)fileStream.Length);
                        byte[] buffer = new byte[4096];
                        while (dataStream.Length < fileStream.Length)
                        {
                            int readCount = fileStream.Read(buffer, 0, Math.Min(buffer.Length, (int)(fileStream.Length - dataStream.Length)));
                            if (readCount <= 0)
                            {
                                throw new NotSupportedException();
                            }
                            dataStream.Write(buffer, 0, readCount);
                        }
                    }
                    WriteImageToMemoryCache(imageKey, dataStream.ToArray());
                    return dataStream;
                }
            }
            catch (Exception)
            {
                if (dataStream != null)
                {
                    try { dataStream.Dispose(); }
                    catch (Exception) { }
                }
            }
            return null;
        }

        private void WriteImageToCache(string imageKey, byte[] imageData)
        {
            RequestCachePruning();

            string imageFilePath = GetImageFilePath(imageKey);
            try
            {
                lock (_storeLock)
                {
                    IsolatedStorageFileStream fileStream;
                    if (Store.FileExists(imageFilePath))
                    {
                        fileStream = Store.OpenFile(imageFilePath, FileMode.Create, FileAccess.Write);
                    }
                    else
                    {
                        fileStream = Store.OpenFile(imageFilePath, FileMode.CreateNew, FileAccess.Write);
                    }
                    using (fileStream)
                    {
                        fileStream.Seek(0, SeekOrigin.Begin);
                        while (fileStream.Position < imageData.Length)
                        {
                            fileStream.Write(imageData, (int)fileStream.Position, (int)(imageData.Length - fileStream.Position));
                        }
                    }
                    SetImageTimestamp(imageKey, DateTime.UtcNow);
                }
            }
            catch (Exception)
            {
                try
                {
                    Store.DeleteFile(imageFilePath);
                }
                catch (Exception) { }
            }
        }

        private void PrunePersistentCache()
        {
            try
            {
                lock (_storeLock)
                {
                    string searchPattern = Path.Combine(StoreDirectoryName, string.Format("*.{0}", ImageDataExtension));
                    string[] fileNames = Store.GetFileNames(searchPattern);
                    foreach (string fileName in fileNames)
                    {
                        if (!fileName.EndsWith("." + ImageDataExtension))
                        {
                            continue;
                        }
                        string imageKey = fileName.Remove(Math.Max(fileName.Length - ImageDataExtension.Length - 1, 0));
                        if (GetImageTimestamp(imageKey).Add(ExpirationDelay) < DateTime.UtcNow)
                        {
                            DeleteImageFromCache(imageKey);
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        private void DeleteImageFromCache(string imageKey)
        {
            string imageFilePath = GetImageFilePath(imageKey);
            string timestampFilePath = GetTimestampFilePath(imageKey);
            lock (_storeLock)
            {
                try
                {
                    if (Store.FileExists(imageFilePath))
                    {
                        Store.DeleteFile(imageFilePath);
                    }
                }
                catch (Exception) { }
                try
                {
                    if (Store.FileExists(timestampFilePath))
                    {
                        Store.DeleteFile(timestampFilePath);
                    }
                }
                catch (Exception) { }
            }
        }

		bool DeleteImageFromMemoryCache( string imageKey )
		{
			lock( _memoryCacheLock )
			{
				if( null == _memoryCacheNodes || null == _memoryCacheList || _memoryCacheCapacity == 0 )
					return false;
				LinkedListNode<byte[]> node = null;
				if( !_memoryCacheNodes.TryGetValue( imageKey, out node ) )
					return false;
				_memoryCacheList.Remove( node );
				_memoryCacheNodes.Remove( imageKey );
				return true;
			}
		}

        private void DeleteAllImagesFromStore()
        {
            lock (_storeLock)
            {
                string searchPattern = Path.Combine(StoreDirectoryName, "*.*");
                try
                {
                    string[] fileNames = Store.GetFileNames(searchPattern);
                    foreach (string fileName in fileNames)
                    {
                        string filePath = Path.Combine(StoreDirectoryName, fileName);
                        try
                        {
                            Store.DeleteFile(filePath);
                        }
                        catch (Exception) { }
                    }
                }
                catch (Exception) { }
            }
        }

        private DateTime GetImageTimestamp(string imageKey)
        {
            string timestampFilePath = GetTimestampFilePath(imageKey);
            try
            {
                lock (_storeLock)
                {
                    if (!Store.FileExists(timestampFilePath))
                    {
                        return DateTime.MinValue;
                    }
                    using (IsolatedStorageFileStream fileStream = Store.OpenFile(timestampFilePath, FileMode.Open, FileAccess.Read))
                    using (StreamReader fileStreamReader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        string timestampString = fileStreamReader.ReadToEnd();
                        return DateTime.Parse(timestampString).ToUniversalTime();
                    }
                }
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        private void SetImageTimestamp(string imageKey, DateTime timestamp)
        {
            string timestampFilePath = GetTimestampFilePath(imageKey);
            try
            {
                lock (_storeLock)
                {
                    IsolatedStorageFileStream fileStream;
                    if (Store.FileExists(timestampFilePath))
                    {
                        fileStream = Store.OpenFile(timestampFilePath, FileMode.Create, FileAccess.Write);
                    }
                    else
                    {
                        fileStream = Store.OpenFile(timestampFilePath, FileMode.CreateNew, FileAccess.Write);
                    }
                    using (fileStream)
                    using (StreamWriter fileStreamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                    {
                        fileStreamWriter.Write(timestamp.ToUniversalTime().ToString("u"));
                    }
                }
            }
            catch (Exception) { }
        }

        #endregion

        #region Cache Pruning

        private static readonly TimeSpan CachePruningInterval = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan CachePruningTimerDuration = TimeSpan.FromSeconds(5);

        private DateTime _cachePruningTimestamp = DateTime.MinValue;
        private OneShotDispatcherTimer _cachePruningTimer = null;

        private void RequestCachePruning()
        {
            lock (this)
            {
                if (_cachePruningTimer != null || _cachePruningTimestamp.Add(CachePruningInterval) >= DateTime.UtcNow)
                {
                    return;
                }
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (_cachePruningTimer != null)
                    {
                        return;
                    }
                    _cachePruningTimer = OneShotDispatcherTimer.CreateAndStart(CachePruningTimerDuration, OnCachePruningTimerFired);
                });
            }
        }

        private void OnCachePruningTimerFired(object sender, EventArgs e)
        {
            if (sender != _cachePruningTimer)
            {
                return;
            }
            _cachePruningTimer = null;
            _cachePruningTimestamp = DateTime.UtcNow;
            ThreadPool.QueueUserWorkItem((state) => { PruneCache(); });
        }

        private void PruneCache()
        {
            PrunePersistentCache();
            PruneMemoryCache();
        }

        #endregion

        #region Memory Cache

        private readonly object _memoryCacheLock = new object();
        private int _memoryCacheCapacity = DefaultMemoryCacheCapacity;
        private Dictionary<string, LinkedListNode<byte[]>> _memoryCacheNodes = new Dictionary<string, LinkedListNode<byte[]>>(DefaultMemoryCacheCapacity);
        private LinkedList<byte[]> _memoryCacheList = new LinkedList<byte[]>();

        /// <summary>The capacity of the in-memory cache.
        /// If set to zero, the in-memory cache is disabled.</summary>
        public int MemoryCacheCapacity
        {
            get
            {
                return _memoryCacheCapacity;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                lock (_memoryCacheLock)
                {
                    _memoryCacheCapacity = value;
                    PruneMemoryCache();
                }
            }
        }

        private Stream LoadImageFromMemoryCache(string imageKey)
        {
            lock (_memoryCacheLock)
            {
                if (_memoryCacheCapacity == 0)
                {
                    return null;
                }
                if (!_memoryCacheNodes.ContainsKey(imageKey))
                {
                    return null;
                }
                LinkedListNode<byte[]> node = _memoryCacheNodes[imageKey];
                if (node.List == _memoryCacheList)
                {
                    _memoryCacheList.Remove(node);
                }
                _memoryCacheList.AddLast(node.Value);
                PruneMemoryCache();
                return new MemoryStream(node.Value);
            }
        }

        private void WriteImageToMemoryCache(string imageKey, byte[] imageData)
        {
            if (string.IsNullOrEmpty(imageKey) || imageData == null || imageData.Length == 0)
            {
                return;
            }
            lock (_memoryCacheLock)
            {
                if (_memoryCacheCapacity == 0)
                {
                    return;
                }
                if (_memoryCacheNodes.ContainsKey(imageKey))
                {
                    _memoryCacheList.Remove(_memoryCacheNodes[imageKey]);
                }
                LinkedListNode<byte[]> newNode = _memoryCacheList.AddLast(imageData);
                PruneMemoryCache();
                _memoryCacheNodes[imageKey] = newNode;
            }
        }

		/// <summary>Truncate the memory cache</summary>
        private void PruneMemoryCache()
        {
            lock (_memoryCacheLock)
            {
                if (_memoryCacheCapacity == 0)
                {
                    ClearMemoryCache();
                    return;
                }
                while (_memoryCacheList.Count > _memoryCacheCapacity)
                {
                    DeleteFirstMemoryCacheNode();
                }
            }
        }

        private void DeleteFirstMemoryCacheNode()
        {
            lock (_memoryCacheLock)
            {
                LinkedListNode<byte[]> node = _memoryCacheList.First;
                if (node == null)
                {
                    return;
                }
                _memoryCacheList.Remove(node);
                foreach (string imageKey in _memoryCacheNodes.Keys)
                {
                    if (_memoryCacheNodes[imageKey] == node)
                    {
                        _memoryCacheNodes.Remove(imageKey);
                        break;
                    }
                }
            }
        }

        private void ClearMemoryCache()
        {
            lock (_memoryCacheLock)
            {
                _memoryCacheNodes.Clear();
                _memoryCacheList.Clear();
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("PersistentImageCache({0})", Name);
        }

		bool SetBitmapSourceStreamImpl( BitmapSource src, Stream stm, string key )
		{
			try
			{
				// if( Global.dbgRandTest( 0.33333 ) ) throw new Exception( "Test Exception" );	// Fail in 33% cases to verify the error handling code.
				src.SetSource( stm );
				return true;
			}
			catch( Exception )
			{
				// Have no idea how to reproduce, however I saw the following crash log in the dev.center:
				// .__c__DisplayClassf._OnImageRequestCompleted_b__c
				// System.Windows.Media.Imaging.BitmapImage.SetSourceInternal
				// System.Windows.Media.Imaging.BitmapSource.SetSourceInternal
				// MS.Internal.XcpImports.BitmapSource_SetSource
				// MS.Internal.XcpImports.CheckHResult
				// ...
				//  xxx_RaiseException
				if( null != key )
				{
					DeleteImageFromCache( key );
					DeleteImageFromMemoryCache( key );
				}
			}
			return false;
		}

		/// <summary>Set the image's source to the specified data stream, catch exceptions, on exception - erase image data from cache.</summary>
		/// <returns>false if called from GUI thread, and SetSource failed. True in all other cases.</returns>
		bool SetBitmapSourceStream( BitmapSource src, Stream stm, string key )
		{
			var disp = Deployment.Current.Dispatcher;
			if( disp.CheckAccess() )
				return SetBitmapSourceStreamImpl( src, stm, key );
			disp.BeginInvoke( () =>
			{
				SetBitmapSourceStreamImpl( src, stm, key );
			} );
			return true;
		}
    }
}