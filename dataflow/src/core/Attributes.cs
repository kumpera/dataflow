/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;

namespace Dataflow.Core {

public abstract class DocumentableAttribute : Attribute {
    public string Name {
        get;
        set;
    }

    public string Category {
        get;
        set;
    }

    public string Tags {
        get;
        set;
    }

    public string ShortDescription {
        get;
        set;
    }

    public string LongDescription {
        get;
        set;
    }

    public string DocumentationUrl {
        get;
        set;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PatchAttribute : DocumentableAttribute {

}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class InletAttribute : DocumentableAttribute {

    public InletAttribute() {
        Mode = ActivationMode.ActivateOnMessage;
    }

    public ActivationMode Mode {
        get;
        set;
    }

}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class OutletAttribute : DocumentableAttribute {

}

}
