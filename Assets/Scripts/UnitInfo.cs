using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _unitInfo;
    private RaycastHit2D[] _hits;
    private int _shopLayer;
    private int _benchLayer;
    private int _boardLayer;
    private int _layerMask;

    private void Start()
    {
        _boardLayer = LayerMask.NameToLayer("Board");
        _shopLayer = LayerMask.NameToLayer("Shop");
        //benchLayer = LayerMask.NameToLayer("Bench");
        _layerMask = ~(1 << _shopLayer);
    }

    private void Update()
    {
       /* Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _hits = Physics2D.RaycastAll(mousePosition, Vector2.zero, Mathf.Infinity, _layerMask);

        foreach (RaycastHit2D hit in _hits)
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("DraggableObject"))
            {
                _draggedObject = hit.collider.gameObject;
                StartDragging();
            }
        }*/
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowUnitInfo();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideUnitInfo();
    }

    public void ShowUnitInfo()
    {
        _unitInfo.SetActive(true);
    }

    public void HideUnitInfo()
    {
        _unitInfo.SetActive(false);
    }
}
