#nullable enable


namespace DTAClient.Domain.Multiplayer.CnCNet;

/// <summary>
/// The status of connection/negotiation between two players.
/// </summary>
public enum NegotiationStatus
{
    NotStarted,
    InProgress,
    Succeeded,
    Failed
}

internal static class NegotiationStatusExtensions
{
    /// <summary>
    /// A human-readable, lower-case description of the status, used in lobby notices.
    /// </summary>
    public static string GetDescription(this NegotiationStatus status) => status switch
    {
        NegotiationStatus.InProgress => "in progress",
        NegotiationStatus.Succeeded => "succeeded",
        NegotiationStatus.Failed => "failed",
        _ => "not started"
    };
}
