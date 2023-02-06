using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Model.ClientModel
{
    public class RegisterModel
    {
        public string UserName { get; set; }

        public string WorryMessage { get; set; }

        public string PassWord { get; set; }

        public string Activation { get; set; }

        public bool RegisterEnabled { get; set; } = true;

        public string UserNameCheckIco { get; set; } = "AlertCircleOutline";

        public string UserNameChecked { get; set; } = "Red";

        public string UserNameWorry { get; set; }


        public string PasswordCheckIco { get; set; } = "AlertCircleOutline";

        public string PasswordChecked { get; set; } = "Red";

        public string PasswordWorry { get; set; }


        public string ActivationChecked { get; set; } = "Red";

        public string ActivationWorry { get; set; } 

        public string ActivationCheckIco { get; set; } = "AlertCircleOutline";
    }

    public class UserRegisterModel
    {
        public string UserName { get; set; }

        public string PassWord { get; set; }

        public string Activation { get; set; }

        public string ComputerInfo { get; set; }

        public string BackMessage { get; set; }
    }
}
