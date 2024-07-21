namespace TheCollector;

public class StatsHooks
{
    public static void Init()
    {
        On.Player.ctor += Player_ctor;
        // initiating

        On.Player.Jump += Player_Jump;
        // boosts to jumps

        On.Player.CanBeSwallowed += Player_CanBeSwallowed;
        // prevents swallowing pearls

        On.Player.UpdateBodyMode += Player_UpdateBodyMode;
        // additions to movement velocities

        On.Player.ThrownSpear += Player_ThrownSpear;
        // tweaks to which type of damage is used

        On.Player.WallJump += Player_WallJump;
        // walljump tweaks
    }

    private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);

        // This will only work if the player is The Collector
        if (!self.IsCollector(out var collector)) return;

        // Turn this to true for even more extra debug logs. a frankly ridiculous amount of them
        collector.NeonWantsDebugLogsUwU = false;
    }

    private static void Player_Jump(On.Player.orig_Jump orig, Player self)
    {
        orig(self);

        if (!self.IsCollector(out var collector)) return;

        if (self != null && self.room != null && self.bodyChunks != null)
        {
            float jumpboost = Mathf.Lerp(1f, 1.15f, self.Adrenaline);
            // jumpboost ranges from 1 to 1.15 depending on adrenaline levels

            self.jumpBoost += 0.70f + jumpboost;
            // standard boost is 1.7f higher than standard jumps, plus 0.15 if on shrooms

            if (self.animation != null && self.animation == Player.AnimationIndex.Flip)
            {
                // bodychunks[0] body, bodychunks[1] hips
                self.bodyChunks[0].vel.y = 10f * jumpboost;
                self.bodyChunks[1].vel.y = 9f * jumpboost;
                self.bodyChunks[0].vel.x = 5f * (float)self.flipDirection * jumpboost;
                self.bodyChunks[1].vel.x = 4f * (float)self.flipDirection * jumpboost;
                // increases the flip velocity. these values replace the default
            }

            if (self.animation == Player.AnimationIndex.RocketJump)
            {
                self.bodyChunks[0].vel.y += 4f + jumpboost;
                self.bodyChunks[1].vel.y += 3f + jumpboost;
                self.bodyChunks[0].vel.x += 3f * self.bodyChunks[0].vel.x < 1 ? 0 : Mathf.Sign(self.bodyChunks[0].vel.x);
                self.bodyChunks[1].vel.x += 2f * self.bodyChunks[0].vel.x < 1 ? 0 : Mathf.Sign(self.bodyChunks[0].vel.x);
                // increases the rocket jump velocity. these values are added to the default. if the velocity of x on the body is 1, theres no jumpboost
            }
        }
    }

    private static bool Player_CanBeSwallowed(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
    {
        if(!self.IsCollector(out _)) return orig(self, testObj);

        if (testObj != null && self != null &&
            // preventing null errors
            testObj is DataPearl)
        {
            return false;
            // collector cannot swallow pearls and will not comprimise on this
        }

        return orig(self, testObj);
    }

    private static void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self)
    {
        orig(self);

        if (!self.IsCollector(out var collector)) return;

        if (self.bodyMode != null && self != null && self.room != null)
        {
            // more bodymode updates go here!

            if (!self.standing && self.dynamicRunSpeed != null &&
                (self.bodyMode == Player.BodyModeIndex.Default || self.bodyMode == Player.BodyModeIndex.Crawl))
            {
                // so long as they arent standing, but are crawling or in the default bodymode. does "standing" include if theyre on all fours?
                var power = 1.2f;

                self.dynamicRunSpeed[0] += power;
                self.dynamicRunSpeed[1] += power;
                // manual speed add to both crawling and walking
            }

            if (self.animation != null && self.animation == AnimationIndex.BellySlide)
            {
                float longSlideVelocity = 10f;
                float notLongSlideVelocity = 15f;
                // i imagine these were here to make for easier editing? gave them names that are more followable
                //      since "vector" / "vector2" are not helpful

                self.bodyChunks[0].vel.x += (self.longBellySlide ? longSlideVelocity : notLongSlideVelocity) * (float)self.rollDirection *
                    Mathf.Sin((float)self.rollCounter / (self.longBellySlide ? 29f : 15f) * (float)Math.PI);
                // self.bodyChunks[body].vel.x = (longslide ? 10f : 15f) * (float)self.rollDirection * Mathf.Sin((float)self.rollCounter /
                //      (longslide ? 29f : 15f (why do these have set values and the vectors above dont?)) * pi);
                if (self.IsTileSolid(0, 0, -1) || self.IsTileSolid(0, 0, -2))
                {
                    // if running into a solid tile while sliding
                    self.bodyChunks[0].vel.y -= 2.3f;
                    // roblox oof sfx. launches in the opposite direction
                }
            }
            // end collector bodymode update
        }
    }

    private static void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
    {
        orig(self, spear);

        if (!self.IsCollector(out var collector)) return;

        if (self != null && spear != null && self.room != null && spear.room != null)
        {
            switch (TheCollectorOptionsMenu.Damage.Value)
            {
                // changes the damage bonus based on the remix value
                case "1. Monk":
                    {
                        // presumably, monk stats are listed in the .json
                        // update: they are not. are they in the code, or should this be fixed?
                        break;
                    }
                case "2. Survivor":
                    {
                        spear.spearDamageBonus = UnityEngine.Random.Range(1f, 1.1f);
                        break;
                    }
                case "3. Hunter":
                    {
                        spear.spearDamageBonus = UnityEngine.Random.Range(1.1f, 1.2f);
                        break;
                    }
                default:
                    break;
            }

            if (self.room.game.IsArenaSession)
            {
                spear.spearDamageBonus = UnityEngine.Random.Range(0.6f, 1.2f);
                // this ones interesting and pretty cool. if in an arena, the damage bonus instead ranges from monk min to hunter max,
                //      though it will err on the side of lower than average.
            }
        }
    }

    private static void Player_WallJump(On.Player.orig_WallJump orig, Player self, int direction)
    {
        orig(self, direction);
        if (self.bodyChunks != null && self != null && self.room != null &&
            self.slugcatStats.name.value == "TheCollector")
        {
            float adrenalineboost = Mathf.Lerp(1f, 1.15f, self.Adrenaline);

            self.bodyChunks[0].vel.y = 10f * adrenalineboost;
            self.bodyChunks[1].vel.y = 9f * adrenalineboost;
            self.bodyChunks[0].vel.x = 8f * adrenalineboost * (float)direction;
            self.bodyChunks[1].vel.x = 6f * adrenalineboost * (float)direction;
            // this replaces vanilla. i cant tell if this is higher or lower than average. i presume higher?
        }
    }
    // end stat tweaks
}
