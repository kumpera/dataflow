//
// AnimatedVboxActor.cs
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
using System.Collections.Generic;
using Gdk;
using Gtk;

using Hyena.Gui.Theatrics;

namespace Hyena.Widgets
{
    internal enum AnimationState
    {
        Coming,
        Idle,
        IntendingToGo,
        Going
    }
    
    internal sealed class AnimatedWidget : Container
    {
        public event EventHandler WidgetDestroyed;
        
        public Widget Widget;
        public Rectangle Alloc;
        public Easing Easing;
        public Blocking Blocking;
        public AnimationState AnimationState;
        public uint Duration;
        public double Bias = 1.0;
        public LinkedListNode <AnimatedWidget> Node;
        
        private double percent;
        private int size;
        private int value;
        private bool has_value;
        private Pixmap canvas;
        
        public AnimatedWidget (Widget widget, uint duration, Easing easing, Blocking blocking)
        {
            Widget = widget;
            Duration = duration;
            Easing = easing;
            Blocking = blocking;
            AnimationState = AnimationState.Coming;

            Widget.Parent = this;
            Widget.Destroyed += OnWidgetDestroyed;
            ShowAll ();
        }
        
        protected AnimatedWidget (IntPtr raw) : base (raw)
        {
        }
        
        private void OnWidgetDestroyed (object sender, EventArgs args)
        {
            if (!IsRealized) {
                return;
            }
            
            canvas = new Pixmap (GdkWindow, Alloc.Width, Alloc.Height);
            canvas.DrawDrawable (Style.BackgroundGC (State), GdkWindow,
                Alloc.X, Alloc.Y, 0, 0, Alloc.Width, Alloc.Height);
            
            if (AnimationState != AnimationState.Going) {
                WidgetDestroyed (this, args);
            }
        }
        
#region Overrides
        
        protected override void OnRemoved (Widget widget)
        {
            if (widget == Widget) {
                widget.Unparent ();
                Widget = null;
            }
        }
        
        protected override void OnRealized ()
        {
            WidgetFlags |= WidgetFlags.Realized;
            
            Gdk.WindowAttr attributes = new Gdk.WindowAttr ();
            attributes.WindowType = Gdk.WindowType.Child;
            attributes.X = Allocation.X;
            attributes.Y = Allocation.Y;
            attributes.Width = Allocation.Width;
            attributes.Height = Allocation.Height;
            attributes.Wclass = Gdk.WindowClass.InputOutput;
            attributes.EventMask = (int)Gdk.EventMask.ExposureMask;
            
            Gdk.WindowAttributesType attributes_mask = 
                Gdk.WindowAttributesType.X | 
                Gdk.WindowAttributesType.Y;
                
            GdkWindow = new Gdk.Window (Parent.GdkWindow, attributes, attributes_mask);
            GdkWindow.UserData = Handle;
            GdkWindow.Background = Style.Background (State);
            Style.Attach (GdkWindow);
        }
        
        protected override void OnSizeRequested (ref Requisition requisition)
        {
            if (Widget != null) {
                Requisition req = Widget.SizeRequest ();
                Alloc.Width = req.Width;
                Alloc.Height = req.Height;
            }
            requisition.Width = Alloc.Width;
            requisition.Height = Alloc.Height;
        }
        
        protected override void OnSizeAllocated (Rectangle allocation)
        {
            if (Widget != null) {
                Widget.SizeAllocate (Alloc);
            }
            base.OnSizeAllocated (allocation);
        }

        
        protected override bool OnExposeEvent (EventExpose evnt)
        {
            if (canvas != null) {
                GdkWindow.DrawDrawable (Style.BackgroundGC (State), canvas,
                    0, 0, Alloc.X, Alloc.Y, Alloc.Width, Alloc.Height);
                return true;
            } else {
                return base.OnExposeEvent (evnt);
            }
        }

        protected override void ForAll (bool include_internals, Callback callback)
        {
            if (Widget != null) {
                callback (Widget);
            }
        }
        
#endregion
        
#region Properties
        
        public int Size {
            get { return size; }
            set {
                size = value;
                has_value = false;
            }
        }
        
        public double Percent {
            get { return percent; }
            set {
                percent = value * Bias;
                has_value = false;
            }
        }
        
        public int Value {
            get {
                if (!has_value) {
                    this.value = Choreographer.Compose (percent, size, Easing);
                }
                return this.value;
            }
        }
        
        public bool IsFirst {
            get { return Node.Previous == null; }
        }
        
        public bool IsLast {
            get { return Node.Next == null; }
        }
        
        public AnimatedWidget Next {
            get { return Node.Next == null ? null : Node.Next.Value; }
        }
        
#endregion
        
    }
}