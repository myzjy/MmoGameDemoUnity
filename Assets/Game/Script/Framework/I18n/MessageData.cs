using UnityEngine;

namespace ZJYFrameWork.I18n
{
    public class MessageI18
    {
        public int index;
        public string key;
        public string value;
    }
    public class MessageData : ScriptableObject
    {
        [SerializeField] public MessageI18[] messages;
    }
}