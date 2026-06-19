using System;
using System.Linq;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;

namespace aiinspect
{
    class Program
    {
        static void Main()
        {
            var asm = typeof(ApplicationInsightsServiceOptions).Assembly;
            Console.WriteLine("ASSEMBLY: " + asm.FullName);
            foreach (var t in asm.GetTypes().Where(t => t.Name.Contains("Telemetry", StringComparison.OrdinalIgnoreCase) || t.Name.Contains("Processor", StringComparison.OrdinalIgnoreCase) || t.Name.Contains("Initializer", StringComparison.OrdinalIgnoreCase) || t.Name.Contains("Module", StringComparison.OrdinalIgnoreCase) || t.Name.Contains("Filter", StringComparison.OrdinalIgnoreCase)))
                Console.WriteLine(t.FullName);
        }
    }
}
