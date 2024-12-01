using System;
using System.Collections.Generic;
using FundXchange.Domain.Entities;
using FundXchange.Domain.ValueObjects;

namespace FundXchange.Model.ViewModels.TimeSales
{
    public interface ITimeAndSalesView
    {
        void AddTrades(List<Trade> trades);
        void ClearGrid();
        void UpdateGridWithInstrument(Instrument instrument);
    }
}
