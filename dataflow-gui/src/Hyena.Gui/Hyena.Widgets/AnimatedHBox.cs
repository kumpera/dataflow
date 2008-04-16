//
// AnimatedHBox.cs
//
// Authors:
//   Scott Peterson <lunchtimemama@gmail.com>
//
// Copyright (C) 2008 Scott Peterson
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
using Gdk;
using Gtk;

namespace Hyena.Widgets
{
    public class AnimatedHBox : AnimatedBox
    {
        protected override void OnSizeRequested (ref Requisition requisition)
        {
            int width = 0;
            int height = 0;
            
            foreach (AnimatedWidget widget in Widgets) {
                Requisition req = widget.SizeRequest ();
                widget.Size = req.Width + Spacing;
                width += widget.Value;
                
                if (req.Height > height) {
                    height = req.Height;
                }
            }
            
            requisition.Width = width;
            requisition.Height = height;
        }
        
        protected override void OnSizeAllocated (Rectangle allocation)
        {
            base.OnSizeAllocated (allocation);
            
            foreach (AnimatedWidget widget in Widgets) {
                allocation.Width = widget.Value;
                widget.Alloc.X = StartSpacing;
                
                if (widget.Blocking == Blocking.Downstage) {
                    widget.Alloc.X += widget.Value - widget.Size;
                }
                
                widget.Alloc.Height = allocation.Height;
                widget.SizeAllocate (allocation);
                allocation.X += allocation.Width;
            }
        }
    }
}
