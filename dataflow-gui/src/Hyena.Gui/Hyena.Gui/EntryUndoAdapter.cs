//
// EntryUndoAdapter.cs
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
    public class EntryUndoAdapter
    {
        private Entry entry;
        private UndoManager undo_manager = new UndoManager();
        private AccelGroup accel_group = new AccelGroup();

        public EntryUndoAdapter(Entry entry)
        {
            this.entry = entry;

            entry.KeyPressEvent += OnKeyPressEvent;
            entry.TextDeleted += OnTextDeleted;
            entry.TextInserted += OnTextInserted;
            entry.PopulatePopup += OnPopulatePopup;
        }

        private void OnKeyPressEvent(object o, KeyPressEventArgs args)
        {
            if((args.Event.State & Gdk.ModifierType.ControlMask) != 0) {
                switch(args.Event.Key) {
                    case Gdk.Key.z:
                        undo_manager.Undo();
                        args.RetVal = true;
                        break;
                    case Gdk.Key.Z:
                    case Gdk.Key.y:
                        undo_manager.Redo();
                        args.RetVal = true;
                        break;
                }
            }

            args.RetVal = false;
        }

        [GLib.ConnectBefore]
        private void OnTextDeleted(object o, TextDeletedArgs args)
        {
            if(args.StartPos != args.EndPos) {
                undo_manager.AddUndoAction(new EntryEraseAction(entry, 
                    args.StartPos, args.EndPos));
            }
        }

        [GLib.ConnectBefore]
        private void OnTextInserted(object o, TextInsertedArgs args)
        {
            undo_manager.AddUndoAction(new EntryInsertAction(entry, 
                args.Position, args.Text, args.Length));
        }

        private void OnPopulatePopup(object o, PopulatePopupArgs args)
        {
            Menu menu = args.Menu;
            MenuItem item;

            item = new SeparatorMenuItem();
            item.Show();
            menu.Prepend(item);

            item = new ImageMenuItem(Stock.Redo, null);
            item.Sensitive = undo_manager.CanRedo;
            item.Activated += delegate { undo_manager.Redo(); };
            item.AddAccelerator("activate", accel_group, (uint)Gdk.Key.z, 
                Gdk.ModifierType.ControlMask | Gdk.ModifierType.ShiftMask, 
                AccelFlags.Visible);
            item.Show();
            menu.Prepend(item);

            item = new ImageMenuItem(Stock.Undo, null);
            item.Sensitive = undo_manager.CanUndo;
            item.Activated += delegate { undo_manager.Undo(); };
            item.AddAccelerator("activate", accel_group, (uint)Gdk.Key.z, 
                Gdk.ModifierType.ControlMask, AccelFlags.Visible);
            item.Show();
            menu.Prepend(item);
        }
        
        public UndoManager UndoManager {
            get { return undo_manager; }
        }
    }
}
