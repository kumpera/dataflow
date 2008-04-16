//
// AnimatedVBox.cs
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
    public class AnimatedVBox : AnimatedBox
    {
        protected override void OnSizeRequested (ref Requisition requisition)
        {
            int width = 0;
            int height = 0;
            int rollover_Spacing = 0;
            
            foreach (AnimatedWidget widget in Widgets) {
                Requisition req = widget.SizeRequest ();
                widget.Size = req.Height + rollover_Spacing;
                widget.Alloc.Y = rollover_Spacing;
                
                if (widget.IsFirst) {
                    if (widget.Next != null) {
                        if (widget.AnimationState != AnimationState.Idle) {
                            widget.Size += Spacing;
                        } else {
                            widget.Size += EndSpacing;
                            rollover_Spacing = StartSpacing;
                        }
                    }
                } else if (widget.Next != null && widget.Next.IsLast && 
                    widget.Next.AnimationState != AnimationState.Idle) {
                    rollover_Spacing = Spacing;
                } else if (!widget.IsLast) {
                    widget.Size += EndSpacing;
                    rollover_Spacing = StartSpacing;
                }
                
                height += widget.Value;
                if (req.Width > width) {
                    width = req.Width;
                }
            }
            
            requisition.Width = width;
            requisition.Height = height;
        }
        
        protected override void OnSizeAllocated (Rectangle allocation)
        {
            base.OnSizeAllocated (allocation);
            foreach (AnimatedWidget widget in Widgets) {
                allocation.Height = widget.Value;
                if (widget.Blocking == Blocking.Downstage) {
                    widget.Alloc.Y += widget.Value - widget.Size;
                }
                widget.Alloc.Width = allocation.Width;
                widget.SizeAllocate (allocation);
                allocation.Y += allocation.Height;
            }
        }
    }
}