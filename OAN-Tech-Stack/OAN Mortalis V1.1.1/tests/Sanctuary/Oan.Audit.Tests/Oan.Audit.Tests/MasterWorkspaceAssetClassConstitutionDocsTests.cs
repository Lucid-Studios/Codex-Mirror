namespace San.Audit.Tests;

public sealed class MasterWorkspaceAssetClassConstitutionDocsTests
{
    [Fact]
    public void Master_Workspace_Asset_Class_Posture_Is_Aligned_Across_Root_And_Lines()
    {
        var repoRoot = GetRepoRoot();
        var constitutionPath = Path.Combine(repoRoot, "docs", "governance", "MASTER_WORKSPACE_ASSET_CLASS_CONSTITUTION.md");
        var labReadmePath = Path.Combine(repoRoot, "Lab SaaS Assets", "README.md");
        var rootReadmePath = Path.Combine(repoRoot, "README.md");
        var agentsPath = Path.Combine(repoRoot, "AGENTS.md");
        var v111ReadinessPath = Path.Combine(repoRoot, "OAN Mortalis V1.1.1", "docs", "BUILD_READINESS.md");
        var v121ReadinessPath = Path.Combine(repoRoot, "OAN Mortalis V1.2.1", "docs", "BUILD_READINESS.md");
        var rootAtlasBoundaryPath = Path.Combine(repoRoot, "OAN Mortalis V1.2.1", "docs", "ROOTATLAS_REMOTE_SOURCE_BOUNDARY.md");
        var sanctuaryGelBootstrapPath = Path.Combine(repoRoot, "OAN Mortalis V1.2.1", "docs", "SANCTUARY_GEL_BOOTSTRAP_LAW.md");

        var constitutionText = File.ReadAllText(constitutionPath);
        var labReadmeText = File.ReadAllText(labReadmePath);
        var rootReadmeText = File.ReadAllText(rootReadmePath);
        var agentsText = File.ReadAllText(agentsPath);
        var v111ReadinessText = File.ReadAllText(v111ReadinessPath);
        var v121ReadinessText = File.ReadAllText(v121ReadinessPath);
        var rootAtlasBoundaryText = File.ReadAllText(rootAtlasBoundaryPath);
        var sanctuaryGelBootstrapText = File.ReadAllText(sanctuaryGelBootstrapPath);
        var normalizedV111ReadinessText = NormalizeWhitespace(v111ReadinessText);
        var normalizedV121ReadinessText = NormalizeWhitespace(v121ReadinessText);
        var normalizedRootAtlasBoundaryText = NormalizeWhitespace(rootAtlasBoundaryText);
        var normalizedSanctuaryGelBootstrapText = NormalizeWhitespace(sanctuaryGelBootstrapText);

        Assert.Contains("LabFormationAsset", constitutionText, StringComparison.Ordinal);
        Assert.Contains("DerivedPayload", constitutionText, StringComparison.Ordinal);
        Assert.Contains("RuntimeAdmittedSubstrate", constitutionText, StringComparison.Ordinal);
        Assert.Contains("PublicReadableDerivative", constitutionText, StringComparison.Ordinal);
        Assert.Contains("LabFormationAsset -> DerivedPayload -> RuntimeAdmittedSubstrate", constitutionText, StringComparison.Ordinal);
        Assert.Contains("`Sanctuary.GEL` is therefore:", constitutionText, StringComparison.Ordinal);
        Assert.Contains("`RuntimeAdmittedSubstrate`", constitutionText, StringComparison.Ordinal);
        Assert.Contains("`RootAtlas` is therefore:", constitutionText, StringComparison.Ordinal);
        Assert.Contains("`LabFormationAsset`", constitutionText, StringComparison.Ordinal);
        Assert.Contains("access `Lab SaaS Assets/` only through configured, bounded target classes", constitutionText, StringComparison.Ordinal);

        Assert.Contains("docs/governance/MASTER_WORKSPACE_ASSET_CLASS_CONSTITUTION.md", rootReadmeText, StringComparison.Ordinal);
        Assert.Contains("master-workspace lab/source surface", rootReadmeText, StringComparison.Ordinal);
        Assert.Contains("Lab SaaS Assets/", agentsText, StringComparison.Ordinal);
        Assert.Contains("master-workspace lab/source surface", agentsText, StringComparison.Ordinal);
        Assert.Contains("MASTER_WORKSPACE_ASSET_CLASS_CONSTITUTION.md", labReadmeText, StringComparison.Ordinal);
        Assert.Contains("LabFormationAsset", labReadmeText, StringComparison.Ordinal);

        Assert.Contains("MASTER_WORKSPACE_ASSET_CLASS_CONSTITUTION.md", v111ReadinessText, StringComparison.Ordinal);
        Assert.Contains("must not claim direct `RootAtlas` residency", normalizedV111ReadinessText, StringComparison.Ordinal);
        Assert.Contains("must not claim direct `RootAtlas` residency", normalizedV121ReadinessText, StringComparison.Ordinal);
        Assert.Contains("`RootAtlas` lives only on Research Servers.", rootAtlasBoundaryText, StringComparison.Ordinal);
        Assert.Contains("Those payloads hydrate `Sanctuary.GEL`, which is the first lawful local substrate.", normalizedRootAtlasBoundaryText, StringComparison.Ordinal);
        Assert.Contains("The first lawful local substrate is `Sanctuary.GEL`.", normalizedSanctuaryGelBootstrapText, StringComparison.Ordinal);
        Assert.Contains("Templates, first knowledge bases, and first local engram-bearing stores are", normalizedSanctuaryGelBootstrapText, StringComparison.Ordinal);
    }

    private static string NormalizeWhitespace(string value)
    {
        var withoutBlockQuoteMarkers = value.Replace(">", " ");
        return System.Text.RegularExpressions.Regex.Replace(withoutBlockQuoteMarkers, "\\s+", " ").Trim();
    }

    private static string GetRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null &&
               (!File.Exists(Path.Combine(current.FullName, "build.ps1")) ||
                !File.Exists(Path.Combine(current.FullName, "README.md"))))
        {
            current = current.Parent;
        }

        return current?.FullName ?? throw new InvalidOperationException("Unable to locate repository root.");
    }
}
