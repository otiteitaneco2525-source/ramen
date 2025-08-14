using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class HeroObj : MonoBehaviour
{
    [SerializeField] int hp;

    public void Damage(int value)
    {
        hp -= value;
        // 自身を揺らしたい
        Shake();
        if (hp <= 0)
        {
            Debug.Log("HeroObjがやられた");
            // Destroy(gameObject);
        }
    }

    // 自身を揺らす処理
    async void Shake()
    {
        // ここに揺らす処理を書く
        // 0.3秒間、今いる位置を中心にランダムに揺らす
        float shakeTime = 0.3f;
        float timer = 0;
        Vector3 originalPos = transform.position;
        while (timer < shakeTime)
        {
            timer += Time.deltaTime;
            // ランダムな位置に移動
            transform.position = originalPos + Random.insideUnitSphere * 10f;
            // 1フレーム待つ
            await UniTask.Yield();
        }
        // 元の位置に戻す
        transform.position = originalPos;
    }
}
