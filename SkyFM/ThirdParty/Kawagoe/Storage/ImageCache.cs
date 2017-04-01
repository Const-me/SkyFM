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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kawagoe.Storage
{
    /// <summary>Defines the base class for image cache implementations.</summary>
    public abstract class ImageCache
    {
        /// <summary>
        /// The name of the default image cache.
        /// </summary>
        public const string DefaultImageCacheName = "default";

        private static ImageCache _defaultImageCache = null;
        private static object _defaultImageCacheLock = new object();

        /// <summary>The default image cache. If not set explicitly, a <see cref="PersistentImageCache"/> instance is used by default.</summary>
        public static ImageCache Default
        {
            get
            {
                if (!Deployment.Current.Dispatcher.CheckAccess())
                {
                    throw new UnauthorizedAccessException("invalid cross-thread access");
                }
                lock (_defaultImageCacheLock)
                {
                    if (_defaultImageCache == null)
                    {
                        _defaultImageCache = new PersistentImageCache(DefaultImageCacheName);
                    }
                    return _defaultImageCache;
                }
            }
            set
            {
                if (!Deployment.Current.Dispatcher.CheckAccess())
                {
                    throw new UnauthorizedAccessException("invalid cross-thread access");
                }
                lock (_defaultImageCacheLock)
                {
                    _defaultImageCache = value;
                }
            }
        }

        /// <summary>Initializes a new <see cref="ImageCache"/> instance.</summary>
        protected ImageCache(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException();
            }
            Name = name;
        }

        /// <summary>The name of the image cache.</summary>
        protected string Name
        {
            get;
            private set;
        }

        /// <summary>Retrieves the source for the image with the specified URI from the cache, downloading it if needed.</summary>
        /// <param name="imageUri">The URI of the image. Must be an absolute URI.</param>
        /// <returns>An ImageSource object, or <c>null</c> if <paramref name="imageUri"/> is <c>null</c> or not an absolute URI.</returns>
        /// <exception cref="UnauthorizedAccessException">The method is not called in the UI thread.</exception>
        public ImageSource Get(Uri imageUri)
        {
            if (!Deployment.Current.Dispatcher.CheckAccess())
            {
                throw new UnauthorizedAccessException("invalid cross-thread access");
            }
            if (imageUri == null || !imageUri.IsAbsoluteUri)
            {
                return null;
            }
            return GetInternal(imageUri);
        }

        /// <summary>Retrieves the source for the image with the specified URI from the cache, downloading it if needed.</summary>
        /// <param name="imageUriString">The URI of the image. Must be an absolute URI.</param>
        /// <returns>An ImageSource object, or <c>null</c> if <paramref name="imageUriString"/> is <c>null</c>, the empty string, or not an absolute URI.</returns>
        /// <exception cref="UnauthorizedAccessException">The method is not called in the UI thread.</exception>
        public ImageSource Get(string imageUriString)
        {
            if (!Deployment.Current.Dispatcher.CheckAccess())
            {
                throw new UnauthorizedAccessException("invalid cross-thread access");
            }
            if (string.IsNullOrEmpty(imageUriString))
            {
                return null;
            }
            Uri imageUri;
            try
            {
                imageUri = new Uri(imageUriString, UriKind.Absolute);
            }
            catch (Exception)
            {
                return null;
            }
            return Get(imageUri);
        }

		public virtual WriteableBitmap GetWriteable( Uri imageUri )
		{
			throw new NotSupportedException();
		}

        /// <summary>The actual implementation of <see cref="ImageCache.Get"/>.</summary>
        protected abstract ImageSource GetInternal(Uri imageUri);

        /// <summary>Deletes all the images from the cache.
        /// This method can block the current thread for a long time; it is advised to call it from
        /// a background thread.</summary>
        public abstract void Clear();

        /// <summary>Overrides object.ToString().</summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("ImageCache:{0}", Name);
        }
    }
}