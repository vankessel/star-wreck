using System.Collections.Generic;
using Godot;
using StarWreck.scripts.autoloads;

namespace StarWreck.scripts;

public partial class Debris : RigidBody2D
{
    private static readonly LinkedList<Debris> DebrisList = [];
    private bool _inList;

    public override void _EnterTree()
    {
        base._EnterTree();

        DebrisList.AddLast(this);
        _inList = true;

        if (DebrisSettings.Instance.MaxInstances >= DebrisList.Count) return;

        Debris oldest = DebrisList.First!.Value;
        DebrisList.RemoveFirst();
        oldest._inList = false;
        oldest.QueueFree();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_inList) DebrisList.Remove(this);
    }
}
