using System.Text;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Mvvm.Controls;
using FrostyEditor.ViewModels.Documents;

namespace FrostyEditor.ViewModels.Docks;

public class FilesDocumentDock : DocumentDock
{
    public FilesDocumentDock()
    {
        CanCreateDocument = false;
    }

    private void CreateNewDocument()
    {
        if (!CanCreateDocument)
        {
            return;
        }

        FileViewModel document = new()
        {
            Title = "Untitled"
        };

        Factory?.AddDockable(this, document);
        Factory?.SetActiveDockable(document);
        Factory?.SetFocusedDockable(this, document);
    }
}
