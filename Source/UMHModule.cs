using Celeste.Mod.CNetHelper;
using Celeste.Mod.CNetHelper.Data;
using Celeste.Mod.UMH.Entities;
using Celeste.Mod.UMH.Packets;
using Monocle;
using System;

namespace Celeste.Mod.UMH;

public class UMHModule : EverestModule {
    public static UMHModule Instance { get; private set; }

    public override Type SettingsType => typeof(UMHModuleSettings);
    public static UMHModuleSettings Settings => (UMHModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(UMHModuleSession);
    public static UMHModuleSession Session => (UMHModuleSession) Instance._Session;

    public override Type SaveDataType => typeof(UMHModuleSaveData);
    public static UMHModuleSaveData SaveData => (UMHModuleSaveData) Instance._SaveData;
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

    public override void Unload() {
        On.Celeste.Player.Die -= On_Death;
    }
}