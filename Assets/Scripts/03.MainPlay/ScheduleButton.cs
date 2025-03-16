using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System.IO;
using System;
using Yarn.Unity;
public class ScheduleButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private Vector3 enlargedScale;
    public GameObject schedulePanel;

    public GameObject YarnUI;
    public DialogueRunner dialogueRunner;
    void Start()
    {
        originalScale = transform.localScale;
        enlargedScale = originalScale * 1.1f;
        YarnUI.SetActive(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //특수 이벤트 조건(날짜 , 스텟 )등등 정의된 함수 작성 해야함bool형태 변수 만든뒤 비교연산 -> 어떤 패널이 열릴지 작성
        //true일경우  특수이벤트 제안하는 패녈(현재 패널), else 평범하게 달력보여주기 
        //나머지 UI치워짐

        schedulePanel.SetActive(true);
        YarnUI.SetActive(true);
        if (dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.Stop();
        }
        dialogueRunner.StartDialogue("ScheduleStart");



    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = enlargedScale;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // 기본 포인터 변경
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // 기본 커서 복원
    }
}
