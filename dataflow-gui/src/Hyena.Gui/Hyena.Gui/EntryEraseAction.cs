//
// EntryEraseAction.cs
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

using Hyena;

namespace Hyena.Gui
{
    internal class EntryEraseAction : IUndoAction
    {
        private Entry entry;
        private string text;
        private int start;
        private int end;
        private bool is_forward;
        private bool is_cut;

        public EntryEraseAction(Entry entry, int start, int end)
        {
            this.entry = entry;
            this.text = entry.GetChars(start, end);
            this.start = start;
            this.end = end;
            this.is_cut = end - start > 1;
            this.is_forward = entry.Position < start;
        }

        public void Undo()
        {
            int start_r = start;
            entry.InsertText(text, ref start_r);
            entry.Position = is_forward ? start_r : end;
        }

        public void Redo()
        {
            entry.DeleteText(start, end);
            entry.Position = start;
        }

        public void Merge(IUndoAction action)
        {
            EntryEraseAction erase = (EntryEraseAction)action;
            if(start == erase.start) {
                text += erase.text;
                end += erase.end - erase.start;
            } else {
                text = erase.text + text;
                start = erase.start;
            }
        }

        public bool CanMerge(IUndoAction action) 
        {
            EntryEraseAction erase = action as EntryEraseAction;
            if(erase == null) {
                return false;
            }

            return !(
                is_cut || erase.is_cut ||                          // don't group separate text cuts
                start != (is_forward ? erase.start : erase.end) || // must meet eachother
                is_forward != erase.is_forward ||                  // don't group deletes with backspaces
                text[0] == '\n' ||                                 // don't group more than one line (inclusive)
                erase.text[0] == ' ' || erase.text[0] == '\t'      // don't group more than one word (exclusive)
            );
        }

        public override string ToString()
        {
            return String.Format("Erased: [{0}] ({1},{2})", text, start, end);
        }
    }
}
