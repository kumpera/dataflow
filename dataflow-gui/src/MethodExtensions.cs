/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/
using System;
using Cairo;

namespace Dataflow.Gui {

static class ArrayExtensions {
	public static void EachWithIndex<T> (this T[] array, Action <T, int> closuse) {
		for (int i = 0; i < array.Length; ++i)
			closuse (array [i], i);
	}
}

}
