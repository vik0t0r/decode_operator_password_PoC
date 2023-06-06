using System.Data.SqlClient;

namespace PoC;

public class DBqueries
{
    private string connectionString = "";

    public DBqueries(string conString)
    {
        connectionString = conString;
    }
    
    public string getSysCodeFromDB()
    {

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                string query1 = "SELECT SysCode FROM dbo.tb_SysCnfg";
                SqlCommand command1 = new SqlCommand(query1, connection);


                connection.Open();

                SqlDataReader reader = command1.ExecuteReader();

                while (reader.Read())
                {
                    string syscode = (string)reader["SysCode"];
                    //    Console.WriteLine($"SysCode (stored in DB): {syscode}");
                    return syscode;
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        return null;
    }

    public  List<Operator> getOperatorDataFromDB()
    {
        List<Operator> ret = new List<Operator>();
        
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            
            string query = "SELECT id_operator, Name, log_username, log_password FROM dbo.tb_Operators";
            
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Operator op = new Operator(reader);
                        ret.Add(op);
                    }
                }
            }
        }

        return ret;
    }
}