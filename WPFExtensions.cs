using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace Sorter
{
    public static class WPFExtensions
    {
        public static IWin32Window GetIWin32Window(this Visual visual)
        {
            var source = PresentationSource.FromVisual(visual) as HwndSource;
            return new OldWindow(source.Handle);
        }

        private class OldWindow : IWin32Window
        {
            private readonly IntPtr _handle;

            public OldWindow(IntPtr handle)
            {
                _handle = handle;
            }

            #region IWin32Window Members

            IntPtr IWin32Window.Handle => _handle;

            #endregion
        }

        public static void DoEvents(this Application a)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                new Action(delegate { }));
        }
    }
}