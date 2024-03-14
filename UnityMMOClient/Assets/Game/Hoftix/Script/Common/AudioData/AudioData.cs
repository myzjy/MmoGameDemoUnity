using UnityEngine;

namespace ZJYFrameWork.Hotfix.Common.AudioDataObj
{
    [CreateAssetMenu(menuName = "ScriptableObject/AudioData")]
    public class AudioData : ScriptableObject
    {
        public enum CullingTypes
        {
            NONE, //不进行控制
            REJECT, //先到先得= Interval&Count AudioObject投入时检查
            OVERWRITE, //先到先得= Count
            PRIORITY, //没有顺序，优先级最低的一个

            TYPE_MAX,
        }

        public uint id;

        public string audioAsset;

        /*
         * 再生优先级(最优先)
         */
        public uint priority;

        /*
         * 最短的再生间隔(指定msec:以前再生相同的SE之后这个ms不站起来就不能再生)
         */
        public float intervalLimit;

        /*
         * 偏移流(0~1)
         */
        public float volumeScale = 1f;

        /*
         * 发音上限数(1以下允许无限)
         */
        public int limitNum;

        /*
         * 优先判定
         */
        public CullingTypes cullingType;

        /*
         * clip
         */
        public AudioClip clip;

        /*
         * 是不是JINGLE
         */
        public bool jingle;
    }
}