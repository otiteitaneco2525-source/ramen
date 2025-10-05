using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VContainer;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class TitleManager : MonoBehaviour
{
    [Inject] private readonly FadeView _fadeView;
    [Inject] private readonly SoundManager _soundManager;
    [SerializeField] private Button _titleButton;
    
    async void Start()
    {
        List<UniTask> taskList = new List<UniTask>();
        taskList.Add(_soundManager.PlayBgm(Ramen.Data.SoundAsset.BGM_OP));
        taskList.Add(_fadeView.FadeOutAsync());
        await UniTask.WhenAll(taskList);
        
        _fadeView.Visible = false;

        _titleButton.onClick.AddListener(async () => await OnTitleButtonClick());
    }

    private async UniTask OnTitleButtonClick()
    {
        _titleButton.interactable = false;
        List<UniTask> taskList = new List<UniTask>();
        taskList.Add(_soundManager.StopBgmAsync());
        taskList.Add(SceneManager.LoadSceneAsync("MapScene").ToUniTask());
        taskList.Add(_fadeView.FadeInAsync());
        await UniTask.WhenAll(taskList);
    }
}
