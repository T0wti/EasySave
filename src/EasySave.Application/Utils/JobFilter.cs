using System;
using System.Collections.Generic;
using System.Text;
using EasySave.Application.DTOs;

namespace EasySave.Application.Utils
{
    public static class JobFilter
    {
        public static bool MatchesSearch(this BackupJobDTO job, string? searchText)
        {
            // To display everything if searchbar empty
            if (string.IsNullOrWhiteSpace(searchText)) return true;

            var lowerSearch = searchText.ToLower();

                   // Search by name
            return (job.Name != null && job.Name.ToLower().Contains(lowerSearch)) ||
                   // Search by source
                   (job.SourcePath != null && job.SourcePath.ToLower().Contains(lowerSearch)) ||
                   // Search by target
                   (job.TargetPath != null && job.TargetPath.ToLower().Contains(lowerSearch)) ||
                   // Search by type
                   (job.Type != null && job.Type.ToLower().Contains(lowerSearch));
        }
    }
}
