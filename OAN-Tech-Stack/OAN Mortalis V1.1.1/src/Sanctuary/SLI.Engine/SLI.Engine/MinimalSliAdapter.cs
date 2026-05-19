using System.Security.Cryptography;
using System.Text;

namespace SLI.Engine;

public enum SliAdapterStatus
{
    NeedsSpec,
    Reject,
    Admissible,
    Enrich,
    Reopen,
    DecantCandidate
}

public static class SliAdapterStatusTokens
{
    public static string ToToken(SliAdapterStatus status) =>
        status switch
        {
            SliAdapterStatus.NeedsSpec => "NEEDS_SPEC",
            SliAdapterStatus.Reject => "REJECT",
            SliAdapterStatus.Admissible => "ADMISSIBLE",
            SliAdapterStatus.Enrich => "ENRICH",
            SliAdapterStatus.Reopen => "REOPEN",
            SliAdapterStatus.DecantCandidate => "DECANT_CANDIDATE",
            _ => "REJECT"
        };
}

public sealed record SliAdapterSettings(
    bool AllowModelBridge = false);

public sealed record SliAdapterRequest(
    string? Prompt,
    IReadOnlyDictionary<string, string?>? Context = null,
    IReadOnlyList<string>? TraceHints = null,
    SliAdapterSettings? Settings = null);

public sealed record SliModelBridgeResult(
    SliAdapterStatus Status,
    string? Payload = null,
    string? Trace = null,
    IReadOnlyList<string>? Notes = null);

public interface ISliModelBridge
{
    SliModelBridgeResult? TryEvaluate(
        SliAdapterRequest request,
        string traceId);
}

public sealed record SliAdapterResponse(
    SliAdapterStatus Status,
    string StatusToken,
    string TraceId,
    string ReasonCode,
    string? Payload,
    IReadOnlyList<string> MissingFields,
    IReadOnlyList<string> Notes,
    string? BridgeTrace,
    bool CandidateOnly,
    bool Terminal);

public sealed class MinimalSliAdapter
{
    private readonly ISliModelBridge? _modelBridge;

    public MinimalSliAdapter(ISliModelBridge? modelBridge = null)
    {
        _modelBridge = modelBridge;
    }

    public SliAdapterResponse Evaluate(SliAdapterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var traceId = CreateTraceId(request);
        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            return CreateResponse(
                status: SliAdapterStatus.NeedsSpec,
                traceId: traceId,
                reasonCode: "sli-adapter-missing-prompt",
                payload: "A prompt is required before the SLI adapter can admit the request.",
                missingFields: ["prompt"],
                notes: ["missing structure is reported instead of completed"]);
        }

        var contextProblem = FindContextProblem(request.Context);
        if (contextProblem is not null)
        {
            return CreateResponse(
                status: SliAdapterStatus.Reject,
                traceId: traceId,
                reasonCode: contextProblem,
                payload: "The context object is structurally invalid.",
                notes: ["invalid context is refused before model bridge evaluation"]);
        }

        if (request.Settings?.AllowModelBridge != true)
        {
            return CreateResponse(
                status: SliAdapterStatus.Admissible,
                traceId: traceId,
                reasonCode: "sli-adapter-structured-input-admissible",
                payload: "The structured request is admissible for later governed work.",
                notes: ["model bridge evaluation was not requested"]);
        }

        var bridgeResult = _modelBridge?.TryEvaluate(request, traceId);
        if (bridgeResult is null)
        {
            return CreateResponse(
                status: SliAdapterStatus.Reopen,
                traceId: traceId,
                reasonCode: "sli-adapter-model-bridge-no-result",
                payload: "Model bridge evaluation was requested, but no bridge result was available.",
                notes: ["the request must reopen rather than inventing a model result"]);
        }

        return CreateResponse(
            status: bridgeResult.Status,
            traceId: traceId,
            reasonCode: DetermineBridgeReasonCode(bridgeResult.Status),
            payload: bridgeResult.Payload,
            notes: NormalizeNotes(bridgeResult.Notes),
            bridgeTrace: string.IsNullOrWhiteSpace(bridgeResult.Trace) ? null : bridgeResult.Trace.Trim());
    }

    private static SliAdapterResponse CreateResponse(
        SliAdapterStatus status,
        string traceId,
        string reasonCode,
        string? payload,
        IReadOnlyList<string>? missingFields = null,
        IReadOnlyList<string>? notes = null,
        string? bridgeTrace = null) =>
        new(
            Status: status,
            StatusToken: SliAdapterStatusTokens.ToToken(status),
            TraceId: traceId,
            ReasonCode: reasonCode,
            Payload: payload,
            MissingFields: missingFields ?? [],
            Notes: notes ?? [],
            BridgeTrace: bridgeTrace,
            CandidateOnly: status == SliAdapterStatus.DecantCandidate,
            Terminal: status is SliAdapterStatus.Reject or SliAdapterStatus.DecantCandidate);

    private static string? FindContextProblem(IReadOnlyDictionary<string, string?>? context)
    {
        if (context is null)
        {
            return null;
        }

        foreach (var entry in context)
        {
            if (string.IsNullOrWhiteSpace(entry.Key))
            {
                return "sli-adapter-context-key-empty";
            }

            if (entry.Value is null)
            {
                return "sli-adapter-context-value-null";
            }
        }

        return null;
    }

    private static IReadOnlyList<string> NormalizeNotes(IReadOnlyList<string>? notes)
    {
        if (notes is null)
        {
            return [];
        }

        return notes
            .Where(static note => !string.IsNullOrWhiteSpace(note))
            .Select(static note => note.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    private static string DetermineBridgeReasonCode(SliAdapterStatus status) =>
        status switch
        {
            SliAdapterStatus.NeedsSpec => "sli-adapter-bridge-needs-spec",
            SliAdapterStatus.Reject => "sli-adapter-bridge-rejected",
            SliAdapterStatus.Admissible => "sli-adapter-bridge-admissible",
            SliAdapterStatus.Enrich => "sli-adapter-bridge-enriched",
            SliAdapterStatus.Reopen => "sli-adapter-bridge-reopen",
            SliAdapterStatus.DecantCandidate => "sli-adapter-bridge-decant-candidate",
            _ => "sli-adapter-bridge-rejected"
        };

    private static string CreateTraceId(SliAdapterRequest request)
    {
        var builder = new StringBuilder();
        builder.AppendLine(request.Prompt?.Trim() ?? string.Empty);

        foreach (var entry in (request.Context ?? new Dictionary<string, string?>())
            .OrderBy(static entry => entry.Key, StringComparer.Ordinal))
        {
            builder.Append(entry.Key.Trim());
            builder.Append('=');
            builder.AppendLine(entry.Value?.Trim() ?? string.Empty);
        }

        foreach (var hint in (request.TraceHints ?? [])
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Select(static item => item.Trim())
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static item => item, StringComparer.Ordinal))
        {
            builder.Append("hint=");
            builder.AppendLine(hint);
        }

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString()));
        return $"sli-trace://{Convert.ToHexString(hash).ToLowerInvariant()[..16]}";
    }
}
