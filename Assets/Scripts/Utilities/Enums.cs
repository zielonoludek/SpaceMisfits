public enum EffectType
{
    Booty,
    Notoriety,
    Health,
    Sight,
    Speed,

    Durability,
    Food,

    CrewMemberSpot,
    CrewMood
}
public enum SettingsPanel { Audio, Graphics, Controls }
public enum FightTier { Easy, Medium, Hard }
public enum EventType { FaintSignal, Waypoint, DevilsMaw, SharpenThoseDirks, Spaceport, Fight, EmptySpace }

public enum GameState { None, Event }
public enum GameScene { None, Map, Ship }

public enum RequestType { Positive, Neutral, Negative }
public enum RequestOriginType
{
    Event,
    
    Booty,
    Notoriety,
    Health,
    Sight,
    Speed,

    Durability,
    Food,

    CrewMemberSpot,
    CrewMood
}

public enum CrewMemberType { None, Cook, Spyglasser, Scientist }