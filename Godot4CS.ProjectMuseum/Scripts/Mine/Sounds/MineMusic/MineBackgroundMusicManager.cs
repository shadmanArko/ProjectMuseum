using System;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Sounds.MineMusic;

public partial class MineBackgroundMusicManager : Node2D
{
    private Random _random;

    [Export] private AudioStreamPlayer2D _streamPlayer;
    [Export] private AudioStream[] _backgroundMusicList;

    public override void _Ready()
    {
        _random = new Random();
        SubscribeToActions();
    }

    private void SubscribeToActions()
    {
        MineActions.OnPlayerLandedIntoTheMine += PlayBackgroundMusic;
        MineActions.OnPlayerReachedBackToCamp += StopBackgroundMusic;
    }

    private void UnsubscribeToActions()
    {
        MineActions.OnPlayerLandedIntoTheMine -= PlayBackgroundMusic;
        MineActions.OnPlayerReachedBackToCamp -= StopBackgroundMusic;
    }

    private void PlayBackgroundMusic()
    {
        var randBackgroundMusic = ShuffleAudioStream(_backgroundMusicList);
        _streamPlayer.Stream = randBackgroundMusic;
        _streamPlayer.Play();
    }

    private void StopBackgroundMusic()
    {
        _streamPlayer.Stop();
    }

    private AudioStream ShuffleAudioStream(AudioStream[] streams) => streams[_random.Next(0, streams.Length)];
    
    public override void _ExitTree()
    {
        StopBackgroundMusic();
    }
}