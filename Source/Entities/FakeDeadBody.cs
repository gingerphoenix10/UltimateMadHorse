using Celeste.Mod.Entities;
using CelesteMod.Publicizer;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.UMH.Entities;

[CustomEntity("UMH/FakeDeadBody")]
[Tracked(true)]
public class FakeDeadBody : Entity
{
    public Action DeathAction;

    public float ActionDelay;

    public bool HasGolden;

    public Color initialHairColor;
    public Vector2 bounce = Vector2.Zero;
    public Player player;
    public PlayerHair hair;
    public PlayerSprite sprite;
    public VertexLight light;
    public DeathEffect deathEffect;
    public Facings facing;
    public float scale = 1f;
    public bool finished;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public FakeDeadBody(Player player, Vector2 direction)
    {
        base.Depth = -1000000;
        this.player = player;
        facing = player.Facing;
        Position = player.Position;

        hair = player.Hair;
        sprite = player.Sprite;
        light = player.Light;
        
        sprite.Color = Color.White;
        initialHairColor = hair.Color;

        bounce = direction;
        Add(new Coroutine(DeathRoutine()));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (!(bounce != Vector2.Zero))
        {
            return;
        }

        if (Math.Abs(bounce.X) > Math.Abs(bounce.Y))
        {
            sprite.Play("deadside");
            facing = (Facings)(-Math.Sign(bounce.X));
            return;
        }

        bounce = Calc.AngleToVector(Calc.AngleApproach(bounce.Angle(), new Vector2(0 - player.Facing, 0f).Angle(), 0.5f), 1f);
        if (bounce.Y < 0f)
        {
            sprite.Play("deadup");
        }
        else
        {
            sprite.Play("deaddown");
        }
    }

    public IEnumerator DeathRoutine()
    {
        var manager = Scene.Tracker.GetEntity<UMHManager>();
        if (manager != null)
            manager.dead = true;
        /*
         *      case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 23:
                case 25:
                    return false;
        */
        Vector2 speed = player.Speed;
        player.StateMachine.state = 11;
        player.StateMachine.Locked = true;
        player.DummyFriction = false;
        player.Speed = speed;
        Level level = SceneAs<Level>();
        if (bounce != Vector2.Zero)
        {
            Audio.Play("event:/char/madeline/predeath", Position);
            scale = 1.5f;
            Celeste.Freeze(0.05f);
            yield return null;
            Vector2 from = Position;
            Vector2 to = from + bounce * 24f;
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.5f, start: true);
            Add(tween);
            tween.OnUpdate = (Tween t) =>
            {
                Position = from + (to - from) * t.Eased;
                scale = 1.5f - t.Eased * 0.5f;
                sprite.Rotation = (float)(Math.Floor(t.Eased * 4f) * 6.2831854820251465);
            };
            yield return tween.Duration * 0.75f;
            tween.Stop();
        }

        Position += Vector2.UnitY * -5f;
        level.Displacement.AddBurst(Position, 0.3f, 0f, 80f);
        level.Shake();
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
        Audio.Play(HasGolden ? "event:/new_content/char/madeline/death_golden" : "event:/char/madeline/death", Position);
        deathEffect = new DeathEffect(initialHairColor, Center - Position);
        deathEffect.OnUpdate = delegate (float f)
        {
            light.Alpha = 1f - f;
        };
        Add(deathEffect);
        yield return deathEffect.Duration * 0.65f;
        if (ActionDelay > 0f)
        {
            yield return ActionDelay;
        }
    }

    bool flashed = false;
    public override void Update()
    {
        base.Update();

        if (sprite.CurrentAnimationFrame == 0 && !flashed)
            hair.Color = Color.White;
        else if (!flashed)
        {
            hair.Color = initialHairColor;
            flashed = true;
        }
    }

    public override void Render()
    {
        if (deathEffect == null)
        {
            sprite.Scale.X = (float)facing * scale;
            sprite.Scale.Y = scale;
            hair.Facing = facing;
            base.Render();
        }
        else
        {
            deathEffect.Render();
        }
    }
}