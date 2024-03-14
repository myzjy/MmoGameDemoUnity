using System.Collections.Generic;
using ZJYFrameWork.Common;
using ZJYFrameWork.Hotfix.Common;
using ZJYFrameWork.Net.CsProtocol.Protocol.Bag;
using ZJYFrameWork.Net.CsProtocol.Protocol.Map;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Setting
{
    [Bean]
    public class ServerDataManager
    {
        /// <summary>
        /// 基础配置相关
        /// </summary>
        private List<ItemBaseData> _itemBaseDataList = new List<ItemBaseData>();

        [Autowired] private LoginClientCacheData LoginCacheData;
        [Autowired] private RegisterPartClientCacheData RegisterPartClientCacheData;

        /// <summary>
        /// 服务器上面道具相关基础配置表
        /// </summary>
        public List<ItemBaseData> ItemBaseDataList => _itemBaseDataList;

        public LoginClientCacheData GetLoginClientCacheData => LoginCacheData;
        public RegisterPartClientCacheData GetRegisterPartClientCacheData => RegisterPartClientCacheData;

        /// <summary>
        /// 地图 关卡配置 config 
        /// </summary>
        public List<Puzzle> PuzzleConfigList = new List<Puzzle>();

        public List<PuzzleChapter> PuzzleChapterConfigList = new List<PuzzleChapter>();


        [AfterPostConstruct]
        public void Init()
        {
        }

        /// <summary>
        /// 保存缓存账号密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        public void SetCacheAccountAndPassword(string account, string password)
        {
            LoginCacheData.account = account;
            LoginCacheData.password = password;
        }

        /// <summary>
        /// 保存缓存账号密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="affirmPassword"></param>
        public void SetCacheRegisterAccountAndPassword(string account, string password, string affirmPassword)
        {
            RegisterPartClientCacheData.Account = account;
            RegisterPartClientCacheData.Password = password;
            RegisterPartClientCacheData.AffirmPassword = affirmPassword;
        }

        public void SetItemBaseDataList(IEnumerable<ItemBaseData> itemBaseList)
        {
            _itemBaseDataList = new List<ItemBaseData>();
            _itemBaseDataList.AddRange(itemBaseList);
        }

        /// <summary>
        /// 保存 地图关卡 配置 list
        /// </summary>
        /// <param name="puzzles"></param>
        public void SetPuzzleConfigList(List<Puzzle> puzzles)
        {
            PuzzleConfigList = new List<Puzzle>();
            PuzzleConfigList.AddRange(puzzles);
        }

        public void SetPuzzleChapterConfigList(List<PuzzleChapter> puzzleChapters)
        {
            PuzzleChapterConfigList = new List<PuzzleChapter>();
            PuzzleChapterConfigList.AddRange(puzzleChapters);
        }
    }
}