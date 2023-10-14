using ArcCreate.Jklss.Model.MainWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Model.Data
{
    public class DataCheckInfoModel
    {
        public string CardName { get; set; }

        public ThumbClass CardClass { get; set; }

        public CheckInfoLevel CheckInfoLevel { get; set; }

        public string Message { get; set; }

        public object Backs { get; set; }
    }

    public enum CheckInfoLevel
    {
        error,
        worry,
    }
}
