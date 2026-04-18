
using API.Data;
using API.IServices;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;

namespace API.Services
{
    public class CoreService(
        AppDbContext dbContext
    ) : ICoreService
    {


        private readonly AppDbContext _dbContext = dbContext;
        // Dynamic Join
        public IQueryable<TResult> SelectJoin<T1, T2, TKey, TResult>(
            Expression<Func<T1, TKey>> outerKeySelector,
            Expression<Func<T2, TKey>> innerKeySelector,
            Expression<Func<T1, T2, TResult>> resultSelector)
            where T1 : class
            where T2 : class
        {
                return _dbContext.Set<T1>()
                               .Join(_dbContext.Set<T2>(),
                                     outerKeySelector,
                                     innerKeySelector,
                                     resultSelector)
                               .AsQueryable();
        }


        //SELECT TABLE OR VIEW
        public async Task<IReadOnlyList<T>> SelectRawAsync<T>(string sqlQuery, object[]? parameters = null, CancellationToken cancellationToken = default) where T : class
        {
            return await _dbContext.Database
                .SqlQueryRaw<T>(sqlQuery, parameters ?? Array.Empty<object>())
                .ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<T>> SelectRawSetAsync<T>(string sqlQuery, object[]? parameters = null, CancellationToken cancellationToken = default) where T : class
        {
            return await _dbContext.Set<T>()
                .FromSqlRaw(sqlQuery, parameters ?? Array.Empty<object>())
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        //USED
        //public async Task<IEnumerable<User>> GetUsersByConditionAsync(string condition)
        //{
        //    string sqlQuery = "SELECT * FROM Users WHERE Name LIKE @p0";
        //    return await _repository.SelectViewAsync<User>(sqlQuery, $"%{condition}%");
        //}

        public async Task<IEnumerable<T>> SelectRawDynamicAsync<T>(string baseQuery, Dictionary<string, object>? conditions, CancellationToken cancellationToken = default) where T : class
        {
            var queryBuilder = new StringBuilder(baseQuery);
            var parameters = new List<object>();

            if (conditions!= null && conditions.Count != 0)
            {
                queryBuilder.Append(" WHERE ");
                var conditionList = new List<string>();
                int paramIndex = 0;

                foreach (var condition in conditions)
                {
                    conditionList.Add($"{condition.Key} = @p{paramIndex}");
                    parameters.Add(condition.Value);
                    paramIndex++;
                }

                queryBuilder.Append(string.Join(" AND ", conditionList));
            }

                return await _dbContext.Set<T>().FromSqlRaw(queryBuilder.ToString(), [.. parameters]).AsNoTracking().ToListAsync(cancellationToken);
                //return await _context.Set<T>().FromSqlRaw(queryBuilder.ToString(), parameters.ToArray()).ToListAsync();
        }

        //USED :
        //EXAMPLE IN REPO USER
        //public async Task<IEnumerable<User>> GetUsersByConditionsAsync(Dictionary<string, object> conditions)
        //{
        //    string baseQuery = "SELECT * FROM Users";
        //    return await _repository.SelectDynamicViewAsync<User>(baseQuery, conditions);
        //}
        //EXAMPLE IN CONTROLLER
        //var conditions = new Dictionary<string, object>
        //{
        //    { "Name", "John" },
        //    { "Age", 30 }
        //};

        //var users = await userService.GetUsersByConditionsAsync(conditions);

        public async Task<IEnumerable<T>> SelectCommandAsync<T>(string baseQuery, Dictionary<string, object>? conditions, CancellationToken cancellationToken = default) where T : class, new()
        {
            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                var queryBuilder = new StringBuilder(baseQuery);
                var parameters = new List<NpgsqlParameter>();

                if (conditions != null && conditions.Count != 0)
                {
                    queryBuilder.Append(" WHERE ");
                    var conditionList = new List<string>();
                    int paramIndex = 0;

                    foreach (var condition in conditions)
                    {
                        var paramName = $"p{paramIndex}";
                        conditionList.Add($"{condition.Key} = @{paramName}");
                        parameters.Add(new NpgsqlParameter(paramName, condition.Value ?? DBNull.Value));
                        paramIndex++;
                    }

                    queryBuilder.Append(string.Join(" AND ", conditionList));
                }

                command.CommandText = queryBuilder.ToString();
                command.Parameters.AddRange(parameters.ToArray());

                await _dbContext.Database.OpenConnectionAsync(cancellationToken);
                using var result = await command.ExecuteReaderAsync(cancellationToken);

                var entities = new List<T>();
                while (await result.ReadAsync())
                {
                    var entity = new T();
                    for (int i = 0; i < result.FieldCount; i++)
                    {
                        var property = typeof(T).GetProperty(result.GetName(i), BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (property != null && !result.IsDBNull(i))
                        {
                            property.SetValue(entity, result.GetValue(i));
                        }
                    }
                    entities.Add(entity);
                }

                await _dbContext.Database.CloseConnectionAsync();
                return entities;
            }
        }


        //public async Task<IEnumerable<T>> SelectCommandAsync<T>(string baseQuery, Dictionary<string, object>? conditions) where T : class, new()
        //{

        //        using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
        //        {
        //            var queryBuilder = new StringBuilder(baseQuery);
        //            var parameters = new List<SqlParameter>();

        //            if (conditions!= null && conditions.Count != 0)
        //            {
        //                queryBuilder.Append(" WHERE ");
        //                var conditionList = new List<string>();
        //                int paramIndex = 0;

        //                foreach (var condition in conditions)
        //                {
        //                    var paramName = $"@p{paramIndex}";
        //                    conditionList.Add($"{condition.Key} = {paramName}");
        //                    parameters.Add(new SqlParameter(paramName, condition.Value ?? DBNull.Value));
        //                    paramIndex++;
        //                }

        //                queryBuilder.Append(string.Join(" AND ", conditionList));
        //            }

        //            command.CommandText = queryBuilder.ToString();
        //            command.Parameters.AddRange(parameters.ToArray());

        //            await _dbContext.Database.OpenConnectionAsync();
        //            using var result = await command.ExecuteReaderAsync();
        //            var entities = new List<T>();
        //            while (await result.ReadAsync())
        //            {
        //                var entity = new T();
        //                for (int i = 0; i < result.FieldCount; i++)
        //                {
        //                    var property = entity.GetType().GetProperty(result.GetName(i));
        //                    if (property != null && !result.IsDBNull(i))
        //                    {
        //                        property.SetValue(entity, result.GetValue(i));
        //                    }
        //                }
        //                entities.Add(entity);
        //            }

        //            await _dbContext.Database.CloseConnectionAsync();
        //            return entities;
        //        }
        //}


        //USED
        //public async Task<IEnumerable<User>> GetUsersByConditionsAsync(Dictionary<string, object> conditions)
        //{
        //    string baseQuery = "SELECT * FROM Users";
        //    return await _repository.SelecCommandViewAsync<User>(baseQuery, conditions);
        //}
        //var conditions = new Dictionary<string, object>
        //{
        //    { "Name", "John" },
        //    { "Age", 30 }
        //};
        //var users = await userService.GetUsersByConditionsAsync(conditions);




        //STORE PROCEDURE

        public async Task<IReadOnlyList<T>> ExecSPRawAsync<T>(string sql, object[]? parameters = null, CancellationToken cancellationToken = default) where T : class
        {
            return await _dbContext.Database
                .SqlQueryRaw<T>(sql, parameters ?? Array.Empty<object>())
                .ToListAsync(cancellationToken);
        }


        public async Task<IReadOnlyList<T>> ExecSPRawSetAsync<T>(string storedProcedure, object[]? parameters = null, CancellationToken cancellationToken = default) where T : class
        {
            return await _dbContext.Set<T>()
                .FromSqlRaw(storedProcedure, parameters ?? Array.Empty<object>())
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        //USED
        //public async Task<IEnumerable<User>> GetUsersByStoredProcedureAsync(string searchTerm)
        //{
        //    string storedProcedure = "EXEC GetUsersBySearchTerm @p0";
        //    return await _repository.ExecuteSPAsync<User>(storedProcedure, searchTerm);
        //}

        //public async Task<IEnumerable<T>> ExecSPRawDynamicAsync<T>(string storedProcedure, Dictionary<string, object>? parameters) where T : class
        //{

        //        SqlParameter[] sqlParameters = Array.Empty<SqlParameter>(); // Initialize sqlParameters
        //        if (parameters != null)
        //        {
        //            sqlParameters = parameters.Select(p => new SqlParameter(p.Key, p.Value)).ToArray();
        //        }
        //        return await _dbContext.Set<T>().FromSqlRaw(storedProcedure, sqlParameters).AsNoTracking().ToListAsync();
        //}


        public async Task<IEnumerable<T>> ExecSPRawDynamicAsync<T>(string storedProcedure, Dictionary<string, object>? parameters, CancellationToken cancellationToken = default) where T : class
        {
            NpgsqlParameter[] npgsqlParameters = Array.Empty<NpgsqlParameter>();

            if (parameters != null)
            {
                npgsqlParameters = parameters
                    .Select(p => new NpgsqlParameter(p.Key, p.Value ?? DBNull.Value))
                    .ToArray();
            }

            return await _dbContext.Set<T>()
                .FromSqlRaw($"SELECT * FROM {storedProcedure}({string.Join(", ", npgsqlParameters.Select(p => "@" + p.ParameterName))})",
                            npgsqlParameters)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }


        //USED
        //public async Task<IEnumerable<User>> GetUsersByStoredProcedureAsync(Dictionary<string, object> parameters)
        //{
        //    string storedProcedure = "EXEC GetUsersBySearchTerm @Name, @Age";
        //    return await _repository.ExecSPRawDynamicAsync<User>(storedProcedure, parameters);
        //}

        //var parameters = new Dictionary<string, object>
        //{
        //    { "Name", "John" },
        //    { "Age", 30 }
        //};

        //var users = await userService.GetUsersByStoredProcedureAsync(parameters);

        //public async Task<IEnumerable<T>> ExecSPCommandAsync<T>(string storedProcedure, Dictionary<string, object>? parameters) where T : class, new()
        //{
        //        using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
        //        {
        //            command.CommandText = storedProcedure;
        //            command.CommandType = CommandType.StoredProcedure;
        //            if (parameters != null && parameters.Count > 0)
        //            {
        //                foreach (var param in parameters)
        //                {
        //                    var sqlParam = new SqlParameter(param.Key, param.Value ?? DBNull.Value);
        //                    command.Parameters.Add(sqlParam);
        //                }
        //            }
        //            var entities = new List<T>();

        //            await _dbContext.Database.OpenConnectionAsync();

        //            using (var result = await command.ExecuteReaderAsync())
        //            {
        //                while (await result.ReadAsync())
        //                {
        //                    var entity = new T();
        //                    for (int i = 0; i < result.FieldCount; i++)
        //                    {
        //                        var property = entity.GetType().GetProperty(result.GetName(i));
        //                        if (property != null && !result.IsDBNull(i))
        //                        {
        //                            property.SetValue(entity, result.GetValue(i));
        //                        }
        //                    }
        //                    entities.Add(entity);
        //                }
        //            }

        //            await _dbContext.Database.CloseConnectionAsync();
        //            return entities;
        //        }
        //}


        public async Task<IEnumerable<T>> ExecSPCommandAsync<T>(string storedProcedure, Dictionary<string, object>? parameters, CancellationToken cancellationToken = default) where T : class, new()
        {
            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = storedProcedure;
                command.CommandType = CommandType.StoredProcedure;

                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var param in parameters)
                    {
                        var npgsqlParam = new NpgsqlParameter(param.Key, param.Value ?? DBNull.Value);
                        command.Parameters.Add(npgsqlParam);
                    }
                }

                var entities = new List<T>();

                await _dbContext.Database.OpenConnectionAsync(cancellationToken);

                using (var result = await command.ExecuteReaderAsync(cancellationToken))
                {
                    while (await result.ReadAsync())
                    {
                        var entity = new T();
                        for (int i = 0; i < result.FieldCount; i++)
                        {
                            var property = typeof(T).GetProperty(result.GetName(i), BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                            if (property != null && !result.IsDBNull(i))
                            {
                                property.SetValue(entity, result.GetValue(i));
                            }
                        }
                        entities.Add(entity);
                    }
                }

                await _dbContext.Database.CloseConnectionAsync();
                return entities;
            }
        }


        //// USED
        //public async Task<IEnumerable<User>> GetUsersByStoredProcedureAsync(Dictionary<string, object> parameters)
        //{
        //    string storedProcedure = "GetUsersBySearchTerm";
        //    return await _repository.ExecuteCommandSPAsync<User>(storedProcedure, parameters);
        //}
        //var parameters = new Dictionary<string, object>
        //{
        //    { "Name", "John" },
        //    { "Age", 30 }
        //};

        //var users = await userService.GetUsersByStoredProcedureAsync(parameters);


        public async Task<DataTable> ExecSPToDataTable(string storedProcedure, Dictionary<string, object>? parameters, CancellationToken cancellationToken = default)
        {
            try
            {
                var dataTable = new DataTable();

                using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = storedProcedure;
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var param in parameters)
                        {
                            var npgsqlParam = new NpgsqlParameter(param.Key, param.Value ?? DBNull.Value);
                            command.Parameters.Add(npgsqlParam);
                        }
                    }

                    await _dbContext.Database.OpenConnectionAsync(cancellationToken);

                    using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        dataTable.Load(reader);
                    }

                    await _dbContext.Database.CloseConnectionAsync();
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure: {ex.Message}", ex);
            }
        }

        public async Task<DataTable> ExecSPToDataTable(string query, NpgsqlParameter[] parameters, CancellationToken cancellationToken = default)
        {
            var dataTable = new DataTable();

            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }

                await _dbContext.Database.OpenConnectionAsync(cancellationToken);

                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    dataTable.Load(reader);
                }

                await _dbContext.Database.CloseConnectionAsync();
            }

            return dataTable;
        }



        //public async Task<DataTable> ExecSPToDataTable(string storedProcedure, Dictionary<string, object>? parameters)
        //{
        //    try
        //    {
        //        var dataTable = new DataTable();

        //            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
        //            {
        //                command.CommandText = storedProcedure;
        //                command.CommandType = CommandType.StoredProcedure;
        //                if (parameters != null && parameters.Count > 0)
        //                {
        //                    foreach (var param in parameters)
        //                    {
        //                        var sqlParam = new SqlParameter(param.Key, param.Value ?? DBNull.Value);
        //                        command.Parameters.Add(sqlParam);
        //                    }
        //                }
        //                await _dbContext.Database.OpenConnectionAsync();

        //                using (var result = await command.ExecuteReaderAsync())
        //                {
        //                    dataTable.Load(result);
        //                }

        //                await _dbContext.Database.CloseConnectionAsync();
        //                return dataTable;
        //            }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message.ToString());
        //    }
        //}
        // For DataTable

        //var parameters = new Dictionary<string, object>
        //{
        //    { "Name", "John" },
        //    { "Age", 30 }
        //};
        //var dataTable = ExecSPToDataTable("GetDynamicProducts",parameters);
        //foreach (DataRow row in dataTable.Rows)
        //{
        //  Console.WriteLine($"{row["Id"]}, {row["Name"]}, {row["Price"]}");
        //}

        //public async Task<DataSet> ExecSPToDataSet(string storedProcedure, Dictionary<string, object>? parameters)
        //{
        //    var dataSet = new DataSet();
        //        using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
        //        {
        //            command.CommandText = storedProcedure;
        //            command.CommandType = CommandType.StoredProcedure;
        //            if (parameters != null && parameters.Count > 0)
        //            {
        //                foreach (var param in parameters)
        //                {
        //                    var sqlParam = new SqlParameter(param.Key, param.Value ?? DBNull.Value);
        //                    command.Parameters.Add(sqlParam);
        //                }
        //            }

        //            await _dbContext.Database.OpenConnectionAsync();

        //            using (var result = await command.ExecuteReaderAsync())
        //            {
        //                do
        //                {
        //                    var dataTable = new DataTable();
        //                    dataTable.Load(result);
        //                    dataSet.Tables.Add(dataTable);
        //                } while (!result.IsClosed && result.NextResult());
        //            }

        //            await _dbContext.Database.CloseConnectionAsync();
        //            return dataSet;
        //        }
        //}


        public async Task<DataSet> ExecSPToDataSet(string storedProcedure, Dictionary<string, object>? parameters, CancellationToken cancellationToken = default)
        {
            var dataSet = new DataSet();

            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = storedProcedure;
                command.CommandType = CommandType.StoredProcedure;

                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var param in parameters)
                    {
                        var npgsqlParam = new NpgsqlParameter(param.Key, param.Value ?? DBNull.Value);
                        command.Parameters.Add(npgsqlParam);
                    }
                }

                await _dbContext.Database.OpenConnectionAsync(cancellationToken);

                using (var result = await command.ExecuteReaderAsync(cancellationToken))
                {
                    do
                    {
                        var dataTable = new DataTable();
                        dataTable.Load(result);
                        dataSet.Tables.Add(dataTable);
                    } while (!result.IsClosed && await result.NextResultAsync(cancellationToken));
                }

                await _dbContext.Database.CloseConnectionAsync();
                return dataSet;
            }
        }

        // For DataSet

        //var parameters = new Dictionary<string, object>
        //{
        //    { "Name", "John" },
        //    { "Age", 30 }
        //};
        //var dataSet = ExecSPToDataSet("GetDynamicProductsMultipleResults",parameters);
        //foreach (DataTable table in dataSet.Tables)
        //{
        //    Console.WriteLine($"Table: {table.TableName}");
        //    foreach (DataRow row in table.Rows)
        //    {
        //        Console.WriteLine($"{row["Id"]}, {row["Name"]}, {row["Price"]}");
        //    }
        //}

        //public async Task <List<DataTable>> ExecSPToDataTables(string storedProcedureName, Dictionary<string, object>? parameters)
        //{
        //    var dataTables = new List<DataTable>();
        //        using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
        //        {
        //            command.CommandText = storedProcedureName;
        //            command.CommandType = CommandType.StoredProcedure;
        //            if (parameters != null && parameters.Count > 0)
        //            {
        //                foreach (var param in parameters)
        //                {
        //                    var sqlParam = new SqlParameter(param.Key, param.Value ?? DBNull.Value);
        //                    command.Parameters.Add(sqlParam);
        //                }
        //            }

        //            await _dbContext.Database.OpenConnectionAsync();

        //            using (var result = await command.ExecuteReaderAsync())
        //            {
        //                do
        //                {
        //                    var dataTable = new DataTable();
        //                    dataTable.Load(result); // Load the current result set into a DataTable  
        //                    dataTables.Add(dataTable); // Add the DataTable to the list  
        //                } while (await result.NextResultAsync()); // Move to the next result set  
        //            }
        //            await _dbContext.Database.CloseConnectionAsync();
        //            return dataTables;
        //        }
        //}


        public async Task<List<DataTable>> ExecSPToDataTables(string storedProcedureName, Dictionary<string, object>? parameters, CancellationToken cancellationToken = default)
        {
            var dataTables = new List<DataTable>();

            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = storedProcedureName;
                command.CommandType = CommandType.StoredProcedure;

                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var param in parameters)
                    {
                        var npgsqlParam = new NpgsqlParameter(param.Key, param.Value ?? DBNull.Value);
                        command.Parameters.Add(npgsqlParam);
                    }
                }

                await _dbContext.Database.OpenConnectionAsync(cancellationToken);

                using (var result = await command.ExecuteReaderAsync(cancellationToken))
                {
                    do
                    {
                        var dataTable = new DataTable();
                        dataTable.Load(result);
                        dataTables.Add(dataTable);
                    } while (await result.NextResultAsync(cancellationToken));
                }

                await _dbContext.Database.CloseConnectionAsync();
                return dataTables;
            }
        }

        // For retrieving DataTables
        //var parameters = new Dictionary<string, object>
        //{
        //    { "Name", "John" },
        //    { "Age", 30 }
        //};
        //var tables = ExecSPToDataTables("GetMultipleResults", parameters);
        //foreach (var dt in tables)
        //{
        //    Console.WriteLine($"Table: {dt.TableName}");
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        // Adjust indices according to your expected columns
        //        Console.WriteLine($"{row["Id"]}, {row["Name"]}, {row["Price"]}");
        //    }
        //}

        //public async Task<DataSet> ExecSPToDataSets(string storedProcedureName, Dictionary<string, object>? parameters)
        //{
        //    var dataSets = new DataSet();
        //        using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
        //        {
        //            command.CommandText = storedProcedureName;
        //            command.CommandType = CommandType.StoredProcedure;
        //            if (parameters != null && parameters.Count > 0)
        //            {
        //                foreach (var param in parameters)
        //                {
        //                    var sqlParam = new SqlParameter(param.Key, param.Value ?? DBNull.Value);
        //                    command.Parameters.Add(sqlParam);
        //                }
        //            }
        //            await _dbContext.Database.OpenConnectionAsync();

        //            using (var result = await command.ExecuteReaderAsync())
        //            {
        //                do
        //                {
        //                    var dataTable = new DataTable();
        //                    dataTable.Load(result); // Load the current result set into a DataTable  
        //                    dataSets.Tables.Add(dataTable); // Add the DataTable to the DataSet  
        //                } while (await result.NextResultAsync()); // Move to the next result set  
        //            }
        //            await _dbContext.Database.CloseConnectionAsync();
        //            return dataSets;
        //        }
        //}


        public async Task<DataSet> ExecSPToDataSets(string storedProcedureName, Dictionary<string, object>? parameters, CancellationToken cancellationToken = default)
        {
            var dataSet = new DataSet();

            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = storedProcedureName;
                command.CommandType = CommandType.StoredProcedure;

                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var param in parameters)
                    {
                        var npgsqlParam = new NpgsqlParameter(param.Key, param.Value ?? DBNull.Value);
                        command.Parameters.Add(npgsqlParam);
                    }
                }

                await _dbContext.Database.OpenConnectionAsync(cancellationToken);

                using (var result = await command.ExecuteReaderAsync(cancellationToken))
                {
                    do
                    {
                        var dataTable = new DataTable();
                        dataTable.Load(result);
                        dataSet.Tables.Add(dataTable);
                    } while (await result.NextResultAsync(cancellationToken));
                }

                await _dbContext.Database.CloseConnectionAsync();
                return dataSet;
            }
        }


        // For retrieving DataTables
        //var parameters = new Dictionary<string, object>
        //{
        //    { "Name", "John" },
        //    { "Age", 30 }
        //};
        //var dataSet = ExecSPToDataSets("GetMultipleResults", parameters);
        //foreach (DataTable table in dataSet.Tables)
        //{
        //    Console.WriteLine($"Table: {table.TableName}");
        //    foreach (DataRow row in table.Rows)
        //    {
        //        // Adjust indices according to your expected columns
        //        Console.WriteLine($"{row["Id"]}, {row["Name"]}, {row["Price"]}");
        //    }
        //}


        //public async Task<bool> SQLBulkDataTable(string _tableName, DataTable? _Data)
        //{
        //        var cancellationToken = new CancellationTokenSource().Token;

        //        try
        //        {
        //            await _dbContext.Database.OpenConnectionAsync(cancellationToken);

        //            using (var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken))
        //            {
        //                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_dbContext.Database.GetDbConnection().ConnectionString, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction))
        //                {
        //                    bulkCopy.DestinationTableName = _tableName;
        //                    bulkCopy.EnableStreaming = true;
        //                    bulkCopy.BatchSize = 5000;
        //                    bulkCopy.BulkCopyTimeout = 300;

        //                    //bulkCopy.NotifyAfter = 50000;
        //                    // Assuming _Data is a DataTable or IDataReader
        //                    await Task.Run(() => bulkCopy.WriteToServer(_Data), cancellationToken);
        //                }

        //                await transaction.CommitAsync(cancellationToken); // Commit the transaction to release the lock
        //            }

        //            await _dbContext.Database.CloseConnectionAsync();
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            return false;
        //            throw new Exception(ex.Message);
        //        }
        //        finally
        //        {
        //            await _dbContext.Database.CloseConnectionAsync();
        //        }

        //}


        public async Task<bool> PostgresBulkInsertAsync(string tableName, DataTable? data, CancellationToken cancellationToken = default)
        {
            if (data == null || data.Rows.Count == 0)
                return false;

            try
            {
                await _dbContext.Database.OpenConnectionAsync(cancellationToken);

                using var writer = _dbContext.Database.GetDbConnection() as NpgsqlConnection;
                using var importer = writer!.BeginBinaryImport($"COPY {tableName} ({string.Join(", ", data.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}) FROM STDIN (FORMAT BINARY)");

                foreach (DataRow row in data.Rows)
                {
                    importer.StartRow();
                    foreach (var item in row.ItemArray)
                    {
                        importer.Write(item ?? DBNull.Value);
                    }
                }

                await importer.CompleteAsync(cancellationToken);
                await _dbContext.Database.CloseConnectionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _dbContext.Database.CloseConnectionAsync();
                throw new Exception($"Bulk insert failed: {ex.Message}", ex);
            }
        }

    }
}
