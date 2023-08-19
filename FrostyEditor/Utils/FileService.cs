using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace FrostyEditor.Utils;

public static class FileService
{
    /// <summary>
    /// Opens file picker dialog.
    /// </summary>
    /// <param name="inOptions">Options for the dialog.</param>
    /// <returns>Array of selected <see cref="IStorageFile"/> or empty collection if user canceled the dialog or null if no MainWindow was found.</returns>
    public static async Task<IReadOnlyList<IStorageFile>?> OpenFilesAsync(FilePickerOpenOptions inOptions)
    {
        TopLevel? topLevel = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
            ?.MainWindow;

        if (topLevel is null)
        {
            return null;
        }
        
        return await topLevel.StorageProvider.OpenFilePickerAsync(inOptions);
    }
}