﻿#region License
/*
The MIT License (MIT)

Copyright (c) 2012-2018 David Wendland

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE
*/
#endregion License

using System;

namespace DW.CodedUI.Utilities
{
    /// <summary>
    /// Holds the old and new <see cref="DW.CodedUI.Utilities.WindowInfo" /> objects for events in the <see cref="DW.CodedUI.Utilities.WindowListener" />.
    /// </summary>
    public class WindowChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a value that indicated what on the window has been changed.
        /// </summary>
        public WindowChangeKind WindowChangeKind { get; set; }

        /// <summary>
        /// Gets the <see cref="DW.CodedUI.Utilities.WindowInfo" /> with the old state.
        /// </summary>
        public WindowInfo OldWindowInfo { get; private set; }

        /// <summary>
        /// Gets the <see cref="DW.CodedUI.Utilities.WindowInfo" /> with the new state.
        /// </summary>
        public WindowInfo NewWindowInfo { get; private set; }

        internal WindowChangedEventArgs(WindowChangeKind windowChangeKind, WindowInfo oldWindowInfo, WindowInfo newWindowInfo)
        {
            WindowChangeKind = windowChangeKind;
            OldWindowInfo = oldWindowInfo;
            NewWindowInfo = newWindowInfo;
        }
    }
}
