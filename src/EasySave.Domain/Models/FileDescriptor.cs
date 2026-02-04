using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Domain.Models
{
    public class FileDescriptor
    {
        public string FullPath { get; init; }
        public long Size { get; init; }
    }
}