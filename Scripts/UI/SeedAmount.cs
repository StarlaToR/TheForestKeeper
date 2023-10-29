using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedAmount : MonoBehaviour
{
    [SerializeField] private CharacterSeeds seeds;
    private int amount = 0;

    [SerializeField] public Image unit;
    [SerializeField] private Image ten;
    [SerializeField] private Image icon;

    [SerializeField] private List<Sprite> numbers;
    [SerializeField] private List<Sprite> glowyNumbers;
    [SerializeField] private List<Sprite> iconSprites;

    private float glowyTimer = 0f;
    private bool glowOn = false;

    // Update is called once per frame
    void Update()
    {
        if (amount != seeds.seedAmount)
        {
            amount = seeds.seedAmount;
            ChangeNumber();
            glowyTimer = 0.5f;
        }

        if (glowyTimer > 0f)
            glowyTimer -= Time.deltaTime;
        else if (glowOn)
            UnglowNumber();
    }

    private void ChangeNumber()
    {
        if (amount <= 9)
        {
            unit.sprite = glowyNumbers[amount];
            ten.sprite = glowyNumbers[0];
        }
        else
        {
            unit.sprite = glowyNumbers[0];
            ten.sprite = glowyNumbers[1];
        }

        icon.sprite = iconSprites[1];
        glowOn = true;
    }

    private void UnglowNumber()
    {
        if (amount <= 9)
        {
            unit.sprite = numbers[amount];
            ten.sprite = numbers[0];
        }
        else
        {
            unit.sprite = numbers[0];
            ten.sprite = numbers[1];
        }

        icon.sprite = iconSprites[0];
        glowOn = false;
    }
}
