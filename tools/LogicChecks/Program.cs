using System;
using System.IO;
using System.Linq;
using ChemLab9.Tests;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
class Program
{
    static int Main(string[] args)
    {
        int passed=0,failed=0;
        foreach(var entry in RegressionScenarios.Cases)
        {
            try { entry.Value(); passed++; Console.WriteLine("PASS "+entry.Key); }
            catch(Exception e) { failed++; Console.WriteLine("FAIL "+entry.Key+": "+e.Message); }
        }
        string root=args.Length>0?args[0]:Path.GetFullPath("../../ChemLab9/Assets"); int scripts=0;
        foreach(string file in Directory.GetFiles(root,"*.cs",SearchOption.AllDirectories))
        {
            scripts++;
            var syntax=CSharpSyntaxTree.ParseText(File.ReadAllText(file),new CSharpParseOptions(LanguageVersion.CSharp9,preprocessorSymbols:new[]{"UNITY_EDITOR","UNITY_6000_0_OR_NEWER"}),file);
            foreach(var error in syntax.GetDiagnostics().Where(d=>d.Severity==DiagnosticSeverity.Error)) { failed++;Console.WriteLine(error); }
        }
        Console.WriteLine($"{passed} logic scenarios passed; {scripts} C# files parsed; {failed} failures. Unity compilation/Play Mode/build are separate checks.");
        return failed==0?0:1;
    }
}
