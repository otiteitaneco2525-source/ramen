using System.Collections.Generic;
using UnityEngine;

namespace Ramen.Data
{
    [CreateAssetMenu(fileName = "SoundAsset", menuName = "Scriptable Objects/SoundAsset")]
    public class SoundAsset : ScriptableObject
    {
        [SerializeField] List<AudioClip> _audioClipList;

        Dictionary<string, AudioClip> _audioClipDictionary;

        public void SetList(List<AudioClip> list)
        {
            _audioClipList = list;
        }

        public AudioClip GetAudioClip(string name)
        {
            if (_audioClipDictionary == null)
            {
                _audioClipDictionary = new Dictionary<string, AudioClip>();

                foreach (var audioClip in _audioClipList)
                {
                    _audioClipDictionary.Add(audioClip.name, audioClip);
                }
            }

            return _audioClipDictionary[name];
        }

        public static readonly string SE01 = "audiostock_470221_決定音";
        public static readonly string SE02 = "audiostock_328352_ターン切替わり音";
        public static readonly string SE03 = "audiostock_1142483_ラーメン失敗音";
        public static readonly string SE04 = "audiostock_1077826_ラーメン完成音";
        public static readonly string SE05 = "05";
        public static readonly string SE06 = "06";
        public static readonly string SE07 = "07";
        public static readonly string SE08 = "08";
        public static readonly string SE09 = "09";
        public static readonly string SE10 = "10";
        public static readonly string SE11 = "11_1";
        public static readonly string SE12 = "11_2";
        public static readonly string SE13 = "12";
        public static readonly string SE14 = "13";
        public static readonly string SE15 = "14";
        public static readonly string BGM_OP = "op";
        public static readonly string BGM_MAP = "map";
        public static readonly string BGM_BATTLE = "battle";
        public static readonly string BGM_BOSS_WIN = "boss_win";
        public static readonly string BGM05 = "audiostock_1063805_敗北曲";
        public static readonly string BGM06 = "audiostock_1117041_ボス戦曲";
        public static readonly string BGM07 = "audiostock_789442_戦闘終了曲";
        public static readonly string BGM08 = "audiostock_217868_回復曲";
        public static readonly string BGM09 = "audiostock_1616157_ショップ曲";
    }
}