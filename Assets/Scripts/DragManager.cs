using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DragManager : MonoBehaviour
{
    private bool _isEnable;
    private bool _isDragging = false;
    private RaycastHit2D[] _hits;
    private GameObject _draggedObject;
    private GameObject _lastHexHovered = null;
    private GameObject _lastBenchSlotHovered = null;

    private int _shopLayer;
    private int _benchLayer;
    private int _boardLayer;
    private int _layerMask;

    private static DragManager _instance;
    public static DragManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DragManager>();
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
    }

    private void Start()
    {
        _shopLayer = LayerMask.NameToLayer("Shop");
        _boardLayer = LayerMask.NameToLayer("Board");
        //benchLayer = LayerMask.NameToLayer("Bench");
        _layerMask = ~(1 << _shopLayer);

        GameManager.Instance.OnPhaseChanged += OnPhaseChanged;
    }

    void Update()
    {
        if (!_isEnable)
        {
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _hits = Physics2D.RaycastAll(mousePosition, Vector2.zero, Mathf.Infinity, _layerMask);
        if (Input.GetMouseButtonDown(0) && !_isDragging)
        {
            // Loop through all hits
            foreach (RaycastHit2D hit in _hits)
            {
                if (hit.collider != null && hit.collider.gameObject.CompareTag("DraggableObject"))
                {
                    _draggedObject = hit.collider.gameObject;
                    StartDragging();
                }
            }
        }
        // Continue dragging gameobject
        else if (_isDragging && Input.GetMouseButton(0))
        {
            ContinueDragging();
            GameObject hexGo = null;
            GameObject benchSlotGo = null;
            foreach (RaycastHit2D hit in _hits)
            {
                if (hit.collider != null && hit.collider.gameObject.CompareTag("Hex"))
                {
                    hexGo = hit.collider.gameObject;
                    if (hexGo.TryGetComponent(out Hex hex))
                    {
                        hex.OnHover();
                    }
                    break;
                }
                if (hit.collider != null && hit.collider.gameObject.CompareTag("BenchSlot"))
                {
                    benchSlotGo = hit.collider.gameObject;
                    if (benchSlotGo.TryGetComponent(out BenchSlot benchSlot))
                    {
                        benchSlot.OnHover();
                    }
                    break;
                }

            }

            // Check if the hex under the mouse has changed
            if (hexGo != _lastHexHovered)
            {
                StopHexHover();
                _lastHexHovered = hexGo;
            }

            // Check if the hex under the mouse has changed
            if (benchSlotGo != _lastBenchSlotHovered)
            {
                StopBenchSlotHover();
                _lastBenchSlotHovered = benchSlotGo;
            }
        }
        // Stop dragging gameobject
        else if (_isDragging && Input.GetMouseButtonUp(0))
        {
            StopDragging();
            StopHexHover();
            StopBenchSlotHover();
        }
    }

    public void OnPhaseChanged(GamePhase newPhase)
    {
        switch (newPhase)
        {
            case GamePhase.Preparation:
                _isEnable = true;
                _layerMask = ~(1 << _shopLayer);
                break;
            case GamePhase.Battle:
                //_isEnable = false;
                _layerMask = ~(1 << _shopLayer) & ~(1 << _boardLayer);
                break;
        }
    }

    private void StopBenchSlotHover()
    {
        if (_lastBenchSlotHovered != null)
        {
            if (_lastBenchSlotHovered.TryGetComponent(out BenchSlot benchSlot))
            {
                benchSlot.OnHoverStopped();
            }
        }
    }

    private void StopHexHover()
    {
        if (_lastHexHovered != null)
        {
            if (_lastHexHovered.TryGetComponent(out Hex lastHex))
            {
                lastHex.OnHoverStopped();
            }
        }
    }

    // Set the starting offset position
    private void StartDragging()
    {
        _isDragging = true;

        _draggedObject.TryGetComponent(out GameUnit gameUnit);
        if (gameUnit != null)
        {
            gameUnit.HandleDragStarted();
            Board.Instance.OnUnitDragStarted();
        }
    }

    // Change position of the draggableObject to the mouse position
    private void ContinueDragging()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _draggedObject.transform.position = new Vector3(mousePosition.x, mousePosition.y);
    }

    private void StopDragging()
    {
        _isDragging = false;
        _draggedObject.TryGetComponent(out GameUnit gameUnit);
        if (gameUnit != null)
        {
            GameObject objDraggedOn = GetObjectDraggedOn(_draggedObject.transform.position, _draggedObject);
            gameUnit.HandleDragStopped(objDraggedOn);
            Board.Instance.OnUnitDragStopped();
        }
    }

    private GameObject GetObjectDraggedOn(Vector3 position, GameObject objToIgnore)
    {
        // Create a layer mask to ignore the collider of the dragged object
        int layerMask = ~(1 << objToIgnore.layer);
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, layerMask);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }
}
