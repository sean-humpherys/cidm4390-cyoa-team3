using System;

public class Player
{
    public Race Race{get; private set;}

    public int Stength{get; private set;}
    public int Agility{get; private set;}

    private readonly D20 d20 = new D20();
    private readonly RaceRules raceRules = new RaceRules();

}
