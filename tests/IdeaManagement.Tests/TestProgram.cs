using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaManagement.Tests;

public class TestProgram
{
    public static void Main(string[] args)
    {
        if (args.Length > 0 && args[0].StartsWith("--connection="))
        {
            var connectionString = args[0].Substring("--connection=".Length);
            TestHelper.SetConnectionString(connectionString);
        }
    }
}
