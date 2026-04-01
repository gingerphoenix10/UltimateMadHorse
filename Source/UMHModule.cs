using Celeste.Mod.CelesteNet;
using Celeste.Mod.CelesteNet.Client;
using Celeste.Mod.CelesteNet.Client.Components;
using Celeste.Mod.CelesteNet.Client.Entities;
using Celeste.Mod.CelesteNet.DataTypes;
using Celeste.Mod.CNetHelper;
using Celeste.Mod.CNetHelper.Data;
using Celeste.Mod.UMH.Entities;
using Celeste.Mod.UMH.Packets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static Celeste.GaussianBlur;
using static Celeste.TrackSpinner;

namespace Celeste.Mod.UMH;

public class UMHModule : EverestModule {
    public static UMHModule Instance { get; private set; }

    public static string currentMap = "";
    public static string currentRoom = ""; // Since we're checking for a UCHManager as well, this should be good enough for ensuring we're in the same level, same room
    public static Hook interactHook; // Since we're checking for a UCHManager as well, this should be good enough for ensuring we're in the same level, same room
    public static Hook ghostHandleGraphicsHook;
    public static Hook ghostHandleDataHook;

    public static bool Invincible
    {
        get
        {
            UMHManager UMH = Engine.Scene.Tracker.GetEntity<UMHManager>();
            if (UMH != null)
                return UMH.mouse.Active;
            return false;
        }
    }

    public UMHModule() {
        Instance = this;
        Logger.SetLogLevel(nameof(UMHModule), LogLevel.Verbose);
    }

    public override async void Load()
    {
        On.Celeste.Player.Die += On_Death;
        On.Celeste.Player.OnTransition += On_Transition;
        On.Celeste.Player.Render += On_Player_Render;
        On.Celeste.LevelLoader.StartLevel += On_StartLevel;
        On.Celeste.PlayerDeadBody.End += On_DieEnd;
        On.Celeste.OuiTitleScreen.ctor += On_Start;
        //On.Monocle.EntityList.Render += On_EntityRender;
        interactHook = new(typeof(Ghost).GetMethod(nameof(Ghost.OnPlayer)), GetType().GetMethod(nameof(On_GhostInteract), BindingFlags.NonPublic | BindingFlags.Instance), this);
        ghostHandleGraphicsHook = new(typeof(CelesteNetMainComponent).GetMethod(nameof(CelesteNetMainComponent.Handle), new Type[] { typeof(CelesteNetConnection), typeof(DataPlayerGraphics) }), GetType().GetMethod(nameof(On_GhostHandleGraphics), BindingFlags.NonPublic | BindingFlags.Instance), this);
        ghostHandleDataHook = new(typeof(CelesteNetMainComponent).GetMethod(nameof(CelesteNetMainComponent.Handle), new Type[] { typeof(CelesteNetConnection), typeof(DataPlayerFrame) }), GetType().GetMethod(nameof(On_GhostHandleData), BindingFlags.NonPublic | BindingFlags.Instance), this);
        CNetHelperModule.RegisterType<ObjectPlace>(ObjectPlace.Receive);
        CNetHelperModule.RegisterType<MatchStart>(MatchStart.Receive);
        CNetHelperModule.RegisterType<MatchJoin>(MatchJoin.Receive);
        CNetHelperModule.RegisterType<MatchLeave>(MatchLeave.Receive);
        CNetHelperModule.RegisterType<ObjectPick>(ObjectPick.Receive);
        CNetHelperModule.OnError += (CNetHelperError error) =>
        {
            Engine.Commands.Log($"ERROR: {error.errorType}");
        };
        //nameof(OuiTitleScreen.Enter)
    }

    private static void On_Start(On.Celeste.OuiTitleScreen.orig_ctor orig, OuiTitleScreen self)
    {
        orig(self);
        CelesteNetClientModule.Instance.Start();
    }

    private static void On_EntityRender(On.Monocle.EntityList.orig_Render orig, EntityList self)
    {
        orig(self);
        /*foreach (Entity entity in self.entities)
        {
            if (entity.Visible && entity is ItemsBox)
                entity.Render();
        }*/
    }

    private void On_GhostInteract(Action<Ghost, Player> orig, Ghost self, Player player)
    {
        //Console.WriteLine(self.PlayerInfo.ID);
        orig(self, player);
    }

    private void On_GhostHandleData(Action<CelesteNetMainComponent, CelesteNetConnection, DataPlayerFrame> orig, CelesteNetMainComponent self, CelesteNetConnection con, DataPlayerFrame frame)
    {
        if (UMHManager.matchID != -1 && !UMHManager.players.Contains(frame.Player.ID))
                return;

        orig(self, con, frame);
    }

    private void On_GhostHandleGraphics(Action<CelesteNetMainComponent, CelesteNetConnection, DataPlayerGraphics> orig, CelesteNetMainComponent self, CelesteNetConnection con, DataPlayerGraphics graphics)
    {
        if (UMHManager.matchID != -1 && !UMHManager.players.Contains(graphics.Player.ID))
            return;

        orig(self, con, graphics);
    }

    private static void On_DieEnd(On.Celeste.PlayerDeadBody.orig_End orig, PlayerDeadBody self)
    {
        orig(self);
        return;
        var nothing = nameof(PlayerDeadBody.End);

        if (!self.finished)
        {
            self.finished = true;
            if (self.DeathAction == null)
            {
                self.DeathAction = () =>
                {
                    PlayerSpriteMode spriteMode = ((!self.SceneAs<Level>().Session.Inventory.Backpack) ? PlayerSpriteMode.MadelineNoBackpack : PlayerSpriteMode.Madeline);
                    Player player = new((Vector2)self.SceneAs<Level>().StartPosition, spriteMode);
                    player.IntroType = Player.IntroTypes.Respawn;
                    self.Scene.Add(player);

                    self.SceneAs<Level>().Entities.UpdateLists();
                    foreach (EntityID key in self.SceneAs<Level>().Session.Keys)
                    {
                        self.SceneAs<Level>().Add(new Key(player, key));
                    }
                };
            }
        }
    }

    private static void Respawn()
    {
       
    }

    private static PlayerDeadBody On_Death(On.Celeste.Player.orig_Die orig, Player self, Microsoft.Xna.Framework.Vector2 direction, bool evenIfInvincible, bool registerDeathInStats)
    {
        if (Invincible)
            return null;

        var manager = self.Scene.Tracker.GetEntity<UMHManager>();
        if (manager != null)
        {
            if (self.Scene.Tracker.GetEntity<FakeDeadBody>() == null)
            {
                FakeDeadBody body = new(self, Vector2.Zero);
                self.Scene.Add(body);
            }
            return null;
        }

        return orig(self, direction, evenIfInvincible, registerDeathInStats);
    }

    private static void On_Transition(On.Celeste.Player.orig_OnTransition orig, Player self)
    {
        currentMap = self.level.Session.Area.SID;
        currentRoom = self.level.Session.Level;
        orig(self);
    }

    private static void On_StartLevel(On.Celeste.LevelLoader.orig_StartLevel orig, LevelLoader self)
    {
        currentMap = self.Level.Session.Area.SID;
        currentRoom = self.Level.Session.Level;
        orig(self);
    }

    public override void Unload()
    {
        On.Celeste.Player.Die -= On_Death;
        On.Celeste.Player.OnTransition -= On_Transition;
        On.Celeste.LevelLoader.StartLevel -= On_StartLevel;
    }

    private static void On_Player_Render(On.Celeste.Player.orig_Render orig, Player self)
    {
        if (SaveData.Instance.Assists.InvisibleMotion && self.InControl && ((!self.onGround && self.StateMachine.State != 1 && self.StateMachine.State != 3) || self.Speed.LengthSquared() > 800f))
        {
            return;
        }

        Vector2 renderPosition = self.Sprite.RenderPosition;
        self.Sprite.RenderPosition = self.Sprite.RenderPosition.Floor();
        if (self.StateMachine.State == 14)
        {
            DeathEffect.Draw(self.Center + self.deadOffset, self.Hair.Color, self.introEase);
        }
        else
        {
            if (self.StateMachine.State != 19)
            {
                if (self.IsTired && self.flash)
                {
                    self.Sprite.Color = Color.Red;
                }
                else
                {
                    self.Sprite.Color = Color.White;
                }
            }

            var manager = self.Scene.Tracker.GetEntity<UMHManager>();
            if (manager != null && manager.dead)
            {
                self.Sprite.Color = Color.Red * 0.5f;
                self.Hair.Alpha = 0.5f;
                self.ForceCameraUpdate = true;
                if (self.Top > ((float)self.SceneAs<Level>().Bounds.Bottom+4))
                {
                    self.Visible = false;
                    self.Ac = false;
                    self.ForceCameraUpdate = false;
                    return;
                }
            }

            if (self.reflection.IsRendering && self.FlipInReflection)
            {
                self.Facing = (Facings)(0 - self.Facing);
                self.Hair.Facing = self.Facing;
            }

            self.Sprite.Scale.X *= (float)self.Facing;
            if (self.sweatSprite.LastAnimationID == "idle")
            {
                self.sweatSprite.Scale = self.Sprite.Scale;
            }
            else
            {
                self.sweatSprite.Scale.Y = self.Sprite.Scale.Y;
                self.sweatSprite.Scale.X = Math.Abs(self.Sprite.Scale.X) * (float)Math.Sign(self.sweatSprite.Scale.X);
            }

            self.Components.Render(); // Replacement to base.Render(); since we can't access base from a patch
            if (self.Sprite.CurrentAnimationID == "startStarFly")
            {
                float num = (float)self.Sprite.CurrentAnimationFrame / (float)self.Sprite.CurrentAnimationTotalFrames;
                GFX.Game.GetAtlasSubtexturesAt("characters/player/startStarFlyWhite", self.Sprite.CurrentAnimationFrame).Draw(self.Sprite.RenderPosition, self.Sprite.Origin, self.starFlyColor * num, self.Sprite.Scale, self.Sprite.Rotation, SpriteEffects.None);
            }

            self.Sprite.Scale.X *= (float)self.Facing;
            if (self.reflection.IsRendering && self.FlipInReflection)
            {
                self.Facing = (Facings)(0 - self.Facing);
                self.Hair.Facing = self.Facing;
            }
        }

        self.Sprite.RenderPosition = renderPosition;
    }
}