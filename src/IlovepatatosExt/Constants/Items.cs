using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class Items
{
    public static class Weapons
    {
        public static readonly ItemDefinition Sar = ItemManager.FindItemDefinition("rifle.semiauto");

        public static readonly ItemDefinition Ak = ItemManager.FindItemDefinition("rifle.ak");
        public static readonly ItemDefinition AkIce = ItemManager.FindItemDefinition("rifle.ak.ice");
        public static readonly ItemDefinition AkDiver = ItemManager.FindItemDefinition("rifle.ak.diver");

        public static readonly ItemDefinition Lr = ItemManager.FindItemDefinition("rifle.lr300");
        public static readonly ItemDefinition M39 = ItemManager.FindItemDefinition("rifle.m39");
        public static readonly ItemDefinition Bolt = ItemManager.FindItemDefinition("rifle.bolt");
        public static readonly ItemDefinition L96 = ItemManager.FindItemDefinition("rifle.l96");
        public static readonly ItemDefinition M249 = ItemManager.FindItemDefinition("lmg.m249");

        // ReSharper disable once InconsistentNaming
        public static readonly ItemDefinition HMLMG = ItemManager.FindItemDefinition("hmlmg");

        public static readonly ItemDefinition Mp5 = ItemManager.FindItemDefinition("smg.mp5");
        public static readonly ItemDefinition Thompson = ItemManager.FindItemDefinition("smg.thompson");
        public static readonly ItemDefinition Smg = ItemManager.FindItemDefinition("smg.2");

        public static readonly ItemDefinition Revolver = ItemManager.FindItemDefinition("pistol.revolver");
        public static readonly ItemDefinition P250 = ItemManager.FindItemDefinition("pistol.semiauto");
        public static readonly ItemDefinition Python = ItemManager.FindItemDefinition("pistol.python");
        public static readonly ItemDefinition M92 = ItemManager.FindItemDefinition("pistol.m92");
        public static readonly ItemDefinition Prototype = ItemManager.FindItemDefinition("pistol.prototype17");

        public static readonly ItemDefinition WaterPipe = ItemManager.FindItemDefinition("shotgun.waterpipe");
        public static readonly ItemDefinition DoubleBarrel = ItemManager.FindItemDefinition("shotgun.double");
        public static readonly ItemDefinition Pump = ItemManager.FindItemDefinition("shotgun.pump");

        public static readonly ItemDefinition Bow = ItemManager.FindItemDefinition("bow.hunting");
        public static readonly ItemDefinition CompoundBow = ItemManager.FindItemDefinition("bow.compound");
        public static readonly ItemDefinition Crossbow = ItemManager.FindItemDefinition("crossbow");
    }

    public static class Attachments
    {
        public static readonly ItemDefinition SimpleSight = ItemManager.FindItemDefinition("weapon.mod.simplesight");
        public static readonly ItemDefinition HoloSight = ItemManager.FindItemDefinition("weapon.mod.holosight");

        // ReSharper disable once InconsistentNaming
        public static readonly ItemDefinition x8 = ItemManager.FindItemDefinition("weapon.mod.small.scope");

        // ReSharper disable once InconsistentNaming
        public static readonly ItemDefinition x16 = ItemManager.FindItemDefinition("weapon.mod.8x.scope");

        public static readonly ItemDefinition Laser = ItemManager.FindItemDefinition("weapon.mod.lasersight");
        public static readonly ItemDefinition Flashlight = ItemManager.FindItemDefinition("weapon.mod.flashlight");

        public static readonly ItemDefinition Silencer = ItemManager.FindItemDefinition("weapon.mod.silencer");
        public static readonly ItemDefinition MuzzleBoost = ItemManager.FindItemDefinition("weapon.mod.muzzleboost");
        public static readonly ItemDefinition MuzzleBrake = ItemManager.FindItemDefinition("weapon.mod.muzzlebrake");

        public static readonly ItemDefinition Burst = ItemManager.FindItemDefinition("weapon.mod.burstmodule");
    }

    public static class Ammo
    {
        public static readonly ItemDefinition Arrow = ItemManager.FindItemDefinition("arrow.wooden");
        public static readonly ItemDefinition ArrowBone = ItemManager.FindItemDefinition("arrow.bone");
        public static readonly ItemDefinition ArrowHighVelocity = ItemManager.FindItemDefinition("arrow.hv");
        public static readonly ItemDefinition ArrowFire = ItemManager.FindItemDefinition("arrow.fire");

        public static readonly ItemDefinition Pistol = ItemManager.FindItemDefinition("ammo.pistol");
        public static readonly ItemDefinition PistolHighVelocity = ItemManager.FindItemDefinition("ammo.pistol.hv");
        public static readonly ItemDefinition PistolIncendiary = ItemManager.FindItemDefinition("ammo.pistol.fire");

        public static readonly ItemDefinition Rifle = ItemManager.FindItemDefinition("ammo.rifle");
        public static readonly ItemDefinition RifleHighVelocity = ItemManager.FindItemDefinition("ammo.rifle.hv");
        public static readonly ItemDefinition RifleIncendiary = ItemManager.FindItemDefinition("ammo.rifle.incendiary");
        public static readonly ItemDefinition RifleExplosive = ItemManager.FindItemDefinition("ammo.rifle.explosive");

        public static readonly ItemDefinition Handmade = ItemManager.FindItemDefinition("ammo.handmade.shell");
        public static readonly ItemDefinition Shotgun = ItemManager.FindItemDefinition("ammo.shotgun");
        public static readonly ItemDefinition ShotgunFire = ItemManager.FindItemDefinition("ammo.shotgun.fire");
        public static readonly ItemDefinition ShotgunSlug = ItemManager.FindItemDefinition("ammo.shotgun.slug");
    }

    public static class Armor
    {
        public static readonly ItemDefinition MetalFacemask = ItemManager.FindItemDefinition("metal.facemask");
        public static readonly ItemDefinition MetalChestPlate = ItemManager.FindItemDefinition("metal.plate.torso");

        public static readonly ItemDefinition RoadsignJacket = ItemManager.FindItemDefinition("roadsign.jacket");
        public static readonly ItemDefinition RoadsignKilt = ItemManager.FindItemDefinition("roadsign.kilt");

        public static readonly ItemDefinition Hoodie = ItemManager.FindItemDefinition("hoodie");
        public static readonly ItemDefinition Pants = ItemManager.FindItemDefinition("pants");
        public static readonly ItemDefinition Boots = ItemManager.FindItemDefinition("shoes.boots");

        public static readonly ItemDefinition TacticalGloves = ItemManager.FindItemDefinition("tactical.gloves");
        public static readonly ItemDefinition RoadsignGloves = ItemManager.FindItemDefinition("roadsign.gloves");

        public static readonly ItemDefinition Hazmatsuit = ItemManager.FindItemDefinition("hazmatsuit");

        public static readonly ItemDefinition GlowingEyes = ItemManager.FindItemDefinition("gloweyes");
    }

    public static class Food
    {
        public static readonly ItemDefinition Pumpkin = ItemManager.FindItemDefinition("pumpkin");
    }

    public static class Healing
    {
        public static readonly ItemDefinition Bandage = ItemManager.FindItemDefinition("bandage");
        public static readonly ItemDefinition Syringe = ItemManager.FindItemDefinition("syringe.medical");
        public static readonly ItemDefinition Medkit = ItemManager.FindItemDefinition("largemedkit");
    }

    public static class Resources
    {
        public static readonly ItemDefinition Cloth = ItemManager.FindItemDefinition("cloth");
        public static readonly ItemDefinition LowGradeFuel = ItemManager.FindItemDefinition("lowgradefuel");
        public static readonly ItemDefinition Scrap = ItemManager.FindItemDefinition("scrap");
    }

    public static class Buildings
    {
        public static readonly ItemDefinition HighWoodenWall = ItemManager.FindItemDefinition("wall.external.high");
        public static readonly ItemDefinition ElectricButton = ItemManager.FindItemDefinition("electric.button");
    }

    public static class Deployables
    {
        public static readonly ItemDefinition SleepingBag = ItemManager.FindItemDefinition("sleepingbag");
    }
}