using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class DataManager : MonoBehaviour
{

    public DataGame[] dataGames;


    public static DataManager instance;
    private DataGame _currentDataGame;

    private void Awake()
    {
        instance = this;
    }

    //CAN USE ENUM FOR QUIEZ AND GET DATA WITH LIST REFRENCE OF ENUME
    public void GetRefrence()
    {
     
            string m_Json = File.ReadAllText(Application.dataPath+_currentDataGame.sourceDataPath);
            Dictionary<string, string> _tmpList = JsonConvert.DeserializeObject<Dictionary<string, string>>(m_Json);
            List<string> _refrenceSprites = new List<string>();
            foreach (var item in _tmpList)
            {
                _refrenceSprites.Add(item.Value);
            }
            _currentDataGame.dataRefrences = _refrenceSprites.ToArray();      
    }

    internal DataGame GetGameData(GameDataEnum datagametype)
    {
        List<DataGame> _datagamestmp = new List<DataGame>();
        _datagamestmp = dataGames.ToList();
        DataGame _datagame = _datagamestmp.Find(x => x.Gametype == datagametype);
        if (_datagame != null)
        {
            _currentDataGame = _datagame;
            GetRefrence();
            return _currentDataGame;
        }
        else
            return null;

    }
}
