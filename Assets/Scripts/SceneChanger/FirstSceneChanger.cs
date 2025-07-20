using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstSceneChanger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey){
            //아무 버튼이나 누르면 LoadScene으로 이동
            //나중에 타이틀 이미지로 채울 예정 
            SceneManager.LoadScene("LoadScene");


        }
           
        
    }
}
