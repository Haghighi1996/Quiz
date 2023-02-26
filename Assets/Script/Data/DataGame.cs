using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DataGame
{
    public GameDataEnum Gametype;

    public GameObject minGamePrefab;

    public GameObject[] subPrefabs;

    public string sourceDataPath;

    public string[] dataRefrences;

}
