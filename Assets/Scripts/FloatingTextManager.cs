using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager instance;

    public Transform textContainer;
    public GameObject textPrefab;

    private void Awake()
    {
        instance = this;
    }

    public void ShowFloatingText(string text,int fontSize, Color color, Vector3 position, string animTrigger , float destroyTimer)
    {       
        GameObject floatingText = Instantiate(textPrefab, textContainer);
        Text txt = floatingText.GetComponentInChildren<Text>();

        floatingText.GetComponentInChildren<Transform>().transform.position = position;
        txt.text = text;
        txt.fontSize = fontSize;
        txt.color = color;

        if (animTrigger != null)
            floatingText.gameObject.GetComponentInChildren<Animator>().SetTrigger(animTrigger);

        Destroy(floatingText, destroyTimer);
    }
}
