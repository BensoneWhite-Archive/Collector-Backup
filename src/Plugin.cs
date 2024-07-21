namespace TheCollector;

[BepInDependency("slime-cubed.slugbase", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("dressmyslugcat", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("MSC", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("MoreSlugcats", BepInDependency.DependencyFlags.SoftDependency)] // theres two cause i forget which is its name. skull emoji
[BepInPlugin("TheCollector", "The Collector", "0.1.0")]
public class Plugin : BaseUnityPlugin
{
    public const string MOD_ID = "TheCollector";
    public const string MOD_NAME = "The Collector";
    public const string VERSION = "0.1.0";

    private static bool _Initialized;
    private TheCollectorOptionsMenu optionsMenuInstance;

    public static void DebugLog(object ex) => Logger.LogInfo(ex);

    public static void DebugWarning(object ex) => Logger.LogWarning(ex);

    public static void DebugError(object ex) => Logger.LogError(ex);

    public static void DebugFatal(object ex) => Logger.LogFatal(ex);

    public new static ManualLogSource Logger;

    public void OnEnable()
    {
        try
        {
            //Since 1.9.14 update the logs are a bit broken, it's better to use the BepInEx Logger
            Logger = base.Logger;
            DebugWarning($"{MOD_NAME} is loading.... {VERSION}");

            TCEnums.Init();

            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            // initiates collector and all submodules
        }
        catch (Exception ex)
        {
            DebugError(ex);
            Debug.LogException(ex); //Log exception from unity is the only exception to the base game update
        }
    }

    private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (_Initialized) return;
            _Initialized = true;

            DebugWarning($"Initializing OnModsInit {MOD_NAME}");

            PearlCollar.Init();

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
            DebugError($"Remix Menu: Hook_OnModsInit options failed init error {optionsMenuInstance}{ex}");
            DebugError("COLLECTOR ERROR: " + ex);
            Debug.LogError(ex);
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
        // end dms sprite setup
    }

    // end plugin
}