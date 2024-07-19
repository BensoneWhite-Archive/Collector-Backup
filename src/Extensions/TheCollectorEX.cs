using SlugBase;

namespace TheCollector
{
    public class TheCollectorEX
    {
        public float SlideRecovery => UnlockedExtraStamina ? SlideStaminaRecoveryBase * 1.2f : SlideStaminaRecoveryBase;
        public float MinimumSlideStamina => SlideStaminaMax * 0.1f;

        public readonly float SlideStaminaRecoveryBase;
        public float SlideStamina;
        public float SlideSpeed;

        public int SlideStaminaMax => UnlockedExtraStamina ? (int)(SlideStaminaMaxBase * 1.6f) : SlideStaminaMaxBase;

        public readonly int SlideStaminaMaxBase;
        public int slideStaminaRecoveryCooldown;
        public int slideDuration;
        public int timeSinceLastSlide;
        public int preventSlide;
        public int preventGrabs;

        public bool CanSlide => SlideStaminaMax > 0 && SlideSpeed > 0;

        public readonly bool Collector;
        public readonly bool isCollector;
        public bool isSliding;
        public bool UnlockedExtraStamina = false;
        public bool UnlockedVerticalFlight = false;

        public readonly SlugcatStats.Name Name;
        public SlugBaseCharacter Character;
        public WeakReference<Player> collectorRef;

        public DynamicSoundLoop windSound;

        public TheCollectorEX(Player player)
        {
            isCollector =
                Plugin.slideStamina.TryGet(player, out SlideStaminaMaxBase) &&
                Plugin.SlideRecovery.TryGet(player, out SlideStaminaRecoveryBase) &&
                Plugin.SlideSpeed.TryGet(player, out SlideSpeed) &&
                Plugin.Collector.TryGet(player, out Collector);

            collectorRef = new WeakReference<Player>(player);

            if (ExtEnumBase.TryParse(typeof(SlugcatStats.Name), "TheCollector", true, out var extEnum))
            {
                Name = extEnum as SlugcatStats.Name;
            }
            windSound = new ChunkDynamicSoundLoop(player.bodyChunks[0]);
            windSound.sound = TCEnums.Sound.wind;
            windSound.Pitch = 1f;
            windSound.Volume = 1f;
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
            if (!collectorRef.TryGetTarget(out var player))
            {
                return;
            }
            player.bodyMode = Player.BodyModeIndex.Default;
            player.animation = Player.AnimationIndex.None;
            player.wantToJump = 0;
            slideDuration = 0;
            timeSinceLastSlide = 0;
            isSliding = true;
        }

        public bool CanSustainFlight()
        {
            if (!collectorRef.TryGetTarget(out var player))
            {
                return false;
            }
            return SlideStamina > 0
                && preventSlide == 0
                && player.canJump <= 0
                && player.bodyMode != Player.BodyModeIndex.Crawl
                && player.bodyMode != Player.BodyModeIndex.CorridorClimb
                && player.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut
                && player.animation != Player.AnimationIndex.HangFromBeam
                && player.animation != Player.AnimationIndex.ClimbOnBeam
                && player.bodyMode != Player.BodyModeIndex.WallClimb
                && player.bodyMode != Player.BodyModeIndex.Swimming
                && player.Consious
                && !player.Stunned
                && player.animation != Player.AnimationIndex.AntlerClimb
                && player.animation != Player.AnimationIndex.VineGrab
                && player.animation != Player.AnimationIndex.ZeroGPoleGrab;
        }

        public int JumpCollectorLock;
        public int cooldownAlone;
        public int JumpCollectorCount;
        public int NoGrabCollector;
        public int Jumptimer;

        public bool CollectorJumped;
        public bool Jumping;

    }
}
