using System;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private Player _localPlayer;

    private static GameManager _instance;
    public static GameManager Instance => _instance;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        _localPlayer = LocalPlayer.Instance;
    }

    public void EndGame()
    {

    }
}
