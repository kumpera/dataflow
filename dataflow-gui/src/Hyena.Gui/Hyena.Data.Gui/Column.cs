//
// Column.cs
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
using System.Collections;
using System.Collections.Generic;
using Gtk;

using Hyena;
using Hyena.Data;

namespace Hyena.Data.Gui
{
    public class Column : ColumnDescription, IEnumerable<ColumnCell>
    {
        private ColumnCell header_cell;
        private List<ColumnCell> cells = new List<ColumnCell> ();
        
        private int minWidth = 0;
        private int maxWidth = Int32.MaxValue;
        private double minRelativeWidth = 0;
        private double relativeWidth = 0;
        
        public Column (ColumnDescription description) :
            this (description, new ColumnCellText (description.Property, true))
        {
        }
        
        public Column (ColumnDescription description, ColumnCell cell) :
            this (description.Title, cell, description.Width, description.Visible)
        {
        }
        
        public Column (string title, ColumnCell cell, double width) 
            : this (null, title, cell, width, true)
        {
        }
        
        public Column (string title, ColumnCell cell, double width, bool visible) 
            : this (null, title, cell, width, visible)
        {
            this.header_cell = new ColumnHeaderCellText(HeaderCellDataHandler);
        }
        
        public Column (ColumnCell header_cell, string title, ColumnCell cell, double width)
            : this (header_cell, title, cell, width, true)
        {
        }
        
        public Column (ColumnCell header_cell, string title, ColumnCell cell, double width, bool visible)
            : this (header_cell, title, cell, width, visible, 0, Int32.MaxValue)
        {
        }
        
        public Column (ColumnCell header_cell, string title, ColumnCell cell, double width, bool visible, int minWidth, int maxWidth)
            : base (cell.Property, title, width, visible)
        {
            this.minWidth = minWidth;
            this.maxWidth = maxWidth;
            this.header_cell = header_cell;
            PackStart(cell);
        }
        
        private Column HeaderCellDataHandler ()
        {
            return this;
        }
        
        public void PackStart (ColumnCell cell)
        {
            cells.Insert (0, cell);
        }
        
        public void PackEnd (ColumnCell cell)
        {
            cells.Add (cell);
        }
        
        public ColumnCell GetCell (int index) 
        {
            return cells[index];
        }
        
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return cells.GetEnumerator ();
        }
        
        IEnumerator<ColumnCell> IEnumerable<ColumnCell>.GetEnumerator ()
        {
            return cells.GetEnumerator ();
        }
        
        public ColumnCell HeaderCell {
            get { return header_cell; }
            set { header_cell = value; }
        }
        
        public int MinWidth {
            get { return minWidth; }
            set { minWidth = value; }
        }

        internal double MinRelativeWidth {
            get { return minRelativeWidth; }
            set { minRelativeWidth = value; }
        }
        
        public int MaxWidth {
            get { return maxWidth; }
            set { maxWidth = value; }
        }

        public double RelativeWidth {
            get { return relativeWidth; }
            set { relativeWidth = value; }
        }

        public string Id {
            get { return StringUtil.CamelCaseToUnderCase (GetCell (0).Property); }
        }
    }
}
