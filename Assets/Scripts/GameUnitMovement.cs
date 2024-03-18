using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(GameUnit))]
[RequireComponent(typeof(GameUnitAttack))]
public class GameUnitMovement : MonoBehaviour
{
    private GameUnit _gameUnit;
    private GameUnit _currentTarget;

    private List<Hex> _currentPath;
    private int _distanceToTarget = int.MaxValue;
    private readonly float _moveSpeed = 1.2415f * 1.5f; // 1.2415f is movement speed of 1 hex per second

    private GameUnitAttack _gameUnitAttack;
    private CancellationTokenSource _moveTowardsEnemyCTS;
    private CancellationTokenSource _moveUnitCTS;

    private void Awake()
    {
        _gameUnit = GetComponent<GameUnit>();
        _gameUnitAttack = GetComponent<GameUnitAttack>();
        GameUnit.OnDeath += OnDeath;
        GameManager.Instance.OnPhaseChanged += OnPhaseChanged;
    }

    private void OnDestroy()
    {
        _moveUnitCTS?.Cancel();
        _moveTowardsEnemyCTS?.Cancel();
        //GameManager.Instance.OnPhaseChanged -= OnPhaseChanged;
    }

    private void OnPhaseChanged(GamePhase newPhase)
    {
        switch (newPhase)
        {
            case GamePhase.Preparation:
                break;
            case GamePhase.Battle:
                StartBattle();
                break;
        }
    }

    // Start fighting when battle begins
    private void StartBattle()
    {
        if (_gameUnit.IsOnBoard)
        {
            UpdatePathfinding();
        }
        _moveUnitCTS = new();
        _moveTowardsEnemyCTS = new();
    }

    // Given the current path, move towards enemy checking after each move if there is one to attack
    public async Task MoveTowardsEnemy()
    {
        // Stop the task if it was cancelled
        if (_moveTowardsEnemyCTS.IsCancellationRequested)
        {
            return;
        }

        CheckForEnemy();

        if (_currentPath != null)
        {
            if (!_gameUnitAttack.IsAttacking)
            {
                Hex nextHex = _currentPath[0];

                // Move the unit to the next hex
                _gameUnit.AnimateMovement();
                await MoveUnit(nextHex);
                _gameUnit.StopAnimateMovement();

                // Check if enemy was found and update path after reaching the next hex
                CheckForEnemy();
            }
            else
            {
                return;
            }
        }
        UpdatePathfinding();

    }

    // Move the unit object from current hex to target hex 
    private async Task MoveUnit(Hex targetHex)
    {
        // Instantly change the unit's hex to allow other units to interact with it directly 
        // or calculate paths accodingly
        _gameUnit.ChangeHex(targetHex);
        transform.SetParent(targetHex.transform);

        // Move the unit towards the target position
        Vector3 targetPosition = targetHex.transform.position;
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Calculate the next position to move towards
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);
            transform.position = newPosition;

            // Allow other tasks to run before continuing execution
            await Task.Yield();

            //cancellationToken.ThrowIfCancellationRequested();
            // Check if the task has been canceled
            if (_moveUnitCTS.IsCancellationRequested)
            {
                return;
            }
        }

        // Snap the unit to the exact position of the hex
        transform.position = targetHex.transform.position;
    }

    // Calculate a new path from current hex to closest enemy
    public async void UpdatePathfinding()
    {
        // Small delay to make sure not all units search for a path at the same time
        int rnd = Random.Range(100, 200);
        await Task.Delay(rnd);

        // Unit is attacking, no need to search for a new enemy
        if (_gameUnitAttack.IsAttacking)
        {
            return;
        }

        List<GameUnit> enemyUnits = _gameUnit.GetEnemyUnits();
        Pathfinding pathfinding = new();

        Hex newTargetHex = pathfinding.FindClosestEnemy(enemyUnits, _gameUnit.CurrentHex);

        // No enemies left- battle phase has ended
        if (newTargetHex == null)
        {
            Debug.Log("No enemies left");
            GameManager.Instance.SwitchToPhase(GamePhase.BattleResult);
            return;
        }

        // Get the path without ignoring obstacles
        List<Hex> newPath = pathfinding.FindShortestPath(_gameUnit.CurrentHex, newTargetHex, false);
        _currentPath = newPath;
        _currentTarget = newTargetHex.UnitOnHex;

        // Ignore obstacles to get the heuristic path and the distance
        pathfinding = new();
        List<Hex> heuristicPath = pathfinding.FindShortestPath(_gameUnit.CurrentHex, newTargetHex, true);
        _distanceToTarget = heuristicPath.Count;

        await MoveTowardsEnemy();
    }

    // Attack if there is an enemy unit in adjacent hexes or in range
    private void CheckForEnemy()
    {
        if (_moveTowardsEnemyCTS.IsCancellationRequested)
        {
            return;
        }

        List<GameUnit> enemyUnits = _gameUnit.GetEnemyUnits();
        foreach (GameUnit enemyUnit in enemyUnits)
        {
            if (_distanceToTarget <= _gameUnit.Range && _currentTarget == enemyUnit)
            {
                _gameUnitAttack.Attack(_currentTarget);
            }
        }
    }

    // Called when the unit dies
    private void OnDeath(GameUnit gameUnit)
    {
        if (gameUnit == _gameUnit)
        {
            _moveUnitCTS?.Cancel();
            _moveTowardsEnemyCTS?.Cancel();
        }
    }
}
