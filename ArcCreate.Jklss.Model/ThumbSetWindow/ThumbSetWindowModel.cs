using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Model.ThumbSetWindow
{
    public class ThumbSetWindowModel
    {
        public bool IsEnabel { get; set; }

        public bool IsNegate { get; set; }

        public bool UseItem { get; set; } = false;

        public string ClassificationsSeleted { get; set; }

        public string TermsSeleted { get; set; }

        public string ItemNum { get; set; }

        public List<string> Classifications { get; set; }

        public List<string> Terms { get; set; }
    }
}
