#region License
/*--------------------------------------------------------------------------------
    Copyright (c) 2012 David Wendland

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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using DW.CodedUI.Waiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DW.CodedUI
{
    internal static class CodedUIConfiguration
    {
        private static readonly Dictionary<Type, int> _classSpeeds = new Dictionary<Type, int>();
        private static readonly Dictionary<MethodBase, int> _methodSpeeds = new Dictionary<MethodBase, int>();

        internal static int GetSpeed()
        {
            var testMethod = GetTestMethod();
            if (testMethod == null)
                return 0;

            if (_methodSpeeds.ContainsKey(testMethod))
                return _methodSpeeds[testMethod];

            var methodSpeed = GetSpeedValue(testMethod);
            if (methodSpeed != null)
            {
                _methodSpeeds.Add(testMethod, methodSpeed.Value);
                return methodSpeed.Value;
            }

            if (_classSpeeds.ContainsKey(testMethod.DeclaringType))
                return _classSpeeds[testMethod.DeclaringType];

            var classSpeed = GetSpeedValue(testMethod.DeclaringType);
            if (classSpeed != null)
            {
                _classSpeeds.Add(testMethod.DeclaringType, classSpeed.Value);
                return classSpeed.Value;
            }

            return 0;
        }

        private static MethodBase GetTestMethod()
        {
            var stackTrace = new StackTrace();
            int frameNo = 3;
            var method = stackTrace.GetFrame(frameNo).GetMethod();
            while (method != null)
            {
                if (IsTestMethod(method) || IsSetupMethod(method) || IsTeardownMethod(method))
                    return method;
                method = stackTrace.GetFrame(++frameNo).GetMethod();
            }
            return null;
        }

        private static bool IsTestMethod(MethodBase method)
        {
            return method.GetCustomAttributes(typeof(TestMethodAttribute), false).Length > 0;
        }

        private static bool IsSetupMethod(MethodBase method)
        {
            return method.GetCustomAttributes(typeof(TestInitializeAttribute), false).Length > 0;
        }

        private static bool IsTeardownMethod(MethodBase method)
        {
            return method.GetCustomAttributes(typeof(TestCleanupAttribute), false).Length > 0;
        }

        private static int? GetSpeedValue(Type testClass)
        {
            var classSpeed = testClass.GetCustomAttributes(typeof(ExecutionSpeed), false);
            if (classSpeed.Length > 0)
                return ((ExecutionSpeed)classSpeed[0]).Milliseconds;
            return null;
        }

        private static int? GetSpeedValue(MethodBase testMethod)
        {
            var methodSpeed = testMethod.GetCustomAttributes(typeof(ExecutionSpeed), false);
            if (methodSpeed.Length > 0)
                return ((ExecutionSpeed)methodSpeed[0]).Milliseconds;
            return null;
        }
    }
}