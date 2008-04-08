/*
 * Copyright © 2008 Rodrigo Kumpera
*
* Permission to use, copy, modify, distribute, and sell this software and its
* documentation for any purpose is hereby granted without fee, provided that
* the above copyright notice appear in all copies and that both that
* copyright notice and this permission notice appear in supporting
* documentation, and that the name of the Authors not be used in advertising or
* publicity pertaining to distribution of the software without specific,
* written prior permission. The Authors makes no representations about the
* suitability of this software for any purpose.  It is provided "as is"
* without express or implied warranty.
*
* The AUTHORS DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE, INCLUDING ALL
* IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS, IN NO EVENT SHALL SuSE
* BE LIABLE FOR ANY SPECIAL, INDIRECT OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
* WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION
* OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN
* CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*
* Author:  Rodrigo Kumpera
*/

using System;
using C5;

namespace Dataflow.Core {
public enum ActivationMode {
    /*
     * Passive means the patch won't be enqueued when the value is set.
     */
    Passive,

    /*
     * ActivateOnMessage means that the patch will be activated at most once per cicle
     * no mather thhe value.
     */
    ActivateOnMessage,

    /*
     * ActivateOnMessage means that the patch will be activated at most once per cicle
     * only if the value changes.
     */
    ActivateOnChange
}

public class Inlet<T> : IInlet {
    IPatchContainer patch;
    string name;
    T value;
    int frame;
    ActivationMode mode;
    bool valueChanged;

    //EqualityComparer<T> comparer;// = EqualityComparer<T>.Default;

    public Inlet(string name, IPatchContainer patch, ActivationMode mode) {
        this.name = name;
        this.patch = patch;
        this.mode = mode;
    }

    public T Value {
        get {
            return value;
        }
        set {
            this.valueChanged = !EqualityComparer<T>.Equals(this.value, value);
            this.value = value;
            this.frame = patch.CurrentFrame;
        }
    }

    public bool HasChanged {
        get {
            return frame == patch.CurrentFrame;
        }
    }

    public string Name {
        get {
            return name;
        }
    }

    public IPatchContainer Container {
        get {
            return patch;
        }
    }

    public bool ExecutionEnabled {
        get {
            switch (mode) {
            case ActivationMode.Passive:
                return false;
            case ActivationMode.ActivateOnMessage:
                return HasChanged;
            case ActivationMode.ActivateOnChange:
                return HasChanged && valueChanged;
            }
            throw new InvalidOperationException("Invalid Inlet execution mode");
        }
    }
}
}
