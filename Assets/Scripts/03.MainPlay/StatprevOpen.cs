using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class StatprevOpen : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    public GameObject PrevPanel;

    void Start()
    {
        PrevPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        PrevPanel.SetActive(true);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PrevPanel.SetActive(false);
    }
}
