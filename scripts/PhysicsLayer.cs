namespace StarWreck.scripts;

public enum PhysicsLayer : uint
{
    Default = 1u << PhysicsLayerBit.Default,
    Player = 1u << PhysicsLayerBit.Player,
    Ball = 1u << PhysicsLayerBit.Ball,
    Enemies = 1u << PhysicsLayerBit.Enemies,
    Bullets = 1u << PhysicsLayerBit.Bullets,
    Debris = 1u << PhysicsLayerBit.Debris,
    Planets = 1u << PhysicsLayerBit.Planets,
    ForceFields = 1u << PhysicsLayerBit.ForceFields
}

public enum PhysicsLayerBit
{
    Default,
    Player,
    Ball,
    Enemies,
    Bullets,
    Debris,
    Planets,
    ForceFields
}
