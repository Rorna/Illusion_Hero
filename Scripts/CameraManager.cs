using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    static public CameraManager instance; //복수생성 방지용 변수

    public GameObject target; //카메라가 따라갈 대상
    public float moveSpeed; //카메라가 얼마나 빠른 속도로
    private Vector3 targetPosition; //대상의 현재 위치 값

    //카메라 이동을 영역안으로 제한시킴
    public BoxCollider2D bound;
    //박스 컬라이더 영역의 최소 최대 xyz값을 지님
    private Vector3 minBound;
    private Vector3 maxBound;
    //카메라의 반너비, 반높이 값을 지닐 변수
    private float halfWidth;
    private float halfHeight;
    //카메라의 반높이값을 구할 속성을 이용하기 위한 변수
    private Camera theCamera;

    private void Awake()
    {
        if (instance != null) Destroy(this.gameObject);
        else //instance==null
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
    }
    void Start()
    {
        theCamera = GetComponent<Camera>();
        //바운드
        minBound = bound.bounds.min;
        maxBound = bound.bounds.max;
        halfHeight = theCamera.orthographicSize;
        halfWidth = halfHeight * Screen.width / Screen.height; //해상도를 이용하여 반너비 구함
    }

    // Update is called once per frame
    void Update()
    {
        if(target.gameObject!=null)
        {
            targetPosition.Set(target.transform.position.x, target.transform.position.y, this.transform.position.z);
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, moveSpeed * Time.deltaTime);

            //카메라 영역 제한(바운드)
            float clampedX = Mathf.Clamp(this.transform.position.x, minBound.x + halfWidth, maxBound.x - halfWidth); 
            float clampedY = Mathf.Clamp(this.transform.position.y, minBound.y + halfHeight, maxBound.y - halfHeight);
            this.transform.position = new Vector3(clampedX, clampedY, this.transform.position.z);
        }
    }

    public void SetBound(BoxCollider2D newBound) //바운드 교체 함수, 카메라 가두기 함수
    {
        bound = newBound;
        minBound = bound.bounds.min;
        maxBound = bound.bounds.max;
    }
}
