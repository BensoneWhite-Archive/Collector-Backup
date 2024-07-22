namespace TheCollector;

public class FlapAbility
{
    //This should be inside the CWT
    public static bool LimitSpeed = true;

    public static float speed;

    public static void init()
    {
        On.Player.Update += Glide; // temporarily disabled due to issues
        On.Player.Update += Flap;
    }

    private static void Flap(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (!self.IsCollector(out var collector)) return;

        if (!self.dead && self != null && self.room != null && self.Consious && self.firstChunk.pos != null)
        {
            try
            {
                var room = self.room;
                var input = self.input;

                // BOOLS FOR FLAPPING
                bool meatmaulback = self.eatMeat > 20 || self.maulTimer > 15 || self.onBack != null; // eating meat, mauling, on another slugcats back
                bool onfloor = self.lowerBodyFramesOnGround > 1; // touching the ground for 2+ frames
                bool inshortcut = self.enteringShortCut != null || self.inShortcut || self.shortcutDelay != 0; // in shortcut
                bool notawake = !self.Consious || self.dead || self.canWallJump > 0 || self.jumpStun != 0; // not stunned or walljump locked
                bool xboxleikwater = self.submerged || self.goIntoCorridorClimb > 0; // underwater or in a pipe
                bool zerog = self.bodyMode == Player.BodyModeIndex.ZeroG || room.gravity == 0f || self.gravity == 0f; // is in 0 gravity

                // and the long ones
                bool polemode = self.animation == Player.AnimationIndex.HangFromBeam ||
                    self.animation == Player.AnimationIndex.ClimbOnBeam ||
                    self.animation == Player.AnimationIndex.ZeroGPoleGrab ||
                    self.animation == Player.AnimationIndex.GetUpOnBeam ||
                    self.animation == Player.AnimationIndex.StandOnBeam ||
                    self.animation == Player.AnimationIndex.AntlerClimb ||
                    self.animation == Player.AnimationIndex.HangUnderVerticalBeam ||
                    self.animation == Player.AnimationIndex.BeamTip ||
                    self.animation == Player.AnimationIndex.VineGrab ||
                    self.animation == Player.AnimationIndex.GetUpToBeamTip;
                // on a pole or vine, or interacting with such
                bool bodymodesaysno = self.bodyMode == Player.BodyModeIndex.CorridorClimb ||
                    self.bodyMode == Player.BodyModeIndex.Crawl ||
                    self.bodyMode == Player.BodyModeIndex.WallClimb ||
                    self.bodyMode == Player.BodyModeIndex.Swimming ||
                    self.bodyMode == Player.BodyModeIndex.ClimbingOnBeam ||
                    self.bodyMode == Player.BodyModeIndex.ClimbIntoShortCut;
                // not in a corridor, not crawling, not wallclimbing, not swimming, not climbing on pole, not going into shortcut
                bool animationsaysno = self.animation == Player.AnimationIndex.CorridorTurn ||
                    self.animation == Player.AnimationIndex.LedgeCrawl ||
                    self.animation == Player.AnimationIndex.BellySlide ||
                    self.animation == Player.AnimationIndex.SurfaceSwim ||
                    self.animation == Player.AnimationIndex.DeepSwim;
                // not turning in pipe, not on ledge, not sliding, not swimming (x2)
                // END BOOLS FOR FLAPPING

                if (collector.NeonWantsDebugLogsUwU)
                {
                    if (self.wantToJump > 0)
                    {
                        Plugin.DebugLog("Collector wants to jump!");
                        if (self.canJump <= 0)
                        {
                            Plugin.DebugLog("Collector should not ordinarily be able to jump...");
                            if (collector.Jumptimer <= 0) { Plugin.DebugLog("But jumptimer is zero, so wingflapping is ok!"); }
                        }
                    }
                    if (meatmaulback) { Plugin.DebugLog("Collector eating meat, mauling, or on another slugcat's back"); }
                    if (onfloor) { Plugin.DebugLog("Collector is on the ground"); }
                    if (inshortcut) { Plugin.DebugLog("Collector is playing in shortcuts"); }
                    if (notawake) { Plugin.DebugLog("Collector dead, stunned, walljumping, or jumpstunned"); }
                    if (xboxleikwater) { Plugin.DebugLog("Collector underwater or in pipe"); }
                    if (polemode) { Plugin.DebugLog("Collector on a pole, vine, or similar"); }
                    if (zerog) { Plugin.DebugLog("Ew, zero gravity"); }
                    if (bodymodesaysno) { Plugin.DebugLog("Bodymode says no flap"); }
                    if (animationsaysno) { Plugin.DebugLog("Animation says no flap"); }
                }

                // END VARIABLES ----------------------------------------------------------------------------------------------
                // now we get to the juice
                if (self.wantToJump > 0 && collector.Jumptimer <= 0 && self.canJump <= 0 &&
                    // attempting to jump
                    !meatmaulback && !onfloor && !inshortcut && !notawake &&
                    !xboxleikwater && !zerog && !polemode &&
                    !bodymodesaysno && !animationsaysno)
                {
                    if (collector.NeonWantsDebugLogsUwU) { Plugin.DebugLog("Wings flapped!"); }
                    collector.CollectorJumped = true; // flapped wings
                    collector.Jumptimer += 40; // prevents doing so again for this long
                    collector.NoGrabCollector = 5; // locks grabs for this long
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
                        self.bodyChunks[0].vel.y += Mathf.Min(self.bodyChunks[0].vel.y, 0f) + 4f;
                        self.bodyChunks[1].vel.y += Mathf.Min(self.bodyChunks[1].vel.y, 0f) + 3f;
                        // returns whichever number is smaller, the y velocity or 0f, and adds the float to it
                        self.jumpBoost = 6f;
                        // sets the jump boost to a hard 6f
                    }

                    if (input[0].x == 0 || input[0].y == 1)
                    {
                        // if left/right input is zero OR up is being held
                        self.bodyChunks[0].vel.y += 6f;
                        self.bodyChunks[1].vel.y += 7f;
                        self.jumpBoost = 10f;
                    }

                    if (input[0].y == 1)
                    {
                        // if input is up, goes a shorter distance left/right
                        self.bodyChunks[0].vel.x += 2f * input[0].x;
                        self.bodyChunks[1].vel.x += 3f * input[0].x;
                    }
                    else
                    {
                        // otherwise, goes a longer distance
                        self.bodyChunks[0].vel.x += 4f * input[0].x;
                        self.bodyChunks[1].vel.x += 5f * input[0].x;
                    }

                    collector.JumpCollectorCount++;
                    // counts how many times the jump key was pressed
                }

                if (self.bodyMode != null &&
                    self.bodyMode == Player.BodyModeIndex.WallClimb && collector.Jumptimer <= 0)
                {
                    // if wallclimbing with a jumptimer less than or equal to zero
                    float JumpDelay = 0.25f;
                    collector.Jumptimer = (int)(JumpDelay * 40f);
                }

                if (self.bodyMode != null &&
                    self.bodyMode == Player.BodyModeIndex.CorridorClimb && collector.Jumptimer <= 0)
                {
                    // if wallclimbing with a jumptimer less than or equal to zero
                    float JumpDelay = 0.75f;
                    collector.Jumptimer = (int)(JumpDelay * 40f);
                }

                if (collector.Jumptimer > 0)
                {
                    collector.Jumptimer--;
                    // counts down timer
                }

                if (self.bodyMode != null && self.animation != null &&
                    (!collector.isSliding && (self.canJump > 0 || !self.Consious || self.Stunned || self.lowerBodyFramesOnGround >= 1 ||
                    // if not sliding, can jump, not consious, not stunned, or on the ground
                    self.animation == Player.AnimationIndex.HangFromBeam || self.animation == Player.AnimationIndex.ClimbOnBeam ||
                    self.bodyMode == Player.BodyModeIndex.WallClimb || self.animation == Player.AnimationIndex.AntlerClimb ||
                    self.animation == Player.AnimationIndex.VineGrab || self.animation == Player.AnimationIndex.ZeroGPoleGrab ||
                    self.bodyMode == Player.BodyModeIndex.Swimming || self.bodyMode == Player.BodyModeIndex.ZeroG)))
                {
                    // checking if anything was touched to reset the wingflap
                    collector.CollectorJumped = false;
                    // reenables the ability to jump
                    collector.JumpCollectorCount = 0;
                    // resets the jump count
                }

                // end wingflap
            }
            catch (Exception e)
            {
                Plugin.DebugError("Collector (flap) is being a little bitch: " + e);
            }
        }
    }

    private static void Glide(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (!self.IsCollector(out var collector)) return;

        //Keeping this, the CanSustainSlide method from player data is updated
        bool canglide = collector.SlideStamina > 0
             && collector.preventSlide == 0
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

        const float normalGravity = 0.9f;
        const float normalAirFriction = 0.999f;
        const float flightGravity = -0.25f;
        const float flightAirFriction = 0.83f;
        const float flightKickinDuration = 6f;

        if (self != null && self.room != null && !self.dead && self.bodyMode != null && self.firstChunk.pos != null)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Plugin.DebugError(ex);
            }
        }

        Plugin.DebugWarning($"collector can slide: {collector.CanSlide}");
        Plugin.DebugWarning($"collector jumped: {collector.CollectorJumped}");

        Plugin.DebugFatal($"collector SlideStaminaMax: {collector.SlideStaminaMax}");
        Plugin.DebugFatal($"collector SlideSpeed: {collector.SlideSpeed}");

        if (collector.CanSlide)
        {
            if (self.animation != null && self.animation == Player.AnimationIndex.HangFromBeam)
            {
                collector.preventSlide = 15;
            }
            else if (collector.preventSlide > 0)
            {
                collector.preventSlide--;
            }

            if (!collector.isSliding)
            {
                speed = 2f;
                LimitSpeed = true;
                // if not sliding, limit the speed  to 2f
                // if she IS sliding
                collector.windSound.Volume = Mathf.Lerp(0f, 0.4f, collector.slideDuration / flightKickinDuration);

                collector.slideDuration++;

                self.AerobicIncrease(0.0001f);

                self.gravity = Mathf.Lerp(normalGravity, flightGravity, collector.slideDuration / flightKickinDuration);

                self.airFriction = Mathf.Lerp(normalAirFriction, flightAirFriction, collector.slideDuration / flightKickinDuration);

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

                        try { collector.windSound.Update(); }
                        catch (Exception e) { Plugin.DebugLog("Collector (windsound update) is being a little bitch: " + e); }
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

                    if (speed <= 1.2f)
                    {
                        collector.StopSliding();
                    }

                    if (!self.input[0].jmp || !collector.CanSustainSlide())
                    {
                        collector.StopSliding();
                    }
                }

                collector.slideStaminaRecoveryCooldown = 40;
                collector.SlideStamina--;
            }
            else
            {
                collector.windSound.Volume = Mathf.Lerp(1f, 0f, collector.timeSinceLastSlide / flightKickinDuration);

                collector.timeSinceLastSlide++;

                collector.windSound.Volume = 0f;

                if (collector.slideStaminaRecoveryCooldown > 0)
                {
                    collector.slideStaminaRecoveryCooldown--;
                }
                else
                {
                    collector.SlideStamina = Mathf.Min(collector.SlideStamina + collector.SlideRecovery, collector.SlideStaminaMax);
                }

                if (self.wantToJump > 0 && collector.SlideStamina > collector.MinimumSlideStamina && collector.CanSustainSlide() && collector.CollectorJumped)
                {
                    collector.InitiateSlide();
                }

                self.airFriction = normalAirFriction;
                self.gravity = normalGravity;

                if (collector.preventGrabs > 0)
                {
                    collector.preventGrabs--;
                }
                collector.windSound.Update();
            }
        }

    }
}