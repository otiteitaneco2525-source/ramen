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

        public static readonly string SE01 = "01";
        public static readonly string SE02 = "02";
        public static readonly string SE03 = "03";
        public static readonly string SE04 = "04";
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
        public static readonly string BGM01 = "15";
        public static readonly string BGM02 = "16";
        public static readonly string BGM03 = "17";
        public static readonly string BGM04 = "18";
        public static readonly string BGM05 = "19";
        public static readonly string BGM06 = "20";
        public static readonly string BGM07 = "21";
        public static readonly string BGM08 = "22";   
    }
}