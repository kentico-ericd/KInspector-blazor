using System.Collections.Generic;

namespace KInspector.Core.Models
{
    public class TableResult<T>
    {
        public string Name { get; set; }

        public IEnumerable<T> Rows { get; set; }
    }
}