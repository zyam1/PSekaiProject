using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System.IO;
using System;
using Yarn.Unity;

public class ConversationButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private Vector3 enlargedScale;






    void Start()
    {
        originalScale = transform.localScale;
        enlargedScale = originalScale * 1.1f;

    }





    // 대화 종료 시 대화창 비활성화 


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
