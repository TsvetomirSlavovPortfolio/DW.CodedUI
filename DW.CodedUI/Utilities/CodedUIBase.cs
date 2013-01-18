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

namespace DW.CodedUI.Utilities
{
    // ReSharper disable UnusedMember.Global
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable ConvertIfStatementToNullCoalescingExpression
    // ReSharper disable CompareNonConstrainedGenericWithNull

    /// <summary>
    /// Brings you an easiert access to the recordered UIMap
    /// </summary>
    /// <example>
    /// <code lang="cs">
    /// <![CDATA[
    /// [CodedUITest]
    /// public class TryOut : CodedUIBase<UIMap>
    /// {
    ///     [TestMethod]
    ///     public void Method_TestCondition_ExpectedResult()
    ///     {
    ///         var map = UIMap;
    /// 
    ///         RefreshUIMap();
    ///     }
    /// }]]>
    /// </code>
    /// </example>
    public abstract class CodedUIBase<T> where T : new()
    {
        private T _uiMap;

        /// <summary>
        /// Gets the recorded UIMap
        /// </summary>
        protected T UIMap
        {
            private set { _uiMap = value; }
            get
            {
                if (_uiMap == null)
                    _uiMap = new T();
                return _uiMap;
            }
        }

        /// <summary>
        /// Recreates the recordered UIMap to force a refresh
        /// </summary>
        protected void RefreshUIMap()
        {
            UIMap = new T();
        }
    }

    // ReSharper restore UnusedMember.Global
    // ReSharper restore MemberCanBePrivate.Global
    // ReSharper restore ConvertIfStatementToNullCoalescingExpression
    // ReSharper restore CompareNonConstrainedGenericWithNull
}