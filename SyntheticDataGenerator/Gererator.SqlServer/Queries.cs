using Dapper;
using Generator.Common.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Generator.SqlServer
{
	public class Queries : IQueries
	{
		private readonly IDbConnection _connection;

		public Queries(string connectionString)
		{
			_connection = new SqlConnection(connectionString);
		}

		public async Task<IEnumerable<TableDetails>> GetTableDetailsAsync()
		{
			string query = """
			               SELECT DISTINCT
			               	schemas.name as [Schema],
			               	tables.name as [Table]
			               FROM sys.schemas [schemas]
			               INNER JOIN sys.tables [tables]
			               	ON [schemas].schema_id = [tables].schema_id
			               ORDER BY [Schema], [Table]
			               """;

			return await _connection.QueryAsync<TableDetails>(query);
		}

		public async Task<IEnumerable<ColumnDetails>> GetColumnDetailsAsync(TableDetails tableDetails)
		{
			string query = """
			               WITH q AS (
			                   
			                   SELECT
			                       c.TABLE_SCHEMA,
			                       c.TABLE_NAME,
			                       c.COLUMN_NAME,
			                       c.DATA_TYPE,
			                       CASE
			                           WHEN c.DATA_TYPE IN ( N'binary', N'varbinary'                    ) THEN ( CASE c.CHARACTER_OCTET_LENGTH   WHEN -1 THEN N'(max)' ELSE CONCAT( N'(', c.CHARACTER_OCTET_LENGTH  , N')' ) END )
			                           WHEN c.DATA_TYPE IN ( N'char', N'varchar', N'nchar', N'nvarchar' ) THEN ( CASE c.CHARACTER_MAXIMUM_LENGTH WHEN -1 THEN N'(max)' ELSE CONCAT( N'(', c.CHARACTER_MAXIMUM_LENGTH, N')' ) END )
			                           WHEN c.DATA_TYPE IN ( N'datetime2', N'datetimeoffset'            ) THEN CONCAT( N'(', c.DATETIME_PRECISION, N')' )
			                           WHEN c.DATA_TYPE IN ( N'decimal', N'numeric'                     ) THEN CONCAT( N'(', c.NUMERIC_PRECISION , N',', c.NUMERIC_SCALE, N')' )
			                       END AS DATA_TYPE_PARAMETER,
			                       CASE c.IS_NULLABLE
			                           WHEN N'NO'  THEN 0
			                           WHEN N'YES' THEN 1
			                       END AS IS_NULLABLE2
			                   FROM
			                       INFORMATION_SCHEMA.COLUMNS AS c
			               )
			               SELECT
			                   q.TABLE_SCHEMA AS [Schema],
			                   q.TABLE_NAME AS [Table],
			                   q.COLUMN_NAME AS [Column],
			                   q.DATA_TYPE AS [Type],
			               	ISNULL( q.DATA_TYPE_PARAMETER, N'default' ) AS [Lenght]
			               	,q.IS_NULLABLE2 AS [Nullable]
			               
			               FROM
			                   q
			               WHERE
			                   q.TABLE_SCHEMA = @Schema AND
			                   q.TABLE_NAME   = @Table
			               
			               ORDER BY
			                  [Schema], [Table];
			               """;

			return await _connection.QueryAsync<ColumnDetails>(query, param: new { tableDetails.Schema, tableDetails.Table });
		}

		public void Dispose()
		{
			_connection.Dispose();
		}
	}
}
