using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;
public class GalleryButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private Vector3 enlargedScale;

    void Start()
    {
        originalScale = transform.localScale;
        enlargedScale = originalScale * 1.1f;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //갤러리 씬으로 이동
        SceneManager.LoadScene("GalleryScene");
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
