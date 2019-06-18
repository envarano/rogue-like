using System;
using System.Collections.Generic;
using SplashKitSDK;
class ButchyMen
{
    Player _Player;
    GameLevel _GameLevel;
    Monument _startMonument;
    int levelCount = 1;
    List<Isopod> _Isopods = new List<Isopod> ();
    List<Item> _Items = new List<Item> ();
    List<Obstacle> _Obstacles = new List<Obstacle> ();
    Window _Window;
    public bool Quit
    {
        get
        {
            return _Player.Quit;
        }
    }
    public ButchyMen (Window window)
    {
        _Window = window;
        _GameLevel = new GameLevel (window, levelCount, DoorDirection.South);
        _Player = new Player (window);
        // Create rand num of isopods
        int isopodCount = SplashKit.Rnd (3) + 1;
        for (int i = 0; i < isopodCount; i++) _Isopods.Add (new Isopod (window, _Player));
        // Create rand num of obstacles
        int obstacleCount = SplashKit.Rnd (5) + 1;
        for (int i = 0; i < obstacleCount; i++) _Obstacles.Add (new Obstacle (window));
        // Removal of obstacles that spawn on top of each other
        List<Obstacle> removeObstacles = new List<Obstacle> ();
        foreach (Obstacle obstaclei in _Obstacles)
        {
            foreach (Obstacle obstaclej in _Obstacles)
                if (obstaclei != obstaclej & obstaclei.CollidedWith (obstaclej))
                    removeObstacles.Add (obstaclei);
            if (_Player.CollidedWith (obstaclei))
                removeObstacles.Add (obstaclei);
        }
        foreach (Obstacle obstacle in removeObstacles)
            _Obstacles.Remove (obstacle);
        if(levelCount == 1)_startMonument = new Monument(window);
    }
    public void HandleInput ()
    {
        _Player.HandleInput (_Window);
        _Player.StayOnWindow (_Window);
    }
    public void Update ()
    {
        checkCollisions ();
        foreach (Isopod isopod in _Isopods) isopod.Update (_Player);
        // Istructions to be completed for next level
        if (_GameLevel.nextLevelStarting (_Player, _Window))
        {
            _Player.Update (_GameLevel, _Window);
            levelCount++;
            _GameLevel = new GameLevel (_Window, levelCount, _GameLevel.DoorDir);
            // Add new game objects and assign over old ones
            _Isopods = new List<Isopod> ();
            int isopodCount = SplashKit.Rnd (3) + 1;
            for (int i = 0; i < isopodCount; i++) _Isopods.Add (new Isopod (_Window, _Player));
            _Items = new List<Item> ();
            _Obstacles = new List<Obstacle> ();
            int obstacleCount = SplashKit.Rnd (5) + 1;
            for (int i = 0; i < obstacleCount; i++) _Obstacles.Add (new Obstacle (_Window));
            // Removal of overlapping obstacles
            List<Obstacle> removeObstacles = new List<Obstacle> ();
            foreach (Obstacle obstaclei in _Obstacles)
            {
                foreach (Obstacle obstaclej in _Obstacles)
                {
                    if (obstaclei != obstaclej & obstaclei.CollidedWith (obstaclej))
                        removeObstacles.Add (obstaclei);
                }
                if (_Player.CollidedWith (obstaclei))
                    removeObstacles.Add (obstaclei);
            }
            foreach (Obstacle obstacle in removeObstacles)
                _Obstacles.Remove (obstacle);
        }
        _GameLevel.Update (_Isopods.Count);
        _Player.Update (_GameLevel, _Window);
    }
    void checkCollisions ()
    {
        List<Isopod> removeIsopods = new List<Isopod> ();
        List<Item> itemsPickedUp = new List<Item> ();
        foreach (Isopod isopod in _Isopods)
        {
            if(_Player.CollidedWith (isopod))
            {
                _Player.TakeDamage(10);
                if(_Player.Health <= 0)
                {
                    levelCount = 1;
                    _Player = new Player(_Window);
                    _GameLevel = new GameLevel (_Window, levelCount, _GameLevel.DoorDir);
                }
            }
            if (_Player.BulletCollision (isopod))
            {
                isopod.Health -= (int) _Player.Damage;
                if (isopod.Health <= 0) removeIsopods.Add (isopod);
            }
        }
        foreach (Obstacle obstacle in _Obstacles)
        {
            if (_Player.CollidedWith (obstacle)) _Player.MoveBack ();
            _Player.BulletCollision (obstacle);
            foreach (Isopod isopod in _Isopods)
                if (isopod.CollidedWith (obstacle)) isopod.MoveBack ();
        }
        // Spawns a random item at a random location on isopod death
        foreach (Isopod isopod in removeIsopods)
        {
            _Isopods.Remove (isopod);
            int randItemNum = SplashKit.Rnd (6) + 1;
            switch (randItemNum)
            {
                case 1:
                    _Items.Add (new ButchyBoot (_Window));
                    break;
                case 2:
                    _Items.Add (new DemonicButchyBoot (_Window));
                    break;
                case 3:
                    _Items.Add (new FireFly (_Window));
                    break;
                case 4:
                    _Items.Add (new FireAntSoul (_Window));
                    break;
                case 5:
                    _Items.Add (new BulletAntSoul (_Window));
                    break;
                case 6:
                    _Items.Add (new PapaLongLegsThread (_Window));
                    break;
            }
        }
        foreach (Item item in _Items)
            if (_Player.CollidedWith (item)) itemsPickedUp.Add (item);

        foreach (Item item in itemsPickedUp)
        {
            _Player.StoreItem (item);
            _Items.Remove (item);
        }
        
    }
    public void Draw ()
    {
        _Window.Clear (Color.White);
        _GameLevel.Draw ();
        _Player.Draw (_Window);
        foreach (Isopod isopod in _Isopods) isopod.Draw (_Window);
        foreach (Item item in _Items) item.Draw (_Window);
        foreach (Obstacle obstacle in _Obstacles) obstacle.Draw (_Window);
        SplashKit.DrawText (_Player.Health.ToString () + "%", Color.White, 188, 70);
        if(levelCount == 1)
        {
            _startMonument.Draw();
            if(_startMonument.HasPlayerNear(_Player))
            {
                SplashKit.DrawText ("Monument De Butch: 'e'", Color.White, _startMonument.X-_startMonument.Width/8, _startMonument.Y-_startMonument.Width/8);
                if (SplashKit.KeyDown (KeyCode.EKey)) SplashKit.DrawBitmap("ButchyHistory.png", _Window.Width/20, _Window.Height/20);
            }
        }
        _Window.Refresh ();
    }
}