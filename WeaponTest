using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class WeaponTests
{
    [TestMethod]
    public void LongSword_HasExpectedValues()
    {
        Assert.AreEqual("long sword", Weapon.LongSword.type);
        Assert.AreEqual(12, Weapon.LongSword.maxDamage);
        Assert.AreEqual("-)=====>", Weapon.LongSword.asciiArt);
    }

    [TestMethod]
    public void LightningBoltSpell_HasExpectedValues()
    {
        Assert.AreEqual("lightning bolt spell", Weapon.LightningBoltSpell.type);
        Assert.AreEqual(12, Weapon.LightningBoltSpell.maxDamage);
        Assert.AreEqual("zap~~~~~~", Weapon.LightningBoltSpell.asciiArt);
    }

    [TestMethod]
    public void Dagger_HasExpectedValues()
    {
        Assert.AreEqual("dagger", Weapon.Dagger.type);
        Assert.AreEqual(6, Weapon.Dagger.maxDamage);
        Assert.AreEqual("-)==>", Weapon.Dagger.asciiArt);
    }

    [TestMethod]
    public void LongBow_HasExpectedValues()
    {
        Assert.AreEqual("long bow", Weapon.LongBow.type);
        Assert.AreEqual(8, Weapon.LongBow.maxDamage);
        Assert.AreEqual("}    -->", Weapon.LongBow.asciiArt);
    }
}