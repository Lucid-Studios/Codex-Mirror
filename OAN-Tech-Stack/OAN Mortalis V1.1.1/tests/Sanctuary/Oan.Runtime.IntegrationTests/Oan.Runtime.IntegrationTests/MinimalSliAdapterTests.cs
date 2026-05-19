using SLI.Engine;

namespace San.Runtime.IntegrationTests;

public sealed class MinimalSliAdapterTests
{
    [Fact]
    public void Evaluate_Missing_Prompt_Returns_NeedsSpec()
    {
        var adapter = new MinimalSliAdapter();

        var response = adapter.Evaluate(new SliAdapterRequest(Prompt: " "));

        Assert.Equal(SliAdapterStatus.NeedsSpec, response.Status);
        Assert.Equal("NEEDS_SPEC", response.StatusToken);
        Assert.Equal("sli-adapter-missing-prompt", response.ReasonCode);
        Assert.Contains("prompt", response.MissingFields, StringComparer.Ordinal);
        Assert.StartsWith("sli-trace://", response.TraceId, StringComparison.Ordinal);
        Assert.False(response.CandidateOnly);
        Assert.False(response.Terminal);
    }

    [Fact]
    public void Evaluate_Invalid_Context_Returns_Reject()
    {
        var adapter = new MinimalSliAdapter();
        var request = new SliAdapterRequest(
            Prompt: "Classify this request.",
            Context: new Dictionary<string, string?>
            {
                ["scope"] = null
            });

        var response = adapter.Evaluate(request);

        Assert.Equal(SliAdapterStatus.Reject, response.Status);
        Assert.Equal("REJECT", response.StatusToken);
        Assert.Equal("sli-adapter-context-value-null", response.ReasonCode);
        Assert.True(response.Terminal);
    }

    [Fact]
    public void Evaluate_Valid_Request_Without_ModelBridge_Returns_Admissible_And_Deterministic_Trace()
    {
        var adapter = new MinimalSliAdapter();
        var request = new SliAdapterRequest(
            Prompt: "Classify this request.",
            Context: new Dictionary<string, string?>
            {
                ["domain"] = "sli",
                ["layer"] = "adapter"
            },
            TraceHints: ["trace://existing"]);

        var first = adapter.Evaluate(request);
        var second = adapter.Evaluate(request);

        Assert.Equal(SliAdapterStatus.Admissible, first.Status);
        Assert.Equal("ADMISSIBLE", first.StatusToken);
        Assert.Equal("sli-adapter-structured-input-admissible", first.ReasonCode);
        Assert.Equal(first.TraceId, second.TraceId);
        Assert.False(first.CandidateOnly);
        Assert.False(first.Terminal);
    }

    [Theory]
    [InlineData(SliAdapterStatus.Enrich, "ENRICH", "sli-adapter-bridge-enriched")]
    [InlineData(SliAdapterStatus.Reopen, "REOPEN", "sli-adapter-bridge-reopen")]
    [InlineData(SliAdapterStatus.DecantCandidate, "DECANT_CANDIDATE", "sli-adapter-bridge-decant-candidate")]
    public void Evaluate_ModelBridge_Result_Preserves_Status_And_Trace(
        SliAdapterStatus status,
        string token,
        string reasonCode)
    {
        var adapter = new MinimalSliAdapter(
            new StubSliModelBridge(
                new SliModelBridgeResult(
                    Status: status,
                    Payload: "bridge payload",
                    Trace: "bridge-trace://result",
                    Notes: ["bridge note"])));
        var request = new SliAdapterRequest(
            Prompt: "Evaluate through the bridge.",
            Context: new Dictionary<string, string?>
            {
                ["domain"] = "sli"
            },
            Settings: new SliAdapterSettings(AllowModelBridge: true));

        var response = adapter.Evaluate(request);

        Assert.Equal(status, response.Status);
        Assert.Equal(token, response.StatusToken);
        Assert.Equal(reasonCode, response.ReasonCode);
        Assert.Equal("bridge payload", response.Payload);
        Assert.Equal("bridge-trace://result", response.BridgeTrace);
        Assert.Contains("bridge note", response.Notes, StringComparer.Ordinal);
        Assert.Equal(status == SliAdapterStatus.DecantCandidate, response.CandidateOnly);
    }

    [Fact]
    public void Evaluate_ModelBridge_Allowed_Without_Bridge_Reopens_Instead_Of_Inventing_Result()
    {
        var adapter = new MinimalSliAdapter();
        var request = new SliAdapterRequest(
            Prompt: "Evaluate through a missing bridge.",
            Settings: new SliAdapterSettings(AllowModelBridge: true));

        var response = adapter.Evaluate(request);

        Assert.Equal(SliAdapterStatus.Reopen, response.Status);
        Assert.Equal("REOPEN", response.StatusToken);
        Assert.Equal("sli-adapter-model-bridge-no-result", response.ReasonCode);
        Assert.Contains("no bridge result", response.Payload, StringComparison.Ordinal);
    }

    [Fact]
    public void SliEngine_Does_Not_Directly_Reference_SanHostedLlm()
    {
        var referencedAssemblyNames = typeof(MinimalSliAdapter)
            .Assembly
            .GetReferencedAssemblies()
            .Select(static assemblyName => assemblyName.Name)
            .ToArray();

        Assert.DoesNotContain("San.HostedLlm", referencedAssemblyNames, StringComparer.Ordinal);
    }

    private sealed class StubSliModelBridge : ISliModelBridge
    {
        private readonly SliModelBridgeResult _result;

        public StubSliModelBridge(SliModelBridgeResult result)
        {
            _result = result;
        }

        public SliModelBridgeResult? TryEvaluate(
            SliAdapterRequest request,
            string traceId)
        {
            Assert.False(string.IsNullOrWhiteSpace(request.Prompt));
            Assert.StartsWith("sli-trace://", traceId, StringComparison.Ordinal);
            return _result;
        }
    }
}
