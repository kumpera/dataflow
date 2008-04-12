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
