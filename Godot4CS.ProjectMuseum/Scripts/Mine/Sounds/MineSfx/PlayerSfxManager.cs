using System;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Sounds.MineSfx;

public partial class PlayerSfxManager : Node2D
{
    private Random _random;

    [Export] private AudioStreamPlayer2D _streamPlayer;
    
    [Export] private AudioStream[] _digSfxList;
    [Export] private AudioStream[] _swordSlashSfxList;

    public override void _Ready()
    {
        SubscribeToActions();
        _random = new Random();
    }

    #region Subscribe and Unsubscribe

    private void SubscribeToActions()
    {
        MineActions.OnSuccessfulDigActionCompleted += PlayDigSfx;
    }
    
    private void UnsubscribeToActions()
    {
        MineActions.OnSuccessfulDigActionCompleted -= PlayDigSfx;
    }

    #endregion

    private void PlayDigSfx()
    {
        var randDigSfx = ShuffleAudioStream(_digSfxList);
        _streamPlayer.Stream = randDigSfx;
        _streamPlayer.Play();
    }

    private void PlaySwordSlashSfx()
    {
        if(_streamPlayer.IsPlaying()) return;
        var randDigSfx = ShuffleAudioStream(_swordSlashSfxList);
        _streamPlayer.Stream = randDigSfx;
        _streamPlayer.Play();
    }

    private AudioStream ShuffleAudioStream(AudioStream[] streams) => streams[_random.Next(0, streams.Length)];
    
    public override void _ExitTree()
    {
        UnsubscribeToActions();
    }
}