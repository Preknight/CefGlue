using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Shared;

namespace Xilium.CefGlue.Demo.Avalonia
{
    class Program
    {

        static int Main(string[] args)
        {
            AppBuilder.Configure<App>()
                      .UsePlatformDetect()
                      .With(new Win32PlatformOptions()
                      {
                          // CompositionMode = new [] { Win32CompositionMode.WinUIComposition }
                      })
                      .AfterSetup(_ => CefRuntimeLoader.Initialize(new CefSettings() {
#if WINDOWLESS
                          WindowlessRenderingEnabled = true
#else
                          WindowlessRenderingEnabled = false
#endif
                          ,
                          //BackgroundColor = new CefColor(0x00, 0xff, 0xff, 0xff),
                          Locale="zh-CN",

                      },
                      flags:new Dictionary<string, string> {
                          {"--ignore-urlfetcher-cert-requests", "1" },
                          {"--ignore-certificate-errors", "1" },
                          {"--disable-web-security", "1" }
                      }.ToArray(),
                      customSchemes: new[] {
                        new CustomScheme()
                        {
                            SchemeName = "test",
                            SchemeHandlerFactory = new CustomSchemeHandler()
                        }
                      }))
                      .StartWithClassicDesktopLifetime(args);
                      
            return 0;
        }
    }
}
