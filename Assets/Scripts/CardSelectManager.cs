using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class CardSelectManager : MonoBehaviour
{
    [SerializeField] private CardSelectView _cardSelectView;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _cardSelectView.DealCards(new List<string> { "1", "2", "10", "11", "14", "15" });
            _cardSelectView.OnShowAsync().Forget();
        }
    }
}
