namespace TheCollector;

public static class TheCollectorExtensions
{
    //Implementing graphics modules for later
    public static Color? GetColor(this PlayerGraphics pg, PlayerColor color) => color.GetColor(pg);

    public static Color? GetColor(this Player player, PlayerColor color) => (player.graphicsModule as PlayerGraphics)?.GetColor(color);

    public static Player Get(this WeakReference<Player> weakRef)
    {
        _ = weakRef.TryGetTarget(out Player result);
        return result;
    }

    public static PlayerGraphics PlayerGraphics(this Player player) => (PlayerGraphics)player.graphicsModule;

    public static TailSegment[] Tail(this Player player) => player.PlayerGraphics().tail;

    //CWT for extension
    private static readonly ConditionalWeakTable<AbstractCreature, TheCollectorData> _cwttc = new();

    //Attaching the class data to the Player class and AbstractCreature class
    public static TheCollectorData Collector(this Player player) => _cwttc.GetValue(player.abstractCreature, _ => new TheCollectorData(player.abstractCreature));

    public static TheCollectorData Collector(this AbstractCreature player) => _cwttc.GetValue(player, _ => new TheCollectorData(player));

    //Making bools for easy access
    public static bool IsCollector(this Player player) => player.Collector().isCollector;

    public static bool IsCollector(this PlayerGraphics pg, out TheCollectorData collector) => pg.player.IsCollector(out collector);

    public static bool IsCollector(this Player player, out TheCollectorData Collector)
    {
        Collector = player.Collector();
        return Collector.isCollector;
    }
}