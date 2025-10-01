using UnityEngine;

[CreateAssetMenu(fileName =nameof(GameConfiguration), menuName = "Game Configs/"+nameof(GameConfiguration))]
public class GameConfiguration : ScriptableObject
{
    public TileSpawnConfiguration TileSpawnConfiguration;
}


[System.Serializable]
public class TileSpawnConfiguration
{
    public int TilesOnInit = 3;
    public int TilesAfterPlayerMove = 2;
}
