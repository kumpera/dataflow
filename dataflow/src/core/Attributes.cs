/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;

namespace Dataflow.Core {

[AttributeUsage(AttributeTargets.Class)]
public class PatchAttribute : Attribute {
    string name;
    public PatchAttribute(string name, string category) {
        this.name = name;
    }

    public string Name {
        get {
            return name;
        }
    }
}
}
