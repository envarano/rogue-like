using System;
using System.Collections.Generic;
using SplashKitSDK;

public enum ProjectileType
{
    bullet,
    flame
}
public abstract class Projectile : GameObject
{
    public Vector2D Velocity { get; private set; }
    public int Rotation { get; private set; }
    public double Damage {get; private set;}
    const int BASE_SPEED = 6;
    public Projectile (Window window, Player player)
    {
        _Bitmap = new Bitmap ("bullet", "bullet.png");
        X = player.X;
        Y = player.Y;
        Point2D fromPt = new Point2D () { X = X, Y = Y };
        Point2D toPt = new Point2D () { X = SplashKit.MousePosition ().X, Y = SplashKit.MousePosition ().Y };
        Vector2D dir;
        dir = SplashKit.UnitVector (SplashKit.VectorPointToPoint (fromPt, toPt));
        Velocity = SplashKit.VectorMultiply (dir, BASE_SPEED + player.Haste);
        Vector2D targetPos = new Vector2D ();
        targetPos.X = SplashKit.MousePosition ().X;
        targetPos.Y = SplashKit.MousePosition ().Y;
        Vector2D Pos = new Vector2D ();
        Pos.X = player.X;
        Pos.Y = player.Y;
        Rotation = (int) SplashKit.AngleBetween (targetPos, Pos) + 90;
        _Bitmap.Draw (X, Y, SplashKit.OptionRotateBmp (Rotation));
        Damage = player.Damage;
    }
    public void Draw ()
    {
        _Bitmap.Draw (X, Y, SplashKit.OptionRotateBmp (Rotation));
    }
    public void Update ()
    {
        X += Convert.ToInt32 (Velocity.X);
        Y += Convert.ToInt32 (Velocity.Y);
    }
    public bool CollidedWithWall (Window limit)
    {
        if (X < WALL_SIZE) return true;
        else if (X + (_Bitmap.Width) > limit.Width - WALL_SIZE) return true;
        else if (Y < WALL_SIZE / 2) return true;
        else if (Y + (_Bitmap.Height) > limit.Height - WALL_SIZE / 2) return true;
        return false;
    }
}
public class Flame : Projectile
{
    public Flame (Window window, Player player) : base (window, player)
    {
        _Bitmap = new Bitmap ("flame", "flame.png");
    }
}
public class Bullet : Projectile
{
    public Bullet (Window window, Player player) : base (window, player)
    {
        _Bitmap = new Bitmap ("bullet", "bullet.png");
    }
}