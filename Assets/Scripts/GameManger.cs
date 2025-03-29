using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    public MapControl mapControl;
    public void InitGame()
    {
        mapControl = new GameObject("MapControl").AddComponent<MapControl>();
    }
    private void Start()
    {
        InitGame();
    }
}
