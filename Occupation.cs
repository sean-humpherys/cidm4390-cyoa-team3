using System;

public enum Occupation
{
    Fighter,
    Magician,
    Theif,
    Archer
}

public class OccupationRules
{
    public Weapon GetWeapon(Occupation occupation)
    {
        switch (occupation)
        {
            case Occupation.Fighter:
                return Weapon.LongSword;
            case Occupation.Magician:
                return Weapon.LightningBoltSpell;
            case Occupation.Theif:
                return Weapon.Dagger;
            case Occupation.Archer:
                return Weapon.LongBow;
            default:
                throw new ArgumentOutOfRangeException(nameof(occupation));
        }
    }
}