using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUnit : Unit, IPointerDownHandler
{
    private Sprite _shopImage;
    public Sprite ShopImage { get => _shopImage; set => _shopImage = value; }

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

     public override void SetUnitData(UnitData unitData)
    {
        base.SetUnitData(unitData);
        _shopImage = _unitData.ShopImage;
    }
}
