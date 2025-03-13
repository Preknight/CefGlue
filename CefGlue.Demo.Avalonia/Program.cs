
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Avalonia;
using Avalonia.Vulkan;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Shared;

namespace Xilium.CefGlue.Demo.Avalonia
{
    class Program
    {

        static int Main(string[] args)
        {
            var cachePath = Path.Combine(Path.GetTempPath(), "CefGlue_" + Guid.NewGuid().ToString().Replace("-", null));

            AppDomain.CurrentDomain.ProcessExit += delegate { Cleanup(cachePath); };


            AppBuilder.Configure<App>()
                      .UsePlatformDetect()
                      .With(new Win32PlatformOptions()
                      {
                          RenderingMode = new[]{Win32RenderingMode.Vulkan}
                      })
                       .With(new X11PlatformOptions() { RenderingMode = new[] { X11RenderingMode.Vulkan } })
                      .AfterSetup(_ => CefRuntimeLoader.Initialize(new CefSettings()
                      {
                          RootCachePath = cachePath,
#if WINDOWLESS

                          WindowlessRenderingEnabled = true
#else
                          WindowlessRenderingEnabled = false
#endif
                          ,
                          //BackgroundColor = new CefColor(0xff, 0xff, 0xff, 0),//A通道只支持255
                          NoSandbox = true,
                          Locale = "zh-CN",

                      },
                      flags: new Dictionary<string, string> {
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

        private static void Cleanup(string cachePath)
        {
            CefRuntime.Shutdown(); // must shutdown cef to free cache files (so that cleanup is able to delete files)

            try
            {
                var dirInfo = new DirectoryInfo(cachePath);
                if (dirInfo.Exists)
                {
                    dirInfo.Delete(true);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // ignore
            }
            catch (IOException)
            {
                try
                {
                    var dirInfo = new DirectoryInfo(cachePath);
                    if (dirInfo.Exists)
                    {
                        dirInfo.Delete(true);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // ignore
                }
                catch (IOException)
                {
                    // ignore
                }
            }
        }
    }
}
