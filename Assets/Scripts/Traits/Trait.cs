using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class Trait : ScriptableObject
{
    public string traitName;
    public string traitDescription;
    public Color BackgroundColor;
    public Sprite traitSprite;
    //protected List<BoardUnit> _unitsWithTrait; // Units on board with current trait
    [SerializeField] protected int[] _unitNumNeeded; // Units needed on board to activate each trait stage
    public int[] UnitNumNeeded { get => _unitNumNeeded; set => _unitNumNeeded = value; }

    //public abstract void ActivateTrait(Player owner, int stage);
    //public abstract void DeactivateTrait(Player owner, int stage);
    public abstract void StageUp(List<BoardUnit> unitsWithTrait, int currentStage);
    public abstract void StageDown(List<BoardUnit> unitsWithTrait, int currentStage);
    public abstract void UpdateTrait(List<BoardUnit> unitsWithTrait, BoardUnit lastUnit, int currentStage, int lastStage);

}
