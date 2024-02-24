using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Common.Models
{
	public record ColumnDetails
	{
		public string Schema { get; set; }
		public string Table { get; set; }
		public string Column { get; set; }
		public string Type { get; set; }
		public string Lenght { get; set; }
		public bool Nullable { get; set; }
	}
}
