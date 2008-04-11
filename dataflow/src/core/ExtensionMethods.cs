/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using System.Reflection;


namespace Dataflow.Core.Extensions {

internal static class CoreExtendions {

    internal static T GetCustomAttribute<T> (this ICustomAttributeProvider self) where T : Attribute {
        var attrs = (T[]) self.GetCustomAttributes(typeof(T), true);
        if (attrs == null || attrs.Length == 0)
            return null;
        return attrs [0];
    }

    internal static bool HasCustomAttribute<T> (this ICustomAttributeProvider self) where T : Attribute {
        var attrs = self.GetCustomAttributes(typeof(T), true);
        return attrs != null && attrs.Length > 0;
    }

}
}
