using System.Collections.Generic;
using FundXchange.Domain.Entities;

namespace FundXchange.Model.ViewModels.Charts
{
    public interface IChartView
    {
        void RefreshInstrumentHeader(Instrument instrument);
        void LoadInitialCandlesticks(List<Domain.ValueObjects.Candlestick> candlesticks);
        void CreateCandlestick(Domain.ValueObjects.Candlestick candlestick);
        void UpdateLatestCandlestick(Domain.ValueObjects.Candlestick candlestick);
        void CloseLatestCandlestick(Domain.ValueObjects.Candlestick candlestick);
    }
}
