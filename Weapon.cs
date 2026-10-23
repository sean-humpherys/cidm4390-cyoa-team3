public class Weapon
{
    public string type { get; }
    public int maxDamage { get; }
    public string asciiArt { get; }

    public Weapon(string type, int maxDamage, string asciiArt)
    {
        this.type = type;
        this.maxDamage = maxDamage;
        this.asciiArt = asciiArt;
    }

    public static readonly Weapon LongSword = new Weapon("long sword", 12, "-)=====>");
    public static readonly Weapon LightningBoltSpell = new Weapon("lightning bolt spell", 12, "zap~~~~~~");
    public static readonly Weapon Dagger = new Weapon("dagger", 6, "-)==>");
    public static readonly Weapon LongBow = new Weapon("long bow", 8, "}    -->");
}