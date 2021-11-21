using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    static public BGMManager instance; //싱글턴
    public AudioClip[] clips; //배경 음악들
    private AudioSource source; //재생기

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);
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

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
      
    }

    public void Play(int _playMusicTrack)
    {
        source.volume = 1; //fadein, fadeout사용후를 위한 볼륨 초기화
        source.clip = clips[_playMusicTrack];
        source.Play();
    }

    public void SetVolumn(float _volumn)
    {
        source.volume = _volumn;
    }
    public void Pause() //일시정지
    {
        source.Pause();
    }
    public void Unpause() //재개
    {
        source.UnPause();
    }
    public void Stop()
    {
        source.Stop();
    }
    public void FadeOutMusic()
    {
        StopAllCoroutines(); //서로다른코루틴 동시실행 방지용, 모든 코루틴 끄고 시작
        StartCoroutine(FadeOutMusicCoroutine());

    }

    IEnumerator FadeOutMusicCoroutine()
    {
        for (float i = 1.0f; i >= 0; i -= 0.01f) //점차 감소
        {
            source.volume = i;
            yield return waitTime;
        }
    }

    
    public void FadeInMusic()
    {
        StopAllCoroutines(); //서로다른코루틴 동시실행 방지용, 모든 코루틴 끄고 시작
        StartCoroutine(FadeInMusicCoroutine());
    }
    IEnumerator FadeInMusicCoroutine()
    {
        for (float i = 0f; i <= 1f; i += 0.01f) //점차 증가
        {
            source.volume = i;
            yield return waitTime;
        }
    }
}
