using Godot;

namespace StarWreck.scripts;

[GlobalClass]
public partial class SmartAudioStreamPlayer : AudioStreamPlayer
{
    public float InitialVolumeDb { get; private set; }
    public float InitialLinearVolume { get; private set; }

    public override void _EnterTree()
    {
        base._EnterTree();

        InitialVolumeDb = VolumeDb;
        InitialLinearVolume = Mathf.DbToLinear(VolumeDb);
    }

    public void FadeOut(float seconds)
    {
        Tween tween = CreateTween();
        tween.TweenProperty(this, AudioStreamPlayer.PropertyName.VolumeLinear.ToString(), 0f, seconds);
        tween.TweenCallback(Callable.From(Stop));
        tween.TweenCallback(Callable.From(() => { VolumeDb = InitialVolumeDb; }));
    }

    public void FadeIn(float seconds)
    {
        Tween tween = CreateTween();
        if (!IsPlaying())
        {
            VolumeLinear = 0f;
            Play();
        }
        tween.TweenProperty(this, AudioStreamPlayer.PropertyName.VolumeLinear.ToString(), InitialLinearVolume, seconds);
    }
}
