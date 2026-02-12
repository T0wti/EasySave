using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Domain.Models
{
    // Represents a single file with its path and size
    public class FileDescriptor
    {
        public required string FullPath { get; init; }
        public long Size { get; init; }
    }
}