/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using System.Collections.Generic;
using System.Text;

using Hyena.Gui;
using Hyena.Collections;
using Hyena.Gui.Theming;
using Hyena.Gui.Theatrics;
using Hyena.Data.Gui;
using Hyena.Data;

using Dataflow.Core;

namespace Dataflow.Gui {


public class PatchListView : ListView<PatchInfo> {

    private ColumnController columnController;

    public PatchListView () {
        columnController = new ColumnController ();

        SortableColumn categoryColumn = new SortableColumn ("Category", new ColumnCellText ("Category", true), 0.225, "Category", true);
        SortableColumn nameColumn = new SortableColumn ("Name", new ColumnCellText ("Name", true), 0.225, "Name", true);

        columnController.AddRange (categoryColumn, nameColumn);
        
        //columnController.Load ();
        
        ColumnController = DefaultColumnController;
        ColumnController.DefaultSortColumn = categoryColumn;

        RulesHint = true;
        RowSensitivePropertyName = "Category";
       
        ForceDragSourceSet = true;
        Reorderable = false;
    }

#region Drag and Drop

    protected override void OnDragDataGet (Gdk.DragContext context, Gtk.SelectionData data, uint info, uint time) {
		Console.WriteLine ("---OnDragDataGet");
		string patchName = null;
		foreach (int idx in Selection) {
			Console.WriteLine ("selected {0}", idx);
			patchName = Model[idx].Name;
		}

		if (patchName != null) 
			data.Set (
				Gdk.Atom.Intern (ListViewDragDropTarget.ModelSelection.Target, true),
				8,
				Encoding.UTF8.GetBytes (patchName));
    }

#endregion
    
    public ColumnController DefaultColumnController {
        get { return columnController; }
    }
}
}

