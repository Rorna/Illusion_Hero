using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이펙트 스크립트, 설정한 시간 뒤에 소멸
public class Effect : MonoBehaviour
{
    public float deleteTime;

    // Update is called once per frame
    void Update()
    {
        deleteTime -= Time.deltaTime;
        if (deleteTime <= 0)
            Destroy(this.gameObject);
    }
}
