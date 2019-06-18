using System;
using SplashKitSDK;
public class Isopod : GameObject
{
    const int SPEED = 3;
    Player _Target;
    int _Rotation = 0;
    double _prevX;
    double _prevY;
    Vector2D _Velocity;
    public int Health {get; set;}
    public Isopod (Window window, Player player)
    {
        _Bitmap = new Bitmap ("isopod", "Isopod1.png");
        if (SplashKit.Rnd () < 0.5)
        {
            X = SplashKit.Rnd (window.Width);
            if (SplashKit.Rnd () < 0.5) Y = -Height;
            else Y = window.Height;
        }
        else
        {
            Y = SplashKit.Rnd (window.Width);
            if (SplashKit.Rnd () < 0.5) X = -Width;
            else X = window.Width;
        }
        Point2D fromPt = new Point2D (){X = X, Y = Y};
        Point2D toPt = new Point2D (){X = player.X, Y = player.Y};
        Vector2D dir;
        dir = SplashKit.UnitVector (SplashKit.VectorPointToPoint (fromPt, toPt));
        _Velocity = SplashKit.VectorMultiply (dir, SPEED);
        Health = 30;
        _Target = player;
    }
    // Updates to chase target's X and Y coords
    public void Update (Player player)
    {
        _prevX = X;
        _prevY = Y;
        Point2D fromPt = new Point2D () { X = X, Y = Y };
        Point2D toPt = new Point2D () { X = player.X, Y = player.Y };
        Vector2D dir;
        dir = SplashKit.UnitVector (SplashKit.VectorPointToPoint (fromPt, toPt));
        _Velocity = SplashKit.VectorMultiply (dir, SPEED);
        X += Convert.ToInt32 (_Velocity.X);
        Y += Convert.ToInt32 (_Velocity.Y);
    }
    // Draws with rotation facing the target
    public void Draw (Window window)
    {
        Vector2D targetPos = new Vector2D ();
        targetPos.X = _Target.X;
        targetPos.Y = _Target.Y;
        Vector2D Pos = new Vector2D ();
        Pos.X = X;
        Pos.Y = Y;
        _Rotation = (int) SplashKit.AngleBetween (targetPos, Pos) + 90;
        _Bitmap.Draw (X, Y, SplashKit.OptionRotateBmp (_Rotation));
    }
    // Projectile collision
    public bool CollidedWith (Projectile projectile)
    {
        if (_Bitmap.CircleCollision (X, Y, projectile.CollisionCircle))
        {
            Health -= (int) projectile.Damage;
            Console.WriteLine (projectile.Damage);
            Console.WriteLine (Health);
            return true;
        }
        return false;
    }
    // Moves object back a step
    public void MoveBack ()
    {
        X = _prevX;
        Y = _prevY;
    }
}