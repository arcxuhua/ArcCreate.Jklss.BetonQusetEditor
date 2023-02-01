using ArcCreate.Jklss.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Services
{
    public class MachineNumberService
    {
        public static string GetComputerInfo()
        {
            List<string> cpuID = new List<string>();
            List<string> zbID = new List<string>();
            List<string> ypID = new List<string>();
            List<string> biosID = new List<string>();

            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                cpuID.Add(mo.Properties["ProcessorId"].Value.ToString());
            }

            ManagementClass mcs = new ManagementClass("Win32_BaseBoard");
            ManagementObjectCollection mocs = mcs.GetInstances();
            
            foreach (ManagementObject mo in mocs)
            {
                zbID.Add(mo.Properties["SerialNumber"].Value.ToString());
            }

            ManagementClass mcss = new ManagementClass("Win32_PhysicalMedia");
            ManagementObjectCollection mocss = mcss.GetInstances();
            
            foreach (ManagementObject mo in mocss)
            {
                ypID.Add(mo.Properties["SerialNumber"].Value.ToString());
            }

            ManagementClass mcsss = new ManagementClass("Win32_BIOS");
            ManagementObjectCollection mocsss = mcsss.GetInstances();
            
            foreach (ManagementObject mo in mocsss)
            {
                biosID.Add(mo.Properties["SerialNumber"].Value.ToString());
            }

            var list = new List<MachineModel>()
            {
                new MachineModel()
                {
                    Name = "CPU",
                    IDs = cpuID
                },
                new MachineModel()
                {
                    Name = "ZB",
                    IDs = zbID
                },
                new MachineModel()
                {
                    Name = "YP",
                    IDs = ypID
                },
                new MachineModel()
                {
                    Name = "BIOS",
                    IDs = biosID
                },
            };

            var toJson = FileService.SaveToJson(list);

            return toJson;
        }
        
    }
}
