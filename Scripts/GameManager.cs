using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Bound[] bounds;
    private PlayerManager thePlayer;
    private CameraManager theCamera;
    private FadeManager theFade; //화면 페이드효과
    private Menu theMenu; //메뉴에 붙어있는 카메라를 이용하여 시스템 캔버스의 카메라를 붙여넣을 거임
    private DialogueManager theDM;
    private Camera cam;

    //체력바 마나바 활성화 
    public GameObject hpBar;
    public GameObject mpBar;

    public void LoadStart() //로드
    {
        StartCoroutine(LoadWaitCoroutine());
    }

    IEnumerator LoadWaitCoroutine() //실질적 로드 함수
    {
        yield return new WaitForSeconds(0.5f); //다른 객체들이 다 참조될 때까지 일정시간 대기

        //초기화
        thePlayer = FindObjectOfType<PlayerManager>();
        bounds = FindObjectsOfType<Bound>(); //로드할때 마다 씬에 있는 바운드로 바뀜
        theCamera = FindObjectOfType<CameraManager>();
        theFade = FindObjectOfType<FadeManager>();
        theMenu = FindObjectOfType<Menu>();
        theDM = FindObjectOfType<DialogueManager>();
        cam = FindObjectOfType<Camera>();

        //투명했던 플레이어 다시 복원
        Color color = thePlayer.GetComponent<SpriteRenderer>().color;
        color.a = 1f;
        thePlayer.GetComponent<SpriteRenderer>().color = color;

        //타겟 설정
        //카메라의 타겟을 플레이어로 설정
        theCamera.target = GameObject.Find("Player"); //하이어라키에 있는 Player라는 이름의 객체를 찾아 그것을 타겟으로 바꿈
        //마찬가지로 시스템 캔버스도 타겟을 잃어버리지 않도록 설정해줘야함
        theMenu.GetComponent<Canvas>().worldCamera = cam;
        //dialogueManager의 카메라도 타겟 잃어버리지 않도록 설정
        theDM.GetComponent<Canvas>().worldCamera = cam;

        for (int i = 0; i < bounds.Length; i++)
        {
            if (bounds[i].boundName == thePlayer.currentMapName) 
            {
                bounds[i].SetBound(); //그 바운드를 설정
                break;
            }
        }
        //체력 마나바 활성화
        hpBar.SetActive(true);
        mpBar.SetActive(true);

        theFade.FadeIn(); 
    }
}
