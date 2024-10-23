using Avalonia;
using Avalonia.Controls.Presenters;
using Avalonia.Controls;
using Avalonia.Input.TextInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.VisualTree;
using Avalonia.Input;

namespace Xilium.CefGlue.Avalonia.Platform
{
    internal class OsrInputMethodClient : TextInputMethodClient
    {
        private ITextInputMethodImpl _im;
        private Visual _presenter;
        public override Visual TextViewVisual => _presenter!;

        public override bool SupportsPreedit => true;

        public override bool SupportsSurroundingText => true;

        public override string SurroundingText => "";

        public override Rect CursorRectangle => default;

        public override TextSelection Selection { get; set; }

        public void SetPresenter(Visual? presenter)
        {
            _presenter = presenter;
            if (presenter != null)
            {

                _im = (TopLevel.GetTopLevel(_presenter) as ITextInputMethodRoot)?.InputMethod;
                
            }
            else {  _im = null; }
            
        }
    }
}
