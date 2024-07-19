using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Player;

namespace TheCollector
{
    internal class StatsHooks
    {
        public static void Init()
        {
            On.Player.Jump += Player_Jump;
            On.Player.CanBeSwallowed += Player_CanBeSwallowed;
            On.Player.UpdateBodyMode += Player_UpdateBodyMode;
            On.Player.ThrownSpear += Player_ThrownSpear;
            On.Player.CanEatMeat += Player_CanEatMeat;
            On.Player.WallJump += Player_WallJump;
        }

        private static void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);

            float jumpboost = Mathf.Lerp(1f, 1.15f, self.Adrenaline);

            if (self.slugcatStats.name.value == "TheCollector")
            {
                self.jumpBoost += 0.70f + jumpboost;
            }

            if (self.animation == Player.AnimationIndex.Flip && self.slugcatStats.name.value == "TheCollector")
            {

                self.bodyChunks[0].vel.y = 10f * jumpboost;
                self.bodyChunks[1].vel.y = 9f * jumpboost;


                self.bodyChunks[0].vel.x = 5f * (float)self.flipDirection * jumpboost;
                self.bodyChunks[1].vel.x = 4f * (float)self.flipDirection * jumpboost;
            }

            if (self.animation == Player.AnimationIndex.RocketJump && self.slugcatStats.name.value == "TheCollector")
            {

                self.bodyChunks[0].vel.y += 4f + jumpboost;
                self.bodyChunks[1].vel.y += 3f + jumpboost;

                self.bodyChunks[0].vel.x += 3f * self.bodyChunks[0].vel.x < 1 ? 0 : Mathf.Sign(self.bodyChunks[0].vel.x);
                self.bodyChunks[1].vel.x += 2f * self.bodyChunks[0].vel.x < 1 ? 0 : Mathf.Sign(self.bodyChunks[0].vel.x);
            }
        }

        private static bool Player_CanBeSwallowed(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
        {
            if (self.slugcatStats.name.value == "TheCollector" && testObj is DataPearl)
            {
                return false;
            }
            else
            {
                return orig(self, testObj);
            }
        }

        private static void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self)
        {
            orig(self);
            var power = 1.2f;
            if (self.slugcatStats.name.value == "TheCollector" && !self.standing && (self.bodyMode == Player.BodyModeIndex.Default || self.bodyMode == Player.BodyModeIndex.Crawl))
            {
                self.dynamicRunSpeed[0] += power;
                self.dynamicRunSpeed[1] += power;
            }

            if (self.animation == AnimationIndex.BellySlide && self.slugcatStats.name.value == "TheCollector")
            {
                float vector1 = 10f;
                float vector2 = 15f;

                self.bodyChunks[0].vel.x += (self.longBellySlide ? vector1 : vector2) * (float)self.rollDirection * Mathf.Sin((float)self.rollCounter / (self.longBellySlide ? 29f : 15f) * (float)Math.PI);
                if (self.IsTileSolid(0, 0, -1) || self.IsTileSolid(0, 0, -2))
                {
                    self.bodyChunks[0].vel.y -= 2.3f;
                }
            }
        }

        private static void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
        {
            orig(self, spear);
            if (self.slugcatStats.name.value == "TheCollector")
            {
                switch (TheCollectorOptionsMenu.Damage.Value)
                {
                    case "1. Monk":
                        {
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
            }

            if (self.slugcatStats.name.value == "TheCollector" && self.room.game.IsArenaSession)
            {
                spear.spearDamageBonus = UnityEngine.Random.Range(0.6f, 1.2f);
            }
        }

        private static bool Player_CanEatMeat(On.Player.orig_CanEatMeat orig, Player self, Creature crit)
        {
            orig(self, crit);
            if (self.slugcatStats.name.value == "TheCollector" && crit.dead && crit is not Player && (!self.isNPC || !self.isSlugpup) && (self.room.game.IsStorySession || self.room.game.IsArenaSession))
            {
                return true;
            }
            else
            {
                return orig(self, crit);
            }
        }

        private static void Player_WallJump(On.Player.orig_WallJump orig, Player self, int direction)
        {
            orig(self, direction);
            float num = Mathf.Lerp(1f, 1.15f, self.Adrenaline);

            if (self.slugcatStats.name.value == "TheCollector")
            {
                self.bodyChunks[0].vel.y = 10f * num;
                self.bodyChunks[1].vel.y = 9f * num;
                self.bodyChunks[0].vel.x = 8f * num * (float)direction;
                self.bodyChunks[1].vel.x = 6f * num * (float)direction;
            }
        }
    }
}
