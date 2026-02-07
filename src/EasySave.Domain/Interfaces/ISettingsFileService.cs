using EasySave.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Domain.Interfaces
{
    public interface ISettingsFileService
    {
        ApplicationSettings ReadSettings();
        void WriteSettings(ApplicationSettings settings);
    }
}
