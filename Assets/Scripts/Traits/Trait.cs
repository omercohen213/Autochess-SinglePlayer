using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class Trait : ScriptableObject
{
    public string TraitName;
    public string Description;
    public List<GameUnit> UnitsWithTrait;
    public string Stage1Text;
    public string Stage2Text;
    public string Stage3Text;
    public string Stage4Text;
    public Color BackgroundColor;
    public Sprite TraitSprite;
    [SerializeField] protected int[] _unitNumNeeded; // Units needed on board to activate each trait stage

    public int[] UnitNumNeeded { get => _unitNumNeeded; set => _unitNumNeeded = value; }
    public abstract void StageUp(List<GameUnit> unitsWithTrait, int currentStage);
    public abstract void StageDown(List<GameUnit> unitsWithTrait, int currentStage);
    public abstract void UpdateTrait(List<GameUnit> unitsWithTrait, GameUnit lastUnit, int currentStage, int lastStage);

}
