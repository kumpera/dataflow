//
// CellContext.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2007 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

using Hyena.Gui.Theming;

namespace Hyena.Data.Gui
{
    public class CellContext
    {
        private Cairo.Context context;
        private Pango.Layout layout;
        private Gtk.Widget widget;
        private Gdk.Drawable drawable;
        private Theme theme;
        private Gdk.Rectangle area;
        private Gdk.Rectangle clip;
        private bool text_as_foreground = false;
        
        public Cairo.Context Context {
            get { return context; }
            set { context = value; }
        }

        public Pango.Layout Layout {
            get { return layout; }
            set { layout = value; }
        }

        public Gtk.Widget Widget {
            get { return widget; }
            set { widget = value; }
        }

        public Gdk.Drawable Drawable {
            get { return drawable; }
            set { drawable = value; }
        }

        public Theme Theme {
            get { return theme; }
            set { theme = value; }
        }

        public Gdk.Rectangle Area {
            get { return area; }
            set { area = value; }
        }
        
        public Gdk.Rectangle Clip {
            get { return clip; }
            set { clip = value; }
        }
        
        public bool TextAsForeground {
            get { return text_as_foreground; }
            set { text_as_foreground = value; }
        }
    }
}
