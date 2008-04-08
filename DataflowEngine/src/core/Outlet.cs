/*
 * Copyright © 2008 Rodrigo Kumpera
*
* Permission to use, copy, modify, distribute, and sell this software and its
* documentation for any purpose is hereby granted without fee, provided that
* the above copyright notice appear in all copies and that both that
* copyright notice and this permission notice appear in supporting
* documentation, and that the name of the Authors not be used in advertising or
* publicity pertaining to distribution of the software without specific,
* written prior permission. The Authors makes no representations about the
* suitability of this software for any purpose.  It is provided "as is"
* without express or implied warranty.
*
* The AUTHORS DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE, INCLUDING ALL
* IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS, IN NO EVENT SHALL SuSE
* BE LIABLE FOR ANY SPECIAL, INDIRECT OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
* WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION
* OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN
* CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*
* Author:  Rodrigo Kumpera
*/

using System;
using C5;

namespace Dataflow.Core
{	
	public class Outlet<T> : IOutlet
	{
		IPatchContainer patch;
		string name;
		T value;
		int frame = -1;
		ArrayList<Inlet<T>> connections = new ArrayList<Inlet<T>>();

		public Outlet (string name, IPatchContainer patch) {
			this.name = name;
			this.patch = patch;
		}

		public bool PropagateChanges () {
			if (!HasChanged)
				return false;
			foreach (Inlet<T> inlet in connections)
				inlet.Value = value;
			return true;
		}

		
		public void PropagateChanges (IList<IPatchContainer> list) {
			if (!HasChanged)
				return;
			foreach (Inlet<T> inlet in connections) {
				inlet.Value = value;
				if (inlet.ExecutionEnabled && !list.Contains (inlet.Container))
				    list.Add (inlet.Container);
			}
		}

		//FIXME right now we don't support connecting between diferent types, we need a IInlet::Cast 
		public void ConnectTo (IInlet inlet) {
			connections.Add ((Inlet<T>)inlet);
		}

		public T Value {
			get {
				return value;
			}
			set {
				this.value = value;
				this.frame = patch.CurrentFrame;
			}
		}

		public bool HasChanged { get { return frame == patch.CurrentFrame - 1; } }

		public string Name { get { return name; } }

		public IPatchContainer Container { get { return patch; } }
	}
}
