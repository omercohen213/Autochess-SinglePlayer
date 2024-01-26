using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<int> _xpTable;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

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
        _xpTable = new List<int> { 0, 0, 2, 6, 10, 20, 36, 56, 80, 100 };
    }
    private void Start()
    {
        
    }

    public int GetXpToLevelUp(int lvl)
    {
        return _xpTable[lvl];
    }
}
