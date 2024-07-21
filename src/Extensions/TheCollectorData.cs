namespace TheCollector;

public class TheCollectorData
{
    public bool NeonWantsDebugLogsUwU;

    public float SlideRecovery => UnlockedExtraStamina ? SlideStaminaRecoveryBase * 1.2f : SlideStaminaRecoveryBase;
    // time it takes to bounce back after a slide
    public float MinimumSlideStamina => SlideStaminaMax * 0.1f;

    public readonly float SlideStaminaRecoveryBase; // why is the readonly part necessary?
    public float SlideStamina;
    public float SlideSpeed;

    public int SlideStaminaMax => UnlockedExtraStamina ? (int)(SlideStaminaMaxBase * 1.6f) : SlideStaminaMaxBase; // max stamina

    public readonly int SlideStaminaMaxBase;
    public int slideStaminaRecoveryCooldown;
    public int slideDuration;
    public int timeSinceLastSlide; // probably should have a cap to prevent lag issues
    public int preventSlide;
    public int preventGrabs; // isnt this the same as NoGrabCollector?

    public bool CanSlide => SlideStaminaMax > 0 && SlideSpeed > 0;

    public readonly bool Collector;
    public bool isCollector;
    public bool isSliding;
    public bool UnlockedExtraStamina;
    public bool UnlockedVerticalFlight; // should this be removed?

    public readonly Player player;
    public WeakReference<Player> collectorRef; // what is this used for?

    public DynamicSoundLoop windSound; // this causes many errors at the moment.

    public int cooldownAlone; // ??
    public int JumpCollectorCount; // counts the number of jumps made. shouldnt really be necessary
    public int NoGrabCollector; // time that grabbing objects should be prevented
    public int Jumptimer; // prevents col from jumping too soon after other movements

    public bool CollectorJumped; // checks if collector has jumped
    public bool Jumping; // likely redundant?

    public TheCollectorData(AbstractCreature abstractPlayer)
    {
        if (abstractPlayer.realizedCreature is not Player player)
        {
            throw new ArgumentException("AbstractCreature does not belong to a Player");
        }

        isCollector = player.slugcatStats.name == TCEnums.TheCollector;
        this.player = player;
        collectorRef = new WeakReference<Player>(player);

        if (!isCollector) return;

        SetupSounds(player);
    }

    public void StopSliding()
    {
        slideDuration = 0;
        timeSinceLastSlide = 0;
        isSliding = false;
    }

    private void SetupSounds(Player player)
    {
        windSound = new ChunkDynamicSoundLoop(player.bodyChunks[0])
        {
            sound = TCEnums.Sound.wind,
            Pitch = 1f,
            Volume = 1f
        };
    }
}
