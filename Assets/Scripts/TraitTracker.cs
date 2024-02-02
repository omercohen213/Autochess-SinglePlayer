using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraitTracker : MonoBehaviour
{
    [SerializeField] private Transform _activeTraitsTransform;
    [SerializeField] private GameObject _activeTraitPrefab;
    [SerializeField] private Transform _inactiveTraitsTransform;
    [SerializeField] private GameObject _inactiveTraitPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private bool IsTraitActive(Trait trait, int unitCount)
    {
        return unitCount >= trait.UnitNumNeeded[0];
    }

    public void AddTrait(Trait trait, int currentStage, int unitCount)
    {
        GameObject traitPrefab = IsTraitActive(trait, unitCount) ? _activeTraitPrefab : _inactiveTraitPrefab;
        Transform parent = IsTraitActive(trait, unitCount) ? _activeTraitsTransform : _inactiveTraitsTransform;
        GameObject traitUIGo = Instantiate(traitPrefab, parent);

        traitUIGo.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = trait.traitName;
        traitUIGo.transform.Find("CurrentStage").GetComponent<TextMeshProUGUI>().text = currentStage.ToString();
        traitUIGo.transform.Find("Image").GetComponent<Image>().sprite = trait.traitSprite;
        string stagesStr = "";
        for (int i = 0; i < trait.UnitNumNeeded.Length; i++)
        {
            stagesStr += trait.UnitNumNeeded[i].ToString() + " > ";
        }
        traitUIGo.transform.Find("Stages").GetComponent<TextMeshProUGUI>().text = stagesStr;
    }
}