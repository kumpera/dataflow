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
public interface IPatchContainer {
    ICollectionValue<IOutlet> Outlets {
        get;
    }
    ICollectionValue<IInlet> Inlets {
        get;
    }
    int CurrentFrame {
        get;
    }

    void ExecutePatch();

    Outlet<T> AddOutlet<T> (String name);
    Inlet<T> AddInlet<T> (String name);
    Inlet<T> AddPassiveInlet<T> (String name);
    Inlet<T> AddActivateOnChangeInlet<T> (String name);
}

public interface IInlet {
    IPatchContainer Container {
        get;
    }
}

public interface IOutlet {
    IPatchContainer Container {
        get;
    }
    void ConnectTo(IInlet inlet);
    void PropagateChanges(IList<IPatchContainer> fun);
}

public interface IPatch {
    void Init(IPatchContainer container);
    void Execute();
}
}
