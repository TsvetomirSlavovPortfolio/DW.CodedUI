#region License
/*--------------------------------------------------------------------------------
    Copyright (c) 2012-2013 David Wendland

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
    THE SOFTWARE.
--------------------------------------------------------------------------------*/
#endregion License

using System.Windows.Automation;

namespace DW.CodedUI.BasicElements
{
    // ReSharper disable UnusedMember.Global

    /// <summary>
    /// Represents an Expander
    /// </summary>
    public class BasicExpander : BasicElement
    {
        /// <summary>
        /// Initializes a new instance of the BasicExpander class
        /// </summary>
        /// <param name="automationElement">The automation control</param>
        public BasicExpander(AutomationElement automationElement)
            : base(automationElement)
        {
        }

        /// <summary>
        /// Gets if it is expanded or not
        /// </summary>
        /// <remarks>Not tested yet!</remarks>
        public bool IsExpanded // TODO: Test
        {
            get
            {
                var pattern = (ExpandCollapsePattern)AutomationElement.GetCurrentPattern(ExpandCollapsePattern.Pattern);
                return pattern.Current.ExpandCollapseState == ExpandCollapseState.Expanded;
            }
        }
    }

    // ReSharper restore UnusedMember.Global
}