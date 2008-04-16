/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/
using System;
using Gtk;


namespace Dataflow.Gui {

public class Driver {
    static void Main() {
        Application.Init();
        EditorWindow ed = new EditorWindow();
        ed.ShowAll();

        Application.Run();
    }
}
}
