using System.Data.SqlClient;

namespace PoC;

public class Operator
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public string log_username { get; set; }
    public string log_password { get; set; }
    
    public string cleartext_password { get; set; }

    public Operator(SqlDataReader reader)
    {
        this.Id =(int) reader[0];
        this.Name = (string)reader[1];
        this.log_username = (string)reader[2];
        this.log_password = (string)reader[3];
    }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(log_username)}: {log_username}, {nameof(log_password)}: {log_password}, {nameof(cleartext_password)}: {cleartext_password}";
    }
}