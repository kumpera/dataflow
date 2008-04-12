using System;
using Cairo;

namespace Dataflow.Gui {

public class EditorCanvas : Gtk.DrawingArea {
    protected override bool OnExposeEvent(Gdk.EventExpose args) {
        using(Context g = Gdk.CairoHelper.Create(args.Window)) {
            int x, y, width, height, depth;
            GdkWindow.GetGeometry(out x, out y, out width, out height, out depth);
            Draw(g, x, y, width, height, depth);
        }

        return true;
    }

    void Draw(Context cc, int x, int y, int width, int height, int depth) {

        //fill background
        cc.Color = new Color(0.8, 0.8, 0.8);
        cc.Paint();


        cc.Color = new Color(0.6, 0.6, 0.6);
        DrawRoundedRectangle(cc, 40, 40, 80, 120, 10);

        DrawRoundedRectangle(cc, 40, 40, 80, 120, 10);

    }


    void DrawRoundedRectangle(Context cc, double x, double y, double width, double height, double arcRadius) {

        cc.MoveTo(x + arcRadius, y);
        cc.LineTo(x + width - arcRadius, y);
        cc.Arc(x + width - arcRadius, y + arcRadius, arcRadius, -Math.PI / 2 , 0);

        cc.LineTo(x + width, y + height - arcRadius);
        cc.Arc(x + width - arcRadius, y + height - arcRadius, arcRadius, 0, Math.PI / 2);

        cc.LineTo(x + arcRadius,  y + height);
        cc.Arc(x + arcRadius, y + height - arcRadius, arcRadius, Math.PI / 2, Math.PI);

        cc.LineTo(x,  y + arcRadius);
        cc.Arc(x + arcRadius, y + arcRadius, arcRadius, Math.PI, -Math.PI / 2);

        cc.ClosePath();
        cc.Fill();

    }
}

}
