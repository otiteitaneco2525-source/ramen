using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VContainer;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class TitleManager : MonoBehaviour
{
    [Inject] private readonly FadeView _fadeView;
    [SerializeField] private Button _titleButton;
    
    async void Start()
    {
        await _fadeView.FadeOutAsync();
        _fadeView.Visible = false;

        _titleButton.onClick.AddListener(async () => await OnTitleButtonClick());
    }

    private async UniTask OnTitleButtonClick()
    {
        _titleButton.interactable = false;

        List<UniTask> taskList = new List<UniTask>();
        taskList.Add(SceneManager.LoadSceneAsync("MapScene").ToUniTask());
        taskList.Add(_fadeView.FadeInAsync());
        await UniTask.WhenAll(taskList);
    }
}
