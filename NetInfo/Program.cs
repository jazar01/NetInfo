using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.IO;

namespace NetInfo
{
    class NetInfo
    {
        static void Main(string[] args)
        {
            string Name;
            bool Interactive = false;
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                Console.Write("Enter a name: ");
                Name = Console.ReadLine();
                Interactive = true;
            }
            else
                 Name = args[0];

            string LogFileName = "NetInfo.csv";
            bool writeFileHeader = true;
            if (File.Exists(LogFileName))
                writeFileHeader = false;

            FileStream logfs;
            StreamWriter logfile;
            try
            {
                logfs = new FileStream(LogFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                logfile = new StreamWriter(logfs);
            }
            catch (Exception e)
            {
                Console.WriteLine("   Error opening "+ LogFileName );
                Console.WriteLine("   " + e.Message);
                return;
            }
                
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            StringBuilder sb = new StringBuilder();
            string recordHeader = sb.AppendFormat("{0},{1},{2}",Name,computerProperties.HostName, DateTime.Now.ToShortDateString()).ToString();

            if (writeFileHeader)
                logfile.Write("Name,Hostname,Date,Adapter Description,Interface Type,MAC Address,Status,IP Addresses...\n");

            if (nics == null || nics.Length < 1)
            {
                Console.WriteLine(recordHeader + "  No network interfaces found.");
                logfile.Close();
                return;
            }

            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                logfile.Write("{0},{1},{2},{3},{4}",
                    recordHeader,
                    adapter.Description,
                    adapter.NetworkInterfaceType,
                    Regex.Replace(adapter.GetPhysicalAddress().ToString(), "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})",  "$1:$2:$3:$4:$5:$6"),
                    adapter.OperationalStatus
                    );

                UnicastIPAddressInformationCollection uniCast = properties.UnicastAddresses;
                if (uniCast != null)
                    foreach (UnicastIPAddressInformation uni in uniCast)
                        if (uni.Address.ToString().Contains("."))
                               logfile.Write(",{0}", uni.Address);

                logfile.Write("\n");
            }

            logfile.Close();
            Console.WriteLine("Completed: " + nics.Length + " NICs found and recorded for " + Name);
            if (Interactive)
                {
                Console.Write("Press enter");
                Console.ReadLine();
                }

        }
    }
}

