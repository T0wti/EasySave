using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Domain.Interfaces
{
    public interface IDialogService
    {
        Task<string?> OpenFolderPickerAsync();
    }
}
