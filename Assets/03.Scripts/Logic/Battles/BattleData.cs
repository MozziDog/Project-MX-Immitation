using System;
using System.Collections.Generic;
using Logic;

[Serializable]
public class BattleData
{
    /// <summary>
    /// 아군 캐릭터 정보
    /// </summary>
    public List<CharacterData> Characters = new();
    public List<CharacterStatData> CharacterStats = new();

    /// <summary>
    /// 적군 캐릭터 정보
    /// </summary>
    public List<EnemyWave> EnemyWaves = new();

    /// <summary>
    /// 장애물 정보
    /// </summary>
    public List<ObstacleData> Obstacles = new();
    public List<Position2> ObstaclePosition  = new();
    public List<float> ObstacleRotation = new();
}
