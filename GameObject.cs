using System;
using System.Collections.Generic;
using SplashKitSDK;
// Parent class to all objects who need coordinates and collision
public abstract class GameObject
{
    protected const int WALL_SIZE = 130;
    protected Bitmap _Bitmap;
    public double X { get; protected set; }
    public double Y { get; protected set; }
    public Circle CollisionCircle
    {
        get
        {
            Point2D circleMiddle;
            circleMiddle.X = X + (Width / 2);
            circleMiddle.Y = Y + (Height / 2);
            return SplashKit.CircleAt (circleMiddle, Width / 2);
        }
    }

    public bool CollidedWith (GameObject gameObject)
    {
        return (_Bitmap.CircleCollision (X, Y, gameObject.CollisionCircle));
    }
    public Bitmap BMap
    {
        get
        {
            return _Bitmap;
        }
    }

    public int Width
    {
        get
        {
            return _Bitmap.Width;
        }
    }
    public int Height
    {
        get
        {
            return _Bitmap.Height;
        }
    }
}