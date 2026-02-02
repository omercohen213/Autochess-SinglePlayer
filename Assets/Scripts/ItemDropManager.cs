using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
    private GameUnit _gameUnit;

    [SerializeField] private GameObject _itemOrbPrefab;

    private void Awake()
    {
        _gameUnit = GetComponent<GameUnit>();
    }

    private void OnEnable()
    {
        _gameUnit.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        _gameUnit.OnDeath -= OnDeath;
    }

    private void OnDeath(GameUnit gameUnit)
    {
        CreateItemOrb(gameUnit.transform.position);
    }


    // Create the item drop object and its components
    public void CreateItemOrb(Vector3 position)
    {
        Instantiate(_itemOrbPrefab, position, Quaternion.identity, transform);
    }
}
