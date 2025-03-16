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
    public List<int> schedule = new List<int>();

    public DialogueRunner dialogueRunner;  // Yarn Spinner 대화연결
    public void AddToSchedule(int number)
    {
        //독서1,알바2,공부3,휴식4
        if (schedule.Count < 4) // 최대 4개까지만 추가
        {
            schedule.Add(number);
            Debug.Log($"추가됨: {number}, 현재 스케줄: {string.Join(", ", schedule)}");
            // 대화 중이면 강제로 종료 후 시작
            if (dialogueRunner.IsDialogueRunning)
            {
                dialogueRunner.Stop();
                Debug.Log("이전 대화 종료 후 새 대화 시작");
            }
            dialogueRunner.StartDialogue($"Schedule{number}");

            if (schedule.Count == 4) // 4개가 쌓이면 확정
            {
                ConfirmSchedule();
                if (dialogueRunner.IsDialogueRunning)
                {
                    dialogueRunner.Stop();
                }
                dialogueRunner.StartDialogue("ScheduleConfirm");

            }
        }
        else
        {
            Debug.LogWarning("스케줄이 이미 4개 찼습니다!");
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
        schedule.Clear();
        //초기화 시킨뒤 
        Animepanel.SetActive(true);
        //애니메이션 패널이 열릴경우 우측ui false
        RightUI.SetActive(false);
        YarnUI.SetActive(false);


    }

}
