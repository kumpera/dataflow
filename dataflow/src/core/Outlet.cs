/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using C5;

namespace Dataflow.Core {
public class Outlet<T> : IOutlet {
    IPatchContainer patch;
    string name;
    T value;
    int frame = -1;
    ArrayList<Inlet<T>> connections = new ArrayList<Inlet<T>>();

    public Outlet(string name, IPatchContainer patch) {
        this.name = name;
        this.patch = patch;
    }

    public bool PropagateChanges() {
        if (!HasChanged)
            return false;
        foreach (Inlet<T> inlet in connections)
            inlet.Value = value;
        return true;
    }


    public void PropagateChanges(IList<IPatchContainer> list) {
        if (!HasChanged)
            return;
        foreach (Inlet<T> inlet in connections) {
            inlet.Value = value;
            if (inlet.ExecutionEnabled && !list.Contains(inlet.Container))
                list.Add(inlet.Container);
        }
    }

    //FIXME right now we don't support connecting between diferent types, we need a IInlet::Cast
    public void ConnectTo(IInlet inlet) {
        connections.Add((Inlet<T>)inlet);
    }

    public T Value {
        get {
            return value;
        }
        set {
            this.value = value;
            this.frame = patch.CurrentFrame;
        }
    }

    public bool HasChanged {
        get {
            return frame == patch.CurrentFrame - 1;
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
}
}
