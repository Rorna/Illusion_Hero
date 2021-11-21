using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    private FadeManager theFade; //화면 페이드 효과
    private AudioManager theAudio;

    public string click_sound;

    private PlayerManager thePlayer; //시작시 캐릭터에게 현재 씬위치와 맵이름을 넣어줄거임
    private GameManager theGM; //시작후 씬 이동시 카메라 위치 설정시 필요

    void Start()
    {
        theFade = FindObjectOfType<FadeManager>();
        theAudio = FindObjectOfType<AudioManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        theGM = FindObjectOfType<GameManager>();
    }

    public void StartGame() //start 버튼 누르면 이 함수 호출
    {
        StartCoroutine(GameStartCoroutine());
    }

    IEnumerator GameStartCoroutine() //start 코루틴
    {
        
        theFade.FadeOut(); 
        theAudio.Play(click_sound);
        yield return new WaitForSeconds(2f);

        Color color = thePlayer.GetComponent<SpriteRenderer>().color;
        color.a = 1f;
        thePlayer.GetComponent<SpriteRenderer>().color = color;
        thePlayer.currentMapName = "forest";
        thePlayer.currentSceneName = "start";

        theGM.LoadStart(); 

        SceneManager.LoadScene("start"); 
    }

    public void ExitGame() //종료
    {
        theAudio.Play(click_sound);
        Application.Quit();
    }
}
