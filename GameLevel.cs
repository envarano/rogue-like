using System;
using System.Collections.Generic;
using SplashKitSDK;

public enum DoorDirection
{
    North,
    East,
    South,
    West
}
public class GameLevel
{
    Bitmap _FloorBitmap, _FloorBitmap2;
    int _LevelCount;
    double _LevelHeight, _LevelWidth;
    Bitmap _WallBitmap, _WallBitmap2;
    List<Circle> _Doors = new List<Circle> ();
    List<Circle> _DoorCollisions = new List<Circle> ();
    List<Bitmap> _DoorBitmaps = new List<Bitmap> ();
    List<Bitmap> _OpenDoorBitmaps = new List<Bitmap> ();
    Point2D doorLoc, doorCollisionLoc;
    bool _LevelComplete;
    int _FloorNum, _WallNum, _DoorSize;
    public Vector2D PlayerStartLoc { get; set; }
    public DoorDirection DoorDir { get; set; }

    // Initializes level bitmaps
    public GameLevel (Window window, int levelCount, DoorDirection doorDir)
    {
        _FloorBitmap = new Bitmap ("floor", "floor.png");
        _FloorBitmap2 = new Bitmap ("floor2", "floor2.png");
        _WallBitmap2 = new Bitmap ("RegularWall", "RegularWalls.png");
        _WallBitmap = new Bitmap ("wall", "TreasureWalls.png");
        _DoorSize = new Bitmap ("door", "DoorN.png").Width;
        PlayerStartLoc = new Vector2D ();
        _FloorNum = SplashKit.Rnd (1, 3);
        _WallNum = SplashKit.Rnd (1, 3);
        _LevelComplete = false;
        _LevelHeight = window.Height;
        _LevelWidth = window.Width;
        _LevelCount = levelCount;
        int doorCount = SplashKit.Rnd (3) + 1;
        CreateDoor (doorDir, window);
        for (int i = 1; i < doorCount; i++)
        {
            if (SplashKit.Rnd () > 0.75) doorDir = DoorDirection.North;
            else if (SplashKit.Rnd () > 0.5) doorDir = DoorDirection.East;
            else if (SplashKit.Rnd () > 0.25) doorDir = DoorDirection.South;
            else doorDir = DoorDirection.West;
            CreateDoor (doorDir, window);
        }
    }
    /*
        CreateDoor(DoorDirection, Window) Creates a door in a north, south, east or west direction

        @doorDir : DoorDirection                  enum selection of the direction e.g: DoorDirection.North
        @window : Window                            Window sizing for door positions
    */
    void CreateDoor (DoorDirection doorDir, Window window)
    {
        Circle door = new Circle ();
        Circle doorCollisions = new Circle ();
        door.Radius = 75;
        doorCollisions.Radius = 150;
        doorLoc = new Point2D ();
        doorCollisionLoc = new Point2D ();
        switch (doorDir)
        {
            case DoorDirection.North:
                _DoorBitmaps.Add (new Bitmap ("northDoor", "doorN.png"));
                _OpenDoorBitmaps.Add (new Bitmap ("northDoorOpen", "doorOpenN.png"));
                doorLoc.X = window.Width / 2 - _DoorBitmaps[_DoorBitmaps.Count - 1].Width / 2;
                doorLoc.Y = 0;
                doorCollisionLoc.X = window.Width / 2;
                doorCollisionLoc.Y = -25;
                break;
            case DoorDirection.East:
                _DoorBitmaps.Add (new Bitmap ("eastDoor", "doorE.png"));
                _OpenDoorBitmaps.Add (new Bitmap ("eastDoorOpen", "doorOpenE.png"));
                doorLoc.X = window.Width - _DoorBitmaps[_DoorBitmaps.Count - 1].Width;
                doorLoc.Y = window.Height / 2 - _DoorBitmaps[_DoorBitmaps.Count - 1].Height / 2;
                doorCollisionLoc.X = window.Width;
                doorCollisionLoc.Y = window.Height / 2;
                break;
            case DoorDirection.South:
                _DoorBitmaps.Add (new Bitmap ("southDoor", "doorS.png"));
                _OpenDoorBitmaps.Add (new Bitmap ("southDoorOpen", "doorOpenS.png"));
                doorLoc.X = window.Width / 2 - _DoorBitmaps[_DoorBitmaps.Count - 1].Width / 2;
                doorLoc.Y = window.Height - _DoorBitmaps[_DoorBitmaps.Count - 1].Height;
                doorCollisionLoc.X = window.Width / 2;
                doorCollisionLoc.Y = window.Height + 25;
                break;
            case DoorDirection.West:
                _DoorBitmaps.Add (new Bitmap ("westDoor", "doorW.png"));
                _OpenDoorBitmaps.Add (new Bitmap ("westDoorOpen", "doorOpenW.png"));
                doorLoc.X = 0;
                doorLoc.Y = window.Height / 2 - _DoorBitmaps[_DoorBitmaps.Count - 1].Height / 2;
                doorCollisionLoc.X = 0;
                doorCollisionLoc.Y = window.Height / 2;
                break;
        }
        door.Center = doorLoc;
        doorCollisions.Center = doorCollisionLoc;
        _Doors.Add (door);
        _DoorCollisions.Add (doorCollisions);
    }
    // Draws the randomly selected walls, floors and doors.
    public void Draw ()
    {
        if (_FloorNum == 1) _FloorBitmap.Draw (0, 0);
        else _FloorBitmap2.Draw (0, 0);
        if (_WallNum == 1) _WallBitmap.Draw (0, 0);
        else _WallBitmap2.Draw (0, 0);
        for (int i = 0; i < _DoorBitmaps.Count; i++)
        {
            if (_LevelComplete) _OpenDoorBitmaps[i].Draw (_Doors[i].Center.X, _Doors[i].Center.Y);
            else _DoorBitmaps[i].Draw (_Doors[i].Center.X, _Doors[i].Center.Y);
        }
    }
    /*
        nextLevelStarting(Player, Window) checks if the player is attempting to start the next level

        @player : Player                    checks coords of player for entering door
        @window : Window            checks window sizing for new player coords on next level
    */
    public bool nextLevelStarting (Player player, Window window)
    {
        if (_LevelComplete)
            foreach (Circle door in _DoorCollisions)
                // Entered West door
                if (player.BMap.CircleCollision (player.X, player.Y, door))
                {
                    if (door.Center.X > window.Width / 2 + _DoorBitmaps[0].Width)
                    {
                        if (SplashKit.KeyDown (KeyCode.DKey))
                        {
                            PlayerStartLoc = new Vector2D{ X = door.Center.X-window.Width, Y = door.Center.Y};
                            DoorDir = DoorDirection.West;
                            return true;
                        }
                    }
                    // Entered South door
                    else if (door.Center.Y > window.Height / 2 + _DoorBitmaps[0].Height)
                    {
                        if (SplashKit.KeyDown (KeyCode.SKey))
                        {
                            PlayerStartLoc = new Vector2D{ X = door.Center.X, Y = door.Center.Y - window.Height};
                            DoorDir = DoorDirection.North;
                            return true;
                        }
                    }
                    // Entered East Door
                    else if (door.Center.X < window.Width / 2 - _DoorBitmaps[0].Width)
                    {
                        if (SplashKit.KeyDown (KeyCode.AKey))
                        {
                            PlayerStartLoc = new Vector2D{ X = door.Center.X + window.Width, Y = door.Center.Y};
                            DoorDir = DoorDirection.East;
                            return true;
                        }
                    }
                    // Entered North Door
                    else if (door.Center.Y < window.Height / 2 - _DoorBitmaps[0].Height)
                    {
                        if (SplashKit.KeyDown (KeyCode.WKey))
                        {
                            PlayerStartLoc = new Vector2D{ X = door.Center.X, Y = door.Center.Y + window.Height};
                            DoorDir = DoorDirection.South;
                            return true;
                        }
                    }
                }
        return false;
    }
    public void Update (int isopodCount)
    {
        _LevelComplete = (isopodCount == 0);
    }
}