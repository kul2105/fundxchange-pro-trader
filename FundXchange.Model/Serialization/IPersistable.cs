using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FundXchange.Model.Serialization
{
    public interface IPersistable
    {
        bool IsSavedToDisk{get; set;}
        string FileName { get; set; }
        string FilePath { get; set; }
    }
}
