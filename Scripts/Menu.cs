using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    //싱글턴
    public static Menu instance;
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            Debug.Log("재생성");
        }
    }

    public GameObject go; //메뉴 오브젝트 활성화 비활성화
    public AudioManager theAudio;

    public string call_sound; //메뉴 호출 사운드
    public string cancel_sound; //메뉴 껐을 때 사운드

    public OrderManager theOrder; //메뉴 호출시 캐릭터 움직임 제한

    public GameObject[] gos; //게임 오브젝트들을 전부다 삭제하는 식으로 초기화 진행

    private bool activated;

    public void Exit() //종료
    {
        Application.Quit(); //게임 종료
    }

    public void Continue() //계속
    {
        activated = false;
        go.SetActive(false);
        theOrder.Move();
        theAudio.Play(cancel_sound);
    }

    public void GoToTitle() //타이틀로
    {
        for(int i=0; i<gos.Length; i++) //모든 객체 파괴
            Destroy(gos[i]);
        go.SetActive(false); //메뉴 감춤
        activated = false; //키면서 true로 했으니 원복
        SceneManager.LoadScene("title");
    }    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) //esc 키로 메뉴 호출
        {
            activated = !activated;

            if(activated)
            {
                theOrder.NotMove();
                go.SetActive(true);
                theAudio.Play(call_sound);
            }
            else
            {
                go.SetActive(false);
                theAudio.Play(cancel_sound);
                theOrder.Move();
            }
        }
    }
}
