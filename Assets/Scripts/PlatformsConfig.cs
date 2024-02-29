using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class PlatformInfo
{
    public PlatformType type;
    public GameObject gameObject;
}


[CreateAssetMenu(fileName = "PlatformsConfig", menuName = "PlatformsConfig", order = 51)]
public class PlatformsConfig : ScriptableObject
{
    public int lenght = 30;
    public List<PlatformInfo> Platforms = new List<PlatformInfo>();
}