#if(DEBUG)

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.Build.Utilities;
using NUnit.Framework;

[TestFixture]
[UseReporter(typeof(DiffReporter))]
public class ApprovedTests
{
    [Test]
    public void ILDasm()
    {
        Approvals.Verify(Decompile(AssemblyWeaver.AfterAssemblyPath));
    }

    private static string Decompile(string assemblyPath)
    {
        var exePath = GetPathToILDasm();
        var process = Process.Start(new ProcessStartInfo(exePath, "\"" + assemblyPath + "\" /text /linenum")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });

        var projectFolder = Path.GetFullPath(Path.GetDirectoryName(assemblyPath) + "\\..\\..\\..").Replace("\\", "\\\\");
        projectFolder = Char.ToLower(projectFolder[0]) + projectFolder.Substring(1) + "\\\\";

        process.WaitForExit(10000);
        return string.Join(Environment.NewLine,
            Regex.Split(process.StandardOutput.ReadToEnd(), Environment.NewLine)
                .Where(l => !l.StartsWith("// Image base:"))
                .Select(l => l.Replace(projectFolder, ""))
        );
    }

    private static string GetPathToILDasm()
    {
        var path = Path.Combine(ToolLocationHelper.GetPathToDotNetFrameworkSdk(TargetDotNetFrameworkVersion.Version40), @"bin\NETFX 4.0 Tools\ildasm.exe");
        if (!File.Exists(path))
            path = path.Replace("v7.0", "v8.0");
        if (!File.Exists(path))
            Assert.Ignore("ILDasm could not be found");
        return path;
    }
}

#endif