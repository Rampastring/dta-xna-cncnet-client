namespace DTAClient.Domain.Multiplayer.CnCNet;

// Upstream defaults, kept here because DTA does not use NetworkDefinitions.ini.
internal static class V3TunnelConfiguration
{
    public const int V3NonDeciderTotalTimeoutMs = 20000;
    public const int V3ConnectedPhaseTimeoutMs = 15000;
    public const int V3ConnectedPhaseTimeoutSyncedMs = 5000;
    public const int V3DeciderPingPhaseTimeoutMs = 12000;
    public const int V3PingsPerTunnel = 10;
    public const int V3PingTimeoutMs = 1000;
    public const int V3P2PPingsPerTunnel = 6;
    public const int V3P2PPingTimeoutMs = 1000;
    public const int V3NonDeciderConnectedIntervalMs = 500;
    public const int V3P2PCandidateExchangeTimeoutMs = 3000;
    public const int V3P2PCandidateSendCount = 3;
    public const int V3P2PCandidateSendIntervalMs = 150;
    public const int V3P2PUpgradeNonDeciderTimeoutMs = 10000;
    public const int V3P2PUpgradeConnectedTimeoutMs = 3000;
    public const int V3TunnelChoiceRetryIntervalMs = 1000;
    public const int V3NonDeciderAckLingerRetries = 3;
    public const int V3TunnelChoiceMaxRetries = 10;
    public const double V3EarlySelectionThreshold = 0.8;
    public const int V3PacketLossWeight = 10;
    public const double V3CurrentTunnelPingIntervalSeconds = 20.0;
    public const uint V3CyclesPerTunnelListRefresh = 3;
    public const int V3RetainedPingFailures = 2;
    public const double V3KeepAliveIntervalSeconds = 15.0;
    public const double V3KeepAliveTickSeconds = 5.0;
    public const int V3KeepAliveMaxMisses = 3;
    public const double V3ProbeReplyCacheSeconds = 5.0;
    public const bool V3MatchmakingEnabled = true;
    public const int V3MatchmakingCandidateCount = 6;
    public const int V3MatchmakingDiversitySlots = 1;
    public const double V3MatchmakingCapacityThreshold = 0.85;
    public const int V3MatchmakingExchangeTimeoutMs = 10000;
    public const int V3MatchmakingRetryIntervalMs = 400;
    public const int V3MatchmakingFallbackCandidateCount = 8;
    public const int V3MatchmakingFallbackDeterministicSlots = 4;
    public const int V3MatchmakingHandshakeFailureThreshold = 2;
    public const string P2PStunServers = "";
}
