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
                return new Weapon("Sword", 10, "/==>");
            case Occupation.Magician:
                return new Weapon("Staff", 8, "|*");
            case Occupation.Theif:
                return new Weapon("Dagger", 6, "/>");
            case Occupation.Archer:
                return new Weapon("Bow", 7, ")-->");
            default:
                throw new ArgumentOutOfRangeException(nameof(occupation));
        }
    }
}