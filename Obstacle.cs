using System;
using System.Collections.Generic;
using SplashKitSDK;
public class Obstacle : GameObject
{
    public Obstacle (Window window)
    {
        _Bitmap = new Bitmap ("obstacle", "Rock.png");
        X = (int) SplashKit.Rnd (100, window.Width - 220);
        Y = (int) SplashKit.Rnd (100, window.Height - 220);
    }
    public void Draw (Window window)
    {
        _Bitmap.Draw (X, Y);
    }
}

public class Monument : Obstacle
{
    public Monument (Window window) : base(window)
    {
        _Bitmap = new Bitmap ("monument", "Monument.png");
        X = window.Width/2 - _Bitmap.Width/2;
        Y = window.Height/4;
    }
    public void Draw ()
    {
        _Bitmap.Draw (X, Y);
    }
    public bool HasPlayerNear (Player player)
    {
        return (_Bitmap.CircleCollision (X, Y, player.PromptCircle));
    }
}