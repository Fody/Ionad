#if(DEBUG)

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ApprovalTests;
using Microsoft.Build.Utilities;
using NUnit.Framework;

[TestFixture]
public class ApprovedTests
{
    [Test]
    public void ClassWithBrokenReplacement()
    {
        Approvals.Verify(Decompile(AssemblyWeaver.AfterAssemblyPath, "ClassWithBrokenReplacement"));
    }

    [Test]
    public void ClassWithDateTime()
    {
        Approvals.Verify(Decompile(AssemblyWeaver.AfterAssemblyPath, "ClassWithDateTime"));
    }

    [Test]
    public void ClassWithGenericMethodUsage()
    {
        Approvals.Verify(Decompile(AssemblyWeaver.AfterAssemblyPath, "ClassWithGenericMethodUsage"));
    }

    [Test]
    public void ClassWithGenericUsage()
    {
        Approvals.Verify(Decompile(AssemblyWeaver.AfterAssemblyPath, "ClassWithGenericUsage"));
    }

    private static string Decompile(string assemblyPath, string identifier)
    {
        var exePath = GetPathToILDasm();

        if (!string.IsNullOrEmpty(identifier))
            identifier = "/item:" + identifier;

        using (var process = Process.Start(new ProcessStartInfo(exePath, "\"" + assemblyPath + "\" /text /linenum " + identifier)
        {
            RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true
        }))
        {
            var projectFolder = Path.GetFullPath(Path.GetDirectoryName(assemblyPath) + "\\..\\..\\..").Replace("\\", "\\\\");
            projectFolder = Char.ToLower(projectFolder[0]) + projectFolder.Substring(1) + "\\\\";

            process.WaitForExit(10000);
            return string.Join(Environment.NewLine,
                Regex.Split(process.StandardOutput.ReadToEnd(), Environment.NewLine)
                    .Where(l => !l.StartsWith("// Image base:") && !l.StartsWith("//  Microsoft (R) .NET Framework IL Disassembler.  Version"))
                    .Select(l => l.Replace(projectFolder, ""))
                );
        }
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