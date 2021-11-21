using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    
    private PlayerManager thePlayer; //플레이어가 바라보고 있는 방향 알아야함 animator.getfloat 반환값 이용
    private Vector2 vector; //그 반환값 저장용 변수

    private Quaternion rotation; //회전(각도)을 담당하는 Vector4 x y z w

    // Start is called before the first frame update
    void Start()
    {
        thePlayer = FindObjectOfType<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = thePlayer.transform.position; //player위치와 동기화

        //회전
        vector.Set(thePlayer.animator.GetFloat("DirX"), thePlayer.animator.GetFloat("DirY"));
        if (vector.x == 1f) //오른쪽 보고 있을 경우
        {
            rotation = Quaternion.Euler(0, 0, 90);
            this.transform.rotation = rotation; //현재 위치값을 넘김
        }
        else if (vector.x == -1f) //왼쪽
        {
            rotation = Quaternion.Euler(0, 0, -90);
            this.transform.rotation = rotation; //현재 위치값을 넘김
        }
        else if (vector.y == 1f) //위쪽
        {
            rotation = Quaternion.Euler(0, 0, 180);
            this.transform.rotation = rotation; //현재 위치값을 넘김
        }
        else if (vector.y == -1f) //아래쪽
        {
            rotation = Quaternion.Euler(0, 0, 0);
            this.transform.rotation = rotation; //현재 위치값을 넘김
        }
    }
}
