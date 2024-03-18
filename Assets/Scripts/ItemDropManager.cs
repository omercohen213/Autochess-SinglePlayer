using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
    public static ItemDropManager Instance;
    [SerializeField] private GameObject _itemOrbPrefab;


    private void Awake()
    {
        Instance = this;
    }

    // Create the item drop object and its components
    public void CreateItemOrb(Vector3 position)
    {
        Instantiate(_itemOrbPrefab, position, Quaternion.identity, transform);
    }
}
