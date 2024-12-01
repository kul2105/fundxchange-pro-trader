using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using JSONMaitlandLib;


namespace M4.BOService_DataModels
{
    class clsMaitlandServiceModel
    {
        public List<CashDetailMTD> GetCashMTD(string PortNum, DateTime startdt)
        {

            var content = GetServiceCall("GetCashDetailMTD", PortNum, startdt);
            if (content == string.Empty)
                return new List<CashDetailMTD>();
            JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            CashDetailMTDResponse _socketResponce = serializer.Deserialize<CashDetailMTDResponse>((content));

            return _socketResponce.lstData;
            //ds4Tables.dtCashMTDDataTable dt = ConvertListToCashTable(_socketResponce.lstData);
            //return dt;
        }
        public List<DailyFundPerformance> GetFundDailyPerformance(string PortNum, DateTime startdt)
        {
            var content = GetServiceCall("GetDailyFundPerformance", PortNum, startdt);
            if (content == string.Empty)
                return new List<DailyFundPerformance>();
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            DailyFundPerformanceResponse _socketResponce = serializer.Deserialize<DailyFundPerformanceResponse>((content));
            return _socketResponce.lstData;

        }

        internal List<FundSummary> GetFundSummary(string PortNum, DateTime startdt)
        {
            var content = GetServiceCall("GetFundSummary", PortNum, startdt);
            if (content == string.Empty)
                return new List<FundSummary>();
            JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            FundSummaryResponse _socketResponce = serializer.Deserialize<FundSummaryResponse>((content));
            return _socketResponce.lstData;

        }

        internal List<PortfolioDetail> GetPortfolioDetail(string PortNum, DateTime startdt)
        {
            var content = GetServiceCall("GetPortfolioDetail", PortNum, startdt);
            if (content == string.Empty)
                return new List<PortfolioDetail>();
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            PortfolioDetailResponse _socketResponce = serializer.Deserialize<PortfolioDetailResponse>((content));
            return _socketResponce.lstData;
        }

        internal List<ExpenseDetail> GetExpenseDetails(string PortNum, DateTime startdt)
        {
            var content = GetServiceCall("GetExpenseDetail", PortNum, startdt);
            if (content == string.Empty)
                return new List<ExpenseDetail>();
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            ExpenseDetailResponse _socketResponce = serializer.Deserialize<ExpenseDetailResponse>((content));
            return _socketResponce.lstData;

        }


        internal List<ExpenseSummary> GetExpSummary(string PortNum, DateTime startdt)
        {
            var content = GetServiceCall("GetExpenseSummary", PortNum, startdt);
            if (content == string.Empty)
                return new List<ExpenseSummary>();
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            ExpenseSummaryResponse _socketResponce = serializer.Deserialize<ExpenseSummaryResponse>((content));
            return _socketResponce.lstData;

        }

        internal List<TransactionDetail> GetTransDetail(string PortNum, DateTime startdt)
        {
            var content = GetServiceCall("GetTransDetail", PortNum, startdt);
            if (content == string.Empty)
                return new List<TransactionDetail>();
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            TransactionDetailResponse _socketResponce = serializer.Deserialize<TransactionDetailResponse>((content));
            return _socketResponce.lstData;
        }

        internal List<WithHoldingTax> GetWithHoldTax(string PortNum, DateTime startdt)
        {
            var content = GetServiceCall("GetWithHoldTax", PortNum, startdt);
            if (content == string.Empty)
                return new List<WithHoldingTax>();
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            WithHoldingTaxResponse _socketResponce = serializer.Deserialize<WithHoldingTaxResponse>((content));
            return _socketResponce.lstData;
        }

        internal List<TrialBalance> GetTrialBal(string PortNum, DateTime startdt)
        {
            var content = GetServiceCall("GetTrialBalace", PortNum, startdt);
            if (content == string.Empty)
                return new List<TrialBalance>();
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            TrialBalanceResponse _socketResponce = serializer.Deserialize<TrialBalanceResponse>((content));
            return _socketResponce.lstData;
        }

        internal List<StatMDetail> GetStatMDetail(string PortNum, DateTime startdt)
        {
            var content = GetServiceCall("GetStatMDetail", PortNum, startdt);
            if (content == string.Empty)
                return new List<StatMDetail>();
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            StatMDetailResponse _socketResponce = serializer.Deserialize<StatMDetailResponse>((content));
            return _socketResponce.lstData;
        }

        internal List<Settlement> GetSettl10Days(string PortNum, DateTime startdt)
        {
            var content = GetServiceCall("GetSettlement", PortNum, startdt);
            if (content == string.Empty)
                return new List<Settlement>();
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            SettlementResponse _socketResponce = serializer.Deserialize<SettlementResponse>((content));
            return _socketResponce.lstData;
        }

        internal List<AssetAllocation> GetAssetAllocation(string PortNum, DateTime startdt)
        {
            var content = GetServiceCall("GetAssetAllocation", PortNum, startdt);
            if (content == string.Empty)
                return new List<AssetAllocation>();
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            AssetAllocationResponse _socketResponce = serializer.Deserialize<AssetAllocationResponse>((content));
            return _socketResponce.lstData;
        }

        internal List<AnalysisofIncome> GetAnalysisTable(string PortNum, DateTime startdt)
        {
            var content = GetServiceCall("GetAnalysisofIncome", PortNum, startdt);
            if (content == string.Empty)
                return new List<AnalysisofIncome>();
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            AnalysisofIncomeResponse _socketResponce = serializer.Deserialize<AnalysisofIncomeResponse>((content));

            return _socketResponce.lstData;
        }

        internal List<NAV> GetNAVTable(string PortNum, DateTime startdt)
        {
            var content = GetServiceCall("GetNAV", PortNum, startdt);
            if (content == string.Empty)
                return new List<NAV>();
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            NAVResponse _socketResponce = serializer.Deserialize<NAVResponse>(content);//(System.Text.Encoding.UTF8.GetString(content));
            return _socketResponce.lstData;
        }

        private DataTable ConvertListToDataTable<T>(List<T> lstData)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.GetField | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in lstData)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;


            if (lstData.Count > 0)
            {
                foreach (var item in lstData[0].GetType().GetFields())
                {
                    dataTable.Columns.Add();
                }
                // Add rows.
                foreach (var row in lstData)
                {

                    //table.Rows.Add(row);
                }
            }
            return dataTable;
        }

        public string GetServiceCall(string MethodName, string PortNum, DateTime startdt)
        {
            try
            {
                Int32 unixTimestampStart = (Int32)(startdt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;


                string footer = PortNum + "/" + unixTimestampStart;
                var url = @"http://129.232.181.61:7777/MaitlandDataService.svc/" + MethodName + "/" + footer;

                var syncClient = new WebClient();
                var content = syncClient.DownloadString(url);//DownloadData(url);
                return content.ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
           
        }


    }
}
