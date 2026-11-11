namespace NexusRetail.WinForms.Services;

/// <summary>
/// Cổng cung cấp các dịch vụ POS tách biệt theo từng interface nghiệp vụ (Composition Root).
/// </summary>
public static class WinFormsApiClient
{
    private static readonly PosSessionContext Session = new();

    public static IPosAuthClient Auth { get; } = new PosAuthClient(Session);
    public static IPosCatalogClient Catalog { get; } = new PosCatalogClient(Session);
    public static IPosSalesClient Sales { get; } = new PosSalesClient(Session);
    public static IPosInboundClient Inbound { get; } = new PosInboundClient(Session);
    public static IPosReportClient Report { get; } = new PosReportClient(Session);
}
