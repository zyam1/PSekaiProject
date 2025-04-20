using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class SchedulePanel : MonoBehaviour
{
    public GameObject panel;
    public GameObject Animepanel;
    public GameObject ConfirmElements;
    public GameObject scheduleLiCon;
    public GameObject RightUI;
    public GameObject YarnUI;
    public GameObject GrimorieButton;

    public List<int> schedule = new List<int>();

    public DialogueRunner dialogueRunner;  // Yarn Spinner 대화연결
    public AnimationStatEvent animationStatEvent;


    public void AddToSchedule(int number)
    {
        if (schedule.Count >= 28) // 일주일 (7개씩 4세트) 완료 시 추가 불가
        {
            Debug.LogWarning("스케줄이 이미 가득 찼습니다!");
            return;
        }

        // 성공 확률 정의
        //1.금기서적 2.푸드코드 알바 3.발레 4.피아노연주알바 5.마법연구 6.검술대련 7.책읽기 8.공연 9.기도 
        //10.산책(봄) 11.산책(여름) 12.산책(가을) 21.봉사활동
        //!(스케쥴,성공확률)
        Dictionary<int, float> successRates = new Dictionary<int, float>
    {
        { 1, 1f }, { 2, 1f }, { 3, 0.6f }, { 4, 0.5f },
        { 5, 0.6f }, { 6, 0.7f }, { 7, 0.9f }, { 8, 0.7f },
        { 9, 0.9f }, { 10, 1f }, { 11, 1f }, { 12, 1f },
        {21,0.5f}
    };

        // 실패 시 번호 정의
        Dictionary<int, int> failureValues = new Dictionary<int, int>
    {
         { 3, 14 }, { 4, 15 }, { 5, 16 },
        { 6, 17 }, { 7, 18 }, { 8, 19 }, { 9, 20 },
        { 21, 22 }
    };

        // 스케줄 자동 채우기 (7개 추가)
        for (int i = 0; i < 7; i++)
        {
            if (successRates.ContainsKey(number))
            {
                float successChance = successRates[number];
                bool isSuccess = UnityEngine.Random.value <= successChance;

                // 성공 시: 원본 번호 추가 / 실패 시: 실패 번호 추가
                if (isSuccess || !failureValues.ContainsKey(number))
                {
                    schedule.Add(number);  // 성공 시
                }
                else
                {
                    schedule.Add(failureValues[number]);  // 실패 시
                }
            }
            else
            {
                schedule.Add(number);  // 확률이 없는 경우 그대로 추가
            }


        }

        Debug.Log($"활동 추가됨: {string.Join(", ", schedule)}");
        if (dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.Stop();
        }
        dialogueRunner.StartDialogue($"Schedule{number}");

        // 스케줄이 28개가 찼다면 확정
        if (schedule.Count >= 28)
        {
            ConfirmSchedule();
        }
    }

    void ConfirmSchedule()
    {
        Debug.Log("스케줄 확정!");
        scheduleLiCon.SetActive(false);// 기존 스케줄을 담고 있는 빈 객체 삭제
        ConfirmElements.SetActive(true); // 확정 여부 판단 GameObject 활성화


    }
    void Start()
    {
        panel.SetActive(false);
        ConfirmElements.SetActive(false);
        Animepanel.SetActive(false);
        GrimorieButton.SetActive(false);



    }

    // Update is called once per frame
    public void Closepanel()
    {
        panel.SetActive(false);
        scheduleLiCon.SetActive(true);
        ConfirmElements.SetActive(false);
        schedule.Clear();
        YarnUI.SetActive(false);
    }

    public void ResetSchedule()
    {
        scheduleLiCon.SetActive(true);
        ConfirmElements.SetActive(false);
        schedule.Clear();
        YarnUI.SetActive(true);
        if (dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.Stop();
        }
        dialogueRunner.StartDialogue("ScheduleStart");

    }
    public void StartSchedule()
    {
        Debug.Log($"스케줄 시작 현재 스케줄: {string.Join(", ", schedule)}");

        panel.SetActive(false);
        scheduleLiCon.SetActive(true);
        ConfirmElements.SetActive(false);

        Animepanel.SetActive(true);
        RightUI.SetActive(false);
        YarnUI.SetActive(false);



        animationStatEvent.PlayScheduleAnimation(schedule);




    }


}
