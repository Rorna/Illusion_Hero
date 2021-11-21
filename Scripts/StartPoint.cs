using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//startpoint값과 currentmapname을 비교해서 같으면 캐릭터를 스타트 포인트로 이동시킴
public class StartPoint : MonoBehaviour
{
    public string startPoint; //맵이 이동후, 플레이어가 시작될 위치
    private PlayerManager thePlayer;
    private CameraManager theCamera; //연출을 위한 변수

    void Start()
    {
        theCamera = FindObjectOfType<CameraManager>();
        thePlayer = FindObjectOfType<PlayerManager>(); 
        if(startPoint == thePlayer.currentMapName)
        {
            theCamera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, theCamera.transform.position.z);
            thePlayer.transform.position=this.transform.position;
        }
    }
}
