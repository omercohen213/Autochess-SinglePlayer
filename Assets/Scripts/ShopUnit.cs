using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUnit : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    public int Cost { get; private set; }

    [SerializeField] private string unitName;
    public string UnitName { get => unitName; private set => unitName = value; }
    public Image UnitImage { get; private set; }

    [SerializeField] private UnitData unitData;
    public UnitData UnitData { get => unitData; private set => unitData = value; }


    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");

    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");
    }

    // Set the unit's properties based on unitData
    public void SetShopUnitData(int id)
    {
        UnitData = UnitsDatabase.Instance.FindUnitById(id);
        Cost = UnitData.Cost;
        UnitName = UnitData.UnitName;
        UnitImage = UnitData.UnitShopImage;
    }
}
