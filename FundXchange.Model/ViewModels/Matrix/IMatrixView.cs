using System.Collections.Generic;
using FundXchange.Domain.Entities;

namespace FundXchange.Model.ViewModels.Matrix
{
    public interface IMatrixView
    {
        void AddGridRowItems(List<MatrixViewItem> items);
        void UpdateGridRowItems(List<MatrixViewItem> items);
        void UpdateGridWithInstrument(Instrument instrument);
        void ClearGrid();
        void UpdateTotalTradeVolume(long totalTradeVolume);
        void UpdateTotalBidSize(long totalBidSize);
        void UpdateTotalOfferSize(long totalOfferSize);
    }
}
