using System;
using System.Collections.Generic;
using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;
using FrostyEditor.ViewModels.Docks;
using FrostyEditor.ViewModels.Documents;
using FrostyEditor.ViewModels.Tools;

namespace FrostyEditor.ViewModels;

public class DockFactory : Factory
{
    private IRootDock? m_rootDock;
    private IDocumentDock? m_documentDock;
    private ITool? m_findTool;

    public override IDocumentDock CreateDocumentDock() => new FilesDocumentDock();

    public override IRootDock CreateLayout()
    {
        DefaultPageViewModel untitledFileViewModel = new()
        {
            Title = "Home"
        };

        DataExplorerViewModel dataExplorerViewModel = new()
        {
            Id = "Data Explorer",
            Title = "Data Explorer"
        };

        FilesDocumentDock documentDock = new()
        {
            Id = "Files",
            Title = "Files",
            IsCollapsable = false,
            Proportion = double.NaN,
            ActiveDockable = untitledFileViewModel,
            VisibleDockables = CreateList<IDockable>
            (
                untitledFileViewModel
            ),
            CanCreateDocument = false
        };

        ProportionalDock tools = new()
        {
            Proportion = 0.2,
            Orientation = Orientation.Vertical,
            VisibleDockables = CreateList<IDockable>
            (
                new ToolDock
                {
                    ActiveDockable = dataExplorerViewModel,
                    VisibleDockables = CreateList<IDockable>
                    (
                        dataExplorerViewModel
                    ),
                    Alignment = Alignment.Left,
                    GripMode = GripMode.Visible
                }
            )
        };

        IRootDock windowLayout = CreateRootDock();
        windowLayout.Title = "Default";
        ProportionalDock windowLayoutContent = new()
        {
            Orientation = Orientation.Horizontal,
            IsCollapsable = false,
            VisibleDockables = CreateList<IDockable>
            (
                tools,
                new ProportionalDockSplitter(),
                documentDock
            )
        };
        windowLayout.IsCollapsable = false;
        windowLayout.VisibleDockables = CreateList<IDockable>(windowLayoutContent);
        windowLayout.ActiveDockable = windowLayoutContent;

        IRootDock rootDock = CreateRootDock();

        rootDock.IsCollapsable = false;
        rootDock.VisibleDockables = CreateList<IDockable>(windowLayout);
        rootDock.ActiveDockable = windowLayout;
        rootDock.DefaultDockable = windowLayout;

        m_documentDock = documentDock;
        m_rootDock = rootDock;
        m_findTool = dataExplorerViewModel;

        return rootDock;
    }

    public override void InitLayout(IDockable layout)
    {
        ContextLocator = new Dictionary<string, Func<object?>>
        {
            ["Find"] = () => layout
        };

        DockableLocator = new Dictionary<string, Func<IDockable?>>
        {
            ["Root"] = () => m_rootDock,
            ["Files"] = () => m_documentDock,
            ["Data Explorer"] = () => m_findTool
        };

        HostWindowLocator = new Dictionary<string, Func<IHostWindow?>>
        {
            [nameof(IDockWindow)] = () => new HostWindow()
        };

        base.InitLayout(layout);
    }
}
