using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public class DbConnect
{ 
    public class ConnectionString
    {
        public const string CMS = "CMSConnection";
    }
    public class Connection
    { 
        // connect db cms
        public static IDbConnection CMS
        {
            get
            {
                return new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionString.CMS].ConnectionString);
            }
        }
    }
}
