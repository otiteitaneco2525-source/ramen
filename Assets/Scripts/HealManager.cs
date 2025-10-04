using UnityEngine;
using Cysharp.Threading.Tasks;

public class HealManager : MonoBehaviour
{
    [SerializeField] private HealView _healView;

    void Start()
    {
        _healView.OnCloseButtonClicked += OnCloseButtonClicked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _healView.OnShowAsync().Forget();
        }
    }

    private void OnCloseButtonClicked()
    {
        _healView.Visible = false;
    }
}
