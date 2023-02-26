using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameLogic 
{
    public delegate void OnEndGame(bool isendgame);
    public event OnEndGame OnendGame;

    public void Init();

    public void ResetData();

    public void End();
}
