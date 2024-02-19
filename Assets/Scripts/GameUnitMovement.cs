using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(GameUnit))]
[RequireComponent(typeof(Pathfinding))]
public class GameUnitMovement : MonoBehaviour
{
    private Pathfinding _pathfinding;
    private GameUnit _gameUnit;

    private Hex _currentHex;
    private List<Hex> _currentShortestPath;
    private List<GameUnit> _enemyUnits;
    private Hex _currentTargetHex;
    private int _distanceToTarget;

    public readonly float MOVE_SPEED = 1.2415f; // movement speed of 1 hex per second

    private void Awake()
    {
        _gameUnit = GetComponent<GameUnit>();
        _pathfinding = GetComponent<Pathfinding>();
        GameManager.Instance.OnPhaseChanged += OnPhaseChanged;
    }

    private void OnPhaseChanged(GamePhase newPhase)
    {
        switch (newPhase)
        {
            case GamePhase.Preparation:
                break;
            case GamePhase.Battle:
                if (_gameUnit.IsOnBoard)
                {
                    MoveTowardsEnemyUnit();
                }
                break;
            case GamePhase.BattleWon:
            case GamePhase.BattleLost:
                break;
        }
    }

    public void MoveTowardsEnemyUnit()
    {
        StartCoroutine(MoveTowardsEnemyUnitCoroutine());
    }

    private IEnumerator MoveTowardsEnemyUnitCoroutine()
    {
        UpdatePathfinding();

        if (_currentShortestPath == null)
        {
            yield return new WaitForSeconds(1.001f);
            UpdatePathfinding();
        }


        if (_currentTargetHex != null)
        {
            if (_currentShortestPath != null)
            {
                // Move one step at a time along the path until reaching a hex adjacent to the enemy unit's hex
                foreach (Hex nextHex in _currentShortestPath)
                {
                    if (_currentHex.IsAdjacentToHex(nextHex))
                    {
                        if (_distanceToTarget <= _gameUnit.Range
                            || _currentHex.IsAdjacentToHex(_currentTargetHex))
                        {
                            _gameUnit.Attack(_currentTargetHex.UnitOnHex);
                            break;
                        }
                        else
                        {
                            yield return StartCoroutine(MoveCoroutine(nextHex));
                            UpdatePathfinding();
                        }
                    }
                }
            }
            else
            {
                Debug.Log("empty path");
            }
        }
        else
        {
            Debug.LogWarning("No enemy found");
        }

    }

    // Coroutine for moving the unit
    private IEnumerator MoveCoroutine(Hex hex)
    {
        _gameUnit.ChangeHex(hex);

        // Get the target position of the hex
        Vector3 targetPosition = hex.transform.position;

        // Move the unit towards the target position
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Calculate the next position to move towards
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, MOVE_SPEED * Time.deltaTime);
            transform.position = newPosition;

            // Wait for the next frame
            yield return null;
        }

        // Snap the unit to the exact position of the hex
        transform.SetParent(hex.transform);
        transform.position = hex.transform.position;
        _currentHex = _gameUnit.CurrentHex;
    }

    private void UpdatePathfinding()
    {
        // Find the shortest path to the enemy unit's hex
        if (_gameUnit.Owner == LocalPlayer.Instance)
        {
            _enemyUnits = Opponent.Instance.BoardUnits;
        }
        else
        {
            _enemyUnits = LocalPlayer.Instance.BoardUnits;
        }

        _currentHex = _gameUnit.CurrentHex;
        _currentTargetHex = _pathfinding.FindClosestEnemy(_enemyUnits, _currentHex);
        _currentShortestPath = _pathfinding.FindShortestPath(_currentHex, _currentTargetHex);
        if (_currentShortestPath != null)
        {
            _distanceToTarget = _currentShortestPath.Count;
            string str = "";
            foreach (Hex hex in _currentShortestPath)
            {
                str += hex.ToString() + " ";
            }
            Debug.Log(_gameUnit.UnitName + ": " + str);
        }

        _gameUnit.StopAttack(_currentTargetHex.UnitOnHex);


    }
}
