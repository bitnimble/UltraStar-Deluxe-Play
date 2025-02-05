﻿using System;
using System.Collections.Generic;

[Serializable]
public class SingSceneData : SceneData
{
    public SongMeta SelectedSongMeta { get; set; }
    public List<PlayerProfile> SelectedPlayerProfiles { get; set; } = new();
    public Dictionary<PlayerProfile, MicProfile> PlayerProfileToMicProfileMap { get; set; } = new();
    public Dictionary<PlayerProfile, string> PlayerProfileToVoiceNameMap { get; set; } = new();
    public double PositionInSongInMillis { get; set; }
    public bool IsRestart { get; set; }
    public int NextBeatToScore { get; set; }
    public Dictionary<PlayerProfile, PlayerScoreControllerData> PlayerProfileToScoreDataMap { get; set; } = new();
}
