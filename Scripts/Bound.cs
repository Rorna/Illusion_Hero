using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bound : MonoBehaviour
{
    private BoxCollider2D bound;
    public string boundName; //로드시 이동할 바운드의 이름 필요
    private CameraManager theCamera; //카메라매니저의 Setbound함수 이용목적

    void Start()
    {
        bound = GetComponent<BoxCollider2D>();
        theCamera = FindObjectOfType<CameraManager>();
    }
    public void SetBound() //바운드 설정, 로드시 필요
    {
        if(theCamera!=null)
        {
            theCamera.SetBound(bound);
        }
    }
   
}
