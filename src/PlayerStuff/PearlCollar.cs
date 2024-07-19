// using [xyz]

namespace TheCollector;
public class PearlCollar  
{
    public static void Init()
    {
        On.Player.GrabUpdate += GrabTheGoddamnPorlOrDontYourChoice;

        On.Player.GraphicsModuleUpdated += UpdatePorlGraphicsModuleYeBloodyEejit;
    }

    private static void GrabTheGoddamnPorlOrDontYourChoice(On.Player.orig_GrabUpdate orig, Player self, bool eu)
    {
        orig(self, eu);
        if (self.GetCollector().isCollector)
        {
            for (int index = 0; index < self.grasps.Length; index++)
            {
                if (self.grasps[index]?.grabbed is IPlayerEdible) return;
                // if its edible, dont even bother
            }

            if (self.Yippee().pearlstorage != null)
            {
                self.Yippee().pearlstorage.increment = self.input[0].pckp;
                self.Yippee().pearlstorage.Update(eu, self);
            }
        }
    }

    private static void UpdatePorlGraphicsModuleYeBloodyEejit(On.Player.orig_GraphicsModuleUpdated orig, Player self, bool actuallyViewed, bool eu)
    {
        try
        {
            if (self.slugcatStats.name.value != "TheCollector")
            {
                orig(self, actuallyViewed, eu);
                return;
            }
        }
        catch (Exception err)
        {
            Debug.Log("Error involving slugcat name within Player.GraphicsModuleUpdated:" + err);
            orig(self, actuallyViewed, eu);
            return;
        }
        self.Yippee().pearlstorage?.GraphicsModuleUpdated(actuallyViewed, eu);
        orig(self, actuallyViewed, eu); // why is the graphics module updated again after this? is this redundant, or needed?
    }


    // end pearlcollar
}