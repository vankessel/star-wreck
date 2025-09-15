using Godot;

namespace StarWreck.scripts.autoloads;

public partial class AudioManager : Node
{
    [Export] private SmartAudioStreamPlayer _menuPlayer;
    [Export] private SmartAudioStreamPlayer _gameplayPlayer;
    [Export] private SmartAudioStreamPlayer _bossPlayer;

    public SmartAudioStreamPlayer MenuPlayer => _menuPlayer;
    public SmartAudioStreamPlayer GameplayPlayer => _gameplayPlayer;
    public SmartAudioStreamPlayer BossPlayer => _bossPlayer;

    public static AudioManager Instance { get; private set; }

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;
    }
}
