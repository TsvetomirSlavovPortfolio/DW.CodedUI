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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Automation;
using DW.CodedUI.BasicElements;
using DW.CodedUI.Internal;
using AndCondition = DW.CodedUI.Internal.AndCondition;

namespace DW.CodedUI
{
    /// <summary>
    /// Searches for Windows, Dialogs or MessageBoxes.
    /// </summary>
    public static class WindowFinder
    {
        /// <summary>
        /// Searches for a window by the given conditions. Default settings are And.Assert().And.Timeout(10000).
        /// </summary>
        /// <param name="use">Defines the conditions to be used for searching for a window.</param>
        /// <returns>The found window if any; otherwise an exception is shown.</returns>
        /// <exception cref="DW.CodedUI.WindowNotFoundException">The window could not be found.</exception>
        /// <exception cref="DW.CodedUI.WrongSetupException">Dll is missing or in the wrong version.</exception>
        /// <remarks>To change the default And settings globaly consider changing the values in the <see cref="DW.CodedUI.CodedUIEnvironment" />.</remarks>
        public static BasicWindow Search(Use use)
        {
            return Search(use, new Is(), new CombinableAnd());
        }

        /// <summary>
        /// Searches for a window by the given conditions. Default settings are And.Assert().And.Timeout(10000).
        /// </summary>
        /// <param name="use">Defines the conditions to be used for searching for a window.</param>
        /// <param name="is">Defines the relation of the window to another object.</param>
        /// <returns>The found window if any; otherwise an exception is shown.</returns>
        /// <exception cref="DW.CodedUI.WindowNotFoundException">The window could not be found.</exception>
        /// <exception cref="DW.CodedUI.WrongSetupException">Dll is missing or in the wrong version.</exception>
        public static BasicWindow Search(Use use, Is @is)
        {
            return Search(use, @is, new CombinableAnd());
        }

        /// <summary>
        /// Searches for a window by the given conditions and settings. If not disabled And.Assert().And.Timeout(10000) gets appended.
        /// </summary>
        /// <param name="use">Defines the conditions to be used for searching for a window.</param>
        /// <param name="settings">Defines the settings to be used while searching.</param>
        /// <returns>The found window if any; otherwise an exception as long its not disabled by And.NoAssert(). If its disabled the return is null.</returns>
        /// <exception cref="DW.CodedUI.WindowNotFoundException">The window could not be found. (If not disabled.)</exception>
        /// <exception cref="DW.CodedUI.WrongSetupException">Dll is missing or in the wrong version.</exception>
        /// <remarks>To change the default And settings globaly consider changing the values in the <see cref="DW.CodedUI.CodedUIEnvironment" />.</remarks>
        public static BasicWindow Search(Use use, And settings)
        {
            return Search(use, new Is(), settings);
        }

        /// <summary>
        /// Searches for a window by the given conditions and settings. If not disabled And.Assert().And.Timeout(10000) gets appended.
        /// </summary>
        /// <param name="use">Defines the conditions to be used for searching for a window.</param>
        /// <param name="is">Defines the relation of the window to another object.</param>
        /// <param name="settings">Defines the settings to be used while searching.</param>
        /// <exception cref="DW.CodedUI.WindowNotFoundException">The window could not be found. (If not disabled.)</exception>
        /// <exception cref="DW.CodedUI.WrongSetupException">Dll is missing or in the wrong version.</exception>
        /// <returns>The found window if any; otherwise an exception as long its not disabled by And.NoAssert(). If its disabled the return is null.</returns>
        public static BasicWindow Search(Use use, Is @is, And settings)
        {
            var condition = use.GetCondition();
            var settingsConditions = settings.GetConditions();
            var instanceNumber = settings.GetInstanceNumber();
            var timeout = settings.GetTimeout();
            var useTimeout = settingsConditions.Contains(AndCondition.Timeout);
            var assertResult = settingsConditions.Contains(AndCondition.Assert);
            var checkInstance = settingsConditions.Contains(AndCondition.Instance);
            var useInterval = settingsConditions.Contains(AndCondition.Interval);
            var interval = settings.GetInterval();

            if (CodedUIEnvironment.LoggerSettings.ShortLogging)
                LogPool.Append("Search for a window. {0}", use.GetConditionDescription());
            else
                LogPool.Append("Search for a window. {0}", MessageBuilder.BuildMessage(use, useTimeout, useInterval, interval, timeout, @is));

            var watch = new Stopwatch();
            watch.Start();
            while (true)
            {
                var foundInstance = 0;
                var windows = GetAllWindows();
                foreach (var window in windows)
                {
                    var matchingWindow = Matches(window, condition, @is);
                    if (matchingWindow == null)
                        continue;

                    if (!checkInstance || ++foundInstance == instanceNumber)
                    {
                        LogPool.Append("Window '{0}' found.", matchingWindow);
                        return matchingWindow;
                    }
                }

                if (!useTimeout || watch.Elapsed.TotalMilliseconds >= timeout)
                {
                    if (assertResult)
                        throw new WindowNotFoundException(use, useTimeout, useInterval, interval, watch.Elapsed, @is);
                    return null;
                }

                if (useInterval)
                    Thread.Sleep((int)interval);
            }
        }

        /// <summary>
        /// Searches for window kinds like Window, MessageBox or system dialogs. Default settings are And.Assert().And.Timeout(10000).
        /// </summary>
        /// <typeparam name="TWindow">The type of window to be created.</typeparam>
        /// <param name="use">Defines the conditions to be used for searching for a window.</param>
        /// <returns>The found window if any; otherwise an exception is shown.</returns>
        /// <exception cref="DW.CodedUI.WindowNotFoundException">The window could not be found.</exception>
        /// <exception cref="DW.CodedUI.WrongSetupException">Dll is missing or in the wrong version.</exception>
        /// <remarks>To change the default And settings globaly consider changing the values in the <see cref="DW.CodedUI.CodedUIEnvironment" />.</remarks>
        public static TWindow Search<TWindow>(Use use) where TWindow : BasicWindowBase
        {
            return Search<TWindow>(use, new Is(), new CombinableAnd());
        }

        /// <summary>
        /// Searches for window kinds like Window, MessageBox or system dialogs. Default settings are And.Assert().And.Timeout(10000).
        /// </summary>
        /// <typeparam name="TWindow">The type of window to be created.</typeparam>
        /// <param name="use">Defines the conditions to be used for searching for a window.</param>
        /// <param name="is">Defines the relation of the window to another object.</param>
        /// <returns>The found window if any; otherwise an exception is shown.</returns>
        /// <exception cref="DW.CodedUI.WindowNotFoundException">The window could not be found.</exception>
        /// <exception cref="DW.CodedUI.WrongSetupException">Dll is missing or in the wrong version.</exception>
        /// <remarks>To change the default And settings globaly consider changing the values in the <see cref="DW.CodedUI.CodedUIEnvironment" />.</remarks>
        public static TWindow Search<TWindow>(Use use, Is @is) where TWindow : BasicWindowBase
        {
            return Search<TWindow>(use, @is, new CombinableAnd());
        }

        /// <summary>
        /// Searches for window kinds like Window, MessageBox or system dialogs. If not disabled And.Assert().And.Timeout(10000) gets appended.
        /// </summary>
        /// <typeparam name="TWindow">The type of window to be created.</typeparam>
        /// <param name="use">Defines the conditions to be used for searching for a window.</param>
        /// <param name="settings">Defines the settings to be used while searching.</param>
        /// <returns>The found window if any; otherwise an exception as long its not disabled by And.NoAssert(). If its disabled the return is null.</returns>
        /// <exception cref="DW.CodedUI.WindowNotFoundException">The window could not be found. (If not disabled.)</exception>
        /// <exception cref="DW.CodedUI.WrongSetupException">Dll is missing or in the wrong version.</exception>
        /// <remarks>To change the default And settings globaly consider changing the values in the <see cref="DW.CodedUI.CodedUIEnvironment" />.</remarks>
        public static TWindow Search<TWindow>(Use use, And settings) where TWindow : BasicWindowBase
        {
            return Search<TWindow>(use, new Is(), settings);
        }

        /// <summary>
        /// Searches for window kinds like Window, MessageBox or system dialogs. If not disabled And.Assert().And.Timeout(10000) gets appended.
        /// </summary>
        /// <typeparam name="TWindow">The type of window to be created.</typeparam>
        /// <param name="use">Defines the conditions to be used for searching for a window.</param>
        /// <param name="is">Defines the relation of the window to another object.</param>
        /// <param name="settings">Defines the settings to be used while searching.</param>
        /// <returns>The found window if any; otherwise an exception as long its not disabled by And.NoAssert(). If its disabled the return is null.</returns>
        /// <exception cref="DW.CodedUI.WindowNotFoundException">The window could not be found. (If not disabled.)</exception>
        /// <exception cref="DW.CodedUI.WrongSetupException">Dll is missing or in the wrong version.</exception>
        /// <remarks>To change the default And settings globaly consider changing the values in the <see cref="DW.CodedUI.CodedUIEnvironment" />.</remarks>
        public static TWindow Search<TWindow>(Use use, Is @is, And settings) where TWindow : BasicWindowBase
        {
            var window = Search(use, @is, settings);
            if (window != null)
                return (TWindow)Activator.CreateInstance(typeof(TWindow), window.AutomationElement);
            return null;
        }

        /// <summary>
        /// Gets the window which is actually in the foreground.
        /// </summary>
        /// <returns>The found window if any; otherwise null.</returns>
        /// <exception cref="DW.CodedUI.WrongSetupException">Dll is missing or in the wrong version.</exception>
        public static BasicWindow GetForegroundWindow()
        {
            LogPool.Append("Search for the current foreground window.");

            var windowHandle = WinApi.GetForegroundWindow();
            if (windowHandle == IntPtr.Zero)
            {
                LogPool.Append("No window found.");
                return null;
            }
            try
            {
                var foregroundWindow = new BasicWindow(AutomationElement.FromHandle(windowHandle));
                LogPool.Append("Window '{0}' found.", foregroundWindow);
                return foregroundWindow;
            }
            catch (FileNotFoundException ex)
            {
                throw new WrongSetupException(ex);
            }
        }

        private static BasicWindow Matches(KeyValuePair<IntPtr, string> window, Predicate<BasicWindow> condition, Is @is)
        {
            try
            {
                var automationElement = AutomationElement.FromHandle(window.Key);
                var basicWindow = new BasicWindow(automationElement);
                if (basicWindow.IsAvailable && condition(basicWindow) && RelationMatches(basicWindow, @is))
                    return basicWindow;
            }
            catch (FileNotFoundException ex)
            {
                throw new WrongSetupException(ex);
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        private static bool RelationMatches(BasicWindow window, Is @is)
        {
            switch (@is.GetCondition())
            {
                case IsCondition.ChildOf:
                    return IsChildOf(window, @is.GetWindow());
                case IsCondition.MainWindow:
                    return IsMainWindow(window);
                case IsCondition.Nothing:
                    return true;
                case IsCondition.OwnerOf:
                    return IsOwnerOf(window, @is.GetElement());
                case IsCondition.ParentOf:
                    return IsParentOf(window, @is.GetWindow());
            }
            return false;
        }

        private static bool IsChildOf(BasicWindow potentialChildWindow, BasicWindow parentWindow)
        {
            return IsParentOf(parentWindow, potentialChildWindow);
        }

        private static bool IsMainWindow(BasicWindow window)
        {
            var mainWindow = new BasicWindow(AutomationElement.FromHandle(window.OwningProcess.MainWindowHandle));
            return window.Equals(mainWindow);
        }

        internal static bool IsOwnerOf(BasicWindow window, BasicElement potentialChildElement)
        {
            var condition = By.Condition(e => e.Properties.NativeWindowHandle == window.Properties.NativeWindowHandle);
            var settings = With.NoAssert().And.NoTimeout();
            var element = potentialChildElement.GetParent(condition, settings);
            return element != null;
        }

        private static bool IsParentOf(BasicWindow parentWindow, BasicWindow potentialChildWindow)
        {
            var childParentWindow = potentialChildWindow.GetParentWindow(false);

            return parentWindow.Equals(childParentWindow);
        }

        private static Dictionary<IntPtr, string> GetAllWindows()
        {
            var collection = new Dictionary<IntPtr, string>();
            WinApi.EnumDelegate filter = delegate(IntPtr hWnd, int lParam)
            {
                if (WinApi.IsWindowVisible(hWnd))
                {
                    var strbTitle = new StringBuilder(255);
                    WinApi.GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
                    var strTitle = strbTitle.ToString();
                    if (!string.IsNullOrEmpty(strTitle))
                        collection[hWnd] = strTitle;
                }
                return true;
            };
            WinApi.EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero);
            return collection;
        }
    }
}
