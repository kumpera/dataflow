/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using C5;

namespace Dataflow.Core {
public class Engine {
    ArrayList <PatchContainer> rootSet = new ArrayList<PatchContainer>();
    HashDictionary <IPatch, PatchContainer> mapping = new HashDictionary<IPatch, PatchContainer> ();

    public Engine() {
    }

    public void Add(IPatch patch) {
        PatchContainer pc = new PatchContainer(patch);
        mapping.Add(patch, pc);
        rootSet.Add(pc);
    }

    public void Connect(IPatch from, string outlet, IPatch to, string inlet) {
        PatchContainer fromCont = mapping [from];
        PatchContainer toCont = mapping [to];
        rootSet.Remove(toCont);

        fromCont.GetOutlet(outlet).ConnectTo(toCont.GetInlet(inlet));
    }

    /*
     * This is the central method of the whole Dataflow engine as it executes the patches.
     *
     * A breath first search over the entire graph is done, starting from the rootSet.
     * All patches that have no connected inlets are part of the rootSet.
     *
     * The algorith is the follow
     *
     * 1) push all elements from the root set into the execution queue
     *
     * 2) dequeue a patch from the execution queue
     * 3) if execution criteria is met execute it *1
     * 4) propagate the values of all changed outlets and add the receiving patches to the discovered set *2 *3
     * 5) goto to step 2 if the execution queue is not empty
     * 6) move all elements from the discovered set to the execution queue
     * 7) goto step 2 if the execution queue is not empty
     * 8) finish the frame
     *
     * *1 right now the only criteria is 'execute allways'
     * *2 outlet values propagation and patch enqueuing are done from first to last outlet.
     * *3 order of discovery is maintained during execution.
     */
    public void StepFrame() {
        LinkedList<IPatchContainer> executionQueue = new LinkedList<IPatchContainer> ();
        HashedLinkedList<IPatchContainer> discoveredSet = new HashedLinkedList<IPatchContainer> ();

        executionQueue.AddAll(this.rootSet);

        do {
            while (executionQueue.Count > 0) {
                IPatchContainer patch = executionQueue.RemoveFirst();
                patch.ExecutePatch();
                foreach (IOutlet outlet in patch.Outlets)
                    outlet.PropagateChanges(discoveredSet);
            }
            if (discoveredSet.Count > 0) {
                executionQueue.AddAll(discoveredSet);
                discoveredSet.Clear();
            }
        } while (executionQueue.Count > 0);
    }
}
}
