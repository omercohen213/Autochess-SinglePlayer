using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUnitUI : MonoBehaviour
{
    private GameUnit _gameUnit;

    private void Awake()
    {
        _gameUnit = GetComponent<GameUnit>();
    }

    public void UpdateUnitHPBar()
    {
        if (transform.Find("Bars").Find("HpBar").TryGetComponent<Slider>(out var fill))
        {
            int maxHp = _gameUnit.MaxHp;
            int currentHp = _gameUnit.Hp;
            float fillAmount = (float)currentHp / maxHp;
            fill.value = fillAmount;
        }
        else
        {
            Debug.LogWarning("Missing HP bar objects");
        }
    }

    public void UpdateUnitMPBar()
    {
        if (transform.Find("Bars").Find("MpBar").TryGetComponent<Slider>(out var fill))
        {
            int maxMp = _gameUnit.MaxMp;
            int currentMp = _gameUnit.Mp;
            float fillAmount = (float)currentMp / +maxMp;
            fill.value = fillAmount;
        }
        else
        {
            Debug.LogWarning("Missing MP bar objects");
        }
    }

    public void ShowBars()
    {
        Transform bars = transform.Find("Bars");
        if (bars != null)
        {
            bars.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Missing bars object");
        }
        UpdateUnitHPBar();
        UpdateUnitMPBar();
    }

    public void HideBars()
    {
        Transform bars = transform.Find("Bars");
        if (bars != null)
        {
            bars.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Missing bars object");
        }
    }

    public void HideStars()
    {
        Transform starsParent = transform.Find("Stars");
        if (starsParent != null)
        {
            starsParent.gameObject.SetActive(false);
        }
    }
}
