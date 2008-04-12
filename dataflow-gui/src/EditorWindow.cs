using System;
using Gtk;


namespace Dataflow.Gui {

public class EditorWindow : Gtk.Window {
    static void SetResize(Gtk.Paned paned, Gtk.Widget child, bool resize) {
        ((Gtk.Paned.PanedChild)(paned[child])).Resize = resize;
    }

    static void AddOkButton(Gtk.HPaned leftPanel) {
        Gtk.Button okButton = new Gtk.Button();
        okButton.Label = "Me clica";
        leftPanel.Add(okButton);
        SetResize(leftPanel, okButton, false);
        okButton.Clicked += (obj, arg) = > Application.Quit();
    }

    static void AddLogArea(Gtk.VPaned middlePanel) {
        Gtk.ScrolledWindow scroll = new Gtk.ScrolledWindow();
        scroll.ShadowType = Gtk.ShadowType.In;

        Gtk.TextView logArea = new Gtk.TextView();
        scroll.Add(logArea);
        middlePanel.Add(scroll);
    }

    static void AddDrawingArea(Gtk.VPaned middlePanel) {
        Gtk.DrawingArea canvas = new EditorCanvas();
        middlePanel.Add(canvas);
        SetResize(middlePanel, canvas, true);
    }

    public EditorWindow(): base("Dataflow Editor") {
        DefaultWidth = 671;
        DefaultHeight = 480;

        Gtk.HPaned leftPanel = new Gtk.HPaned();
        leftPanel.Position = 75;

        AddOkButton(leftPanel);

        Gtk.VPaned middlePanel = new Gtk.VPaned();
        middlePanel.Position = 317;

        AddDrawingArea(middlePanel);
        AddLogArea(middlePanel);

        leftPanel.Add(middlePanel);

        this.Add(leftPanel);

        DeleteEvent += (obj, arg) => Application.Quit();
    }
}
}
