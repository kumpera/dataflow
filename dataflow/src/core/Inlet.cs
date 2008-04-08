/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
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
