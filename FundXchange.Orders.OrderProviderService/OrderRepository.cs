using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace FundXchange.Orders.OrderProviderService
{
    public class OrderRepository
    {
        private string _ConnectionString;

        public OrderRepository()
        {
            _ConnectionString = ConfigurationManager.ConnectionStrings["OrdersConnection"].ConnectionString;
        }

        public int InsertTradingAccount(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("TradingAccountAdd", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    SqlParameter outParam = new SqlParameter("@TradingAccountId", SqlDbType.Int);
                    outParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outParam);

                    cmd.ExecuteNonQuery();
                    int tradingAccountId = Convert.ToInt32(cmd.Parameters["@TradingAccountId"].Value);
                    return tradingAccountId;

                }
            }
            catch (Exception ex)
            {
                //todo: logging
            }
            return -1;
        }

        public void DeleteTradingAccount(int tradingAccountId)
        {
            ExecuteDeleteCommand("TradingAccountDel", "@TradingAccountId", tradingAccountId);
        }

        private void ExecuteDeleteCommand(string sp, string idParam, object idValue)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sp, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue(idParam, idValue);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //todo: logging
            }
        }
    }
}
