using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Model
{
    public class ReturnModel
    {
        public bool Succese { get; set; }

        public string Text { get; set; }

        public object Backs { get; set; }

        public void SetSuccese(string txt="",object obj=null)
        {
            Succese = true;
            Text = txt;
            Backs = obj;
        }

        public void SetError(string txt = "",object obj = null)
        {
            Succese = false;
            Text = txt;
            Backs = obj;
        }
    }
    public class MachineModel
    {
        public string Name { get; set; }

        public List<string> IDs { get; set; } = new List<string>();
    }
}
