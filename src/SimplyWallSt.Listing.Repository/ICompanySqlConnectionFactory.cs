using System.Data.SQLite;

namespace SimplyWallSt.Listing.Repository
{
    public interface ICompanySqlConnectionFactory
    {
        SQLiteConnection GetConnection();
    }
}