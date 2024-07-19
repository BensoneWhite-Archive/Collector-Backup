using System.Runtime.CompilerServices;

namespace TheCollector.Extensions;

public static class CollarCWT
{
    private static readonly ConditionalWeakTable<Player, Disdain> CWT = new();
    public static Disdain Yippee(this Player player) => CWT.GetValue(player, _ => new(player));
    public class Disdain
    {
        public PorlCollection porlztorage;

        public class AbstractPorls : AbstractPhysicalObject.AbstractObjectStick
        {
            public AbstractPhysicalObject Player
            {
                get { return A; }
                set { A = value; }
            }

            public AbstractPhysicalObject DataPearl
            {
                get { return B; }
                set { B = value; }
            }

            public AbstractPorls(AbstractPhysicalObject player, AbstractPhysicalObject pearl)
                : base(player, pearl)
            {
            }
        }

        public class PorlCollection
        {
            public Player Owner;
            public Stack<DataPearl> deezNuts;
            public Stack<AbstractPorls> abstractNutz;
            public int capacity = 3;
            public bool increment;
            private int counter;
            private bool locked;

            public PorlCollection(Player owner)
            {
                if (deezNuts == null)
                {
                    deezNuts ??= new Stack<DataPearl>(capacity);
                    abstractNutz ??= new Stack<AbstractPorls>(capacity);
                }
                Owner = owner;
            }

            public void GraphicsModuleUpdated(bool actuallyViewed, bool eu)
            {
                if (deezNuts.Count > 0)
                {
                    PlayerGraphics playerGraphics = (PlayerGraphics)Owner.graphicsModule;
                    Vector2 vector1 = playerGraphics.drawPositions[0, 0];
                    Vector2 vector2 = playerGraphics.drawPositions[1, 0];
                    Vector2 pos = playerGraphics.head.pos;
                    Vector2 vector3 = new Vector2(12f, 0f);
                    Vector2 vector4 = new Vector2(-12f, -14f);
                    Vector2 vector5 = vector1 - vector2;
                    Vector2 normalized = vector5.normalized;
                    vector3 = Custom.RotateAroundOrigo(vector3, Custom.VecToDeg(normalized));
                    vector4 = Custom.RotateAroundOrigo(vector4, Custom.VecToDeg(normalized));
                    vector3 = vector1 + vector3;
                    vector4 = vector1 + vector4;
                    Vector2 vector6 = vector4 - vector3;
                    int index = 0;
                    foreach (DataPearl nut in deezNuts)
                    {
                        float porls = (index + 1f) / (deezNuts.Count + 1f);
                        Vector2 moveTo = vector3 + vector6 * porls;
                        nut.firstChunk.MoveFromOutsideMyUpdate(eu, moveTo);
                        nut.firstChunk.vel = Owner.mainBodyChunk.vel;
                        index++;
                    }
                }
            }

            public void Update(bool evenUpdates, Player self)
            {
                if (increment)
                {
                    counter++;
                    if (counter > 20 && deezNuts.Count < capacity)
                    {
                        if (deezNuts.Count == 0)
                        {
                            for (int index = 0; index < 2; index++)
                            {
                                if (Owner.grasps[index]?.grabbed is DataPearl nut)
                                {
                                    CanYouUnderstandWithoutLookingThisUp(nut, self, index);
                                    counter = 0;
                                    break;
                                }
                            }
                        }
                        else if (Owner.grasps[0]?.grabbed is DataPearl nut)
                        {
                            CanYouUnderstandWithoutLookingThisUp(nut, self);
                            counter = 0;
                        }
                    }
                    if (counter > 20 && deezNuts.Count > 0)
                    {
                        GoogleTranslateDidntWork(evenUpdates, self);
                        counter = 0;
                    }
                }
                else
                {
                    counter = 0;
                }

                if (!Owner.input[0].pckp)
                {
                    locked = false;
                }
                increment = false;
            }

            private void CanYouUnderstandWithoutLookingThisUp(DataPearl DataUnfold, Player self, int Foot = 0)
            {

                if (deezNuts.Count >= capacity)
                {
                    return;
                }


                Owner.ReleaseGrasp(Foot);
                DataUnfold.CollideWithObjects = false;
                deezNuts.Push(DataUnfold);
                locked = true;
                Owner.noPickUpOnRelease = 20;
                Owner.room?.PlaySound(TCEnums.Sound.porl, Owner.mainBodyChunk);
                abstractNutz.Push(new AbstractPorls(Owner.abstractPhysicalObject, DataUnfold.abstractPhysicalObject));
                Debug.LogWarning("Moved porl from paw to strage! Storage index is now: " + deezNuts.Count);
            }
            private void GoogleTranslateDidntWork(bool Cause, Player self)
            {

                for (int index = 0; index < Owner.grasps.Length; index++)
                {
                    if (Owner.grasps[index] is not null && Owner.Grabability(Owner.grasps[index].grabbed) >= Player.ObjectGrabability.TwoHands)
                    {
                        return;
                    }
                }

                int toPaw = Owner.FreeHand();

                if (toPaw == -1)
                {
                    return;
                }

                if (abstractNutz is not null && deezNuts is not null)
                {
                    if (toPaw != -1)
                    {
                        DataPearl nut = deezNuts.Pop();
                        AbstractPorls aNut = abstractNutz.Pop();
                        if (Owner.graphicsModule is not null && aNut is not null && nut is not null)
                        {
                            nut.firstChunk.MoveFromOutsideMyUpdate(Cause, (Owner.graphicsModule as PlayerGraphics).hands[toPaw].pos);
                        }
                        aNut?.Deactivate();
                        nut.CollideWithObjects = false;
                        Owner.SlugcatGrab(nut, toPaw);
                        locked = true;
                        Owner.ReleaseGrasp(0);
                        Owner.noPickUpOnRelease = 20;
                        Owner.room?.PlaySound(TCEnums.Sound.porl, Owner.mainBodyChunk);
                        Debug.LogWarning("Successfully moved porl from storage to paw! Storage index is now: " + deezNuts.Count);
                    }
                    else
                    {
                        Debug.LogWarning("Could not move porl from storage to paw! Storage index is now: " + deezNuts.Count);
                    }
                }
            }
        }

        public Disdain(Player player)
        {
            porlztorage = new PorlCollection(player);
        }
    }

}