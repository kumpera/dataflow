//
// Choreographer.cs
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
using Hyena.Widgets;

namespace Hyena.Widgets
{
    public enum Blocking
    {
        Upstage,
        Downstage
    }
    
    public enum Easing
    {
        Linear,
        QuadraticIn,
        QuadraticOut,
        QuadraticInOut,
        ExponentialIn,
        ExponentialOut,
        ExponentialInOut
    }
}

namespace Hyena.Gui.Theatrics
{
    public static class Choreographer
    {
        public static int Compose (double percent, int size, Easing easing)
        {
            switch (easing) {
                case Easing.QuadraticIn:
                    return (int)(percent * percent * size);
                case Easing.QuadraticOut:
                    return (int)(-size * percent * (percent - 2));
                case Easing.QuadraticInOut:
                    percent *= 2;
                    return (percent < 1)
                        ? (int)(percent * percent * (size / 2))
                        : (int)((-size / 2) * (--percent * (percent - 2) - 1));
                case Easing.ExponentialIn:
                    return (int)(size * Math.Pow (2, 10 * (percent - 1)));
                case Easing.ExponentialOut:
                    return (int)(size * (-Math.Pow (2, -10 * percent) + 1));
                case Easing.ExponentialInOut:
                    percent *= 2;
                    return (percent < 1)
                        ? (int)(size/2 * Math.Pow (2, 10 * (percent - 1)))
                        : (int)(size/2 * (-Math.Pow (2, -10 * --percent) + 2));
                default:
                    return (int)(percent * size);
            }
        }
    }
}