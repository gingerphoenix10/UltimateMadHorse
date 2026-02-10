using Celeste.Mod.CelesteNet.Client.Entities;
using Celeste.Mod.Entities;
using Celeste.Mod.UMH.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.UMH.Triggers;

[Tracked]
[CustomEntity("UMH/UMHStartZone")]
public class UMHStartZone : CNetTrigger
{
    StartMatchButton button;
    public string arenaName;
    public int StartID;
    public UMHStartZone(EntityData data, Vector2 offset) : base(data, offset)
    {
        this.Visible = true;

        button = new StartMatchButton(Position + new Vector2(Width/2 - 8, Height - 8), data.Level, this);
        arenaName = data.Attr("room", "arena");
        StartID = data.ID;
    }

    public override void Added(Scene scene)
    {
        scene.Add(button);
        base.Added(scene);
    }

    public override void OnPlayerLeft(Actor player)
    {
        base.OnPlayerLeft(player);
        string players = "";
        foreach (Actor plr in playersInside)
        {
            string usrname = (plr is Ghost ghost) ? ghost.PlayerInfo.Name : "localplayer";
            players += (players.Length == 0 ? "" : ", ") + usrname;
        }
        Console.WriteLine(players);
    }

    public override void OnPlayerEntered(Actor player)
    {
        base.OnPlayerEntered(player);
        Console.WriteLine("ID: "+StartID);
        string players = "";
        foreach (Actor plr in playersInside)
        {
            string usrname = (plr is Ghost ghost) ? ghost.PlayerInfo.Name : "localplayer";
            players += (players.Length == 0 ? "" : ", ") + usrname;
        }
        Console.WriteLine(players);
        Session ses = SceneAs<Level>().Session;
        Console.Write($"{ses.Area}, {ses.Level}");
    }

    public override void Render()
    {
        Collider.Render(SceneAs<Level>().Camera, Color.Red);
        Draw.Rect(Position, Width, Height, Color.DarkRed * 0.5f);
        base.Render();
    }
}