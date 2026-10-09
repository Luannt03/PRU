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
        if(args.Length!=1) return 2;
        int passed=0,failed=0,scripts=0;
        foreach(var test in RegressionScenarios.Cases.Concat(QuantityScenarios.Cases))
        { try { test.Value(); passed++; Console.WriteLine("PASS "+test.Key); } catch(Exception e) { failed++; Console.WriteLine("FAIL "+test.Key+": "+e.Message); } }
        foreach(string file in Directory.GetFiles(args[0],"*.cs",SearchOption.AllDirectories))
        {
            scripts++; var tree=CSharpSyntaxTree.ParseText(File.ReadAllText(file),new CSharpParseOptions(LanguageVersion.CSharp9,preprocessorSymbols:new[]{"UNITY_EDITOR","UNITY_6000_0_OR_NEWER"}),file);
            foreach(var error in tree.GetDiagnostics().Where(d=>d.Severity==DiagnosticSeverity.Error)) { failed++;Console.WriteLine(error); }
        }
        Console.WriteLine($"{passed} production logic cases passed; {scripts} C# files parsed; {failed} failures. Unity API/shader compilation, Play Mode and Windows build require Unity Editor.");
        return failed==0?0:1;
    }
}
