using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatPanel : MonoBehaviour
{
    public TextMeshProUGUI intelligenceText, strengthText, stressText, charismaText, eleganceText, moralityText, fameText, magicText, faithText, artText;

    void Update()
    {
        var stats = DataManager.instance.nowStats;

        intelligenceText.text = stats.intelligence.ToString();
        strengthText.text = stats.strength.ToString();
        stressText.text = stats.stress.ToString();
        charismaText.text = stats.charisma.ToString();
        eleganceText.text = stats.elegance.ToString();
        moralityText.text = stats.morality.ToString();
        fameText.text = stats.fame.ToString();
        magicText.text = stats.magic.ToString();
        faithText.text = stats.faith.ToString();
        artText.text = stats.art.ToString();
    }
}
