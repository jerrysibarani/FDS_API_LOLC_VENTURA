
using Npgsql;
using System.Data;

namespace API.Helpers
{
    public static class DbParameterHelper
    {
        // 🔧 Helper Methods

        //public static Dictionary<string, object> CreateUserParameters(string userId, string clientCode, string customerCode, DateTime startDate, DateTime endDate)
        //{
        //    return new Dictionary<string, object>
        //    {
        //        { "UserId", userId },
        //        { "ClientCode", clientCode },
        //        { "CustomerCode", customerCode },
        //        { "StartDate", startDate },
        //        { "EndDate", endDate }
        //    };
        //}
        //public static SqlParameter[] CreateParameters(string clientCode, string customerCode, DateTime startDate, DateTime endDate)
        //{
        //    return new[]
        //    {
        //        new SqlParameter("@ClientCode", clientCode),
        //        new SqlParameter("@CustomerCode", customerCode),
        //        new SqlParameter("@StartDate", startDate ),
        //        new SqlParameter("@EndDate", endDate )
        //    };

        //}


        public static NpgsqlParameter[] CreateParameters(string clientCode, string customerCode, DateTime startDate, DateTime endDate)
        {
            return new[]
            {
                new NpgsqlParameter("ClientCode", clientCode),
                new NpgsqlParameter("CustomerCode", customerCode),
                new NpgsqlParameter("StartDate", startDate),
                new NpgsqlParameter("EndDate", endDate)
            };
        }

        public static NpgsqlParameter[] CreateUserParameters(string userId, string clientCode, string customerCode, DateTime startDate, DateTime endDate)
        {
            return new[]
            {
                new NpgsqlParameter("UserId", userId),
                new NpgsqlParameter("ClientCode", clientCode),
                new NpgsqlParameter("CustomerCode", customerCode),
                new NpgsqlParameter("StartDate", startDate),
                new NpgsqlParameter("EndDate", endDate)
            };
        }


        public static Dictionary<string, object> CreatePagingParameters(string userId, string clientCode, string customerCode, int startTake, int endTake, string? searchColumn, string? searchValue)
        {
            return new Dictionary<string, object>
            {
                { "UserId", userId },
                { "ClientCode", clientCode },
                { "CustomerCode", customerCode },
                { "StartTake", startTake },
                { "EndTake", endTake },
                { "SearchColumn", ToDbValue(searchColumn)  },
                { "SearchValue", ToDbValue(searchValue)}
            };
        }

        //public static SqlParameter[] CreateTotalParameters(string userId, string clientCode, string customerCode, string? searchColumn, string? searchValue)
        //{
        //    return new[]
        //    {
        //        new SqlParameter("@UserId", userId),
        //        new SqlParameter("@ClientCode", clientCode),
        //        new SqlParameter("@CustomerCode", customerCode),
        //        new SqlParameter("@SearchColumn", SqlDbType.NVarChar) { Value = ToDbValue(searchColumn) },
        //        new SqlParameter("@SearchValue", SqlDbType.NVarChar) { Value = ToDbValue(searchValue) }
        //    };
        //}

        public static NpgsqlParameter[] CreateTotalParameters(string userId, string clientCode, string customerCode, string? searchColumn, string? searchValue)
        {
            return new[]
            {
                new NpgsqlParameter("UserId", userId),
                new NpgsqlParameter("ClientCode", clientCode),
                new NpgsqlParameter("CustomerCode", customerCode),
                new NpgsqlParameter("SearchColumn", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ToDbValue(searchColumn) },
                new NpgsqlParameter("SearchValue", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ToDbValue(searchValue) }
            };
        }


        /// <summary>
        /// Converts a nullable string to a valid SQL parameter value.
        /// </summary>
        private static object ToDbValue(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? DBNull.Value : value!;
        }

        /// <summary>
        /// Converts a nullable int to a valid SQL parameter value.
        /// </summary>
        private static object ToDbValue(int? value)
        {
            return value.HasValue ? value.Value : DBNull.Value;
        }

        /// <summary>
        /// Converts a nullable DateTime to a valid SQL parameter value.
        /// </summary>
        private static object ToDbValue(DateTime? value)
        {
            return value.HasValue ? value.Value : DBNull.Value;
        }

        /// <summary>
        /// Converts a nullable bool to a valid SQL parameter value.
        /// </summary>
        private static object ToDbValue(bool? value)
        {
            return value.HasValue ? value.Value : DBNull.Value;
        }

        /// <summary>
        /// Converts any nullable struct to a valid SQL parameter value.
        /// </summary>
        private static object ToDbValue<T>(T? value) where T : struct
        {
            return value.HasValue ? value.Value : DBNull.Value;
        }

    }
}
