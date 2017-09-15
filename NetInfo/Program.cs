using System;
using System.Text;
using System.Net.NetworkInformation;
using System.IO;
using System.Text.RegularExpressions;


namespace NetInfo
{
    class NetInfo
    {
        static void Main(string[] args)
        {
            string Name = args[0];
            string LogFileName = "NetInfo.csv";
            bool writeheader = true;
            if (File.Exists(LogFileName))
                writeheader = false;

            FileStream logfs = new FileStream(LogFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter logfile = new StreamWriter(logfs);

            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            StringBuilder sb = new StringBuilder();
            string recordHeader = sb.AppendFormat("{0},{1},{2}",Name,computerProperties.HostName, DateTime.Now.ToShortDateString()).ToString();
            if (writeheader)
                logfile.Write("Name,Hostname,Date,Adapter Description,Interface Type,MAC Address,Status,IP Addresses\n");
            if (nics == null || nics.Length < 1)
            {
                logfile.Write(recordHeader + "  No network interfaces found.\n");
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
        }
    }
}

