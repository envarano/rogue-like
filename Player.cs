using System;
using System.Collections.Generic;
using SplashKitSDK;

public class Player : GameObject
{
    const int BASE_SPEED = 5;
    const int BASE_DAMAGE = 10;
    const int BASE_HEALTH = 0;
    const int BASE_HASTE = 0;
    const int SHOOT_CAP = 500;
    Bitmap _HealthBitmap;
    Bitmap _HealthBarBitmap;
    double _Speed = 5;
    double _Haste = 0;
    double _Dexterity = 0;
    int _Damage = 10;
    uint shootCooldown = 0;
    uint DamageCooldown = 0;
    uint _Rotation = 0;
    ProjectileType _ProjectileType;
    Timer _AliveTime;
    public double prevX { get; private set; }
    public double prevY { get; private set; }
    public int _Health;
    public bool Quit { get; private set; }
    List<Projectile> _ProjectileList = new List<Projectile> ();
    List<Item> _Items = new List<Item> ();

    public List<Projectile> ProjectileList
    {
        get
        {
            return _ProjectileList;
        }
    }
    public double Damage
    {
        get
        {
            return BASE_DAMAGE + _Damage;
        }
    }
    public int Health
    {
        get
        {
            return BASE_HEALTH + _Health;
        }
    }
    public double Haste
    {
        get
        {
            return BASE_HASTE + _Haste;
        }
    }
    public Circle PromptCircle
    {
        get
        {
            Point2D circleMiddle;
            circleMiddle.X = X + (Width / 2);
            circleMiddle.Y = Y + (Height / 2);
            return SplashKit.CircleAt (circleMiddle, Width * 2);
        }
    }
    public Player (Window window)
    {
        _Bitmap = new Bitmap ("player", "IsopodPlayer.png");
        _HealthBarBitmap = new Bitmap ("healthBar", "healthBarEmpty.png");
        _HealthBitmap = new Bitmap ("health", "healthVial.png");
        X = (window.Width - Width) / 2;
        Y = (window.Height - Height) / 2;
        _Health = 100;
        _AliveTime = new Timer ("Alive Time");
        _AliveTime.Start ();
        _ProjectileType = ProjectileType.bullet;
    }
    // Draws player and projectiles
    public void Draw (Window window)
    {
        _Bitmap.Draw (X, Y, SplashKit.OptionRotateBmp (_Rotation));
        foreach (Projectile projectile in _ProjectileList) projectile.Draw ();
        _HealthBarBitmap.Draw (50, 50);
        _HealthBitmap.Draw (50, 50, SplashKit.OptionPartBmp (0, 0, _HealthBitmap.Width * (_Health / 100.0), _HealthBitmap.Height));
    }
    // Handles keyboard input
    public void HandleInput (Window gameWindow)
    {
        prevX = X;
        prevY = Y;
        if (SplashKit.KeyDown (KeyCode.WKey))
        {
            Y -= _Speed;
            _Rotation = 180;
            if (SplashKit.KeyDown (KeyCode.AKey))
            {
                X -= _Speed;
                _Rotation = 135;
            }
            else if (SplashKit.KeyDown (KeyCode.DKey))
            {
                X += _Speed;
                _Rotation = 225;
            }
        }
        else if (SplashKit.KeyDown (KeyCode.AKey))
        {
            X -= _Speed;
            _Rotation = 90;
            if (SplashKit.KeyDown (KeyCode.SKey))
            {
                Y += _Speed;
                _Rotation = 45;
            }
        }
        else if (SplashKit.KeyDown (KeyCode.SKey))
        {
            Y += _Speed;
            _Rotation = 0;
            if (SplashKit.KeyDown (KeyCode.DKey))
            {
                X += _Speed;
                _Rotation = 315;
            }
        }
        else if (SplashKit.KeyDown (KeyCode.DKey))
        {
            X += _Speed;
            _Rotation = 270;
            if (SplashKit.KeyDown (KeyCode.WKey))
            {
                Y -= _Speed;
                _Rotation = 225;
            }
        }
        else if (SplashKit.KeyDown (KeyCode.EscapeKey)) Quit = true;
        if (SplashKit.KeyDown (KeyCode.SpaceKey))
        {
            //Shoots only when cooldown is over, increased dexterity shortens cooldown
            if (_AliveTime.Ticks - shootCooldown > SHOOT_CAP - _Dexterity)
            {
                shootCooldown = _AliveTime.Ticks;
                Shoot (gameWindow);
            }
        }
    }
    public void StayOnWindow (Window limit)
    {
        if (X < WALL_SIZE) X += _Speed;
        else if (X + (_Bitmap.Width) > limit.Width - WALL_SIZE) X -= _Speed;
        else if (Y < WALL_SIZE / 2) Y += _Speed;
        else if (Y + (_Bitmap.Height) > limit.Height - WALL_SIZE / 2) Y -= _Speed;
    }
    // Checks if projectiles have collided with a wall or a new level has started
    public void Update (GameLevel gameLevel, Window window)
    {
        List<Projectile> ProjectileRemoval = new List<Projectile> ();
        foreach (Projectile projectile in _ProjectileList) projectile.Update ();
        foreach (Projectile projectile in _ProjectileList)
            if (projectile.CollidedWithWall (window))
                ProjectileRemoval.Add (projectile);
        foreach (Projectile projectile in ProjectileRemoval)
            _ProjectileList.Remove (projectile);

        if (gameLevel.nextLevelStarting (this, window))
        {
            Y = gameLevel.PlayerStartLoc.Y;
            X = gameLevel.PlayerStartLoc.X;
        }

    }
    // Short invincibility period to avoid being chain hit
    public bool CanTakeDamage ()
    {
        if (_AliveTime.Ticks - DamageCooldown > 1000)
        {
            DamageCooldown = _AliveTime.Ticks;
            return true;
        }
        return false;
    }
    // Checks if bullets have collided with isopod
    public bool BulletCollision (Isopod isopod)
    {
        bool hit = false;
        List<Projectile> removalProjectiles = new List<Projectile> ();
        foreach (Projectile projectile in _ProjectileList)
            if (projectile.CollidedWith (isopod))
            {
                removalProjectiles.Add (projectile);
                hit = true;
            }
        foreach (Projectile projectile in removalProjectiles)
        {
            _ProjectileList.Remove (projectile);
        }
        return hit;
    }
    // Checks if bullets have collided with obstacle
    public void BulletCollision (Obstacle obstacle)
    {
        List<Projectile> removalProjectiles = new List<Projectile> ();
        foreach (Projectile projectile in _ProjectileList)
            if (projectile.CollidedWith (obstacle)) removalProjectiles.Add (projectile);
        foreach (Projectile projectile in removalProjectiles)
            _ProjectileList.Remove (projectile);
    }
    public void MoveBack ()
    {
        X = prevX;
        Y = prevY;
    }
    // Creates either a bullet or flame projectile depending on item pickup
    public void Shoot (Window gameWindow)
    {
        switch (_ProjectileType)
        {
            case ProjectileType.bullet:
                _ProjectileList.Add (new Bullet (gameWindow, this));
                break;
            case ProjectileType.flame:
                _ProjectileList.Add (new Flame (gameWindow, this));
                break;
        }
    }
    public void TakeDamage (int damage)
    {
        if (_AliveTime.Ticks - DamageCooldown >= 500)
        {
            _Health -= damage;
            DamageCooldown = _AliveTime.Ticks;
        }
    }
    // Adds picked up item to a list and increases players stats according to item
    public void StoreItem (Item item)
    {
        switch (item.itemType)
        {
            case ItemType.PassiveDamage:
                _Damage += item.Damage_Bonus;
                break;
            case ItemType.PassiveSpeed:
                _Speed += item.Speed_Bonus;
                break;
            case ItemType.PassiveHaste:
                _Haste += item.Haste_Bonus;
                break;
            case ItemType.Projectile:
                _Damage += item.Damage_Bonus;
                _Dexterity += item.Dexterity_Bonus;
                _ProjectileType = item.projectileType;
                break;
            case ItemType.Other:
                _Damage += item.Damage_Bonus;
                _Haste += item.Haste_Bonus;
                _Speed += item.Speed_Bonus;
                _Dexterity += item.Dexterity_Bonus;
                break;
        }
        _Items.Add (item);
    }
}