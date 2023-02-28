using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[System.Serializable] // �ν����� â���� ������ �� �ְ� ���ִ� ���
public class Sound
{
    public string name;
    public AudioClip clip; // ���� Mp3 ���� �� ���̺� ������ ���⿡ �־��ָ� �ȴ�. 
}


public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider BgmSlider;
    public Slider SfxSlider;


    [Multiline] public string BelowTwoListDescription;
    public string[] SceneNamesList;
    public string[] BgmNameList;


    private void Start()
    {
        //���۽� ���� ������ �����ͼ� �����Ѵ�.
        audioMixer.SetFloat("BGM", Mathf.Log10(PlayerPrefs.GetFloat("BgmVolume")) * 20);
        audioMixer.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat("SfxVolume")) * 20);

        // BgmSlider���� null �� �ƴ϶�� ���� ����� Volume���� �����̴� ���� ����.
        if (BgmSlider != null && SfxSlider != null)
        {
            BgmSlider.value = PlayerPrefs.GetFloat("BgmVolume");
            SfxSlider.value = PlayerPrefs.GetFloat("SfxVolume");

        }

        string nowSceneBgmName=null;

        #region �� �̸��� ���� BGM �̸�(nowSceneBgmName) ����
        string nowSceneName = SceneManager.GetActiveScene().name.ToString();

        //�� �̸��� ���� BGM �̸� ����
        for (int i = 0; i < SceneNamesList.Length; i++)
        {
            if (nowSceneName == SceneNamesList[i])
            {
                if (i == 1 || i == 2) //EndingScene�� ��� �÷��̾� ���� ����� ���� 
                {
                    string winnerNick = PlayerPrefs.GetString("Winner"); // ����� �̸� ����
                    if (winnerNick == PlayerPrefs.GetString("_myNick")) //���ӽ¸��ڰ� �������� Ȯ�� 
                    {
                        nowSceneBgmName = BgmNameList[1]; // �¸��� BGM
                        Debug.Log(nowSceneBgmName);
                        break;
                    }
                    else
                    {
                        nowSceneBgmName = BgmNameList[2]; // ���� �� BGM
                        Debug.Log(nowSceneBgmName);
                        break;
                    }

                }
                nowSceneBgmName = BgmNameList[i];
                Debug.Log(nowSceneBgmName);
                break;
            }

        }
        #endregion �� �̸��� ���� BGM �̸�(nowSceneBgmName) ����

        Debug.Log(nowSceneBgmName);
        PlayBGM(nowSceneBgmName);

    }

    public static AudioManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<AudioManager>();
            }

            return m_instance;
        }
    }

    private static AudioManager m_instance; // �̱����� �Ҵ�� ����

    [SerializeField] Sound[] sfx = null;
    [SerializeField] Sound[] bgm = null;

    [SerializeField] AudioSource bgmPlayer = null; // ������� �ϳ��̹Ƿ� �迭�� ������x
    [SerializeField] AudioSource[] sfxPlayer = null;  // ȿ������ �������̹Ƿ� �迭�� �����!

    public void PlayBGM(string p_bgmName)
    {
        for(int i =0; i < bgm.Length; i++)
        {
            if(p_bgmName == bgm[i].name)
            {
                bgmPlayer.clip = bgm[i].clip;
                bgmPlayer.Play();
            }
        }
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    public void PlaySFX(string p_sfxmName)
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            if (p_sfxmName == sfx[i].name)
            {
                // Debug.Log("����� Ŭ���� ã�Ҵ� : " + sfx[i].name);
                for(int x = 0; x < sfxPlayer.Length; x++)
                {
                    if (!sfxPlayer[x].isPlaying)
                    {
                        // Debug.Log("��������� ���� ������� ã�Ҵ�! : " + p_sfxmName);
                        //sfx[x].clip = sfx[i].clip;
                        sfxPlayer[x].PlayOneShot(sfx[i].clip);
                        //Debug.Log("�÷����Ͽ��� " + sfx[i].name);
                        return;
                    }
                }

                //Debug.Log("��� ����� �÷��̾ ������Դϴ�.");
                return;
            }
        }

        //Debug.Log(p_sfxmName + "�̸��� ȿ������ �����ϴ�.");
    }


    public void SetBgmVolume()
    {
        ////�α׿��갪 ����
        audioMixer.SetFloat("BGM", Mathf.Log10(BgmSlider.value) * 20);
        PlayerPrefs.SetFloat("BgmVolume", BgmSlider.value);

        //audioMixer.SetFloat("BGM", BgmSlider.value);
    }

    public void SetSfxVolume()
    {
        ////�α׿��갪 ����
        audioMixer.SetFloat("SFX", Mathf.Log10(SfxSlider.value) * 20);
        PlayerPrefs.SetFloat("SfxVolume", SfxSlider.value);
        //audioMixer.SetFloat("SFX",SfxSlider.value);
    }

}
