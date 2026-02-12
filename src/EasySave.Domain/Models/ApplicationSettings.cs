using EasySave.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Domain.Models
{
    // Represents configurable application settings for EasySave
    public class ApplicationSettings
    {
            public Language Language { get; set; }
            public int MaxBackupJobs { get; set; }
            public string LogDirectoryPath { get; set; }
            public string StateFileDirectoryPath { get; set; }
            public int LogFormat { get; set; }
            public string? BusinessSoftwareName { get; set; }

        // Default constructor
        public ApplicationSettings() { }
    }
}
