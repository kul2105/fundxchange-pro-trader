using System;
using System.Collections.Generic;
using FundXchange.Model.ViewModels.DataManager;

namespace FundXchange.Model.ViewModels
{
    public class DataManagerViewModel
    {
        public Dictionary<string, DataManagerInstrument> Instruments { get; set; }
    }
}
