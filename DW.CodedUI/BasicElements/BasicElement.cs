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
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using DW.CodedUI.BasicElements.Data;
using DW.CodedUI.Utilities;

namespace DW.CodedUI.BasicElements
{
    /// <summary>
    /// Represents a UI control.
    /// </summary>
    public class BasicElement
    {
        private Highlighter _highlighter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DW.CodedUI.BasicElements.BasicElement" /> class.
        /// </summary>
        /// <param name="automationElement">The automation control.</param>
        public BasicElement(AutomationElement automationElement)
        {
            AutomationElement = automationElement;
        }

        /// <summary>
        /// Gets the automation control.
        /// </summary>
        public AutomationElement AutomationElement { get; private set; }

        /// <summary>
        /// Gets an array or supported patterns.
        /// </summary>
        public AutomationPattern[] SupportedPatterns
        {
            get { return AutomationElement.GetSupportedPatterns(); }
        }

        /// <summary>
        /// Gets the properties of the current control.
        /// </summary>
        public AutomationElement.AutomationElementInformation Properties
        {
            get { return AutomationElement.Current; }
        }

        /// <summary>
        /// Gets the automation ID.
        /// </summary>
        public string AutomationId
        {
            get { return Properties.AutomationId; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { return Properties.Name; }
        }

        /// <summary>
        /// Gets a value that indicates if the control is visible.
        /// </summary>
        public bool IsVisible
        {
            get { return !Properties.IsOffscreen; }
        }

        /// <summary>
        /// Gets of the control is enabled
        /// </summary>
        public bool IsEnabled
        {
            get { return Properties.IsEnabled; }
        }

        /// <summary>
        /// Waits that the control enables.
        /// </summary>
        /// <remarks>It waits a maximum of 30 seconds and checks every 100 milliseconds the state.</remarks>
        public void WaitForControlEnabled()
        {
            WaitForControlEnabled(TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Waits that the control enables.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <remarks>It checks every 100 milliseconds the state.</remarks>
        public void WaitForControlEnabled(TimeSpan timeout)
        {
            WaitForControlEnabled(timeout, TimeSpan.FromMilliseconds(100));
        }

        /// <summary>
        /// Waits that the control enables.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="waitCycle">The interval for check the IsEnabled state.</param>
        public void WaitForControlEnabled(TimeSpan timeout, TimeSpan waitCycle)
        {
            WaitForCondition(timeout, waitCycle, e => !e.Properties.IsEnabled);
        }

        /// <summary>
        /// Waits that the control gets visible.
        /// </summary>
        /// <remarks>It waits a maximum of 30 seconds and checks every 100 milliseconds the state.</remarks>
        public void WaitForControlVisible()
        {
            WaitForControlVisible(TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Waits that the control gets visible.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <remarks>It checks every 100 milliseconds the state.</remarks>
        public void WaitForControlVisible(TimeSpan timeout)
        {
            WaitForControlVisible(timeout, TimeSpan.FromMilliseconds(100));
        }

        /// <summary>
        /// Waits that the control gets visible.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="waitCycle">The interval for check the IsVisible state.</param>
        public void WaitForControlVisible(TimeSpan timeout, TimeSpan waitCycle)
        {
            WaitForCondition(timeout, waitCycle, e => e.Properties.IsOffscreen);
        }

        internal void WaitForCondition(TimeSpan timeout, TimeSpan waitCycle, Func<BasicElement, bool> condition)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (condition(this))
            {
                Thread.Sleep(waitCycle);
                if (stopwatch.Elapsed >= timeout)
                {
                    stopwatch.Stop();
                    return;
                }
            }
        }

        /// <summary>
        /// Shows up the control highlight.
        /// </summary>
        public void BeginHighlight()
        {
            if (_highlighter != null)
                _highlighter.Close();
            _highlighter = new Highlighter();
            _highlighter.Highlight(AutomationElement);
        }

        /// <summary>
        /// Removes the highlight.
        /// </summary>
        public void EndHighlight()
        {
            if (_highlighter != null)
                _highlighter.Close();
            _highlighter = null;
        }

        /// <summary>
        /// Gets a value that indicates if the control is still available.
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                try
                {
                    var elementAvailbilityCheck = Name;
                    return true;
                }
                catch (ElementNotAvailableException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Provides a good visible feedback of the control.
        /// </summary>
        /// <returns>A good name of the control with the automation ID if any.</returns>
        public override string ToString()
        {
            if (!IsAvailable)
                return "<N.A.>";

            var name = AutomationElement.Current.Name;
            if (string.IsNullOrWhiteSpace(name))
                name = "<no name>";

            var automationId = AutomationElement.Current.AutomationId;
            if (string.IsNullOrWhiteSpace(automationId))
                return name;

            return string.Format("{0} [{1}]", name, automationId);
        }

        /// <summary>
        /// Gets a combinable Do to be able to append additional settings.
        /// </summary>
        public CombinableDo Do
        {
            get { return new CombinableDo(); }
        }

        /// <summary>
        /// Make a shadow copy of the element at the current state which stays available even the element is gone.
        /// </summary>
        /// <returns>A shadow copy of the current element.</returns>
        public BasicElementData GetDataCopy()
        {
            var data = new BasicElementData();
            FillData(data);
            return data;
        }

        /// <summary>
        /// Fills the given BasicElementData object with the properties of the BasicElement.
        /// </summary>
        /// <param name="data">The BasicElementData to fill.</param>
        protected void FillData(BasicElementData data)
        {
            data.AutomationId = GetSafeData(() => Properties.AutomationId);
            data.Name = GetSafeData(() => Properties.Name);
            data.BoundingRectangle = GetSafeData(() => Properties.BoundingRectangle);
            data.ClassName = GetSafeData(() => Properties.ClassName);
            data.NativeWindowHandle = GetSafeData(() => Properties.NativeWindowHandle);
            data.ProcessId = GetSafeData(() => Properties.ProcessId);
            data.IsEnabled = GetSafeData(() => Properties.IsEnabled);
            data.IsVisible = GetSafeData(() => IsVisible);
        }

        /// <summary>
        /// Tries to reload the given data. default(T) if it crashes.
        /// </summary>
        /// <typeparam name="T">The type of the property to read.</typeparam>
        /// <param name="data">The function to read the property.</param>
        /// <returns>The property data; default(T) if it crashes.</returns>
        protected T GetSafeData<T>(Func<T> data)
        {
            try
            {
                return data();
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
