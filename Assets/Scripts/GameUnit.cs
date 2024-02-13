using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using static UnityEngine.UI.CanvasScaler;

public class GameUnit : Unit
{
    private Player _owner;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _maxMp;
    [SerializeField] private int _baseAttackDamage;
    [SerializeField] private GameObject _starPrefab;
    [SerializeField] private Transform _starsParent;
    private Animator animator;

    public bool _isOnBoard;
    public Hex _currentHex; // Current hex spot if unit is on board. Null otherwise
    public BenchSlot _currentBenchSlot; // Bench spot if unit is on bench. Null otherwise.
    public Dictionary<Trait, int> TraitStages = new();
    public List<int> _Stages = new();

    public int StarLevel;
    public int AttackDamage;
    public int Range;

    public readonly float MOVE_SPEED = 4f;
    public readonly int MAX_STAR_LEVEL = 3;
    public Player Owner { get => _owner; set => _owner = value; }
    public int MaxHp { get => _maxHp; private set => _maxHp = value; }
    public int MaxMp { get => _maxMp; private set => _maxMp = value; }
    public int BaseAttackDamage { get => _baseAttackDamage; private set => _baseAttackDamage = value; }
    public bool IsOnBoard { get => _isOnBoard; set => _isOnBoard = value; }
    public Hex CurrentHex { get => _currentHex; set => _currentHex = value; }
    public BenchSlot CurrentBenchSlot { get => _currentBenchSlot; set => _currentBenchSlot = value; }


    private void Awake()
    {
        if(!TryGetComponent(out animator))
        {
            Debug.LogWarning("Missing animator on game unit " + UnitName);    
        }
    }

    private void Start()
    {
        AttackDamage = _baseAttackDamage;

        // Initialize all traits with stage 0
        foreach (var trait in Traits)
        {
            TraitStages.Add(trait, 0);
        }
    }

    public void Initialize(Player owner, int id, int starLevel)
    {
        SetUnitData(id);
        _owner = owner;
        SetUnitStarLevel(starLevel);
    }

    public void HandleDragStarted()
    {
        Shop.Instance.ActivateUnitSellField(Cost);
    }

    // Handles a behavior when this unit is stopped being dragged at final position
    public void HandleDragStopped(GameObject objDraggedOn)
    {
        string objTag = objDraggedOn != null ? objDraggedOn.tag : null;

        // The unit was not dragged on a game object - return unit to its current bench or hex
        if (objTag == null)
        {
            ReturnToPlace();
        }
        else
        {
            switch (objTag)
            {
                // Sell Unit
                case "SellField":
                    Shop.Instance.SellUnit(this);
                    Destroy(gameObject);
                    break;

                // Place on board
                case "Hex":
                    Hex hex = objDraggedOn.GetComponent<Hex>();
                    CheckHexPlacement(hex);
                    break;

                // Place on another bench slot
                case "BenchSlot":
                    BenchSlot benchSlotDraggedOn = objDraggedOn.GetComponent<BenchSlot>();
                    _owner.Bench.PutUnitOnBenchSlot(this, benchSlotDraggedOn);
                    break;

                default:
                    ReturnToPlace();
                    break;
            }
        }
        Shop.Instance.DisableUnitSellField();
    }

    // Place unit on board only if allowed
    private void CheckHexPlacement(Hex hex)
    {
        // Allow only if board limit is not reached
        if (Owner.IsBoardLimitReached() && !IsOnBoard)
        {
            ReturnToPlace();
            Debug.Log("Units limit reached");
        }

        // Allow only on player's side of the board
        else if (hex.X < 4)
        {
            Board.Instance.PlaceUnitOnBoard(this, hex);
        }
        else
        {
            ReturnToPlace();
            Debug.Log("Can't place unit on opponent's board");
        }

    }

    // Return unit to its current bench or hex
    private void ReturnToPlace()
    {
        if (IsOnBoard)
        {
            Board.Instance.PlaceUnitOnBoard(this, CurrentHex);
        }
        else
        {
            _owner.Bench.PutUnitOnBenchSlot(this, _currentBenchSlot);
        }
    }

    // Create star objects and set current star level
    public void SetUnitStarLevel(int starLevel)
    {
        for (int i = 0; i < starLevel; i++)
        {
            if (_starPrefab != null && _starsParent != null)
            {
                Instantiate(_starPrefab, _starsParent);
            }
            else
            {
                Debug.LogWarning("Missing star objects on gameUnit " + UnitName);
            }
        }
        StarLevel = starLevel;
    }

    public void MoveTowardsEnemyUnit()
    {
        // Find the shortest path to the enemy unit's hex
        Hex enemyHex = FindClosestEnemy();
        List<Hex> shortestPath = FindShortestPath(_currentHex, enemyHex);

        int distance = shortestPath.Count;

        // Move one step at a time along the path until reaching a hex adjacent to the enemy unit's hex
        foreach (Hex nextHex in shortestPath)
        {
            if (_currentHex.IsAdjacentToHex(nextHex))
            {
                if (distance <= Range || _currentHex.IsAdjacentToHex(enemyHex))
                {
                    Attack(enemyHex.UnitOnHex);
                    break;
                }
                else
                {
                    MoveToHex(nextHex);
                    distance--;
                    break;
                }
            }
        }
    }

    // Find the shortest path using BFS
    private List<Hex> FindShortestPath(Hex startHex, Hex targetHex)
    {
        Dictionary<Hex, Hex> cameFrom = new(); // To reconstruct the path
        Queue<Hex> frontier = new(); // Queue for BFS traversal
        HashSet<Hex> visited = new(); // To keep track of visited hexes

        frontier.Enqueue(startHex);
        visited.Add(startHex);

        while (frontier.Count > 0)
        {
            Hex currentHex = frontier.Dequeue();

            if (currentHex.Equals(targetHex))
            {
                // Reconstruct the path
                List<Hex> path = new();
                Hex current = targetHex;
                while (!current.Equals(startHex))
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Reverse(); // Reverse to get the correct order
                return path;
            }

            // Explore the neighbors
            foreach (Hex neighbor in currentHex.AdjacentHexes)
            {
                // The target hex is taken, but we still want it included in the path
                if (neighbor.Equals(targetHex))
                {
                    if (!visited.Contains(neighbor))
                    {
                        frontier.Enqueue(neighbor);
                        visited.Add(neighbor);
                        cameFrom[neighbor] = currentHex;
                    }
                }
                // For other neighbors we want to check if its taken to find a different path
                else
                {
                    if (!visited.Contains(neighbor) && !neighbor.IsTaken)
                    {
                        frontier.Enqueue(neighbor);
                        visited.Add(neighbor);
                        cameFrom[neighbor] = currentHex;
                    }
                }
            }
        }

        // If no path found, return an empty list
        return new List<Hex>();
    }

    // Find closest enemy unit on board
    private Hex FindClosestEnemy()
    {
        Hex closestHex = null;
        float shortestDistance = float.MaxValue;
        if (_owner == LocalPlayer.Instance)
        {
            foreach (GameUnit enemyUnit in Opponent.Instance.BoardUnits)
            {
                // Calculate distance between current unit and enemy unit
                float distance = Vector2Int.Distance(_currentHex.ToVector2Int(), enemyUnit.CurrentHex.ToVector2Int());

                // Check if this enemy unit is closer than the previously found closest one
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestHex = enemyUnit.CurrentHex;
                }
            }

            return closestHex;
        }
        else
        {
            foreach (GameUnit playerUnit in LocalPlayer.Instance.BoardUnits)
            {
                // Calculate distance between current unit and enemy unit
                float distance = Vector2Int.Distance(_currentHex.ToVector2Int(), playerUnit.CurrentHex.ToVector2Int());

                // Check if this enemy unit is closer than the previously found closest one
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestHex = playerUnit.CurrentHex;
                }
            }
            return closestHex;
        }
    }

    // Method to move the unit to a specific hex
    private void MoveToHex(Hex hex)
    {
        //transform.position = hex.transform.position;
        StartCoroutine(MoveCoroutine(hex));
    }

    // Coroutine for moving the unit
    private IEnumerator MoveCoroutine(Hex hex)
    {
        // Get the target position of the hex
        Vector3 targetPosition = hex.transform.position;

        // Move the unit towards the target position
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            // Calculate the next position to move towards
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, MOVE_SPEED * Time.deltaTime);

            // Update the position of the unit
            transform.position = newPosition;

            // Wait for the next frame
            yield return null;
        }

        // Snap the unit to the exact position of the hex
        PlaceOnHex(hex);
    }

    public void Attack(GameUnit target)
    {
        Debug.Log("attack");
        //animator.SetTrigger("Attack");
    }

    public void UseAbility()
    {
        // Check if the unit has an ability and execute it
        //Ability?.ExecuteAbility();
    }

    // Set the unit's properties based on unitData
    public override void SetUnitData(int id)
    {
        base.SetUnitData(id);
        MaxHp = UnitData.MaxHp;
        MaxMp = UnitData.MaxMp;
        BaseAttackDamage = UnitData.BaseAttackDamage;
    }

    public bool Equals(GameUnit other)
    {
        return UnitData.Id == other.UnitData.Id;
    }

    public void PlaceOnBenchSlot(BenchSlot benchSlot)
    {
        _currentBenchSlot = benchSlot;
        benchSlot.IsTaken = true;
        transform.position = _currentBenchSlot.transform.position;
        transform.SetParent(benchSlot.transform);
    }

    public void RemoveFromBench()
    {
        if (_currentBenchSlot != null)
        {
            _currentBenchSlot.IsTaken = false;
            _currentBenchSlot = null;
        }
    }

    public void PlaceOnHex(Hex hex)
    {
        if (hex == null)
        {
            Debug.LogWarning("hex is null");
            return;
        }
        if (_currentHex != null)
        {
            _currentHex.IsTaken = false;
        }
        _currentHex = hex;
        hex.IsTaken = true;
        hex.UnitOnHex = this;
        _isOnBoard = true;
        transform.SetParent(hex.transform);
        transform.position = hex.transform.position;
        MoveTowardsEnemyUnit();
    }

    public void RemoveFromBoard()
    {
        if (_currentHex != null)
        {
            _currentHex.IsTaken = false;
            _currentHex.UnitOnHex = null;
            _currentHex = null;
        }
        _isOnBoard = false;
        _owner.BoardUnits.Remove(this);
    }
}
