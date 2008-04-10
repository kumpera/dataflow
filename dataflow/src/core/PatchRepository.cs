/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using System.Reflection;
using C5;

namespace Dataflow.Core {
public class PatchRepository {
    HashDictionary <string, PatchMetaClass> patches = new HashDictionary<string, PatchMetaClass> ();

    static bool DependsOnDataflow(Assembly target, Assembly dataflow) {
        String dataflowName = dataflow.GetName().Name;
        foreach (AssemblyName name in target.GetReferencedAssemblies()) {
            if (name.Name == dataflowName) {
                return true;
            }
        }
        return false;
    }

    private void ProcessType(Type type, object [] attr) {
        foreach (object obj in attr) {
            PatchAttribute pa = obj as PatchAttribute;
            PatchMetaClass meta = new PatchMetaClass();
            patches.Add(pa.Name, meta);
        }
    }

    public void Init() {
        Assembly[] all = AppDomain.CurrentDomain.GetAssemblies();
        Assembly dataflow = Assembly.GetCallingAssembly();

        foreach (Assembly asm in all) {
            Type[] types = null;
            try {
                if (asm == dataflow || DependsOnDataflow(asm, dataflow))
                    types = asm.GetTypes();
            } catch (ReflectionTypeLoadException e) {
                types = e.Types;
            }

            if (types == null)
                continue;

            foreach (Type t in types) {
                object[] attr = t.GetCustomAttributes(typeof(PatchAttribute), true);
                if (attr != null && attr.Length > 0)
                    ProcessType(t, attr);
            }
        }
    }

    public PatchMetaClass GetPatchByName(string name) {
        return patches [name];
    }
}

}
