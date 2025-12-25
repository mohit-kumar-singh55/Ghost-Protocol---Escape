using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public Image[] InitializeHealthUI(int totalHealth, GameObject healthUIParent, GameObject healthUIPrefab)
    {
        // initialize health UI
        Image[] healthUIs = new Image[totalHealth];
        for (int i = 0; i < totalHealth; i++) healthUIs[i] = Instantiate(healthUIPrefab, healthUIParent.transform).GetComponent<Image>();
        return healthUIs;
    }

    public void UpdateHealthUI(int currentHealth, Image[] healthUIs, Sprite EmptyhealthImg)
    {
        for (int i = 0; i < healthUIs.Length; i++)
        {
            if (i < currentHealth) continue;
            healthUIs[i].sprite = EmptyhealthImg;
        }
    }
}