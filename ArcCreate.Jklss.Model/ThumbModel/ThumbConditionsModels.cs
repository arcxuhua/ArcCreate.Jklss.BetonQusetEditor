using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Model.ThumbModel
{
    public class ThumbsModels
    {
        public string Config { get; set; }

        public string Type { get; set; }

        public string Text { get; set; }

        public ThumbsModels(string config="",string type="",string text="")
        {
            Config = config;

            Type = type;

            Text = text;
        }
    }
}
