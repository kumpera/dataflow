//
// ListView_Rendering.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2007-2008 Novell, Inc.
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
using System.Collections.Generic;

using Gtk;
using Gdk;

using Hyena.Gui;
using Hyena.Gui.Theming;
using GtkColorClass=Hyena.Gui.Theming.GtkColorClass;

namespace Hyena.Data.Gui
{
    public delegate int ListViewRowHeightHandler (Widget widget);

    public partial class ListView<T> : Widget
    {
        // The list is rendered to an off-screen Drawable; the "canvas". The canvas is large enough to hold
        // the maximum number of rows that can be seen in the list. When new rows comes into view b/c of
        // scrolling, the contents of the canvas are shifted up or down (depending on whether the user is
        // scrolling up or down) to accommodate the new rows. The new rows are then painted in situ on the
        // canvas. The canvas is then painted onto the GdkWindow, offset properly. This means that a row
        // is only rendered once when it comes into view and that rendering persists on the canvas for as
        // long as that row remains in view.
        
        private Pixmap canvas1;
        private Pixmap canvas2;
        private Rectangle canvas_alloc;
        private int canvas_first_row;
        private int canvas_last_row;
        private bool render_everything;
        
        private Cairo.Context cairo_context;
        private CellContext cell_context;
        private Pango.Layout pango_layout;
        
        private Theme theme;
        protected Theme Theme {
            get { return theme; }
        }
        
        protected override void OnStyleSet (Style old_style)
        {
            base.OnStyleSet (old_style);
            RecomputeRowHeight = true;
            theme = new GtkTheme (this);
            cell_context = new CellContext ();
            cell_context.Theme = theme;
            cell_context.Widget = this;
        }
         
        protected override bool OnExposeEvent (EventExpose evnt)
        {
            Rectangle damage = new Rectangle ();
            foreach (Rectangle rect in evnt.Region.GetRectangles ()) {
                damage = damage.Union (rect);
            }
            
            // First we create a cairo context for rendering to the GdkWindow.
            cairo_context = CairoHelper.Create (evnt.Window);
            
            // We render the background, the header (if we have one), and the
            // border to the GdkWindow.
            Theme.DrawFrameBackground (cairo_context, Allocation, true);
            if (header_visible && column_controller != null) {
                PaintHeader (damage);
            }
            
            Theme.DrawFrameBorder (cairo_context, Allocation);
            
            // Then we render the list to the offscreen canvas.
            if (canvas1 == null) {
                canvas1 = new Pixmap (GdkWindow, canvas_alloc.Width, canvas_alloc.Height);
                render_everything = true;
            }
            
            PaintList ();
            
            // Now we blit the offscreen canvas onto the GdkWindow.
            GdkWindow.DrawDrawable (Style.BaseGC (StateType.Normal), canvas1,
                (int)hadjustment.Value, (int)vadjustment.Value % RowHeight,
                list_rendering_alloc.X, list_rendering_alloc.Y,
                list_rendering_alloc.Width, list_rendering_alloc.Height);
            
            // Finally, render the dragging box and dispose of the cairo context.
            PaintDraggingColumn (damage);
            ((IDisposable)cairo_context.Target).Dispose ();
            ((IDisposable)cairo_context).Dispose ();
            
            render_everything = false;
            
            return true;
        }
        
        private void PaintHeader (Rectangle clip)
        {
            Rectangle rect = header_rendering_alloc;
            rect.Height += Theme.BorderWidth;
            clip.Intersect (rect);
            cairo_context.Rectangle (clip.X, clip.Y, clip.Width, clip.Height);
            cairo_context.Clip ();
            
            CairoExtensions.CreateLayout (this, cairo_context, ref pango_layout);
            Theme.DrawHeaderBackground (cairo_context, header_rendering_alloc);
            
            Rectangle cell_area = new Rectangle ();
            cell_area.Y = header_rendering_alloc.Y;
            cell_area.Height = header_rendering_alloc.Height;
            
            cell_context.Context = cairo_context;
            cell_context.Drawable = GdkWindow;
            cell_context.Layout = pango_layout;
            cell_context.TextAsForeground = true;

            for (int ci = 0; ci < column_cache.Length; ci++) {
                if (pressed_column_is_dragging && pressed_column_index == ci) {
                    continue;
                }
				/*Console.WriteLine ("cell_area {0}", cell_area);
				Console.WriteLine ("column_cache {0}", column_cache);
				Console.WriteLine ("column_cache[ci] {0}", column_cache[ci]);
				Console.WriteLine ("Theme {0}", Theme);
				Console.WriteLine ("header_rendering_alloc {0}", header_rendering_alloc);
				Console.WriteLine ("hadjustment {0}", hadjustment);*/

                
                cell_area.X = column_cache[ci].X1 + Theme.TotalBorderWidth + header_rendering_alloc.X - (int)hadjustment.Value;
                cell_area.Width = column_cache[ci].Width;
                PaintHeaderCell (cell_area, clip, ci, false);
            }
            
            if (pressed_column_is_dragging && pressed_column_index >= 0) {
                cell_area.X = pressed_column_x_drag + Allocation.X - (int)hadjustment.Value;
                cell_area.Width = column_cache[pressed_column_index].Width;
                PaintHeaderCell (cell_area, clip, pressed_column_index, true);
            }
            
            cairo_context.ResetClip ();
        }
        
        private void PaintHeaderCell (Rectangle area, Rectangle clip, int ci, bool dragging)
        {
            if (ci < 0 || column_cache.Length <= ci)
                return;

            if (dragging) {
                Theme.DrawColumnHighlight (cairo_context, area, 
                    CairoExtensions.ColorShade (Theme.Colors.GetWidgetColor (GtkColorClass.Dark, StateType.Normal), 0.9));
                    
                Cairo.Color stroke_color = CairoExtensions.ColorShade (Theme.Colors.GetWidgetColor (
                    GtkColorClass.Base, StateType.Normal), 0.0);
                stroke_color.A = 0.3;
                
                cairo_context.Color = stroke_color;
                cairo_context.MoveTo (area.X + 0.5, area.Y + 1.0);
                cairo_context.LineTo (area.X + 0.5, area.Bottom);
                cairo_context.MoveTo (area.Right - 0.5, area.Y + 1.0);
                cairo_context.LineTo (area.Right - 0.5, area.Bottom);
                cairo_context.Stroke ();
            }

            ColumnCell cell = column_cache[ci].Column.HeaderCell;
            
            if (cell != null) {
                cairo_context.Save ();
                cairo_context.Translate (area.X, area.Y);
                cell_context.Area = area;
                cell_context.Clip = clip;
                cell.Render (cell_context, StateType.Normal, area.Width, area.Height);
                cairo_context.Restore ();
            }
            
            if (!dragging && ci < column_cache.Length - 1) {
                Theme.DrawHeaderSeparator (cairo_context, area, area.Right);
            }
        }

        private void PaintList ()
        {
            if (model == null) {
                return;
            }
            
            // Render the sort effect to the GdkWindow.
            if (sort_column_index != -1 && (!pressed_column_is_dragging || pressed_column_index != sort_column_index)) {
                CachedColumn col = column_cache[sort_column_index];
                Theme.DrawRowRule (cairo_context,
                    list_rendering_alloc.X + col.X1 - (int)hadjustment.Value,
                    header_rendering_alloc.Bottom + Theme.BorderWidth,
                    col.Width, list_rendering_alloc.Height + Theme.InnerBorderWidth * 2);
            }

            int rows_in_view = RowsInView;
            int top = (int)vadjustment.Value / RowHeight;
            int bottom = top + rows_in_view;
            
            if (!render_everything && canvas_first_row == top && canvas_last_row == bottom) {
                // We haven't been told to render everything, and all of the same rows are
                // still in view, so we don't render anything.
                return;
            }
            
            // Swap the canvases if nessisary
            if (canvas2 == null) {
                canvas2 = new Pixmap (GdkWindow, canvas_alloc.Width, canvas_alloc.Height);
            } else {
                Pixmap tmp = canvas1;
                canvas1 = canvas2;
                canvas2 = tmp;
            }
            
            // Build a cairo context for the primary canvas.
            Cairo.Context tmp_cr = cairo_context;
            cairo_context = CairoHelper.Create (canvas1);
            CairoExtensions.CreateLayout (this, cairo_context, ref pango_layout);
            
            // Render the background to the primary canvas.
            Theme.DrawListBackground (cairo_context, canvas_alloc, true);
            
            // Render the sort effect to the canvas.
            if (sort_column_index != -1 && (!pressed_column_is_dragging || pressed_column_index != sort_column_index)) {
                CachedColumn col = column_cache[sort_column_index];
                Theme.PushContext ();
                Theme.Context.FillAlpha = 0.5;
                Theme.DrawRowRule (cairo_context, col.X1, 0, col.Width, canvas_alloc.Height);
                Theme.PopContext ();
            }
            
            int first_row = top;
            int last_row = bottom;
            int first_row_y = 0;
            
            if (render_everything) {
                // We need to render everything
            } else if (canvas_last_row < last_row && canvas_last_row > first_row) {
                // We're scrolling down, so shift the contents of the list up and
                // render the new stuff in situ at the bottom.
                int delta = (last_row - canvas_last_row) * RowHeight;
                canvas1.DrawDrawable (Style.BaseGC (StateType.Normal), canvas2, 0, delta, 0, 0,
                    canvas_alloc.Width, canvas_alloc.Height - delta);
                
                // If the bottom of the stuff we're shifting up is part of a selection
                // that continues down into the new stuff, be sure that we render the
                // whole selection block so the gradient looks nice.
                while (Selection.Contains (canvas_last_row) && canvas_last_row > first_row) {
                    canvas_last_row--;
                }
                
                first_row_y = (canvas_last_row - first_row) * RowHeight;
                first_row = canvas_last_row;
            } else if (canvas_first_row > first_row && canvas_first_row < last_row) {
                // We're scrolling up, so shift the contents of the list down and
                // render the new stuff in situ at the top.
                int delta = (canvas_first_row - first_row) * RowHeight;
                canvas1.DrawDrawable (Style.BaseGC (StateType.Normal), canvas2, 0, 0, 0, delta,
                    canvas_alloc.Width, canvas_alloc.Height - delta);
                
                // If the top of the stuff we're shifting down is part of a selection
                // that continues up into the new stuff, be sure that we render the
                // whole selection block so the gradient looks nice.
                while (Selection.Contains (canvas_first_row) && canvas_first_row < last_row) {
                    canvas_first_row++;
                }
                
                last_row = canvas_first_row;
            }
            
            PaintRows (first_row, Math.Min (model.Count, last_row), first_row_y);
            
            // Destroy the cairo context.
            ((IDisposable)cairo_context.Target).Dispose ();
            ((IDisposable)cairo_context).Dispose ();
            cairo_context = tmp_cr;
            
            canvas_first_row = top;
            canvas_last_row = bottom;
        }
        
        private void PaintRows (int first_row, int last_row, int first_row_y)
        {
            cell_context.Context = cairo_context;
            cell_context.Drawable = canvas1;
            cell_context.Layout = pango_layout;
            cell_context.Clip = canvas_alloc;
            cell_context.TextAsForeground = false;
            
            Rectangle selected_focus_alloc = Rectangle.Zero;
            Rectangle single_list_alloc = new Rectangle ();
            
            single_list_alloc.Y = first_row_y;
            single_list_alloc.Width = canvas_alloc.Width;
            single_list_alloc.Height = RowHeight;
            
            int selection_height = 0;
            int selection_y = 0;
            List<int> selected_rows = new List<int> ();

            for (int ri = first_row; ri < last_row; ri++) {
                if (Selection.Contains (ri)) {
                    if (selection_height == 0) {
                        selection_y = single_list_alloc.Y;
                    }
                    
                    selection_height += single_list_alloc.Height;
                    selected_rows.Add (ri);
                    
                    if (focused_row_index == ri) {
                        selected_focus_alloc = single_list_alloc;
                    }
                } else {
                    if (rules_hint && ri % 2 != 0) {
                        Theme.DrawRowRule (cairo_context, single_list_alloc.X, single_list_alloc.Y, 
                            single_list_alloc.Width, single_list_alloc.Height);
                    }
                    
                    if (ri == drag_reorder_row_index && Reorderable) {
                        cairo_context.Save ();
                        cairo_context.LineWidth = 1.0;
                        cairo_context.Antialias = Cairo.Antialias.None;
                        cairo_context.MoveTo (single_list_alloc.Left, single_list_alloc.Top);
                        cairo_context.LineTo (single_list_alloc.Right, single_list_alloc.Top);
                        cairo_context.Color = Theme.Colors.GetWidgetColor (GtkColorClass.Text, StateType.Normal);
                        cairo_context.Stroke ();
                        cairo_context.Restore ();
                    }
                    
                    if (focused_row_index == ri && !Selection.Contains (ri) && HasFocus) {
                        CairoCorners corners = CairoCorners.All;
                        
                        if (Selection.Contains (ri - 1)) {
                            corners &= ~(CairoCorners.TopLeft | CairoCorners.TopRight);
                        }
                        
                        if (Selection.Contains (ri + 1)) {
                            corners &= ~(CairoCorners.BottomLeft | CairoCorners.BottomRight);
                        }
                        
                        Theme.DrawRowSelection (cairo_context, single_list_alloc.X, single_list_alloc.Y, 
                            single_list_alloc.Width, single_list_alloc.Height, false, true, 
                            Theme.Colors.GetWidgetColor (GtkColorClass.Background, StateType.Selected), corners);
                    }
                    
                    if (selection_height > 0) {
                        Theme.DrawRowSelection (cairo_context, 0, selection_y, canvas_alloc.Width, selection_height);
                        selection_height = 0;
                    }
                    
                    PaintRow (ri, single_list_alloc, StateType.Normal);
                }
                
                single_list_alloc.Y += single_list_alloc.Height;
            }
            
            if (selection_height > 0) {
                Theme.DrawRowSelection (cairo_context, 0, selection_y, 
                    canvas_alloc.Width, selection_height);
            }
            
            if (Selection.Count > 1 && !selected_focus_alloc.Equals (Rectangle.Zero) && HasFocus) {
                Theme.DrawRowSelection (cairo_context, selected_focus_alloc.X, selected_focus_alloc.Y, 
                    selected_focus_alloc.Width, selected_focus_alloc.Height, false, true, 
                    Theme.Colors.GetWidgetColor (GtkColorClass.Dark, StateType.Selected));
            }
            
            foreach (int ri in selected_rows) {
                single_list_alloc.Y = (ri - first_row) * single_list_alloc.Height + first_row_y;
                PaintRow (ri, single_list_alloc, StateType.Selected);
            }
        }

        private void PaintRow (int row_index, Rectangle area, StateType state)
        {
            if (column_cache == null) {
                return;
            }
            
            object item = model[row_index];
            bool sensitive = IsRowSensitive (item);
            
            Rectangle cell_area = new Rectangle ();
            cell_area.Height = RowHeight;
            cell_area.Y = area.Y;
            
            for (int ci = 0; ci < column_cache.Length; ci++) {
                if (pressed_column_is_dragging && pressed_column_index == ci) {
                    continue;
                }
                
                cell_area.Width = column_cache[ci].Width;
                cell_area.X = column_cache[ci].X1 + area.X;
                PaintCell (item, ci, row_index, cell_area, sensitive ? state : StateType.Insensitive, false);
            }
            
            if (pressed_column_is_dragging && pressed_column_index >= 0) {
                cell_area.Width = column_cache[pressed_column_index].Width;
                cell_area.X = pressed_column_x_drag - list_interaction_alloc.X;
                PaintCell (item, pressed_column_index, row_index, cell_area, state, true);
            }
        }
        
        private void PaintCell (object item, int column_index, int row_index, Rectangle area, 
            StateType state, bool dragging)
        {
            ColumnCell cell = column_cache[column_index].Column.GetCell (0);
            cell.BindListItem (item);
            
            if (dragging) {
                Cairo.Color fill_color = Theme.Colors.GetWidgetColor (GtkColorClass.Base, StateType.Normal);
                fill_color.A = 0.5;
                cairo_context.Color = fill_color;
                cairo_context.Rectangle (area.X, area.Y, area.Width, area.Height);
                cairo_context.Fill ();
            }
            
            cairo_context.Save ();
            cairo_context.Translate (area.X, area.Y);
            cell_context.Area = area;
            cell.Render (cell_context, dragging ? StateType.Normal : state, area.Width, area.Height);
            cairo_context.Restore ();
        }
        
        private void PaintDraggingColumn (Rectangle clip)
        {
            if (!pressed_column_is_dragging || pressed_column_index < 0) {
                return;
            }
            
            CachedColumn column = column_cache[pressed_column_index];
            
            int x = pressed_column_x_drag + Allocation.X + 1 - (int)hadjustment.Value;
            
            Cairo.Color fill_color = Theme.Colors.GetWidgetColor (GtkColorClass.Base, StateType.Normal);
            fill_color.A = 0.45;
            
            Cairo.Color stroke_color = CairoExtensions.ColorShade (Theme.Colors.GetWidgetColor (
                GtkColorClass.Base, StateType.Normal), 0.0);
            stroke_color.A = 0.3;
            
            cairo_context.Rectangle (x, header_rendering_alloc.Bottom + 1, column.Width - 2,
                list_rendering_alloc.Bottom - header_rendering_alloc.Bottom - 1);
            cairo_context.Color = fill_color;
            cairo_context.Fill ();
            
            cairo_context.MoveTo (x - 0.5, header_rendering_alloc.Bottom + 0.5);
            cairo_context.LineTo (x - 0.5, list_rendering_alloc.Bottom + 0.5);
            cairo_context.LineTo (x + column.Width - 1.5, list_rendering_alloc.Bottom + 0.5);
            cairo_context.LineTo (x + column.Width - 1.5, header_rendering_alloc.Bottom + 0.5);
            
            cairo_context.Color = stroke_color;
            cairo_context.LineWidth = 1.0;
            cairo_context.Stroke ();
        }
        
        public new void QueueDraw ()
        {
            InvalidateHeader ();
            InvalidateList ();
        }
        
        private void InvalidateList ()
        {
            InvalidateList (true);
        }
        
        private void InvalidateList (bool render_everything)
        {
            if (IsRealized) {
                this.render_everything |= render_everything;
                GdkWindow.InvalidateRect (list_rendering_alloc, true);
                base.QueueDraw ();
            }
        }
        
        private void InvalidateHeader ()
        {
            if (IsRealized) {
                GdkWindow.InvalidateRect (header_rendering_alloc, true);
                base.QueueDraw ();
            }
        }
        
        private bool rules_hint = false;
        public bool RulesHint {
            get { return rules_hint; }
            set { 
                rules_hint = value; 
                InvalidateList ();
            }
        }
        
        private ListViewRowHeightHandler row_height_handler;
        public virtual ListViewRowHeightHandler RowHeightProvider {
            get { return row_height_handler; }
            set {
                if (value != row_height_handler) {
                    row_height_handler = value;
                    RecomputeRowHeight = true;
                }
            }
        }
        
        private bool recompute_row_height = true;
        protected bool RecomputeRowHeight {
            get { return recompute_row_height; }
            set { 
                recompute_row_height = value;
                if (value && IsMapped && IsRealized) {
                    QueueDraw ();
                }
            }
        }
        
        private int row_height = 32;
        private int RowHeight {
            get {
                if (RecomputeRowHeight) {
                    row_height = RowHeightProvider != null 
                        ? RowHeightProvider (this) 
                        : ColumnCellText.ComputeRowHeight (this);
                    
                    header_height = 0;
                    MoveResize (Allocation);
                    
                    RecomputeRowHeight = false;
                }
                
                return row_height;
            }
        }
    }
}
