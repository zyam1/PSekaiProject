using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPanel : MonoBehaviour
{
    public GameObject panel;//닫을 패널
                            // Start is called before the first frame update

    void Start()
    {
        panel.SetActive(false);
    }
    public void Closepanel()
    {
        panel.SetActive(false);
    }
}
