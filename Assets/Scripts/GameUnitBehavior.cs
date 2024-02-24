using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using VHierarchy.Libs;
using static UnityEngine.UI.Image;
using Random = UnityEngine.Random;

[RequireComponent(typeof(GameUnit))]
public class GameUnitBehavior : MonoBehaviour
{
    protected GameUnit _gameUnit;
    protected Animator _animator;
    protected bool _isAttacking = false;

    private List<Hex> _currentPath;
    private int _distanceToTarget = int.MaxValue;
    private readonly float _moveSpeed = 1.2415f / 1.5f; // 1.2415f is movement speed of 1 hex per second

    private GameUnit _currentTarget;

    protected virtual void Awake()
    {
        _gameUnit = GetComponent<GameUnit>();

        Transform animationTransform = transform.Find("Animation");
        if (animationTransform != null)
        {
            _animator = animationTransform.GetComponent<Animator>();
        }
        else
        {
            Debug.LogWarning("Missing animation transform on game unit " + _gameUnit.UnitName);
        }
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
                    UpdatePathfinding();
                }
                break;
            case GamePhase.BattleWon:
            case GamePhase.BattleLost:
                break;
        }
    }

    /*    protected override void Awake()
        {
            base.Awake();
            _pathfinding = new();
        }*/

    public async Task MoveTowardsEnemy()
    {
        CheckForEnemy();

        if (_currentPath != null && !_isAttacking)
        {
            Hex nextHex = _currentPath[0];

            // Move the unit to the next hex
            await MoveUnit(nextHex);

            // Check if enemy was found and update path after reaching the next hex
            CheckForEnemy();
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

        if (_isAttacking)
        {
            return;
        }

        List<GameUnit> enemyUnits = _gameUnit.GetEnemyUnits();
        Pathfinding pathfinding = new();

        Hex newTargetHex = pathfinding.FindClosestEnemy(enemyUnits, _gameUnit.CurrentHex);
        List<Hex> newPath = pathfinding.FindShortestPath(_gameUnit.CurrentHex, newTargetHex);
        _currentPath = newPath;
        if (_currentPath != null)
        {
            _distanceToTarget = _currentPath.Count;
        }

        await MoveTowardsEnemy();
    }

    // Attack if there is an enemy unit in adjacent hexes or in range
    private void CheckForEnemy()
    {
        List<GameUnit> enemyUnits = _gameUnit.GetEnemyUnits();
        foreach (Hex hex in _gameUnit.CurrentHex.AdjacentHexes)
        {
            foreach (GameUnit enemyUnit in enemyUnits)
            {
                if (_distanceToTarget <= _gameUnit.Range || hex.UnitOnHex == enemyUnit)
                {
                    Attack(hex.UnitOnHex);
                }
            }
        }
    }

    public void Attack(GameUnit target)
    {
        _isAttacking = true;
        _currentTarget = target;
        if (_animator != null)
        {
            _animator.SetTrigger("Cast0");
        }
        Debug.Log(_gameUnit.UnitName + " attacking");
    }
}
