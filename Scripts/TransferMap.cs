using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//맵 이동스크립트
public class TransferMap : MonoBehaviour
{
    public string transferMapName; //이동할 맵의 이름
    
    public Transform target; //위치 이동용 변수
    public BoxCollider2D targetBound; //목표 바운드

    //문 애니메이션
    public Animator anim_1;
    public Animator anim_2;

    public int door_count; //문이 몇개인지 알려줄 변수

    [Tooltip("UP, DOWN, LEFT, RIGHT 네 방향 가능")]
    public string direction; //캐릭터가 바라보고 있는 방향
    private Vector2 vector; //getfloat("DirX")값 저장

    [Tooltip("문이 있으면: true, 문이 없으면: false")]
    public bool door; //문이 있냐 없냐 인스펙터 창에서 체크


    private PlayerManager thePlayer; 
    private CameraManager theCamera;
    private FadeManager theFade;
    private OrderManager theOrder;

    // Start is called before the first frame update
    void Start()
    {
        theCamera = FindObjectOfType<CameraManager>();
        thePlayer = FindObjectOfType<PlayerManager>(); 
        theFade = FindObjectOfType<FadeManager>();
        theOrder = FindObjectOfType<OrderManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision) //충돌
    {
        if(!door) 
        {
            if (collision.gameObject.name == "Player") 
            {
                StartCoroutine(TransferCoroutine());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) //대기
    {
        if(door)
        {
            if (collision.gameObject.name == "Player") //충돌 오브젝트 이름
            {
                if(Input.GetKeyDown(KeyCode.Z))
                {
                    vector.Set(thePlayer.animator.GetFloat("DirX"), thePlayer.animator.GetFloat("DirY"));

                    switch(direction)
                    {
                        case "UP":
                            if(vector.y==1f) StartCoroutine(TransferCoroutine());
                            break;
                        case "DOWN":
                            if (vector.y == -1f) StartCoroutine(TransferCoroutine());
                            break;
                        case "LEFT":
                            if (vector.x == -1f) StartCoroutine(TransferCoroutine());
                            break;
                        case "RIGHT":
                            if (vector.x == 1f) StartCoroutine(TransferCoroutine());
                            break;
                        default:
                            StartCoroutine(TransferCoroutine());
                            break;
                    }
                    StartCoroutine(TransferCoroutine());
                }  
            }
        }
    }


    IEnumerator TransferCoroutine() //페이드 효과 한순간에 동시 실행되는 것 방지, 대기를 주기 위해 코루틴 생성
    {
        theOrder.PreLoadCharacter(); 
        //맵 이동시 플레이어 움직임 제한
        theOrder.NotMove();
        //맵이동이 이뤄지기전에 화면 어두워짐
        theFade.FadeOut();

        //열림 애니메이션
        if(door)
        {
            anim_1.SetBool("Open", true); 
            if(door_count==2) anim_2.SetBool("Open", true);
        }
        yield return new WaitForSeconds(0.3f); 

        //캐릭터가 문 안으로 들어가는 모습
        theOrder.SetTransparent("player"); 
        //닫힘 애니메이션
        if (door) 
        {
            anim_1.SetBool("Open", false); 
            if (door_count == 2) anim_2.SetBool("Open", false);
        }
        yield return new WaitForSeconds(0.7f); 
        theOrder.SetUnTransparent("player"); 
        thePlayer.currentMapName = transferMapName;
        theCamera.SetBound(targetBound); 
        theCamera.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, theCamera.transform.position.z);
        thePlayer.transform.position = target.transform.position;
        theFade.FadeIn();
        yield return new WaitForSeconds(0.5f);
        theOrder.Move();
    }

}
