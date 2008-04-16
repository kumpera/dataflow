//
// ListView_Header.cs
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
//using Mono.Unix;
using Gtk;

namespace Hyena.Data.Gui
{
    public partial class ListView<T> : Widget
    {
        internal struct CachedColumn
        {
            public static readonly CachedColumn Zero;

            public Column Column;
            public int X1;
            public int X2;
            public int Width;
            public int MinWidth;
            public int MaxWidth;
            public int ResizeX1;
            public int ResizeX2;
            public int Index;
            public double ElasticWidth;
            public double ElasticPercent;
        }
        
        private static Gdk.Cursor resize_x_cursor = new Gdk.Cursor (Gdk.CursorType.SbHDoubleArrow);
        private static Gdk.Cursor drag_cursor = new Gdk.Cursor (Gdk.CursorType.Fleur);

        private bool resizable;
        private int header_width;
        private int sort_column_index = -1;
        private int resizing_column_index = -1;
        private int pressed_column_index = -1;
        private int pressed_column_x = -1;
        private int pressed_column_x_start = -1;
        private int pressed_column_x_offset = -1;
        private int pressed_column_x_drag = -1;
        private int pressed_column_x_start_hadjustment = -1;
        private bool pressed_column_is_dragging = false;
        
        private Pango.Layout column_layout;
        
        private CachedColumn [] column_cache;
        private List<int> elastic_columns;
        
#region Columns
        
        private void InvalidateColumnCache ()
        {
            column_cache = null;
        }
        
        private void GenerateColumnCache ()
        {
            column_cache = new CachedColumn[column_controller.Count];
            
            int i = 0;
            double total = 0.0;
            
            foreach (Column column in column_controller) {
                if (!column.Visible) {
                    continue;
                }
                
                if (column.MinWidth == 0) {
                    int w;
                    int h;
                    column_layout.SetText (column.Title);
                    column_layout.GetPixelSize (out w, out h);
                    column.MinWidth = w;
                }
                
                column_cache[i] = new CachedColumn ();
                column_cache[i].Column = column;
                column_cache[i].Index = i;
                
                total += column.Width;
                i++;
            }
            
            Array.Resize (ref column_cache, i);
            
            double scale_factor = 1.0 / total;
            
            for (i = 0; i < column_cache.Length; i++) {
                column_cache[i].Column.Width *= scale_factor;
            }
            
            RecalculateColumnSizes ();
        }
        
        private void RegenerateColumnCache ()
        {
            if (column_controller == null) {
                return;
            }
            
            if (column_cache == null) {
                GenerateColumnCache ();
            }
            
            for (int i = 0; i < column_cache.Length; i++) {
                column_cache[i].Width = (int)Math.Round (((double)header_width * column_cache[i].Column.Width));
                column_cache[i].X1 = i == 0 ? 0 : column_cache[i - 1].X2;
                column_cache[i].X2 = column_cache[i].X1 + column_cache[i].Width;
                column_cache[i].ResizeX1 = column_cache[i].X2;
                column_cache[i].ResizeX2 = column_cache[i].ResizeX1 + 2;
            }
            
            // TODO handle max width
            int index = column_cache.Length - 1;
            if (index >= 0) {
                column_cache[index].X2 = header_width;
                column_cache[index].Width = column_cache[index].X2 - column_cache[index].X1;
            }
        }
        
        private void RecalculateColumnSizes ()
        {
            if (column_cache == null) {
                return;
            }
            
            ISortable sortable = Model as ISortable;
            sort_column_index = -1;
            int min_header_width = 0;
            for (int i = 0; i < column_cache.Length; i++) {
                if (sortable != null) {
                    ColumnHeaderCellText column_cell = column_cache[i].Column.HeaderCell as ColumnHeaderCellText;
                    if (column_cell != null) {
                        ISortableColumn sort_column = column_cache[i].Column as ISortableColumn;
                        column_cell.HasSort = sort_column != null && sortable.SortColumn == sort_column;
                        if (column_cell.HasSort) {
                            sort_column_index = i;
                        }
                    }
                }
                
                int min_width = column_cache[i].Column.MinWidth;
                IHeaderCell header_cell = column_cache[i].Column.HeaderCell as IHeaderCell;
                if (header_cell != null) {
                    min_width = header_cell.MinWidth;
                }
                column_cache[i].MinWidth = min_width;
                column_cache[i].MaxWidth = column_cache[i].Column.MaxWidth;
                min_header_width += column_cache[i].MinWidth = Math.Max (min_width, column_cache[i].Column.MinWidth);
            }
            
            if (min_header_width >= header_interaction_alloc.Width) {
                header_width = min_header_width;
                resizable = false;
                for (int i = 0; i < column_cache.Length; i++) {
                    column_cache[i].Column.Width = (double)column_cache[i].MinWidth / (double)header_width;
                }
            } else {
                header_width = header_interaction_alloc.Width;
                resizable = true;
                
                if (elastic_columns == null) {
                    elastic_columns = new List<int> (column_cache.Length);
                }
                elastic_columns.Clear ();
                for (int i = 0; i < column_cache.Length; i++) {
                    elastic_columns.Add (i);
                    column_cache[i].ElasticWidth = 0.0;
                    column_cache[i].ElasticPercent = column_cache[i].Column.Width * header_width;
                }
                
                double remaining_width = RecalculateColumnSizes (header_width, header_width);
                
                while (remaining_width != 0 && elastic_columns.Count > 0) {
                    double total_elastic_width = 0.0;
                    foreach (int i in elastic_columns) {
                        total_elastic_width += column_cache[i].ElasticWidth;
                    }
                    remaining_width = RecalculateColumnSizes (remaining_width, total_elastic_width);
                }
                
                for (int i = 0; i < column_cache.Length; i++) {
                    column_cache[i].Column.Width = column_cache[i].ElasticWidth / (double)header_width;
                }
            }
        }
        
        private double RecalculateColumnSizes (double total_width, double total_elastic_width)
        {
            double remaining_width = total_width;
            
            for (int index = 0; index < elastic_columns.Count; index++) {
                int i = elastic_columns[index];
                double percent = column_cache[i].ElasticPercent / total_elastic_width;
                double delta = total_width * percent;
                
                // TODO handle max widths
                double width = column_cache[i].ElasticWidth + delta;
                if (width < column_cache[i].MinWidth) {
                    delta = column_cache[i].MinWidth - column_cache[i].ElasticWidth;
                    elastic_columns.RemoveAt (index);
                    index--;
                } else if (width > column_cache[i].MaxWidth) {
                    delta = column_cache[i].MaxWidth - column_cache[i].ElasticWidth;
                    elastic_columns.RemoveAt (index);
                    index--;
                }
                
                remaining_width -= delta;
                column_cache[i].ElasticWidth += delta;
            }
            
            remaining_width = Math.Round (remaining_width);
            return remaining_width;
        }
        
        protected virtual void OnColumnControllerUpdated ()
        {
            InvalidateColumnCache ();
            RegenerateColumnCache ();
            RegenerateCanvases ();
            UpdateAdjustments ();
            QueueDraw ();
        }
        
        protected virtual void OnColumnRightClicked (Column clickedColumn, int x, int y)
        {
            Menu menu = new Menu ();
            
            if (clickedColumn.Id != null) { // FIXME: Also restrict if the column vis can't be changed
                menu.Append (new ColumnHideMenuItem (clickedColumn));
                menu.Append (new SeparatorMenuItem ());
            }
            
            Column [] columns = ColumnController.ToArray ();
            Array.Sort (columns, delegate (Column a, Column b) {
                // Fully qualified type name to avoid Mono 1.2.4 bug
                return System.String.Compare (a.Title, b.Title);
            });
            
            foreach (Column column in columns) {
                if (column.Id == null) {
                    continue;
                }
                
                menu.Append (new ColumnToggleMenuItem (column));
            }
            
            menu.ShowAll ();
            menu.Popup (null, null, delegate (Menu popup, out int pos_x, out int pos_y, out bool push_in) {
                int win_x, win_y;
                GdkWindow.GetOrigin (out win_x, out win_y);
                
                pos_x = win_x + x;
                pos_y = win_y + y;
                push_in = true;
            }, 3, Gtk.Global.CurrentEventTime);
        }
        
        private void ResizeColumn (double x)
        {
            CachedColumn resizing_column = column_cache[resizing_column_index];
            double resize_delta = x - resizing_column.ResizeX2;
            
            if (resizing_column.Width + resize_delta < resizing_column.MinWidth) {
                resize_delta = resizing_column.MinWidth - resizing_column.Width;
            }
            
            if (resize_delta == 0) {
                return;
            }
            
            int sign = Math.Sign (resize_delta);
            resize_delta = Math.Abs (resize_delta);
            double total_elastic_width = 0.0;
            
            for (int i = resizing_column_index + 1; i < column_cache.Length; i++) {
                total_elastic_width += column_cache[i].ElasticWidth = sign == 1
                    ? column_cache[i].Width - column_cache[i].MinWidth
                    : column_cache[i].Column.MaxWidth - column_cache[i].Width;
            }
            
            if (total_elastic_width == 0) {
                return;
            }
            
            if (resize_delta > total_elastic_width) {
                resize_delta = total_elastic_width;
            }
            
            resize_delta = sign * resize_delta / (double)header_width;
            
            for (int i = resizing_column_index + 1; i < column_cache.Length; i++) {
                column_cache[i].Column.Width += -resize_delta * (column_cache[i].ElasticWidth / total_elastic_width);
            }
            
            resizing_column.Column.Width += resize_delta;
        
            RegenerateColumnCache ();
            QueueDraw ();
        }
        
        private Column GetColumnForResizeHandle (int x)
        {
            if (column_cache == null || !resizable) {
                return null;
            }
            
            x += (int)hadjustment.Value;
            
            for (int i = 0; i < column_cache.Length - 1; i++) {
                if (x >= column_cache[i].ResizeX1 - 2 && 
                    x <= column_cache[i].ResizeX2 + 2 &&
                    column_cache[i].Column.MaxWidth != column_cache[i].Column.MinWidth) {
                    return column_cache[i].Column;
                }
            }
            
            return null;
        }
        
        private Column GetColumnAt (int x)
        {
            if (column_cache == null) {
                return null;
            }
            
            x += (int)hadjustment.Value;
            
            foreach (CachedColumn column in column_cache) {
                if (x >= column.X1 && x <= column.X2) {
                    return column.Column;
                }
            }
            return null;
        }
        
        private CachedColumn GetCachedColumnForColumn (Column col)
        {
            foreach (CachedColumn ca_col in column_cache) {
                if (ca_col.Column == col) {
                    return ca_col;
                }
            }
            
            return CachedColumn.Zero;
        }
                
        private ColumnController column_controller;
        public ColumnController ColumnController {
            get { return column_controller; }
            set { 
                if (column_controller != null) {
                    column_controller.Updated -= OnColumnControllerUpdatedHandler;
                }
                
                column_controller = value;
                
                RegenerateColumnCache ();
                QueueDraw ();
                
                if (column_controller != null) {
                    column_controller.Updated += OnColumnControllerUpdatedHandler;
                }
            }
        }
        
#endregion

#region Header

        private int header_height = 0;
        private int HeaderHeight {
            get {
                if (!header_visible) {
                    return 0;
                }
                
                if (header_height == 0) {
                    int w;
                    int h;
                    column_layout.SetText ("W");
                    column_layout.GetPixelSize (out w, out h);
                    header_height = h;
                    header_height += 10;
                }
                
                return header_height;
            }
        }
        
        private bool header_visible = true;
        public bool HeaderVisible {
            get { return header_visible; }
            set { 
                header_visible = value;
                MoveResize (Allocation);
                RegenerateCanvases ();
            }
        }
        
#endregion

#region Gtk.MenuItem Wrappers for the column context menu

        private class ColumnToggleMenuItem : CheckMenuItem
        {
            private Column column;
            private bool ready = false;
            
            public ColumnToggleMenuItem (Column column) : base (column.Title)
            {
                this.column = column;
                Active = column.Visible; 
                ready = true;
            }
            
            protected override void OnActivated ()
            {
                base.OnActivated ();
                
                if (!ready) {
                    return;
                }
                
                column.Visible = Active;
            }
        }
        
        private class ColumnHideMenuItem : MenuItem
        {
            private Column column;
            
            public ColumnHideMenuItem (Column column) 
                : base (String.Format ("Hide {0}", column.Title))
            {
                this.column = column;
            }
            
            protected override void OnActivated ()
            {
                column.Visible = false;
            }
        }

#endregion

    }
}
