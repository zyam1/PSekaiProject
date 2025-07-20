using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//todo:구현 해야 할것
//1.데이터가 있다면 메인메뉴가 이어하기로 바뀌어야함 , 없다면 좌측 서브메뉴가 이어하기(비활성화 흐리게 처리)
//2.이어하기 
//
public class MainMenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private Vector3 enlargedScale;
    public GameObject panel;
    void Start()
    {
        originalScale = transform.localScale;
        enlargedScale = originalScale * 1.1f; // 크기를 1.2배로 증가
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("이미지 클릭됨!");
        //패널열리게
        panel.SetActive(true);
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
