using System;
using System.IO;
using System.Linq;
using SwimDemo.Tests;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
class Program
{
    static int Main(string[] args)
    {
        int passed = 0, failed = 0, scripts = 0;
        foreach (var scenario in MovementScenarios.Cases)
        {
            try { scenario.Value(); passed++; Console.WriteLine("PASS " + scenario.Key); }
            catch (Exception e) { failed++; Console.WriteLine("FAIL " + scenario.Key + ": " + e.Message); }
        }
        if (args.Length != 1) { Console.Error.WriteLine("Pass the path to SwimDemo/Assets."); return 2; }
        foreach (var file in Directory.GetFiles(args[0], "*.cs", SearchOption.AllDirectories))
        {
            scripts++;
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file), new CSharpParseOptions(LanguageVersion.CSharp9,
                preprocessorSymbols: new[] { "UNITY_EDITOR", "UNITY_6000_0_OR_NEWER" }), file);
            foreach (var error in tree.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error))
            { failed++; Console.WriteLine(error); }
        }
        Console.WriteLine($"{passed} movement scenarios passed; {scripts} C# files parsed; {failed} failures. Unity API compilation, shader compilation, Play Mode and Windows build still require Unity.");
        return failed == 0 ? 0 : 1;
    }
}
