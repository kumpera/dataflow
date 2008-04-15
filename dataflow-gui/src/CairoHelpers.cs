/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/
using System;
using Cairo;


namespace Dataflow.Gui {

internal static class DrawingPrimitives {
	internal static void DrawCircle (Context cc, double cx, double cy, double radius) {
		cc.NewPath ();
        cc.Arc(cx, cy, radius, 0, 2 * Math.PI);
	}

    internal static void DrawRoundedRectangle (Context cc, double x, double y, double width, double height, double arcRadius) {
        cc.MoveTo (x + arcRadius, y);
        cc.LineTo (x + width - arcRadius, y);
        cc.Arc (x + width - arcRadius, y + arcRadius, arcRadius, -Math.PI / 2 , 0);

        cc.LineTo (x + width, y + height - arcRadius);
        cc.Arc (x + width - arcRadius, y + height - arcRadius, arcRadius, 0, Math.PI / 2);

        cc.LineTo (x + arcRadius,  y + height);
        cc.Arc (x + arcRadius, y + height - arcRadius, arcRadius, Math.PI / 2, Math.PI);

        cc.LineTo (x, y + arcRadius);
        cc.Arc (x + arcRadius, y + arcRadius, arcRadius, Math.PI, -Math.PI / 2);

        cc.ClosePath();
    }


	internal static void DrawUpRoundedRectangle (Context cc, double x, double y, double width, double height, double arcRadius) {
        cc.MoveTo(x + arcRadius, y);
        cc.LineTo(x + width - arcRadius, y);
        cc.Arc(x + width - arcRadius, y + arcRadius, arcRadius, -Math.PI / 2 , 0);

        cc.LineTo(x + width, y + height);
        cc.LineTo(x,  y + height);
        cc.LineTo(x, y + arcRadius);
        cc.Arc(x + arcRadius, y + arcRadius, arcRadius, Math.PI, -Math.PI / 2);

        cc.ClosePath();
	}
}
}
