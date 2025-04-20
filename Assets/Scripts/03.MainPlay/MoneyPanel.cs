using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MoneyPanel : MonoBehaviour
{
    public TextMeshProUGUI MoneyText;
    void Update()
    {
        var money = DataManager.instance.nowPlayer.money;
        MoneyText.text = money.ToString();
    }
}
