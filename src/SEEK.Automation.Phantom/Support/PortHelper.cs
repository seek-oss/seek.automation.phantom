using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace SEEK.Automation.Phantom.Support
{
    public class PortHelper
    {
        public static string ListOccupiedPorts()
        {
            var portsStatus = string.Empty;
            var ports = GetNetStatPorts();

            portsStatus += string.Format("{0,-10}{1, -60}{2, -80}{3}", "Port#", "Process Name", "Port Name", Environment.NewLine);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var port in ports)
            {
                portsStatus += string.Format("{0,-10}{1, -60}{2, -80}{3}", port.PortNumber, port.ProcessName, port.Name, Environment.NewLine);
            }

            return portsStatus;
        }

        // Thanks Cheyne, you saved me some time:
        //      http://www.cheynewallace.com/get-active-ports-and-associated-process-names-in-c/
        public static List<Port> GetNetStatPorts()
        {
            var ports = new List<Port>();

            try
            {
                using (var p = new Process())
                {

                    var ps = new ProcessStartInfo
                    {
                        Arguments = "-a -n -o",
                        FileName = "netstat.exe",
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    p.StartInfo = ps;
                    p.Start();

                    var stdOutput = p.StandardOutput;
                    var stdError = p.StandardError;

                    var content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                    var exitStatus = p.ExitCode.ToString();

                    if (exitStatus != "0")
                    {
                        // Command Errored. Handle Here If Need Be
                    }

                    var rows = Regex.Split(content, "\r\n");
                    ports.AddRange(from row in rows
                                   select Regex.Split(row, "\\s+")
                                       into tokens
                                       where tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP"))
                                       let localAddress = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1")
                                       select new Port
                                       {
                                           Protocol = localAddress.Contains("1.1.1.1") ? String.Format("{0}v6", tokens[1]) : String.Format("{0}v4", tokens[1]),
                                           PortNumber = localAddress.Split(':')[1],
                                           ProcessName = tokens[1] == "UDP" ? LookupProcess(Convert.ToInt16(tokens[4])) : LookupProcess(Convert.ToInt16(tokens[5]))
                                       });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ports;
        }

        public static string LookupProcess(int pid)
        {
            string procName;
            try
            {
                procName = Process.GetProcessById(pid).ProcessName;
            }
            catch (Exception)
            {
                procName = "-";
            }

            return procName;
        }
    }

    public class Port
    {
        public string Name
        {
            get
            {
                return string.Format("{0} ({1} port {2})", ProcessName, Protocol, PortNumber);
            }
            set { if (value == null) throw new ArgumentNullException("value"); }
        }

        public string PortNumber { get; set; }
        public string ProcessName { get; set; }
        public string Protocol { get; set; }
    }
}
