using System;
using SplashKitSDK;

public class Program
{
    public static void Main ()
    {
        Window window = new Window ("game window", 1222, 720);
        ButchyMen butchyMen = new ButchyMen (window);
        window.Clear (Color.White);
        do
        {
            SplashKit.ProcessEvents ();
            butchyMen.HandleInput ();
            butchyMen.Update ();
            butchyMen.Draw ();
            window.Refresh (60);
        } while (!butchyMen.Quit);
    }
}