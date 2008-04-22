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

public class InletMetaData {
    PatchMetaClass container;
    PropertyInfo property;
	InletAttribute attrs;

    public InletMetaData (PatchMetaClass container, PropertyInfo property) {
        this.container = container;
        this.property = property;
        this.attrs = property.GetCustomAttribute<InletAttribute> ();
        Name = attrs.Name ?? property.Name;
    }

    public string Name {
        get;
        private set;
    }

	public void Init (PatchContainer container, object obj) {
		//FIXME use delegates and generics magic to avoid reflection here
		property.SetValue (obj, Activator.CreateInstance (property.PropertyType, Name, container, attrs.Mode), null);
	}
}

public class OutletMetaData {
    PatchMetaClass container;
    PropertyInfo property;
	OutletAttribute attrs;

    public OutletMetaData(PatchMetaClass container, PropertyInfo property) {
        this.container = container;
        this.property = property;
        this.	attrs = property.GetCustomAttribute<OutletAttribute> ();
        Name = attrs.Name ?? property.Name;
    }

    public string Name {
        get;
        private set;
    }

	public void Init (PatchContainer container, object obj) {
		//FIXME use delegates and generics magic to avoid reflection here
		property.SetValue (obj, Activator.CreateInstance (property.PropertyType, Name, container), null);
		
	}
}

public class PatchMetaClass {
    Type type;
    InletMetaData[] inlets;
    OutletMetaData[] outlets;

    public PatchMetaClass(Type type) {
        this.type = type;
        var attrs = type.GetCustomAttribute<PatchAttribute> ();

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

        Name = attrs.Name ?? type.Name;
		Category = attrs.Category ?? "Generic";
    }

    public string Name {
        get;
        private set;
    }

    public string Category {
        get;
        private set;
    }

    public InletMetaData[] Inlets {
        get {
            return this.inlets;
        }
    }

    public OutletMetaData[] Outlets {
        get {
            return this.outlets;
        }
    }

	//FIXME we could avoid this thing by supplying a pair of delegates to PatchContainer instead.
	private class PatchAdapter : IPatch {
		public PatchAdapter (object obj) {
			//TODO extract the apropriate delegates
		}

		public void Init(IPatchContainer container) {

		}

		public void Execute() {

		}
	}

	IPatch MakePatch (object obj) {
		IPatch res = obj as IPatch;
		if (res != null)
			return res;
		return new PatchAdapter (obj);
	}
	
	public T NewInstance<T> () where T : class {
		if (typeof (T) != type)
			throw new ArgumentException ("invalid type "+typeof (T));

		var res = Activator.CreateInstance<T> ();

		var pc = new PatchContainer (MakePatch (res));

		foreach (InletMetaData inlet in this.inlets)
			inlet.Init (pc, res);

		foreach (OutletMetaData outlet in this.outlets)
			outlet.Init (pc, res);

		return res;
	}
}
}
