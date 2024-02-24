using Generator.Common.Models;

namespace Generator.SqlServer;

public interface IQueries
{
	Task<IEnumerable<TableDetails>> GetTableDetailsAsync();
	Task<IEnumerable<ColumnDetails>> GetColumnDetailsAsync(TableDetails tableDetails);
	void Dispose();
}