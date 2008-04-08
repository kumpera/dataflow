/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using NUnit.Framework;
using NMock2;
using Dataflow.Core;

namespace Dataflow.Patches {

  [TestFixture()]
  public class NumberKeeperTest {
      protected Mockery mocks;
      protected IPatchContainer mockPatchContainer;

      [SetUp]
      public void SetUp() {
          mocks = new Mockery();
          mockPatchContainer = mocks.NewMock<IPatchContainer>();
          Stub.On(mockPatchContainer).GetProperty("CurrentFrame").Will(Return.Value(0));
      }

      [Test()]
      public void ShouldDefaultToZero() {
          Inlet();
          Outlet<int> result = Outlet();
          
          Run();
          
          Assert.AreEqual(0, result.Value);
      }

      [Test()]
      public void ShouldAlwaysReturnNumberSetInInlet() {
          Inlet<int> inlet = Inlet();
          Outlet<int> outlet = Outlet();
          
          inlet.Value = 10;

          Run();
          
          Assert.AreEqual(10, outlet.Value);
      }
      
      private Inlet<int> Inlet() {
        Inlet<int> inlet = new Inlet<int>("Number", mockPatchContainer, ActivationMode.ActivateOnChange);
        Expect.Once.On(mockPatchContainer).Method("AddInlet").With("Number").Will(Return.Value(inlet));
        return inlet;
      }
      
      private Outlet<int> Outlet() {
        Outlet<int> outlet = new Outlet<int>("result", mockPatchContainer);
        Expect.Once.On(mockPatchContainer).Method("AddOutlet").With("Number").Will(Return.Value(outlet));       
        return outlet;
      }
      
      private void Run() {
        NumberKeeper keeper = new NumberKeeper();
        keeper.Init(mockPatchContainer);
        keeper.Execute();
      }
  }

}