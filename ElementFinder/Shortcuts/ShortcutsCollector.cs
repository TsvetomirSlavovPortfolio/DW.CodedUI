﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace ElementFinder.Shortcuts
{
    public class ShortcutsCollector
    {
        private readonly List<IShortcut> _shortcuts;

        public ShortcutsCollector()
        {
            _shortcuts = new List<IShortcut>();
            _proc = HookCallback;
        }

        public void SetShortcuts(params IShortcut[] shortcuts)
        {
            _shortcuts.AddRange(shortcuts);
        }

        public void ClearShortcuts()
        {
            _shortcuts.Clear();
        }

        public void Start()
        {
            _hookId = SetHook(_proc);
        }

        public void Stop()
        {
            UnhookWindowsHookEx(_hookId);
        }

        private delegate IntPtr HookCallbackProc(int nCode, IntPtr wParam, IntPtr lParam);
        private readonly HookCallbackProc _proc;
        private IntPtr _hookId;

        private IntPtr SetHook(HookCallbackProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
                return SetWindowsHookEx((int)HookType.WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
                return CallNextHookEx(_hookId, nCode, wParam, lParam);

            if (wParam == (IntPtr)KeyboardMessages.WM_KEYDOWN || wParam == (IntPtr)KeyboardMessages.WM_SYSKEYDOWN)
            {
                var key = KeyInterop.KeyFromVirtualKey(Marshal.ReadInt32(lParam));
                foreach (var shortcut in _shortcuts)
                    shortcut.KeyPressed(key);
            }
            else if (wParam == (IntPtr) KeyboardMessages.WM_KEYUP || wParam == (IntPtr) KeyboardMessages.WM_SYSKEYUP)
            {
                var key = KeyInterop.KeyFromVirtualKey(Marshal.ReadInt32(lParam));
                foreach (var shortcut in _shortcuts)
                    shortcut.KeyReleased(key);
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        public enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        private enum KeyboardMessages
        {
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookCallbackProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}