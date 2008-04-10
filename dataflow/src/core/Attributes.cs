/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;

namespace Dataflow.Core {

[AttributeUsage(AttributeTargets.Class)]
public class PatchAttribute : Attribute {
    public PatchAttribute(string name, string category) {
        Name = name;
    }

    public string Name { get; private set; }

}
}
