/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/
using System;
using Gtk;


namespace Dataflow.Gui {

public class EditorWindow : Gtk.Window {
	Gtk.Button okButton;
	Gtk.TextView logArea;
	EditorCanvas canvas;
		

    void SetResize(Gtk.Paned paned, Gtk.Widget child, bool resize) {
        ((Gtk.Paned.PanedChild)(paned[child])).Resize = resize;
    }

    void AddOkButton(Gtk.HPaned leftPanel) {
        okButton = new Gtk.Button();
        okButton.Label = "Me clica";
        leftPanel.Add(okButton);
        SetResize(leftPanel, okButton, false);
    }

    void AddLogArea(Gtk.VPaned middlePanel) {
        Gtk.ScrolledWindow scroll = new Gtk.ScrolledWindow ();
        scroll.ShadowType = Gtk.ShadowType.In;

        logArea = new Gtk.TextView ();
		logArea.Editable = false;
        scroll.Add (logArea);
        middlePanel.Add (scroll);
    }

    void AddDrawingArea(Gtk.VPaned middlePanel) {
        canvas = new EditorCanvas ();
        middlePanel.Add (canvas);
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

		canvas.LogEvent = (text) => logArea.Buffer.InsertAtCursor (text +"\n");
        okButton.Clicked += (obj, arg) => Application.Quit ();
        DeleteEvent += (obj, arg) => Application.Quit ();
    }
}
}
