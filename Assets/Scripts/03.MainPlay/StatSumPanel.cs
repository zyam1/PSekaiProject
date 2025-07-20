using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StatSumPanel : MonoBehaviour
{
    public GameObject statSumPanel;
    public TextMeshProUGUI intelligenceText;
    public TextMeshProUGUI strengthText;
    public TextMeshProUGUI stressText;
    public TextMeshProUGUI charismaText;
    public TextMeshProUGUI eleganceText;
    public TextMeshProUGUI moralityText;
    public TextMeshProUGUI fameText;
    public TextMeshProUGUI magicText;
    public TextMeshProUGUI faithText;
    public TextMeshProUGUI artText;
    public TextMeshProUGUI monthText;
    public TextMeshProUGUI yearText;
    Dictionary<string, int> prevStats;
    Dictionary<string, int> nowStats;
    void Start()
    {
        statSumPanel.SetActive(false);
    }

    public void ShowStatChanges(Dictionary<string, int> prevStats, Dictionary<string, int> currentStats)
    {
        statSumPanel.SetActive(true);

        AnimateText("intelligence", prevStats["intelligence"], currentStats["intelligence"], intelligenceText);
        AnimateText("strength", prevStats["strength"], currentStats["strength"], strengthText);
        AnimateText("stress", prevStats["stress"], currentStats["stress"], stressText);
        AnimateText("charisma", prevStats["charisma"], currentStats["charisma"], charismaText);
        AnimateText("elegance", prevStats["elegance"], currentStats["elegance"], eleganceText);
        AnimateText("morality", prevStats["morality"], currentStats["morality"], moralityText);
        AnimateText("fame", prevStats["fame"], currentStats["fame"], fameText);
        AnimateText("magic", prevStats["magic"], currentStats["magic"], magicText);
        AnimateText("faith", prevStats["faith"], currentStats["faith"], faithText);
        AnimateText("art", prevStats["art"], currentStats["art"], artText);
        //날짜랑 년도는 변화 없이 표기
        monthText.text = prevStats["month"].ToString();
        yearText.text = prevStats["year"].ToString();
    }

    void AnimateText(string statName, int from, int to, TextMeshProUGUI target)
    {
        StartCoroutine(AnimateStatOnly(statName, from, to, target));
    }
    IEnumerator AnimateStatOnly(string statName, int from, int to, TextMeshProUGUI text)
    {
        int current = from;
        int step = (to > from) ? 1 : -1;

        while (current != to)
        {
            current += step;
            text.text = current.ToString();  // ← 숫자만 출력

            yield return new WaitForSeconds(0.03f);  // 부드럽게 변화
        }

        text.text = to.ToString();  // 마지막 값
    }


    // Update is called once per frame
    void Update()
    {
        //여기에 버튼 할당 해야함 
        if (Input.anyKeyDown)
        {
            statSumPanel.SetActive(false);
        }
    }

}
