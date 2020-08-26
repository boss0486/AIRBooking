using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace WebCore.Entities
{
    [AL.NetFrame.Attributes.ConnectionString(DbConnect.ConnectionString.CMS)]
    public class DbConnection
    {
    }
}