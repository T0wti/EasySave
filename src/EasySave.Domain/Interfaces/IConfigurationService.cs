using System;
using System.Collections.Generic;
using System.Text;
using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    public interface IConfigurationService
    {
        ApplicationSettings LoadSettings();

        void SaveSettings(ApplicationSettings settings);

        bool FileExists();
        void EnsureConfigExists();
    }
}
