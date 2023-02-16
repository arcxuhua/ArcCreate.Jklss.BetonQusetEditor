using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Model.Data
{
    public class SelectDataWindowModel
    {
        public string SearchText { get; set; }

        public List<GridData> Data { get; set; }

        public string CreateName { get; set; }

        public string FilePath { get; set; }
    }

    public class GridData
    {
        public int Code { get; set; }

        public string Name { get; set; }

        public string FilePath { get; set; }

        public DateTime CreateDate { get;set; }

        public DateTime UpdateData { get; set; }
    }
}
