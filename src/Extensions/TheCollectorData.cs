namespace TheCollector;

public class TheCollectorData
{
    public bool NeonWantsDebugLogsUwU;

    public float SlideRecovery => SlideStaminaRecoveryBase;

    // time it takes to bounce back after a slide
    public float MinimumSlideStamina => SlideStaminaMax * 0.1f;
    public int SlideStaminaMax => (int)SlideStaminaMaxBase; // max stamina
    private static float SlideStaminaMaxBase => 10000f;

    public float SlideStamina;
    public static float SlideStaminaRecoveryBase => 10f;
    public int slideStaminaRecoveryCooldown;

    public float SlideSpeed;

    public int slideDuration;
    public int timeSinceLastSlide; // probably should have a cap to prevent lag issues
    public int preventSlide;
    public int preventGrabs; // isnt this the same as NoGrabCollector?

    public bool CanSlide => SlideStaminaMax > 0 && SlideSpeed > 0;

    public bool isCollector;
    public bool isSliding;

    public readonly Player player;
    public WeakReference<Player> collectorRef; // what is this used for?

    public DynamicSoundLoop windSound; // this causes many errors at the moment.

    public int JumpCollectorCount; // counts the number of jumps made. shouldnt really be necessary
    public int NoGrabCollector; // time that grabbing objects should be prevented
    public int Jumptimer; // prevents col from jumping too soon after other movements

    public bool CollectorJumped; // checks if collector has jumped

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

        SlideStamina = SlideStaminaMax;
        timeSinceLastSlide = 200;
    }

    public void StopSliding()
    {
        slideDuration = 0;
        timeSinceLastSlide = 0;
        isSliding = false;
    }

    public void InitiateSlide()
    {
        player.bodyMode = BodyModeIndex.Default;
        player.animation = AnimationIndex.None;
        player.wantToJump = 0;
        slideDuration = 0;
        timeSinceLastSlide = 0;
        isSliding = true;
    }

    public bool CanSustainSlide()
    {
        return SlideStamina > 0 &&
               preventSlide <= 0 &&
               player.canJump <= 0 &&
               player.canWallJump == 0 &&
               player.Consious &&
               player.bodyMode != BodyModeIndex.Crawl &&
               player.bodyMode != BodyModeIndex.CorridorClimb &&
               player.bodyMode != BodyModeIndex.ClimbIntoShortCut &&
               player.animation != AnimationIndex.HangFromBeam &&
               player.animation != AnimationIndex.ClimbOnBeam &&
               player.bodyMode != BodyModeIndex.WallClimb &&
               player.bodyMode != BodyModeIndex.Swimming &&
               player.animation != AnimationIndex.AntlerClimb &&
               player.animation != AnimationIndex.VineGrab &&
               player.animation != AnimationIndex.ZeroGPoleGrab;
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