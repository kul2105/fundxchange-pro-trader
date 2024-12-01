using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSONMaitlandLib;
using M4.BOService_DataModels;

namespace M4.BO_Grid_Controllers
{
    class clsMaitServiceController
    {
        IViewBO _view;
        private clsMaitlandServiceModel _serviveModel;

        public clsMaitServiceController(IViewBO View, clsMaitlandServiceModel model)
        {
            _view = View;
            _serviveModel = model;
        }
        public void LoadServiceData(string PortNum, DateTime dt)
        {
            List<CashDetailMTD> dt1 = _serviveModel.GetCashMTD(PortNum, dt);
            List<DailyFundPerformance> dt2 = _serviveModel.GetFundDailyPerformance(PortNum, dt);
            List<ExpenseDetail> dt3 = _serviveModel.GetExpenseDetails(PortNum, dt);
            List<ExpenseSummary> dt4 = _serviveModel.GetExpSummary(PortNum, dt);
            List<AnalysisofIncome> dt5 = _serviveModel.GetAnalysisTable(PortNum, dt);
            List<AssetAllocation> dt6 = _serviveModel.GetAssetAllocation(PortNum, dt);
            List<FundSummary> dt7 = _serviveModel.GetFundSummary(PortNum, dt);
            List<TransactionDetail> dt8 = _serviveModel.GetTransDetail(PortNum, dt);
            List<StatMDetail> dt9 = _serviveModel.GetStatMDetail(PortNum, dt);
            List<Settlement> dt10 = _serviveModel.GetSettl10Days(PortNum, dt);
            List<TrialBalance> dt11 = _serviveModel.GetTrialBal(PortNum, dt);
            List<WithHoldingTax> dt12 = _serviveModel.GetWithHoldTax(PortNum, dt);
            List<PortfolioDetail> dt13= _serviveModel.GetPortfolioDetail(PortNum, dt);
            List<NAV> dt14 = _serviveModel.GetNAVTable(PortNum, dt);

            List<PortfolioDetail> dtPort = _serviveModel.GetPortfolioDetail(PortNum, dt.AddDays(-1));

            _view.BindData(dt1, dt2, dt3, dt4, dt5, dt6, dt7, dt8, dt9, dt10, dt11, dt12,dt13,dt14);
            _view.GetLastDayPortData(dtPort);

        }

    }
}
