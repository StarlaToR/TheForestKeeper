using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image water;
    [SerializeField] private Image waterLogo;
    [SerializeField] private Image waterBar;

    [SerializeField] private Image oxygen;
    [SerializeField] private Image oxLogo;
    [SerializeField] private Image oxBar;

    [SerializeField] private Sprite[] waterLogoSprites;
    [SerializeField] private Sprite[] oxLogoSprites;
    [SerializeField] private Sprite[] barSprites;

    [SerializeField] private CharacterWater characterWater;
    [SerializeField] private CharacterOxygen characterOxygen;

    private bool waterGlow = false;
    private bool oxGlow = false;

    public float waterAmount = 0f;
    public float oxPercent = 0f;

    // Update is called once per frame
    void Update()
    {
        oxPercent = characterOxygen.GetPercentOxygen();
        if (oxygen.fillAmount != oxPercent)
        {
            oxygen.fillAmount = oxPercent;

            if (!oxGlow)
            {
                oxGlow = true;
                oxLogo.sprite = oxLogoSprites[1];
                oxBar.sprite = barSprites[1];
            }
        }
        else if (oxGlow)
        {
            oxGlow = false;
            oxLogo.sprite = oxLogoSprites[0];
            oxBar.sprite = barSprites[0];
        }

        waterAmount = characterWater.currentCarriedWater / characterWater.maxCarriedWater;
        if (water.fillAmount != waterAmount)
        {
            water.fillAmount = waterAmount;

            if (!waterGlow)
            {
                waterGlow = true;
                waterLogo.sprite = waterLogoSprites[1];
                waterBar.sprite = barSprites[1];
            }
        }
        else if (waterGlow)
        {
            waterGlow = false;
            waterLogo.sprite = waterLogoSprites[0];
            waterBar.sprite = barSprites[0];
        }
    }
}
