using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace EasySave.GUI.Services
{
    public class DialogService
    {
        public async Task<string?> OpenFolderPickerAsync()
        {

            var desktop = (IClassicDesktopStyleApplicationLifetime)Avalonia.Application.Current!.ApplicationLifetime!;
            var mainWindow = desktop.MainWindow!;

            var result = await mainWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions {
                Title = "Chose directory", //TO BE CHANGED
                AllowMultiple = false
            });
            
            //OpenFolderPickerAsync returns list
            if (result.Count > 0) return result[0].Path.LocalPath;

            return null;
        }
    }
}
