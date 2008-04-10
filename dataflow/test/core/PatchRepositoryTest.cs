/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using NUnit.Framework;

namespace Dataflow.Core {

[Patch("SamplePatch", "Samples")]
public class MyTestPatch : IPatch {
    public void Init(IPatchContainer container) {
    }

    public void Execute() {
    }
}

[TestFixture()]
public class PatchRepositoryTest {
    [Test()]
    public void RepositoryFindPatchFromLoadedAssemblies() {
        PatchRepository repo = new PatchRepository();
        repo.Init();

        Assert.IsNotNull(repo.GetPatchByName("SamplePatch"), "#1");
    }

    [Test()]
    public void RepositoryFindPatchFromExternalAssembly() {
        Assert.IsTrue(false, "Not Done");
    }
}
}
