using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace MessagesHandlerService
{
    public class SQLDBServer : DBService
    {
        public SQLDBServer(string connectionString) : base(connectionString)
        {
        }

        public override int GetCurrentValue()
        {
            int currentValue;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Select CurrentValue From CoronaStatus", connection))
                {
                    command.CommandType = CommandType.Text;
                    try
                    {
                        currentValue = Convert.ToInt32(command.ExecuteScalar());
                    }
                    catch (Exception)
                    {
                        currentValue = 1;
                    }
                    return currentValue;
                }
            }
        }

        public override bool SetDelta(int currentValue)
        {
           using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    using (SqlCommand command = new SqlCommand("UPDATE CoronaStatus " +
                                                               " SET CurrentValue = " +
                                                               " SELECT ISNULL(CoronaStatus,0) + @CurrentValue FROM CoronaStatus  ", connection))
                    {
                        command.Transaction = transaction;
                        try
                        {
                            command.Parameters.Add("@CurrentValue", SqlDbType.Int).Value = currentValue;
                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }
                }
            }
            return true;
        }

    }
}