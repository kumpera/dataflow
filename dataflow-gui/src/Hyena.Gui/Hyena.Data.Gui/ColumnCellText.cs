//
// ColumnCellText.cs
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
using Gtk;
using Cairo;

using Hyena.Gui;
using Hyena.Gui.Theming;

namespace Hyena.Data.Gui
{
    public class ColumnCellText : ColumnCell
    {
        private static bool use_cairo_pango;
        
        static ColumnCellText ()
        {
            use_cairo_pango = String.IsNullOrEmpty (Environment.GetEnvironmentVariable ("USE_GTK_PANGO"));
            Log.DebugFormat ("Text renderer: {0}", use_cairo_pango ? "Cairo" : "GTK");
        }
        
        public delegate string DataHandler ();
    
        private Pango.Weight font_weight = Pango.Weight.Normal;
        private Pango.EllipsizeMode ellipsize_mode = Pango.EllipsizeMode.End;
        private int text_width;
        private int text_height;
        
        public ColumnCellText (string property, bool expand) : base (property, expand)
        {
        }
    
        public override void Render (CellContext context, StateType state, double cellWidth, double cellHeight)
        {
            context.Layout.Width = (int)((cellWidth - 8) * Pango.Scale.PangoScale);
            context.Layout.FontDescription.Weight = font_weight;
            context.Layout.Ellipsize = EllipsizeMode;
            
            context.Layout.SetText (Text);
            context.Layout.GetPixelSize (out text_width, out text_height);
            
            if (use_cairo_pango) {
                context.Context.MoveTo (4, ((int)cellHeight - text_height) / 2);
                context.Context.Color = context.Theme.Colors.GetWidgetColor (
                    context.TextAsForeground ? GtkColorClass.Foreground : GtkColorClass.Text, state);
                PangoCairoHelper.ShowLayout (context.Context, context.Layout);
            } else {
                Style.PaintLayout (context.Widget.Style, context.Drawable, state, !context.TextAsForeground, 
                    context.Clip, context.Widget, "text", context.Area.X + 4, 
                    context.Area.Y + (((int)cellHeight - text_height) / 2), context.Layout);
            }
        }
        
        protected virtual string Text {
            get { return BoundObject == null ? String.Empty : BoundObject.ToString (); }
        }
        
        protected int TextWidth {
            get { return text_width; }
        }
        
        protected int TextHeight {
            get { return text_height; }
        }
        
        public virtual Pango.Weight FontWeight {
            get { return font_weight; }
            set { font_weight = value; }
        }
        
        public virtual Pango.EllipsizeMode EllipsizeMode {
            get { return ellipsize_mode; }
            set { ellipsize_mode = value; }
        }
        
        internal static int ComputeRowHeight (Widget widget)
        {
            int w_width, row_height;
            Pango.Layout layout = new Pango.Layout (widget.PangoContext);
            layout.SetText ("W");
            layout.GetPixelSize (out w_width, out row_height);
            layout.Dispose ();
            return row_height + 8;
        }
    }
}
