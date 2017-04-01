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
using System.Windows.Threading;

namespace Kawagoe.Threading
{
    /// <summary>
    /// Provides a one-shot timer integrated to the Dispatcher queue.
    /// </summary>
    public class OneShotDispatcherTimer
    {
        /// <summary>
        /// Creates a new <see cref="OneShotDispatcherTimer"/> and starts it.
        /// </summary>
        /// <param name="duration">The duration of the timer.</param>
        /// <param name="callback">The delegate that will be called when the timer fires.</param>
        /// <returns>The newly created timer.</returns>
        public static OneShotDispatcherTimer CreateAndStart(TimeSpan duration, EventHandler callback)
        {
            OneShotDispatcherTimer timer = new OneShotDispatcherTimer();
            timer.Duration = duration;
            timer.Fired += callback;
            timer.Start();
            return timer;
        }

        private TimeSpan _duration = TimeSpan.Zero;
        private DispatcherTimer _timer = null;

        /// <summary>
        /// Initializes a new <see cref="OneShotDispatcherTimer"/> instance.
        /// </summary>
        public OneShotDispatcherTimer()
        {
        }

        /// <summary>
        /// The duration of the timer. The default is 00:00:00.
        /// </summary>
        /// <remarks>
        /// Setting the value of this property takes effect the next time the timer is started.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">The specified value when setting this property represents
        /// a negative time internal.</exception>
        public TimeSpan Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                if (value.TotalMilliseconds < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _duration = value;
            }
        }

        /// <summary>
        /// Indicates whether the timer is currently started.
        /// </summary>
        public bool IsStarted
        {
            get
            {
                return (_timer != null);
            }
        }

        /// <summary>
        /// Occurs when the one-shot timer fires.
        /// </summary>
        public event EventHandler Fired;

        /// <summary>
        /// Raises the <see cref="Fired"/> event.
        /// </summary>
        private void RaiseFired()
        {
            if (Fired != null)
            {
                try
                {
                    Fired(this, EventArgs.Empty);
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Starts the timer.
        /// This method has no effect if the timer is already started.
        /// </summary>
        /// <remarks>
        /// The same <see cref="OneShotDispatcherTimer"/> instance can be started and stopped multiple times.
        /// </remarks>
        public void Start()
        {
            if (_timer != null)
            {
                return;
            }

            _timer = new DispatcherTimer();
            _timer.Interval = _duration;
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        /// <summary>
        /// Stops the timer.
        /// This method has no effect if the timer is not started.
        /// </summary>
        /// <remarks>
        /// The <see cref="Fired"/> event is guaranteed not to be raised once this method has been invoked
        /// and until the timer is started again.
        /// </remarks>
        public void Stop()
        {
            if (_timer == null)
            {
                return;
            }
            try
            {
                _timer.Stop();
            }
            catch (Exception) { }
            _timer = null;
        }

        /// <summary>
        /// Listens to Tick events on the underlying timer.
        /// </summary>
        private void OnTimerTick(object sender, EventArgs e)
        {
            if (sender != _timer)
            {
                return;
            }
            Stop();
            RaiseFired();
        }
    }
}
