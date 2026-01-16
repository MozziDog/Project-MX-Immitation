using System;
using System.Collections.Generic;

namespace Logic
{
    [Serializable]
    public class EnemyWave
    {
        public List<CharacterData> Enemies = new();
        public List<CharacterStatData> EnemyStats = new();
        public List<Position2> EnemyPositions  = new();
        
        public int Count
        {
            get { return Enemies.Count; }
        }

        public (CharacterData, CharacterStatData, Position2) this[int i]
        {
            get { return (Enemies[i], EnemyStats[i], EnemyPositions[i]); }
        }
    }
}