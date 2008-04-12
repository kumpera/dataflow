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
        cc.Color = new Color(0.5, 0.5, 0.5);

        cc.Pattern = new SolidPattern(new Color(1.0, 0, 0), true);

        cc.Rectangle(20, 20, 100, 100);
        cc.Fill();
    }
}

}
