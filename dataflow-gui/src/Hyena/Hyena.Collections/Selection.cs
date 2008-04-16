//
// Selection.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//   Gabriel Burt <gburt@novell.com>
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

#if NET_2_0
using System.Collections.Generic;
#endif

namespace Hyena.Collections
{
#if NET_1_1
    internal
#else
    public 
#endif
    
    class Selection :
#if NET_2_0
        IEnumerable<int>
#else
        IEnumerable
#endif
    {
        RangeCollection ranges = new RangeCollection ();
        private int max_index;
        private int first_selected_index;
        
        public event EventHandler Changed;
        
        public Selection ()
        {
        }

        protected virtual void OnChanged ()
        {
            EventHandler handler = Changed;
            if (handler != null) {
                handler (this, EventArgs.Empty);
            }
        }
        
        public void ToggleSelect (int index)
        {
            if (!ranges.Remove (index)) {
                ranges.Add (index);
            }
            
            OnChanged ();
        }
        
        public void Select (int index)
        {
            ranges.Add (index);
            if (Count == 1)
                first_selected_index = index;
            OnChanged ();
        }

        public void QuietSelect (int index)
        {
            ranges.Add (index);
            if (Count == 1)
                first_selected_index = index;
        }
        
        public void Unselect (int index)
        {
            if (ranges.Remove (index))
                OnChanged ();
        }

        public void QuietUnselect (int index)
        {
            ranges.Remove (index);
        }
                    
        public bool Contains(int index)
        {
            return ranges.Contains (index);
        }

        public void SelectFromFirst (int end, bool clear)
        {
            bool contains = Contains (first_selected_index);

            if (clear)
                Clear(false);

            if (contains)
                SelectRange (first_selected_index, end);
            else
                Select (end);
        }
        
        public void SelectRange (int a, int b)
        {
            int start = Math.Min (a, b);
            int end = Math.Max (a, b);

            int i;
            for (i = start; i <= end; i++) {
                ranges.Add (i);
            }

            if (Count == i)
                first_selected_index = a;
            
            OnChanged ();
        }

        public virtual void SelectAll ()
        {
            SelectRange (0, max_index);
        }

        public void Clear () 
        {
            Clear (true);
        }
        
        public void Clear (bool raise)
        {
            if (ranges.Count <= 0) {
                return;
            }
            
            ranges.Clear ();
            if (raise)
                OnChanged ();
        }
        
        public int Count {
            get { return ranges.Count; }
        }
        
        public int MaxIndex {
            set { max_index = value; }
            get { return max_index; }
        }
        
        public virtual bool AllSelected {
            get { 
                if (ranges.RangeCount == 1) {
                    RangeCollection.Range range = ranges.Ranges[0];
                    return range.Start == 0 && range.End == max_index;
                }
                
                return false;
            }
        }

        public RangeCollection.Range [] Ranges {
            get { return ranges.Ranges; }
        }
        
#if NET_2_0
        public IEnumerator<int> GetEnumerator ()
        {
            return ranges.GetEnumerator ();
        }
        
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator ();
        }
#else
        public IEnumerator GetEnumerator ()
        {
            return ranges.GetEnumerator ();
        }
#endif

        public override string ToString ()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder ();
            sb.AppendFormat ("<Selection Count={0}", Count);
            foreach (RangeCollection.Range range in Ranges) {
                sb.AppendFormat (" ({0}, {1})", range.Start, range.End);
            }
            sb.Append (">");
            return sb.ToString ();
        }
    }
}
