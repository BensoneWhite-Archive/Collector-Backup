#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace TheCollector
{
    [BepInDependency("slime-cubed.slugbase")]
    [BepInDependency("dressmyslugcat", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("TheCollector", "The Collector", "0.1.0")]

    class Plugin : BaseUnityPlugin
    {
        static bool _Initialized;
        private TheCollectorOptionsMenu optionsMenuInstance;

        public void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
        }

        public static readonly PlayerFeature<int> slideStamina = PlayerInt("collector/SlideStamina");
        public static readonly PlayerFeature<float> SlideRecovery = PlayerFloat("collector/SlideRecovery");
        public static readonly PlayerFeature<float> SlideSpeed = PlayerFloat("collector/SlideSpeed");
        public static readonly PlayerFeature<bool> Collector = PlayerBool("collector/Collector");

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                if (_Initialized) { return; }
                _Initialized = true;

                TCEnums.Init();
                PorlCollar.Init();

                StatsHooks.Init();
                FlapAbility.init();

                MachineConnector.SetRegisteredOI("TheCollector", optionsMenuInstance = new TheCollectorOptionsMenu());

                if (ModManager.ActiveMods.Any(mod => mod.id == "dressmyslugcat"))
                {
                    SetupDMSSprites();
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Remix Menu: Hook_OnModsInit options failed init error {optionsMenuInstance}{ex}");
                Logger.LogError(ex);
                Logger.LogMessage("WHOOPS something go wrong"); 
            }
            finally
            {
                orig.Invoke(self);
            }
        }

        public void SetupDMSSprites()
        {
            var sheetID = "TheCollector.Legacy";

            for (int index = 0; index < 4; index++)
            {
                SpriteDefinitions.AddSlugcatDefault(new Customization()
                {
                    Slugcat = "TheCollector",
                    PlayerNumber = index,
                    CustomSprites = new List<CustomSprite>
                    {
                        new CustomSprite() { Sprite = "HEAD", SpriteSheetID = sheetID, Color = Color.white },
                        new CustomSprite() { Sprite = "ARMS", SpriteSheetID = sheetID, Color = Color.white },
                        new CustomSprite() { Sprite = "BODY", SpriteSheetID = sheetID, Color = Color.white },
                        new CustomSprite() { Sprite = "HIPS", SpriteSheetID = sheetID, Color = Color.white },
                        new CustomSprite() { Sprite = "LEGS", SpriteSheetID = sheetID, Color = Color.white },
                        new CustomSprite() { Sprite = "TAIL", SpriteSheetID = sheetID, Color = Color.white },
                        new CustomSprite() { Sprite = "FACE", SpriteSheetID = sheetID, Color = Color.white },
                    },

                    CustomTail = new CustomTail()
                    {
                        Length = 7,
                        Wideness = 4f,
                        Roundness = 0.7f
                    }
                });
            }
        }
    }    
}