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

    public UMHModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(UMHModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(UMHModule), LogLevel.Info);
#endif
    }

    public override void Load() {
        // TODO: apply any hooks that should always be active
    }

    public override void Unload() {
        // TODO: unapply any hooks applied in Load()
    }
}