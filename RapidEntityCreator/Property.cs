using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidEntityCreator
{
    class Property
    {
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public string ColumnName { get; set; }
        public string IsPrimaryKey { get; set; }
        public string IsImage { get; set; }
    }
}
