/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using C5;

namespace Dataflow.Core {

public class PatchContainer : IPatchContainer {
    IPatch patch;
    int frame = 1;

    HashDictionary<string, IInlet> inlets = new HashDictionary<string, IInlet> ();
    HashDictionary<string, IOutlet> outlets = new HashDictionary<string, IOutlet> ();

    public int CurrentFrame {
        get {
            return this.frame;
        }
    }
    public ICollectionValue<IOutlet> Outlets {
        get {
            return this.outlets.Values;
        }
    }

    public ICollectionValue<IInlet> Inlets {
        get {
            return this.inlets.Values;
        }
    }

    public IInlet GetInlet(string name) {
        return inlets [name];
    }

    public IOutlet GetOutlet(string name) {
        return outlets [name];
    }

    public PatchContainer(IPatch patch) {
        this.patch = patch;
        patch.Init(this);
    }

    public Inlet<T> AddInlet<T> (String name) {
        Inlet<T> res = new Inlet<T> (name, this, ActivationMode.ActivateOnMessage);
        this.inlets.Add(name, res);
        return res;
    }

    public Inlet<T> AddPassiveInlet<T> (String name) {
        Inlet<T> res = new Inlet<T> (name, this, ActivationMode.Passive);
        this.inlets.Add(name, res);
        return res;
    }

    public Inlet<T> AddActivateOnChangeInlet<T> (String name) {
        Inlet<T> res = new Inlet<T> (name, this, ActivationMode.ActivateOnChange);
        this.inlets.Add(name, res);
        return res;
    }

    public Outlet<T> AddOutlet<T> (String name) {
        Outlet<T> res = new Outlet<T> (name, this);
        this.outlets.Add(name, res);
        return res;
    }

    public void ExecutePatch() {
        this.patch.Execute();
        ++frame;
    }

	public IPatch Patch { get { return patch; } }
}
}
