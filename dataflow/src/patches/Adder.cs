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
using Dataflow.Core;

namespace Dataflow.Patches
{

	public class Adder : IPatch
	{
		Inlet<int> left;
		Inlet<int> right;
		Outlet<int> result;

		public void Init (IPatchContainer container) {
			left = container.AddInlet<int> ("left");
			right = container.AddInlet<int> ("right");
			result = container.AddOutlet<int> ("result");
		}

		public void Execute ()
		{
			result.Value = left.Value + right.Value;
		}
	}
}
