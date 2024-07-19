using SlugBase;
using System.Runtime.CompilerServices;

namespace TheCollector
{
    public static class TheCollectorEX
    {
        public class VariableInit
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
            public bool isCollector;
            public bool isSliding;
            public bool UnlockedExtraStamina;
            public bool UnlockedVerticalFlight;

            public readonly SlugcatStats.Name Name;
            public SlugBaseCharacter Character;
            public WeakReference<Player> collectorRef;

            public DynamicSoundLoop windSound;

            public int JumpCollectorLock;
            public int cooldownAlone;
            public int JumpCollectorCount;
            public int NoGrabCollector;
            public int Jumptimer;

            public bool CollectorJumped; // checks if collector has jumped
            public bool Jumping; // likely redundant?

            public void StopSliding()
            {
                slideDuration = 0;
                timeSinceLastSlide = 0;
                isSliding = false;
            }
        }

        private static readonly ConditionalWeakTable<Player, VariableInit> ColCWT= new();
        public static VariableInit GetCollector(this Player player) => ColCWT.GetValue(player, _ => new());
        // end cwt and variables
    }
}
