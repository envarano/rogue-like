using System;
using System.Collections.Generic;
using SplashKitSDK;
public enum ItemType
{
    PassiveSpeed,
    PassiveDamage,
    PassiveHaste,
    Projectile,
    Other
}
public abstract class Item : GameObject
{
    public double Speed_Bonus{get; protected set;}
    public int Damage_Bonus{get; protected set;}
    public double Haste_Bonus{get; protected set;}
    public double Dexterity_Bonus{get; protected set;}
    public ItemType itemType{get; protected set;}
    public ProjectileType projectileType{get; protected set;}
    public Item (Window window)
    {
        _Bitmap = new Bitmap ("item", "Item.png");

        X = (int) SplashKit.Rnd (100, window.Width - 200);
        Y = (int) SplashKit.Rnd (100, window.Height - 200);
    }
    public void Draw (Window window)
    {
        _Bitmap.Draw (X, Y);
    }
}
public class ButchyBoot : Item
{
    public ButchyBoot (Window window) : base (window)
    {
        _Bitmap = new Bitmap ("butchy-boot", "Item_ButchyBoots.png");
        itemType = ItemType.PassiveSpeed;
        Speed_Bonus = 0.1;
    }
}

public class DemonicButchyBoot : Item
{
    public DemonicButchyBoot (Window window) : base (window)
    {
        _Bitmap = new Bitmap ("demonic-butchy-boot", "Item_DButchyBoots.png");
        itemType = ItemType.PassiveSpeed;
        Speed_Bonus = 0.2;
    }
}
public class FireAntSoul : Item
{
    public FireAntSoul (Window window) : base (window)
    {
        _Bitmap = new Bitmap ("fire-ant-soul", "Item_FireAntSoul.png");
        itemType = ItemType.Projectile;
        projectileType = ProjectileType.flame;
        Damage_Bonus = 20;
        Dexterity_Bonus = 20;
    }
}
public class FireFly : Item
{
    public FireFly (Window window) : base (window)
    {
        _Bitmap = new Bitmap ("firefly", "Item_Firefly.png");
        itemType = ItemType.PassiveHaste;
        Haste_Bonus = 1;
    }
}
public class BulletAntSoul : Item
{
    public BulletAntSoul (Window window) : base (window)
    {
        _Bitmap = new Bitmap ("web-sling", "Item_BulletAntSoul.png");
        itemType = ItemType.Projectile;
        projectileType = ProjectileType.bullet;
        Damage_Bonus = 10;
        Dexterity_Bonus = 20;
    }
}
public class PapaLongLegsThread : Item
{
    public PapaLongLegsThread (Window window) : base (window)
    {
        _Bitmap = new Bitmap ("ppllt", "Item_PPLLT.png");
        itemType = ItemType.PassiveSpeed;
        Speed_Bonus = 1;
    }
}