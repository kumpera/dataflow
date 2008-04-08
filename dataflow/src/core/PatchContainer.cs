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
}
}
