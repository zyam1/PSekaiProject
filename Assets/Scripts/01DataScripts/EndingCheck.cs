using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EndingCheck : MonoBehaviour
{
    public Text endingText; // UI에 엔딩 목록을 표시할 Text 컴포넌트

    void Start()
    {
        UpdateEndingList();
    }

    //  UI에 엔딩 목록을 업데이트하는 함수
    public void UpdateEndingList()
    {
        if (DataManager.instance == null || DataManager.instance.endingsData == null)
        {
            endingText.text = "데이터를 불러올 수 없습니다.";
            return;
        }

        List<string> completedEndings = DataManager.instance.endingsData.completedEndings;

        if (completedEndings.Count == 0)
        {
            endingText.text = "해금한 엔딩이 없습니다.";
        }
        else
        {
            string allEndings = "해금한 엔딩 목록:\n";
            foreach (string ending in completedEndings)
            {
                allEndings += "- " + ending + "\n";
            }
            endingText.text = allEndings;
        }
    }

    // 📌 버튼을 눌렀을 때 엔딩 목록을 새로고침하는 함수
    public void RefreshEndingList()
    {
        UpdateEndingList();
    }
}
