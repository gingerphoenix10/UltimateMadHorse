using Celeste.Mod.CNetHelper;
using Celeste.Mod.Entities;
using Celeste.Mod.UMH.Packets;
using Celeste.Mod.UMH.Triggers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.UMH.Entities;


[CustomEntity("UMH/StartMatchButton")]
[Tracked(false)]
public class StartMatchButton : DashSwitch
{
    public UMHStartZone trigger;
    public StartMatchButton(Vector2 position, LevelData level, UMHStartZone trigger)
        : base(position, Sides.Down, false, false, new EntityID(level.Name, Calc.Random.Next()), "mirror")
    {
        this.trigger = trigger;
    }

    public override void Update()
    {
        base.Update();
        if (pressed)
        {
            UMHManager.matchID = Calc.Random.Next(int.MaxValue);
            var startmsg = new MatchStart(UMHManager.matchID, trigger.StartID, UMHModule.currentMap, UMHModule.currentRoom);
            CNetHelperModule.Send(startmsg, false);

            Session ses = SceneAs<Level>().Session;
            AreaData.Get(ses).DoScreenWipe(Scene, false, () => {
                ses.Level = trigger.arenaName;
                Celeste.Scene = new LevelLoader(ses);
            });
            //nameof(Player.orig_Die)
            Active = false;
        }
    }
}