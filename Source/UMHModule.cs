using Celeste.Mod.CNetHelper;
using Celeste.Mod.CNetHelper.Data;
using Celeste.Mod.UMH.Entities;
using Celeste.Mod.UMH.Packets;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.UMH;

public class UMHModule : EverestModule {
    public static UMHModule Instance { get; private set; }

    public static string currentMap = "";
    public static string currentRoom = ""; // Since we're checking for a UCHManager as well, this should be good enough for ensuring we're in the same level, same room

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

    public override void Load()
    {
        On.Celeste.Player.Die += On_Death;
        On.Celeste.Player.OnTransition += On_Transition;
        On.Celeste.LevelLoader.StartLevel += On_StartLevel;
        CNetHelperModule.RegisterType<ObjectPlace>(ObjectPlace.Receive);
        CNetHelperModule.OnError += (CNetHelperError error) =>
        {
            Engine.Commands.Log($"ERROR: {error.errorType}");
        };
    }

    private static PlayerDeadBody On_Death(On.Celeste.Player.orig_Die orig, Player self, Microsoft.Xna.Framework.Vector2 direction, bool evenIfInvincible, bool registerDeathInStats)
    {
        if (!Invincible)
            return orig(self, direction, evenIfInvincible, registerDeathInStats);
        return null;
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
}