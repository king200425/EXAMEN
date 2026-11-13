using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    GameObject[] bars;

    [SerializeField]
    private Color[] healthColors;

    public void SetMaxHealth(int maxHealth)
    {
        if (bars != null)
        {
            foreach (GameObject go in bars)
                Destroy(go);

            Array.Clear(bars, 0, bars.Length); //innecessari però aixi veieu un "delete" de c#
        }


        bars = new GameObject[maxHealth];
        bars[0] = transform.GetChild(0).GetChild(0).gameObject;

        Color currentColor = healthColors[Mathf.Min(healthColors.Length, maxHealth) - 1];
        bars[0].GetComponent<Image>().color = currentColor;
        for (int i = 1; i < maxHealth; i++)
        {
            bars[i] = Instantiate(bars[0], transform.GetChild(0)); //poc òptim pero me sirve :^)
            bars[i].GetComponent<Image>().color = currentColor;
        }
    }

    public void UpdateCurrentHealth(int currentAmount)
    {
        if(currentAmount <= 0)
        {
            for (int i = 0; i < bars.Length; i++)
                bars[i].SetActive(false);
            return;
        }

        Color currentColor = healthColors[Mathf.Min(healthColors.Length, currentAmount) - 1];
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].SetActive(i < currentAmount);
            bars[i].GetComponent<Image>().color = currentColor;
        }
    }
}
