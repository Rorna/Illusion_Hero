using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound 
{
    public string name; //사운드의 이름
    public AudioClip clip; //사운드 파일
    private AudioSource source; //사운드 플레이어

    public float Volumn;
    public bool loop;

    public void SetSource(AudioSource _source) //이 함수를 이용하여 오디오삽입, 초기값 삽입
    {
        source = _source;
        source.clip = clip;
        source.loop = loop;
        source.volume = Volumn;
    }
    public void SetVolumn()
    {
        source.volume = Volumn;
    }
    public void Play()
    {
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }
    public void SetLoop()
    {
        source.loop = true;
    }
    public void SetLoopCancel()
    {
        source.loop = false;
    }

}
public class AudioManager : MonoBehaviour
{
    static public AudioManager instance;
    [SerializeField]
    public Sound[] sounds;

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
        for(int i=0; i<sounds.Length; i++)
        {
            GameObject soundObject = new GameObject("사운드 파일 이름: "+i+"="+sounds[i].name);
            sounds[i].SetSource(soundObject.AddComponent<AudioSource>());
            soundObject.transform.SetParent(this.transform);
        }
    }
    public void Play(string _name) //재생
    {
        for(int i = 0; i < sounds.Length; i++)
        {
            if(_name==sounds[i].name) //들어온 파일 이름과 실행시키려는 파일 이름이 일치하면 재생
            {
                sounds[i].Play();
                return;
            }
        }
    }
    public void Stop(string _name) //정지
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_name == sounds[i].name) //들어온 파일 이름과 실행시키려는 파일 이름이 일치하면
            {
                sounds[i].Stop();
                return;
            }
        }
    }
    public void SetLoop(string _name) //루프 설정
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_name == sounds[i].name) //들어온 파일 이름과 실행시키려는 파일 이름이 일치하면
            {
                sounds[i].SetLoop();
                return;
            }
        }
    }
    public void SetLoopCancel(string _name) //루프 캔슬
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_name == sounds[i].name) //들어온 파일 이름과 실행시키려는 파일 이름이 일치하면
            {
                sounds[i].SetLoopCancel();
                return;
            }
        }
    }
    public void SetVolumn(string _name, float _volumn) //볼륨 변경
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_name == sounds[i].name) //들어온 파일 이름과 실행시키려는 파일 이름이 일치하면
            {
                sounds[i].Volumn = _volumn;
                sounds[i].SetVolumn();
                return;
            }
        }
    }
}
