using Dapper;
using service.Infrastructure.Queries.Account;
using System.Data;

namespace service.Infrastructure.Data
{
    public class YourEntityRepository
    {
        private readonly IDbConnection _dbConnection;

        public YourEntityRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Employees>> GetAllAsync()
        {
            var sql = AccountQueries.GetAllUsers;
            return await _dbConnection.QueryAsync<Employees>(sql);
        }

        public async Task<Employees> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM YourEntities WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Employees>(sql, new { Id = id });
        }

        public async Task<int> AddAsync(Employees entity)
        {
            var sql = "INSERT INTO YourEntities (Name) VALUES (@Name)";
            return await _dbConnection.ExecuteAsync(sql, entity);
        }
    }
}
