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
public class PatchRepository {
    HashDictionary <string, PatchMetaClass> patches = new HashDictionary<string, PatchMetaClass> ();

    static bool DependsOnDataflow(Assembly target, Assembly dataflow) {
        var dataflowName = dataflow.GetName().Name;
        foreach (var name in target.GetReferencedAssemblies()) {
            if (name.Name == dataflowName) {
                return true;
            }
        }
        return false;
    }

    private void ProcessType(Type type, PatchAttribute pa) {
        PatchMetaClass meta = new PatchMetaClass(type);
        patches.Add(pa.Name, meta);
    }

    public void Init() {
        var all = AppDomain.CurrentDomain.GetAssemblies();
        var dataflow = Assembly.GetCallingAssembly();

        foreach (var asm in all) {
            Type[] types = null;
            try {
                if (asm == dataflow || DependsOnDataflow(asm, dataflow))
                    types = asm.GetTypes();
            } catch (ReflectionTypeLoadException e) {
                types = e.Types;
            }

            if (types == null)
                continue;

            foreach (var t in types) {
                var attr = t.GetCustomAttribute<PatchAttribute> ();
                if (attr != null)
                    ProcessType(t, attr);
            }
        }
    }

    public PatchMetaClass GetPatchByName(string name) {
        return patches [name];
    }
}

}
