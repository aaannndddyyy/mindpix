/*
    Timeout
    Copyright (C) 2000-2008 Bob Mottram
    fuzzgun@gmail.com

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace cognitivedynamics
{
    public class ThreadTimeout
    {
        private WaitCallback _callback;
		public bool finished = false;

        /// <summary>
        /// constructor
        /// </summary>
        public ThreadTimeout(
            WaitCallback callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            _callback = callback;
        }

        /// <summary>
        /// ThreadStart delegate
        /// </summary>
        public void Execute()
        {
            Update();
            _callback("");
        }

        /// <summary>
        /// client receives data
        /// </summary>
        private void Update()
        {
			DateTime start_time = DateTime.Now;			
			while (!finished)
			{
                System.Threading.Thread.Sleep(100);
				TimeSpan diff = DateTime.Now.Subtract(start_time);
				if (diff.TotalSeconds >= 30) finished = true;
			}
        }
    }
}
