﻿using System;
using DW.CodedUI.BasicElements;

namespace ElementFinder
{
    public class CatchedElementsEventArgs : EventArgs
    {
        public AutomationElementInfo AutomationElementInfo { get; private set; }

        public CatchedElementsEventArgs(AutomationElementInfo automationElementInfo)
        {
            AutomationElementInfo = automationElementInfo;
        }
    }
}