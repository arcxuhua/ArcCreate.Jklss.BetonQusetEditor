using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ArcCreate.Jklss.Model.ClientModel
{
    public class LoginModel
    {
        public string UserName { get; set; }

        public string PassWord { get; set; }

        public string WorryMessage { get; set; }

        public string Eyes { get; set; } = "Eye";

        public bool RememberPassword { get; set; } = false;
    }

    public class UserLoginModel
    {
        public string UserName { get; set; }

        public string PassWord { get; set; }

        public string ComputerInfo { get; set; }
    }

    public class RememberPassword
    {
        public bool AutoLogin { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
