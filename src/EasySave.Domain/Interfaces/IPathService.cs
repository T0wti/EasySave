using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Domain.Interfaces
{
    public interface IPathService
    {
        public string GetUncPath(string path);
    }
}
