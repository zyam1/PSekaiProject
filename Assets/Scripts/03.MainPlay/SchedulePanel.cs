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
    public AnimationStatEvent animationStatEvent;


    public void AddToSchedule(int number)
    {
        //1.금기서적 2.푸드코드 알바 3.발레 4.피아노연주알바 5.마법연구 6.검술대련 7.책읽기 8.공연 9.기도 
        //10.산책(봄) 11.산책(여름) 12.산책(가을) 
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

        Animepanel.SetActive(true);
        RightUI.SetActive(false);
        YarnUI.SetActive(false);



        animationStatEvent.PlayScheduleAnimation(schedule);


    }


}
