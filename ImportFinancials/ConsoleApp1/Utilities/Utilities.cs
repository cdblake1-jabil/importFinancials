﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Utilities
{
    class Utilities
    {
        public static void ExecuteCommand(string command)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", command);
            Process process;

            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;

            process = Process.Start(processInfo);
            process.WaitForExit();
        }
        public static string GenerateReport<T>(List<T> items) where T : class
        {
            var output = "";
            var delimiter = ";";

            var properties = typeof(T).GetProperties()
             .Where(n =>
             n.PropertyType == typeof(string)
             || n.PropertyType == typeof(bool)
             || n.PropertyType == typeof(char)
             || n.PropertyType == typeof(byte)
             || n.PropertyType == typeof(decimal)
             || n.PropertyType == typeof(int)
             || n.PropertyType == typeof(DateTime)
             || n.PropertyType == typeof(DateTime?));

            using (StringWriter sw = new StringWriter())
            {
                var header = properties
                .Select(n => n.Name)
                .Aggregate((a, b) => a + delimiter + b);

                sw.WriteLine(header);

                foreach (var item in items)
                {
                    var row = properties
                    .Select(n => n.GetValue(item, null))
                    .Select(n => n == null ? "null" : n.ToString())
                    .Aggregate((a, b) => a + delimiter + b);

                    sw.WriteLine(row);
                }

                output = sw.ToString();
            }

            return output;
        }
    }
}
