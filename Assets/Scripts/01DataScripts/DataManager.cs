using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    //인스턴스 (=싱글톤:전역 참조 가능) 
    /*
    DataManager.instance.DoSomething();
    외부에서 이런식으로 사용, DoSomething()은 Datamanager 의 메서드 
    */
    public static DataManager instance;
    
    private void Awake(){
        if(instance==null){
            instance=this;
        }else if(instance !=this){
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
