using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _playBtn;
    [SerializeField]
    private GameObject[] _miniGames;
    private GameObject _currentMiniGame;

    public static MainGameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void CreatMiniGame(GameObject _minigame)
    {
        ActivePlay(false);
        _currentMiniGame = GameObject.Instantiate(_minigame);     
    }
    private void ActivePlay(bool _isallowed)
    {
        _playBtn.SetActive(_isallowed);
    }
}
