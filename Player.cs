using System;

public class Player
{
    public Race Race{get; private set;}

    public int Stength{get; private set;}
    public int Agility{get; private set;}

    private readonly Die d20 = new Die(20);
    private readonly RaceRules raceRules = new RaceRules();

}
