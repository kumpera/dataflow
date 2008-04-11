/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using System.Reflection;
using C5;

using Dataflow.Core.Extensions;

namespace Dataflow.Core {

internal class InletMetaData {
    PatchMetaClass container;
    PropertyInfo property;

    public InletMetaData(PatchMetaClass container, PropertyInfo property) {
        this.container = container;
        this.property = property;
        Attributes = property.GetCustomAttribute<InletAttribute> ();
        Name = Attributes.Name != null ? Attributes.Name : property.Name;
    }

    public string Name {
        get;
        private set;
    }

    public InletAttribute Attributes {
        get;
        private set;
    }
}

internal class OutletMetaData {
    PatchMetaClass container;
    PropertyInfo property;

    public OutletMetaData(PatchMetaClass container, PropertyInfo property) {
        this.container = container;
        this.property = property;
        Attributes = property.GetCustomAttribute<OutletAttribute> ();
        Name = Attributes.Name != null ? Attributes.Name : property.Name;
    }

    public string Name {
        get;
        private set;
    }

    public OutletAttribute Attributes {
        get;
        private set;
    }
}

public class PatchMetaClass {
    Type type;
    InletMetaData[] inlets;
    OutletMetaData[] outlets;



    public PatchMetaClass(Type type) {
        this.type = type;
        Attributes = type.GetCustomAttribute<PatchAttribute> ();

        var ins = new ArrayList<InletMetaData> ();
        var outs = new ArrayList<OutletMetaData> ();

        foreach (var prop in type.GetProperties()) {
            if (prop.HasCustomAttribute<InletAttribute> ())
                ins.Add(new InletMetaData(this, prop));
            if (prop.HasCustomAttribute<OutletAttribute> ())
                outs.Add(new OutletMetaData(this, prop));
        }

        this.inlets = ins.ToArray();
        this.outlets = outs.ToArray();

        Name = Attributes.Name != null ? Attributes.Name : type.Name;
    }

    public string Name {
        get;
        private set;
    }

    internal InletMetaData[] Inlets {
        get {
            return this.inlets;
        }
    }
    internal OutletMetaData[] Outlets {
        get {
            return this.outlets;
        }
    }


    public PatchAttribute Attributes {
        get;
        private set;
    }

}
}
