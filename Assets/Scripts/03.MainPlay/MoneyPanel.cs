using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MoneyPanel : MonoBehaviour
{
    public TextMeshProUGUI MoneyText;
    //!레거시 코드
    // void Update()
    // {
    //     var money = DataManager.instance.nowPlayer.money;
    //     MoneyText.text = money.ToString();
    // }
    void Update()
{
    // null 체크를 통한 안전한 접근
    if (DataManager.instance == null || DataManager.instance.nowPlayer == null || MoneyText == null)
    {
        return; // 초기화가 완료되지 않은 경우 종료
    }
    
    var money = DataManager.instance.nowPlayer.money;
    MoneyText.text = money.ToString();
}
}



