using System.Runtime.CompilerServices;
using UnityEngine.Assertions.Must;

namespace TheCollector
{
    internal class FlapAbility
    {
        public static bool LimitSpeed = true;
        public static float speed;

        public static void init()
        {
            On.Player.Update += Glide;
            On.Player.Update += Flap;
        }

        private static void Flap(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (!self.dead && self != null && self.room != null && self.Consious && self.firstChunk.pos != null &&
                self.slugcatStats.name.value == "TheCollector" && self.GetCollector().isCollector)
            {
                try {
                    var room = self.room;
                    var input = self.input;

                    if (self.GetCollector().JumpCollectorLock > 0)
                    {
                        self.GetCollector().JumpCollectorLock--;
                    }
                    Debug.Log("oooou collectortime. jumplock is: " + self.GetCollector().JumpCollectorLock);

                    // END VARIABLES ----------------------------------------------------------------------------------------------
                    // now we get to the juice
                    if (input != null && self.GetCollector().JumpCollectorLock == 0 &&
                        // if not locked out of jumping
                        self.animation != null && self.bodyMode != null &&
                        // makes sure things arent null
                        self.wantToJump > 0 && self.GetCollector().Jumptimer <= 0 && self.canJump <= 0 &&
                        // attempting to jump
                        self.eatMeat <= 20 && self.maulTimer <= 15 && self.onBack == null &&
                        // not eating meat, not mauling, not on another slugcats back
                        self.canJump == 0 && self.lowerBodyFramesOnGround <= 1 &&
                        // cannot normally jump and had its lower body on the ground for less than/equal to 1 frame
                        self.enteringShortCut == null && !self.inShortcut && self.shortcutDelay == 0 &&
                        // not entering, inside a shortcut, or in a shortcut delay state
                        self.Consious && !self.dead && self.canWallJump == 0 && self.jumpStun == 0 &&
                        // is awake, not dead, cannot wall jump, not jumpstunned
                        !self.submerged && self.goIntoCorridorClimb <= 0 &&
                        // is not underwater and not climbing through a pipe
                        self.animation != Player.AnimationIndex.VineGrab &&
                        self.animation != Player.AnimationIndex.CorridorTurn &&
                        self.animation != Player.AnimationIndex.LedgeCrawl &&
                        self.animation != Player.AnimationIndex.BellySlide &&
                        self.animation != Player.AnimationIndex.SurfaceSwim &&
                        self.animation != Player.AnimationIndex.DeepSwim &&
                        // animation indexes
                        self.bodyMode != Player.BodyModeIndex.CorridorClimb &&
                        self.bodyMode != Player.BodyModeIndex.Crawl &&
                        self.bodyMode != Player.BodyModeIndex.WallClimb &&
                        self.bodyMode != Player.BodyModeIndex.Swimming &&
                        self.bodyMode != Player.BodyModeIndex.ClimbingOnBeam &&
                        self.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut &&
                        // body mode indexes
                        self.animation != Player.AnimationIndex.HangFromBeam &&
                        self.animation != Player.AnimationIndex.ClimbOnBeam &&
                        self.animation != Player.AnimationIndex.ZeroGPoleGrab &&
                        self.animation != Player.AnimationIndex.GetUpOnBeam &&
                        self.animation != Player.AnimationIndex.StandOnBeam &&
                        self.animation != Player.AnimationIndex.AntlerClimb &&
                        self.animation != Player.AnimationIndex.HangUnderVerticalBeam &&
                        self.animation != Player.AnimationIndex.BeamTip &&
                        self.animation != Player.AnimationIndex.GetUpToBeamTip &&
                        // beam things
                        self.bodyMode == Player.BodyModeIndex.ZeroG && room.gravity > 0f && self.gravity > 0f &&
                        // makes sure the player IS NOT in zero gravity
                        self.input[0].jmp && !self.input[1].jmp
                        // if the input in that exact frame is bound to the jump key, and the previous frame WAS NOT pressing jump
                        )
                    {
                        Debug.Log("Wings flapped");
                        self.GetCollector().CollectorJumped = true; // flapped wings
                        self.GetCollector().JumpCollectorLock = 40; // prevents doing so again for this long
                        self.GetCollector().NoGrabCollector = 5; // locks grabs for this long
                        Vector2 pos = self.firstChunk.pos; // body position but more readable

                        if (self.input[0].x != 0)
                        {
                            // if left/right input is not zero
                            self.animation = Player.AnimationIndex.RocketJump;
                            self.bodyMode = Player.BodyModeIndex.Default;
                            // switches the animation type to a rocketjump
                        }
                        else
                        {
                            for (int l = 0; l < 2; l++)
                            {
                                // runs twice...?
                                if (self.bodyChunks[l].ContactPoint.x != 0 || self.bodyChunks[l].ContactPoint.y != 0)
                                {
                                    self.animation = Player.AnimationIndex.None;
                                    self.bodyMode = Player.BodyModeIndex.Stand;
                                    self.standing = self.bodyChunks[0].pos.y > self.bodyChunks[1].pos.y;
                                    break;
                                    // if running into something, cancels flight
                                }

                            }
                        }

                        room.PlaySound(TCEnums.Sound.flap, pos);
                        // fwoosh


                        // VELOCITY THINGS ----------------------------------------------------------------------------------------------
                        // added a split since the variables will likely need to be changed
                        if (input[0].x != 0)
                        {
                            // if left/right input is not zero
                            self.bodyChunks[0].vel.y += Mathf.Min(self.bodyChunks[0].vel.y, 0f) + 8f;
                            self.bodyChunks[1].vel.y += Mathf.Min(self.bodyChunks[1].vel.y, 0f) + 7f;
                            // returns whichever number is smaller, the y velocity or 0f, and adds the float to it
                            self.jumpBoost = 6f;
                            // sets the jump boost to a hard 6f
                        }

                        if (input[0].x == 0 || input[0].y == 1)
                        {
                            // if left/right input is zero OR up is being held
                            self.bodyChunks[0].vel.y += 16f;
                            self.bodyChunks[1].vel.y += 15f;
                            self.jumpBoost = 10f;
                        }

                        if (input[0].y == 1)
                        {
                            // if input is up, goes a shorter distance left/right
                            self.bodyChunks[0].vel.x += 10f * input[0].x;
                            self.bodyChunks[1].vel.x += 8f * input[0].x;
                        }
                        else
                        {
                            // otherwise, goes a longer distance
                            self.bodyChunks[0].vel.x += 15f * input[0].x;
                            self.bodyChunks[1].vel.x += 13f * input[0].x;
                        }

                        self.GetCollector().JumpCollectorCount++;
                        // counts how many times the jump key was pressed
                    }

                    if (self.bodyMode != null &&
                        self.bodyMode == Player.BodyModeIndex.WallClimb && self.GetCollector().Jumptimer <= 0)
                    {
                        // if wallclimbing with a jumptimer less than or equal to zero
                        float JumpDelay = 0.25f;
                        self.GetCollector().Jumptimer = (int)(JumpDelay * 40f);
                    }

                    if (self.bodyMode != null &&
                        self.bodyMode == Player.BodyModeIndex.CorridorClimb && self.GetCollector().Jumptimer <= 0)
                    {
                        // if wallclimbing with a jumptimer less than or equal to zero
                        float JumpDelay = 0.75f;
                        self.GetCollector().Jumptimer = (int)(JumpDelay * 40f);
                    }

                    if (self.GetCollector().Jumptimer > 0)
                    {
                        self.GetCollector().Jumptimer--;
                        // counts down timer
                    }

                    if (self.bodyMode != null && self.animation != null &&
                        (!self.GetCollector().isSliding && (self.canJump > 0 || !self.Consious || self.Stunned || self.lowerBodyFramesOnGround >= 1 ||
                        // if not sliding, can jump, not consious, not stunned, or on the ground
                        self.animation == Player.AnimationIndex.HangFromBeam || self.animation == Player.AnimationIndex.ClimbOnBeam ||
                        self.bodyMode == Player.BodyModeIndex.WallClimb || self.animation == Player.AnimationIndex.AntlerClimb ||
                        self.animation == Player.AnimationIndex.VineGrab || self.animation == Player.AnimationIndex.ZeroGPoleGrab ||
                        self.bodyMode == Player.BodyModeIndex.Swimming || self.bodyMode == Player.BodyModeIndex.ZeroG)))
                    {
                        // checking if anything was touched to reset the wingflap
                        self.GetCollector().CollectorJumped = false;
                        // reenables the ability to jump
                        self.GetCollector().JumpCollectorCount = 0;
                        // resets the jump count
                    }

                    // end wingflap
                }
                catch (Exception e)
                {
                    Debug.Log("Collector (flap) is being a little bitch: " + e);
                }
            }

            orig(self, eu);
        }

        private static void Glide(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);

            if (self != null && self.room != null && !self.dead && self.bodyMode != null && self.firstChunk.pos != null &&
                self.GetCollector().isCollector)
            {
                try
                {
                    const float normalGravity = 0.9f;
                    const float normalAirFriction = 0.999f;
                    const float flightGravity = 0.12f;
                    const float flightAirFriction = 0.7f;
                    const float flightKickinDuration = 6f;

                    bool canglide = self.GetCollector().SlideStamina > 0
                    && self.GetCollector().preventSlide == 0
                    && self.canJump <= 0
                    && self.bodyMode != Player.BodyModeIndex.Crawl
                    && self.bodyMode != Player.BodyModeIndex.CorridorClimb
                    && self.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut
                    && self.animation != Player.AnimationIndex.HangFromBeam
                    && self.animation != Player.AnimationIndex.ClimbOnBeam
                    && self.bodyMode != Player.BodyModeIndex.WallClimb
                    && self.bodyMode != Player.BodyModeIndex.Swimming
                    && self.Consious
                    && !self.Stunned
                    && self.animation != Player.AnimationIndex.AntlerClimb
                    && self.animation != Player.AnimationIndex.VineGrab
                    && self.animation != Player.AnimationIndex.ZeroGPoleGrab;


                    if (self.GetCollector().CanSlide)
                    {
                        if (self.animation != null && self.animation == Player.AnimationIndex.HangFromBeam)
                        {
                            self.GetCollector().preventSlide = 15;
                        }
                        else if (self.GetCollector().preventSlide > 0)
                        {
                            self.GetCollector().preventSlide--;
                        }

                        if (!self.GetCollector().isSliding)
                        {
                            speed = 2f;
                            LimitSpeed = true;
                            // if not sliding, limit the speed  to 2f
                        }
                        
                        if (self.GetCollector().isSliding)
                        {
                            // if she IS sliding
                            self.GetCollector().windSound.Volume = Mathf.Lerp(0f, 0.4f, self.GetCollector().slideDuration / flightKickinDuration);
                            self.GetCollector().slideDuration++;
                            self.AerobicIncrease(0.0001f);

                            self.gravity = Mathf.Lerp(normalGravity, flightGravity, self.GetCollector().slideDuration / flightKickinDuration);
                            self.airFriction = Mathf.Lerp(normalAirFriction, flightAirFriction, self.GetCollector().slideDuration / flightKickinDuration);

                            if (LimitSpeed)
                            {
                                speed = Custom.LerpAndTick(speed, 10f, 0.001f, 0.3f);

                                if (speed >= 10f)
                                {
                                    LimitSpeed = false;
                                }

                                if (self.input != null &&
                                    self.input[0].x > 0)
                                {
                                    self.bodyChunks[0].vel.x = self.bodyChunks[0].vel.x + speed;
                                    self.bodyChunks[1].vel.x = self.bodyChunks[1].vel.x - 1f;
                                }
                                else if (self.input != null &&
                                    self.input[0].x < 0)
                                {
                                    self.bodyChunks[0].vel.x = self.bodyChunks[0].vel.x - speed;
                                    self.bodyChunks[1].vel.x = self.bodyChunks[1].vel.x + 1f;
                                }


                                if (self.room.gravity <= 0.5)
                                {
                                    // this probably should not be here, as collector shouldnt be able to fly in 0g? should ask
                                    if (self.input != null &&
                                    self.input[0].y > 0)
                                    {
                                        self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y + speed;
                                        self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y - 1f;
                                    }
                                    else if (self.input != null &&
                                    self.input[0].y < 0)
                                    {
                                        self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y - speed;
                                        self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y + 1f;
                                    }
                                }
                                else if (self.GetCollector().UnlockedVerticalFlight)
                                {
                                    // i assume this is just for fun
                                    if (self.input[0].y > 0)
                                    {
                                        self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y + speed * 0.5f;
                                        self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y - 0.3f;
                                    }
                                    else if (self.input[0].y < 0)
                                    {
                                        self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y - speed;
                                        self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y + 0.3f;
                                    }
                                }
                            }

                            if (!LimitSpeed)
                            {
                                // if collector is not being rate limited
                                speed = Custom.LerpAndTick(speed, 0f, 0.005f, 0.003f);

                                if (speed == 0f)
                                {
                                    LimitSpeed = true;
                                }
                                if (self.input[0].x > 0)
                                {
                                    self.bodyChunks[0].vel.x = self.bodyChunks[0].vel.x + speed;
                                    self.bodyChunks[1].vel.x = self.bodyChunks[1].vel.x - 1f;
                                }
                                else if (self.input[0].x < 0)
                                {
                                    self.bodyChunks[0].vel.x = self.bodyChunks[0].vel.x - speed;
                                    self.bodyChunks[1].vel.x = self.bodyChunks[1].vel.x + 1f;
                                }
                                if (self.room.gravity <= 0.5)
                                {
                                    if (self.input[0].y > 0)
                                    {
                                        self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y + speed;
                                        self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y - 1f;
                                    }
                                    else if (self.input[0].y < 0)
                                    {
                                        self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y - speed;
                                        self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y + 1f;
                                    }
                                }
                                else if (self.input != null &&
                                    self.GetCollector().UnlockedVerticalFlight)
                                {
                                // this should not be called in normal gameplay, and i assume is here for fun
                                    if (self.input[0].y > 0)
                                    {
                                        self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y + speed * 0.5f;
                                        self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y - 0.3f;
                                    }
                                    else if (self.input[0].y < 0)
                                    {
                                        self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y - speed;
                                        self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y + 0.3f;
                                    }
                                }
                                if (speed <= 1.2f)
                                {
                                    self.GetCollector().StopSliding();
                                }
                            }


                            self.GetCollector().slideStaminaRecoveryCooldown = 40;
                            self.GetCollector().SlideStamina--;

                            if (!self.input[0].jmp || !canglide)
                            {
                                self.GetCollector().StopSliding();
                            }

                        }
                        else
                        {
                            // if NOT sliding
                            self.GetCollector().windSound.Volume = Mathf.Lerp(1f, 0f, self.GetCollector().timeSinceLastSlide / flightKickinDuration);
                            self.GetCollector().timeSinceLastSlide++;
                            self.GetCollector().windSound.Volume = 0f;
                            if (self.GetCollector().slideStaminaRecoveryCooldown > 0)
                            {
                                self.GetCollector().slideStaminaRecoveryCooldown--;
                            }
                            else
                            {
                                self.GetCollector().SlideStamina = Mathf.Min(self.GetCollector().SlideStamina +
                                    self.GetCollector().SlideRecovery, self.GetCollector().SlideStaminaMax);
                            }
                            if (self.wantToJump > 0 && self.GetCollector().SlideStamina > self.GetCollector().MinimumSlideStamina &&
                            canglide && self.GetCollector().CollectorJumped)
                            {
                            self.bodyMode = Player.BodyModeIndex.Default;
                            self.animation = Player.AnimationIndex.None;
                            self.wantToJump = 0;
                            self.GetCollector().slideDuration = 0;
                            self.GetCollector().timeSinceLastSlide = 0;
                            self.GetCollector().isSliding = true;
                        }
                            self.airFriction = normalAirFriction;
                            self.gravity = normalGravity;
                        }
                        if (self.GetCollector().preventGrabs > 0)
                        {
                            self.GetCollector().preventGrabs--;
                        }
                    }

                    try { self.GetCollector().windSound.Update(); }
                    catch (Exception e) { Debug.Log("Collector (windsound update) is being a little bitch: " + e); }
                }
                catch (Exception e)
                {
                    Debug.Log("Collector (glide) is being a little bitch: " + e);
                }
            }
        }
        // end flapability
    }
}
