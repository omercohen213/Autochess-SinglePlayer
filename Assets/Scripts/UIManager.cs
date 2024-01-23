using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _xpText;
    [SerializeField] private TextMeshProUGUI _lvlText;

    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new();
                    _instance = singletonObject.AddComponent<UIManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    private void Start()
    {
        UpdateGoldUI();
        UpdateXpUI();
    }

    // Update current gold text on change
    public void UpdateGoldUI()
    {
        _goldText.text = Player.Instance.Gold.ToString() + "$";
    }

    // Update current xp text on change
    public void UpdateXpUI()
    {
        _xpText.text = Player.Instance.Xp.ToString() + "/" + GameManager.Instance.GetXpToLevelUp(Player.Instance.Lvl).ToString();
    }

    // Update current lvl text on change
    public void UpdatePlayerLvlUI()
    {
        _lvlText.text = Player.Instance.Lvl.ToString();
    }


    /* public void ShowUIEntityStats(GameObject gameObject)
     {
         Entity entity = gameObject.GetComponent<Entity>();
         _activeTarget = entity;
         _isTargetActive = true;
     }
     public void UpdateUIEntityStats()
     {
         _entityStats.SetActive(true);
         _hpText.text = _activeTarget.Hp + " / " + _activeTarget.MaxHp;
         float hpRatio = (float)_activeTarget.Hp / _activeTarget.MaxHp;
         _hpBar.localScale = new Vector3(hpRatio, 1, 1);
     }*/
}
