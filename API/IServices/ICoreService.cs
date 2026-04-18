using Npgsql;
using System.Data;
using System.Linq.Expressions;

namespace API.IServices
{
    public interface ICoreService
    {

        // Dynamic Join
        IQueryable<TResult> SelectJoin<T1, T2, TKey, TResult>(
           Expression<Func<T1, TKey>> outerKeySelector,
           Expression<Func<T2, TKey>> innerKeySelector,
           Expression<Func<T1, T2, TResult>> resultSelector)
           where T1 : class
           where T2 : class;


        //SELECT TABLE OR VIEW
        Task<IReadOnlyList<T>> SelectRawAsync<T>(string sqlQuery, object[]? parameters, CancellationToken cancellationToken = default) where T : class;
        Task<IReadOnlyList<T>> SelectRawSetAsync<T>(string sqlQuery, object[]? parameters, CancellationToken cancellationToken = default) where T : class;

        Task<IEnumerable<T>> SelectRawDynamicAsync<T>(string baseQuery, Dictionary<string, object> conditions, CancellationToken cancellationToken = default) where T : class;

        Task<IEnumerable<T>> SelectCommandAsync<T>(string baseQuery, Dictionary<string, object> condition, CancellationToken cancellationToken = default) where T : class, new();



        //STORE PROCEDURE
        Task<IReadOnlyList<T>> ExecSPRawAsync<T>(string storedProcedure, object[]? parameters, CancellationToken cancellationToken = default) where T : class;
        Task<IReadOnlyList<T>> ExecSPRawSetAsync<T>(string storedProcedure, object[]? parameters, CancellationToken cancellationToken = default) where T : class;

        Task<IEnumerable<T>> ExecSPRawDynamicAsync<T>(string storedProcedure, Dictionary<string, object> parameters, CancellationToken cancellationToken = default) where T : class;

        Task<IEnumerable<T>> ExecSPCommandAsync<T>(string storedProcedure, Dictionary<string, object> parameters, CancellationToken cancellationToken = default) where T : class, new();

        Task<DataTable> ExecSPToDataTable(string storedProcedure, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
        Task<DataTable> ExecSPToDataTable(string query, NpgsqlParameter[] parameters, CancellationToken cancellationToken = default);
        Task<DataSet> ExecSPToDataSet(string storedProcedure, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);

        Task<List<DataTable>> ExecSPToDataTables(string storedProcedureName, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);

        Task<DataSet> ExecSPToDataSets(string storedProcedureName, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
        Task<bool> PostgresBulkInsertAsync(string tableName, DataTable? data, CancellationToken cancellationToken = default);
    }
}
