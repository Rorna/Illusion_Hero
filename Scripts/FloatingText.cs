using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed; //얼마나 빠른속도로 텍스트가 떠다닐건지
    public float destroyTime; //일정 시간 지난후 파기

    public Text text;

    private Vector3 vector;

    // Start is called before the first frame update
    void Update()
    {
        //y를 1초에 moveSpeed만큼 움직임
        vector.Set(text.transform.position.x, text.transform.position.y + (moveSpeed * Time.deltaTime), text.transform.position.z);
        text.transform.position = vector;

        destroyTime -= Time.deltaTime; //1초에 1씩 감소
        if (destroyTime <= 0) 
            Destroy(this.gameObject); //파괴
    }

}
