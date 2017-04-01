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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kawagoe.Storage
{
    /// <summary>
    /// Implements an <see cref="ImageCache"/> using the cache mechanism provided by the system.
    /// </summary>
    public class SystemImageCache : ImageCache
    {
        /// <summary>
        /// Initializes a new <see cref="SystemImageCache"/> instance with the specified name.
        /// </summary>
        public SystemImageCache(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Implements <see cref="ImageCache.GetInternal"/>.
        /// </summary>
        protected override ImageSource GetInternal(Uri imageUri)
        {
            return new BitmapImage(imageUri);
        }

        /// <summary>
        /// Implements <see cref="ImageCache.Clear"/>.
        /// </summary>
        public override void Clear()
        {
            // do nothing
        }
    }
}
