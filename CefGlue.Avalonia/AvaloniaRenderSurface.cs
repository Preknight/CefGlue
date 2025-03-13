using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Xilium.CefGlue.Common.Helpers;

namespace Xilium.CefGlue.Avalonia
{
    /// <summary>
    /// The Avalonia builtin surface.
    /// </summary>
    internal class AvaloniaRenderSurface : OffScreenRenderSurface
    {
        private WriteableBitmap _bitmap;
        private IntPtr _destinationBuffer;

        public AvaloniaRenderSurface(Image image)
        {
            Image = image;
        }

        public override void Dispose()
        {
            base.Dispose();
            _destinationBuffer = IntPtr.Zero;
            _bitmap?.Dispose();
            _bitmap = null;
        }

        private Image Image { get; }

        public override bool AllowsTransparency => true;

        protected override int BytesPerPixel => 4;

        protected override int RenderedHeight => _bitmap?.PixelSize.Height ?? 0;

        protected override int RenderedWidth => _bitmap?.PixelSize.Width ?? 0;

        protected override Task ExecuteInUIThread(Action action)
        {
            return Dispatcher.UIThread.InvokeAsync(action).GetTask();
        }

        protected override void CreateBitmap(int width, int height)
        {
            // TODO handle transparency
            _bitmap?.Dispose();
            _bitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(DefaultDpi, DefaultDpi), PixelFormat.Bgra8888, AlphaFormat.Opaque);
            
            Image.Source = _bitmap;
        }

        protected override Action BeginBitmapUpdate()
        {
            var lockedBuffer = _bitmap.Lock();
            _destinationBuffer = lockedBuffer.Address;
            return () =>
            {
                _destinationBuffer = IntPtr.Zero;
                lockedBuffer.Dispose();
                Image.InvalidateVisual();
            };
        }
        
        protected override void UpdateBitmap(IntPtr sourceBuffer, int sourceBufferSize, int stride, CefRectangle updateRegion)
        {
            unsafe
            {
                //Buffer.MemoryCopy(sourceBuffer.ToPointer(), _destinationBuffer.ToPointer(), sourceBufferSize, sourceBufferSize);
                Unsafe.CopyBlock(_destinationBuffer.ToPointer(), sourceBuffer.ToPointer(),
                    (uint)sourceBufferSize);
            }
        }
        public override bool CheckPointTransparent(int x, int y)
        {
            return CheckPointTransparent(new Point(x,y));
        }
        public bool CheckPointTransparent(Point point)
        {
            bool isTransparent = false;
            unsafe
            {
                //只取一个像素，32位，4个字节
                //int size = 4;//(PixelFormat.Bgra8888.BitsPerPixel + 7) / 8;
                //var data = Marshal.AllocHGlobal(size);
                int stride = ((_bitmap.PixelSize.Width * PixelFormat.Bgra8888.BitsPerPixel) + 7) / 8;
                //var pixelData = new byte[4];
                //IntPtr checkBuffer = new IntPtr();
                using(var fb = _bitmap.Lock())
                {
                    var addr = fb.Address + (int)point.X * 4 + (int)point.Y * stride;
                    //Buffer.MemoryCopy(addr.ToPointer(), data.ToPointer(), size, size);
                    byte B= Marshal.ReadByte(addr);
                    byte G = Marshal.ReadByte(addr + 1);
                    byte R = Marshal.ReadByte(addr + 2);
                    byte A = Marshal.ReadByte(addr + 3);
                    if(A == 0x00)
                    {
                        return true;
                    }
                }
                
            }
            return isTransparent;
        }
    }
}
