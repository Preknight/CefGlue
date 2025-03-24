using System;
using Xilium.CefGlue.Avalonia.Platform;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Platform;

namespace Xilium.CefGlue.Avalonia
{
    /// <summary>
    /// The Avalonia CEF browser.
    /// </summary>
    public class AvaloniaCefBrowser : BaseCefBrowser
    {
        static AvaloniaCefBrowser()
        {
            if (CefRuntime.Platform == CefRuntimePlatform.MacOS && !CefRuntimeLoader.IsLoaded)
            {
                CefRuntimeLoader.Load(new AvaloniaBrowserProcessHandler());
            }
        }

        public AvaloniaCefBrowser(Func<CefRequestContext> cefRequestContextFactory = null)
            : base(cefRequestContextFactory)
        { }

        internal override Common.Platform.IControl CreateControl()
        {
            return new AvaloniaControl(this, VisualChildren);
        }
        private AvaloniaOffScreenControlHost OffScreenControlHost;
        internal override IOffScreenControlHost CreateOffScreenControlHost()
        {
            OffScreenControlHost = new AvaloniaOffScreenControlHost(this, VisualChildren);
            return OffScreenControlHost;
        }

        internal override IOffScreenPopupHost CreatePopupHost()
        {
            var popup = new ExtendedAvaloniaPopup
            {
                PlacementTarget = this
            };
            return new AvaloniaPopup(popup, popup.VisualChildren);
        }

        public bool CheckPointTransparent(int x, int y)
        {
            if(OffScreenControlHost!= null)
            {
                return OffScreenControlHost.RenderSurface.CheckPointTransparent(x, y);
            }
            return false;
        }
    }
}
