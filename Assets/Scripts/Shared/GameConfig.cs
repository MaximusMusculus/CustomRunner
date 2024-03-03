using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Config
{
    [System.Serializable]
    public class GameEffectConfig
    {
        [SerializeField] private float _spawnRate;
        //[SerializeField] private List<InfluenceConfig> _effectConfigs;
        
        public float SpawnRate => _spawnRate;
        //public IReadOnlyList<InfluenceConfig> EffectConfigs => _effectConfigs;
    }
}