using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(GameUnit))]
public class GameUnitMovement : MonoBehaviour
{
    private GameUnit _gameUnit;

    private List<Hex> _currentPath;
    private int _distanceToTarget = int.MaxValue;
    private readonly float _moveSpeed = 1.2415f * 1.5f; // 1.2415f is movement speed of 1 hex per second

    private Task _movingLoopTask;
    private CancellationTokenSource _lifetimeCTS; // “alive or dead” token for this unit.
    private CancellationTokenSource _moveCTS; // Only for movement of this unit from one hex to another.
    public event Action OnMovementFinished;


    private RoundManager _roundManager;

    private void Awake()
    {
        _gameUnit = GetComponent<GameUnit>();
        _gameUnit.OnDeath += OnDeath;
    }

    private void OnDestroy()
    {
        // Additional tokens cancallation
        _lifetimeCTS?.Cancel();
        _moveCTS?.Cancel();
        //RoundManager.Instance.OnPhaseChanged -= OnPhaseChanged;
    }

    private void OnEnable()
    {
        _lifetimeCTS = new CancellationTokenSource();

        _roundManager = RoundManager.Instance;

        if (_roundManager != null)
        {
            _roundManager.OnPhaseChanged += OnPhaseChanged;
        }
    }

    private void OnDisable()
    {
        // Every async operation that was using this token will now stop.
        _lifetimeCTS?.Cancel();
        _lifetimeCTS?.Dispose();
        _lifetimeCTS = null;

        if (_roundManager != null)
        {
            _roundManager.OnPhaseChanged -= OnPhaseChanged;
        }
    }

    private void OnPhaseChanged(GamePhase newPhase)
    {
        switch (newPhase)
        {
            case GamePhase.Preparation:
                break;
            case GamePhase.RoundStart:
                StartBattle();
                break;
        }
    }

    // Start fighting when battle begins
    private void StartBattle()
    {
        // Reset movement
        _moveCTS?.Cancel();
        _moveCTS?.Dispose();
        _moveCTS = new CancellationTokenSource();

        if (_gameUnit.IsOnBoard)
        {
            StartMovingLoopIfNotRunning();
        }
    }

    // If pathfinding is already running, don’t start another one
    private void StartMovingLoopIfNotRunning()
    {
        if (_movingLoopTask != null && !_movingLoopTask.IsCompleted)
            return;

        CancellationToken token = _lifetimeCTS != null ? _lifetimeCTS.Token : CancellationToken.None;
        if (!token.IsCancellationRequested)
        {
            _movingLoopTask = MoveTowardsEnemyLoop();
        }
    }

    private async Task MoveTowardsEnemyLoop()
    {
        // Token for both being alive and moving
        CancellationToken token = CancellationTokenSource.CreateLinkedTokenSource(_lifetimeCTS.Token, _moveCTS.Token).Token;

        // Check if the unit is alive and movement is not cancelled
        while (!token.IsCancellationRequested)
        {
            // Find closest enemy and update the path
            bool foundPath = await UpdatePathfinding(token);
            if (!foundPath)
            {
                OnMovementFinished?.Invoke();
                return;
            }
            else
            {
                _gameUnit.StateManager.RequestMove();

                // found path & target
                bool canAttack = CheckForEnemyAttack(token);
                if (canAttack)
                {
                    OnMovementFinished.Invoke();
                    _gameUnit.StateManager.RequestAttack();
                    await WaitUntilFinishedAttack();
                    continue;
                }

                // not in range yet -> move one hex
                await MoveOneHexInPath(token);
            }
        }
    }

    // Find a path to closest enemy and update the current path
    // Return true if found
    private async Task<bool> UpdatePathfinding(CancellationToken token)
    {
        // Small randomized delay to make sure not all units search for a path at the same time
        int rnd = Random.Range(100, 200);
        await Task.Delay(rnd, token);

        // No need for pathfinding while attacking.
        if (_gameUnit.StateManager.CurrentState == UnitState.Attacking)
            return false;

        List<GameUnit> enemyUnits = _gameUnit.GetEnemyUnits();
        Pathfinding pathfinding = new();

        Hex newTargetHex = pathfinding.FindClosestEnemy(enemyUnits, _gameUnit.CurrentHex);

        if (newTargetHex == null)
        {
            return false; // The unit finds no enemy to attack
        }

        // Get the path without ignoring obstacles
        List<Hex> newPath = pathfinding.FindShortestPath(_gameUnit.CurrentHex, newTargetHex, false);
        _currentPath = newPath;
        _gameUnit.CurrentTarget = newTargetHex.UnitOnHex;

        // Ignore obstacles to get the heuristic path and the distance
        pathfinding = new();
        List<Hex> heuristicPath = pathfinding.FindShortestPath(_gameUnit.CurrentHex, newTargetHex, true);
        _distanceToTarget = heuristicPath.Count;

        return true;
    }

    // Start the Unit movement to next hex in the path
    private async Task MoveOneHexInPath(CancellationToken token)
    {
        if (token.IsCancellationRequested || _currentPath == null)
        {
            return;
        }
        Hex nextHex = _currentPath[0];

        await MoveUnit(nextHex, _moveCTS.Token);

        _gameUnit.AnimationController.StopAnimateMovement();
    }

    // Move the unit object visually from current hex to target hex 
    private async Task MoveUnit(Hex targetHex, CancellationToken token)
    {

        if (token.IsCancellationRequested) return;
        await Task.Yield();

        // Instantly change the unit's hex to allow other units to interact with it directly 
        // or calculate paths accodingly
        _gameUnit.ChangeHex(targetHex);
        transform.SetParent(targetHex.transform);

        // Move the unit towards the target position
        Vector3 targetPosition = targetHex.transform.position;
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            if (token.IsCancellationRequested) return;

            // Calculate the next position to move towards
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);
            transform.position = newPosition;

            // Allow other tasks to run before continuing execution
            await Task.Yield();
        }

        // Snap the unit to the exact position of the hex
        transform.position = targetHex.transform.position;
    }

    // Attack if there is an enemy unit in adjacent hexes or in range
    private bool CheckForEnemyAttack(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return false;
        }

        List<GameUnit> enemyUnits = _gameUnit.GetEnemyUnits();
        foreach (GameUnit enemyUnit in enemyUnits)
        {
            if (_distanceToTarget <= _gameUnit.Range && _gameUnit.CurrentTarget == enemyUnit)
            {
                _gameUnit.CurrentTarget = enemyUnit;
                return true;
            }
        }
        return false;
    }

    private async Task WaitUntilFinishedAttack()
    {
        // Safety: if something already canceled, bail out immediately
        if (_lifetimeCTS == null || _lifetimeCTS.IsCancellationRequested) return;
        if (_moveCTS == null || _moveCTS.IsCancellationRequested) return;

        // Wait until either:
        // - GameUnit state is no longer Attacking, OR
        // - GameUnitAttack reports it is not attacking, OR
        // - the unit died, OR
        // - the movement/lifetime tokens are canceled
        while (true)
        {
            // stop waiting if unit died or cancellation requested
            if (_gameUnit.IsDead()) break;
            if (_lifetimeCTS == null || _lifetimeCTS.IsCancellationRequested) break;
            if (_moveCTS == null || _moveCTS.IsCancellationRequested) break;

            // if GameUnit state left attacking, stop waiting
            if (_gameUnit.StateManager.CurrentState != UnitState.Attacking) break;

            // If attack component itself says it finished, stop waiting
            // (keeps logic resilient in case GameUnitAttack doesn't change GameUnit.State)
            if (!(_gameUnit.StateManager.CurrentState == UnitState.Attacking)) break;

            // yield to the next frame / allow cancellation to be observed
            await Task.Yield();
        }
    }

    // Called when the unit dies
    private void OnDeath(GameUnit gameUnit)
    {
        if (gameUnit == _gameUnit)
        {
            _lifetimeCTS?.Cancel();
            _moveCTS?.Cancel();
        }
    }
}
