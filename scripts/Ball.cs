using Godot;

namespace StarWreck.scripts;

public partial class Ball : RigidBody2D
{
    public void OnBodyEntered(Node node)
    {
        if (node is Enemy enemy)
        {
            enemy.TakeKineticDamage(this);
        }
    }
}
