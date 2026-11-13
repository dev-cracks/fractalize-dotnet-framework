using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Efc.Fractalize.Ddd;

namespace Efc.Fractalize.Architecture.Tests;

/// <summary>
/// The 90 % coverage gate of Efc.Fractalize.Ddd must measure every line of the package. Coverlet silently drops
/// code marked with attributes such as <c>[ExcludeFromCodeCoverage]</c> (or any attribute named
/// <c>ExcludeFrom*Coverage</c>), so this test checks the result instead of the source: every source file with
/// executable code in the package PDB (other than members synthesized by the compiler) must appear in the
/// Cobertura report of the gate.
/// </summary>
/// <remarks>
/// CI sets <c>DDD_COVERAGE_REPORT</c> to the folder of that report. Without it, the test only checks that the
/// PDB can be read, so a normal <c>dotnet test</c> still passes.
/// </remarks>
public class DddCoverageCompletenessTest
{
    private const string ProjectFolder = "Efc.Fractalize.Ddd/";

    [Fact]
    public void EverySourceFileWithCode_AppearsInTheDddCoverageReport()
    {
        var withCode = SourceFilesWithCode(typeof(EntityBase<>).Assembly);
        Assert.Contains("EntityBase.cs", withCode);

        var reportFolder = Environment.GetEnvironmentVariable("DDD_COVERAGE_REPORT");
        if (string.IsNullOrWhiteSpace(reportFolder))
        {
            return;
        }

        var reports = Directory.GetFiles(reportFolder, "coverage.cobertura.xml", SearchOption.AllDirectories);
        Assert.NotEmpty(reports);
        var reported = reports.SelectMany(ReportedFiles).ToHashSet(StringComparer.Ordinal);

        var missing = withCode.Except(reported, StringComparer.Ordinal).ToArray();
        Assert.Empty(missing);
    }

    /// <summary>Source files of the project (relative to its folder) with at least one visible sequence point.</summary>
    private static SortedSet<string> SourceFilesWithCode(Assembly assembly)
    {
        var pdb = Path.ChangeExtension(assembly.Location, ".pdb");
        using var stream = File.OpenRead(pdb);
        using var provider = MetadataReaderProvider.FromPortablePdbStream(stream);
        var reader = provider.GetMetadataReader();

        // Members the compiler synthesizes (record equality, ToString...) are not measured by coverlet.
        // A [CompilerGenerated] written by hand is rejected by the source check of ci.yml.
        var files = reader.MethodDebugInformation
            .Where(handle => assembly.ManifestModule.ResolveMethod(
                MetadataTokens.GetToken(handle.ToDefinitionHandle())) is { } method && !IsCompilerGenerated(method))
            .SelectMany(handle => reader.GetMethodDebugInformation(handle).GetSequencePoints())
            .Where(point => !point.IsHidden)
            .Select(point => ToProjectRelative(reader.GetString(reader.GetDocument(point.Document).Name)))
            .OfType<string>();

        return new SortedSet<string>(files, StringComparer.Ordinal);
    }

    private static bool IsCompilerGenerated(MethodBase method) =>
        method.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false)
        || (method.DeclaringType?.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false) ?? false);

    private static IEnumerable<string> ReportedFiles(string report)
    {
        var root = XDocument.Load(report).Root!;
        var sources = root.Descendants("source").Select(source => source.Value).ToArray();
        return root.Descendants("class")
            .Select(cls => (string?)cls.Attribute("filename") ?? "")
            .SelectMany(filename => Path.IsPathRooted(filename) || sources.Length == 0
                ? [filename]
                : sources.Select(source => Path.Join(source, filename)))
            .Select(ToProjectRelative)
            .OfType<string>();
    }

    /// <summary>Path relative to the project folder, or null for files outside it or generated under obj/.</summary>
    private static string? ToProjectRelative(string path)
    {
        var normalised = path.Replace('\\', '/');
        var index = normalised.LastIndexOf(ProjectFolder, StringComparison.Ordinal);
        if (index < 0)
        {
            return null;
        }

        var relative = normalised[(index + ProjectFolder.Length)..];
        return relative.StartsWith("obj/", StringComparison.Ordinal) || relative.StartsWith("bin/", StringComparison.Ordinal)
            ? null
            : relative;
    }
}
