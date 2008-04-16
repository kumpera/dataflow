// 
// GtkUtilities.cs
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

namespace Hyena.Gui
{
    public static class GtkUtilities
    {
        private static Gdk.ModifierType [] important_modifiers = new Gdk.ModifierType [] {
            Gdk.ModifierType.ControlMask,
            Gdk.ModifierType.ShiftMask
        };
        
        public static bool NoImportantModifiersAreSet ()
        {
            return NoImportantModifiersAreSet (important_modifiers);
        }
            
        public static bool NoImportantModifiersAreSet (params Gdk.ModifierType [] modifiers)
        {
            Gdk.ModifierType state;
            
            if (Global.CurrentEvent is Gdk.EventKey) {
                state = ((Gdk.EventKey)Global.CurrentEvent).State;
            } else if (Global.CurrentEvent is Gdk.EventButton) {
                state = ((Gdk.EventButton)Global.CurrentEvent).State;
            } else {
                return false;
            }
            
            foreach (Gdk.ModifierType modifier in modifiers) {
                if ((state & modifier) == modifier) {
                    return false;
                }
            }
            
            return true;
        }
        
        public static Gdk.Color ColorBlend (Gdk.Color a, Gdk.Color b)
        {
            // at some point, might be nice to allow any blend?
            double blend = 0.5;

            if (blend < 0.0 || blend > 1.0) {
                throw new ApplicationException ("blend < 0.0 || blend > 1.0");
            }
            
            double blendRatio = 1.0 - blend;

            int aR = a.Red >> 8;
            int aG = a.Green >> 8;
            int aB = a.Blue >> 8;

            int bR = b.Red >> 8;
            int bG = b.Green >> 8;
            int bB = b.Blue >> 8;

            double mR = aR + bR;
            double mG = aG + bG;
            double mB = aB + bB;

            double blR = mR * blendRatio;
            double blG = mG * blendRatio;
            double blB = mB * blendRatio;

            Gdk.Color color = new Gdk.Color ((byte)blR, (byte)blG, (byte)blB);
            Gdk.Colormap.System.AllocColor (ref color, true, true);
            return color;
        }

    }
}
