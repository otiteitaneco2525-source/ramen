using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Ramen.Data;

public sealed class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource _bgmAudioSource;
    [SerializeField] AudioSource _seAudioSource;
    [SerializeField] SoundAsset _soundAsset;
    [SerializeField] float _fadeTime;

    string _preveBgmName = string.Empty;
    bool _isFade = false;

    public void PlaySe(string name)
    {
        _seAudioSource.PlayOneShot(_soundAsset.GetAudioClip(name));
    }

    public void StopSe()
    {
        _seAudioSource.Stop();
    }

    public async UniTask PlayBgm(string name)
    {
        if (_preveBgmName == name) return;
        if (_isFade) return;

        _isFade = false;

        CancellationToken token = this.GetCancellationTokenOnDestroy();

        if (string.IsNullOrWhiteSpace(_preveBgmName))
        {
            _bgmAudioSource.volume = 0f;
        }
        else
        {
            await FadeBgnVolume(1f, 0f, token);
        }

        _bgmAudioSource.clip = _soundAsset.GetAudioClip(name);
        _bgmAudioSource.Play();

        _preveBgmName = name;

        await FadeBgnVolume(0, 1f, token);

        _isFade = false;
    }

    async UniTask FadeBgnVolume(float fromVolume, float toVolume, CancellationToken token)
    {
        float timer = 0f;

        _bgmAudioSource.volume = fromVolume;

        while (timer < _fadeTime)
        {
            timer += Time.deltaTime;

            _bgmAudioSource.volume = Mathf.Lerp(fromVolume, toVolume, timer / _fadeTime);

            await UniTask.Yield(token);
        }

        _bgmAudioSource.volume = toVolume;
    }
}
