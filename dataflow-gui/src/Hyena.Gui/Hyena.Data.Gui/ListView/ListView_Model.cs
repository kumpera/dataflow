//
// ListView_Model.cs
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
using System.Reflection;

using Gtk;

namespace Hyena.Data.Gui
{
    public partial class ListView<T> : Widget
    {
        public void SetModel (IListModel<T> model)
        {
            SetModel (model, 0.0);
        }

        public void SetModel (IListModel<T> value, double vpos)
        {
            if (model == value) {
                return;
            }

            if (model != null) {
                model.Cleared -= OnModelClearedHandler;
                model.Reloaded -= OnModelReloadedHandler;
            }
            
            model = value;

            ISortable sortable = model as ISortable;
            if (sortable != null && sortable.SortColumn == null && ColumnController.DefaultSortColumn != null) {
                sortable.Sort (ColumnController.DefaultSortColumn);
            }

            if (model != null) {
                model.Cleared += OnModelClearedHandler;
                model.Reloaded += OnModelReloadedHandler;
                selection_proxy.Selection = model.Selection;
            }
            
            RefreshViewForModel (vpos);
        }

        private void RefreshViewForModel (double? vpos)
        {
            UpdateAdjustments ();

            if (vpos != null) {
                ScrollTo ((double) vpos);
            } else {
                if (Model.Count <= RowsInView) {
                    ScrollTo (0.0);
                } else if (Selection.Count > 0) {
                    bool selection_in_view = false;
                    int first_row = GetRowAtY (0);
                    for (int i = 0; i < RowsInView; i++) {
                        if (Selection.Contains (first_row + i)) {
                            selection_in_view = true;
                            break;
                        }
                    }

                    if (!selection_in_view) {
                        CenterOn (Selection.Ranges[0].Start);
                    }
                } else {
                    ScrollTo (vadjustment.Value);
                }
            }
            
            if (Model != null) {
                Selection.MaxIndex = Model.Count - 1;
            }
            
            if (Parent is ScrolledWindow) {
                render_everything = true;
                Parent.QueueDraw ();
            }
        }

        private void OnModelClearedHandler (object o, EventArgs args)
        {
            OnModelCleared ();
        }
        
        private void OnModelReloadedHandler (object o, EventArgs args)
        {
            OnModelReloaded ();
        }

        private void OnColumnControllerUpdatedHandler (object o, EventArgs args)
        {
            OnColumnControllerUpdated ();
        }

        protected virtual void OnModelCleared ()
        {
            RefreshViewForModel (null);
        }
        
        protected virtual void OnModelReloaded ()
        {
            RefreshViewForModel (null);
        }
        
        private IListModel<T> model;
        public virtual IListModel<T> Model {
            get { return model; }
        }
        
        private string row_sensitive_property_name = "Sensitive";
        private PropertyInfo row_sensitive_property_info;
        bool row_sensitive_property_invalid = false;
        
        public string RowSensitivePropertyName {
            get { return row_sensitive_property_name; }
            set { 
                if (value == row_sensitive_property_name) {
                    return;
                }
                
                row_sensitive_property_name = value;
                row_sensitive_property_info = null;
                row_sensitive_property_invalid = false;
                
                InvalidateList ();
            }
        }
        
        private bool IsRowSensitive (object item)
        {
            if (item == null || row_sensitive_property_invalid) {
                return true;
            }
         
            if (row_sensitive_property_info == null || row_sensitive_property_info.ReflectedType != item.GetType ()) {
                row_sensitive_property_info = item.GetType ().GetProperty (row_sensitive_property_name);
                if (row_sensitive_property_info == null || row_sensitive_property_info.PropertyType != typeof (bool)) {
                    row_sensitive_property_info = null;
                    row_sensitive_property_invalid = true;
                    return true;
                }
            }
            
            return (bool)row_sensitive_property_info.GetValue (item, null);
        }
        
        #pragma warning disable 0169
        
        private bool IsRowSensitive (int index)
        {
            return IsRowSensitive (model[index]);
        }
        
        #pragma warning restore 0169
    }
}
