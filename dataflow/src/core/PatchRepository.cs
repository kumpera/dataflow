/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using System.Reflection;
using System.Collections.Generic;

using C5;

using Dataflow.Core.Extensions;


namespace Dataflow.Core {
public class PatchRepository {
    HashDictionary <string, PatchMetaClass> patches = new HashDictionary<string, PatchMetaClass> ();

    static bool DependsOnDataflow (Assembly target, Assembly dataflow) {
        var dataflowName = dataflow.GetName().Name;
        foreach (var name in target.GetReferencedAssemblies()) {
            if (name.Name == dataflowName) {
                return true;
            }
        }
        return false;
    }

    private void ProcessType (Type type, PatchAttribute pa) {
        PatchMetaClass meta = new PatchMetaClass(type);
        patches.Add(pa.Name, meta);
    }

    public void Init () {
        var all = AppDomain.CurrentDomain.GetAssemblies();
        var dataflow = typeof (PatchRepository).Assembly;
		patches.Clear ();

        foreach (var asm in all) {
			//Console.WriteLine ("assembly {0} equals {1} depends {2}", asm, asm == dataflow, DependsOnDataflow (asm, dataflow));
            Type[] types = null;
            try {
                if (asm == dataflow || DependsOnDataflow (asm, dataflow))
                    types = asm.GetTypes();
            } catch (ReflectionTypeLoadException e) {
                types = e.Types;
            }

            if (types == null)
                continue;

            foreach (var t in types) {
				//Console.WriteLine ("type {0} has attr {1}", t, t.HasCustomAttribute<PatchAttribute> ());
				PatchAttribute attr = null;
				try {
		            attr = t.GetCustomAttribute<PatchAttribute> ();
				} catch (Exception ex) {
					//All sorts of exceptions can make this call fails
				}
	            if (attr != null)
	                ProcessType(t, attr);
            }
        }
    }

    public PatchMetaClass GetPatchByName (string name) {
        return patches [name];
    }

    public IEnumerator<PatchMetaClass> GetEnumerator ()
    {
        foreach (var patch in patches)
            yield return patch.Value;
    }

	public int Count { get { return patches.Count; } }
}

}
