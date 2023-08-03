using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Module.UICommon
{
    /// <summary>
    /// 无限循环
    /// </summary>
    public class AdaptiveReuseGrid : MonoBehaviour
    {
        /// <summary>
        /// 是否打开输出日志
        /// </summary>
        public bool DebugLog = false;

        public enum ScrollType
        {
            /// <summary>
            /// 纵向下标 规则
            /// 1 2 3 4
            /// 5 6 7 8
            /// </summary>
            Vertical,

            /// <summary>
            /// 横向坐标规则
            /// 1 3 5
            /// 2 4 6
            /// </summary>
            Horizontal,
        }

        public enum Constraint
        {
            FixedCount = 0,
            FixedColumnCount,
            FixedRowCount
        }

        /// <summary>
        /// 方向
        /// </summary>
        public ScrollType scrollType;

        /// <summary>
        /// Item大小
        /// </summary>
        public Vector2 cellSize;

        /// <summary>
        /// 边距
        /// </summary>
        public Vector2 spacing = Vector2.zero;

        /// <summary>
        /// item GameObject
        /// </summary>
        public GameObject itemGameObject;

        public RectTransform scrollViewRectTransform;

        /// <summary>
        /// 显示回调
        /// </summary>
        public Action<GridItem> OnItemShow;

        public Constraint ConstraintType;

        [InspectorName("显示行 当前界面最多显示多少行item")] public int m_FixedColumnCount = 1;
        [InspectorName("显示列 当前界面最多显示多少列item")] public int m_FixedRowCount = 1;

        public int FixedColumnCount
        {
            get
            {
                m_FixedColumnCount = Mathf.Max(1, m_FixedColumnCount);
                return m_FixedColumnCount;
            }
            set { m_FixedColumnCount = Mathf.Max(1, value); }
        }

        public int FixedRowCount
        {
            get
            {
                m_FixedRowCount = Mathf.Max(1, m_FixedRowCount);
                return m_FixedRowCount;
            }
            set { m_FixedRowCount = Mathf.Max(1, value); }
        }


        /// <summary>
        /// 获得格子块尺寸
        /// </summary>
        /// <returns></returns>
        int GetItemSizeY()
        {
            return (int)(cellSize.y + spacing.y);
        }

        int GetItemSizeX()
        {
            return (int)(cellSize.x + spacing.x);
        }

        /// <summary>
        /// 缓存 RectTransform
        /// </summary>
        RectTransform _mRTF;

        private RectTransform mRTF
        {
            get
            {
                if (_mRTF == null) _mRTF = transform as RectTransform;
                return _mRTF;
            }
        }

        /// <summary>
        /// 实例化的GridItem数量
        /// </summary>
        int itemInstanceCount;

        /// <summary>
        /// 渲染的item数量
        /// </summary>
        int itemRendererCount;


        /// <summary>
        /// item数量
        /// </summary>
        int mItemCount;

        /// <summary>
        /// 行最大数量
        /// </summary>
        int m_rowMaxCount;

        int rowMaxCount
        {
            get
            {
                if (ConstraintType == Constraint.FixedColumnCount)
                {
                    m_rowMaxCount = FixedColumnCount;
                }
                else
                {
                    Vector2 mMaskSize = scrollViewRectTransform.rect.size;

                    if (scrollType == ScrollType.Horizontal)
                    {
                        m_rowMaxCount = (int)mMaskSize.y / GetItemSizeY();
                    }
                    else
                    {
                        m_rowMaxCount = Mathf.CeilToInt(mMaskSize.y / GetItemSizeY()) + 1;
                    }
                }

                return m_rowMaxCount;
            }
            set { m_rowMaxCount = value; }
        }


        /// <summary>
        /// 列最大数
        /// </summary>
        int m_lineMaxCount;

        int lineMaxCount
        {
            get
            {
                if (ConstraintType == Constraint.FixedColumnCount)
                {
                    m_lineMaxCount = FixedRowCount;
                }
                else
                {
                    Vector2 mMaskSize = scrollViewRectTransform.rect.size;
                    if (scrollType == ScrollType.Horizontal)
                    {
                        m_lineMaxCount = Mathf.CeilToInt(mMaskSize.x / GetItemSizeX()) + 1;
                    }
                    else
                    {
                        m_lineMaxCount = (int)mMaskSize.x / GetItemSizeX();
                    }
                }

                return m_lineMaxCount;
            }
            set { m_lineMaxCount = value; }
        }

        /// <summary>
        /// item list
        /// </summary>
        List<GridItem> items = new List<GridItem>();


        /// <summary>
        /// Content 上次的位置
        /// </summary>
        Vector2 lastPosition = Vector2.zero;

        /// <summary>
        /// 开始的下标
        /// </summary>
        int startIndex;

        /// <summary>
        /// 结束的下标
        /// </summary>
        int endIndex;

#if UNITY_EDITOR
        private void Reset()
        {
            if (scrollViewRectTransform == null)
            {
                ScrollRect sr = gameObject.GetComponentInParent<ScrollRect>();
                scrollViewRectTransform = sr?.transform as RectTransform;
            }

            cellSize = Vector2.one;
            Init();
        }

        void OnValidate()
        {
            if (scrollViewRectTransform?.GetComponent<ScrollRect>() == null)
            {
                UnityEngine.Debug.LogError("请绑定ScrollRect的RectTransform");

                if (UnityEditor.EditorUtility.DisplayDialog("绑定错误:",
                        "Scroll View Rect Transform \n绑定的是:" + scrollViewRectTransform.name +
                        " \n请绑定ScrollRect的RectTransform \n\n需要为您自动绑定父级目录的ScrollRect吗?", "自动绑定", "关闭"))
                {
                    ScrollRect sr = gameObject.GetComponentInParent<ScrollRect>();
                    scrollViewRectTransform = sr?.transform as RectTransform;
                }
                else
                {
                    scrollViewRectTransform = null;
                }
            }

            var sr2 = scrollViewRectTransform?.gameObject.GetComponent<ScrollRect>();
            if (sr2 != null)
            {
                sr2.horizontal = scrollType == ScrollType.Horizontal;
                sr2.vertical = scrollType == ScrollType.Vertical;
            }
        }
#endif


        private void Update()
        {
            Vector2 curPosition = mRTF.localPosition;
            if ((curPosition - lastPosition).SqrMagnitude() < 0.1f) return; //如果移动距离小于 0.3 则不计算

            lastPosition = curPosition;


            //如果起止下标有变化 则刷新Grid
            if (CalStartEndIndex())
            {
                RefrashGrid();
            }
        }


        private void Awake()
        {
            //Init();
        }

        private void OnEnable()
        {
            if (CalStartEndIndex())
            {
                //如果起止下标有变化 则刷新Grid
                RefrashGrid();
            }
        }
        

        /// <summary>
        /// 计算开始和结束的下标 如果有变化 则返回true
        /// </summary>
        /// <returns>有变化返回true</returns>
        bool CalStartEndIndex()
        {
            //可显示区域  左上角item的下标 也就是开始的
            int local_startIndex = scrollType == ScrollType.Vertical
                    ? Mathf.Max((int)lastPosition.y, 0) / GetItemSizeY() * lineMaxCount
                    : lastPosition.x > 0
                        ? 0
                        : Mathf.Abs((int)lastPosition.x) / GetItemSizeX() * rowMaxCount
                ;
            local_startIndex = Mathf.Min(local_startIndex, mItemCount - rowMaxCount * lineMaxCount);
            local_startIndex = Mathf.Max(local_startIndex, 0);

            //可显示区域 右下角item的下标 也就是结束的

            int local_endIndex = scrollType == ScrollType.Vertical
                    ? Mathf.Min(mItemCount, local_startIndex + itemRendererCount)
                    : Mathf.Min(mItemCount, local_startIndex + itemRendererCount)
                ;
            bool hsaveChange = false;
            if (startIndex != local_startIndex
                || endIndex != local_endIndex)
            {
                hsaveChange = true;
                startIndex = local_startIndex;
                endIndex = local_endIndex;
                if (DebugLog) UnityEngine.Debug.Log("起止下标:" + startIndex + " - " + endIndex);
            }

            return hsaveChange;
        }

        /// <summary>
        /// 刷新Grip
        /// </summary>
        /// <param name="isCallAction">是否调用刷新回调 默认调用</param>
        void RefrashGrid(bool isCallAction = true)
        {
            //移出越界的item
            for (int i = 0; i < items.Count; i++)
            {
                GridItem item = items[i];
                if (item.Index != -1)
                {
                    Vector2 position = item.itemData.transform.localPosition;
                    int index = 0;
                    if (scrollType == ScrollType.Vertical)
                    {
                        index = Mathf.Abs((int)position.y / GetItemSizeY()) * lineMaxCount +
                                (int)position.x / GetItemSizeX();
                    }
                    else if (scrollType == ScrollType.Horizontal)
                    {
                        index = (int)position.x / GetItemSizeX() * rowMaxCount + -1 * (int)position.y / GetItemSizeY();
                    }

                    if (index < startIndex || index >= endIndex)
                    {
                        if (DebugLog) UnityEngine.Debug.Log("下标" + index + " 越界 item.index:" + item.Index);
                        item.Index = -1;
                        item.itemData.transform.localPosition = new Vector3(-GetItemSizeX(), GetItemSizeY(), 0);
                    }
                }
            }

            //List<int> waitCallActionItemIndex = new List<int>();
            //填充空缺的GridItem
            for (int i = startIndex; i < endIndex; i++)
            {
                bool have = false;
                foreach (var item in items)
                {
                    if (item.Index == i)
                    {
                        have = true;
                        break;
                    }
                }

                if (!have)
                {
                    GridItem item = GetUnoccupiedItem();
                    if (item == null)
                    {
                        //没有闲置的GridItem
                        //System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        //foreach (var temp in items)
                        //{
                        //    sb.Append(temp.index).Append(temp.itemData.Get<Text>("Text").text).Append("\n");
                        //}
                        //Debug.LogError(sb);
                        return;
                    }

                    item.Index = i;
                    if (scrollType == ScrollType.Vertical)
                    {
                        item.itemData.transform.localPosition = new Vector3(item.Index % lineMaxCount * GetItemSizeX(),
                            -(item.Index / lineMaxCount) * GetItemSizeY(), 0);
                    }
                    else if (scrollType == ScrollType.Horizontal)
                    {
                        // ReSharper disable once PossibleLossOfFraction
                        item.itemData.transform.localPosition = new Vector3(item.Index / rowMaxCount * GetItemSizeX(),
                            -item.Index % rowMaxCount * GetItemSizeY(), 0);
                    }

                    if (isCallAction)
                        CallAction(item);
                }
            }
        }
        

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            if (DebugLog) UnityEngine.Debug.Log("AdaptiveReuseGrid.Init()");
            var sr = scrollViewRectTransform.gameObject.GetComponent<ScrollRect>();
            sr.horizontal = scrollType == ScrollType.Horizontal;
            sr.vertical = scrollType == ScrollType.Vertical;

            itemInstanceCount = 0;
            mItemCount = 0;
            itemRendererCount = 0;

            //content自适应后尺寸
            ColContentRowLine();

            foreach (var item in items)
            {
                DestroyImmediate(item.itemData.gameObject);
            }

            items.Clear();
        }

        /// <summary>
        /// 修改 ItemCount 会调用界面重构
        /// </summary>
        public int itemCount
        {
            set
            {
                mItemCount = value;
                RefrashItemGrid();
            }
            get { return mItemCount; }
        }

        /// <summary>
        /// 刷新指定下标
        /// </summary>
        /// <param name="index"></param>
        public void FlushItem(int index)
        {
            foreach (var item in items)
            {
                if (item.Index == index)
                {
                    CallAction(item);
                    return;
                }
            }
        }

        /// <summary>
        /// 调用显示的item的OnItemShow
        /// </summary>
        public void CallShow_OnItemShow()
        {
            foreach (var item in items)
            {
                if (item.Index >= 0)
                {
                    CallAction(item);
                    return;
                }
            }
        }

        /// <summary>
        /// 重构Grid
        /// </summary>
        /// <param name="index"></param>
        public void RefrashItemGrid()
        {
            CalContentChildView();
            CalStartEndIndex();
            RefrashGrid(false);
            foreach (var item in items)
            {
                if (item.Index >= 0)
                {
                    CallAction(item);
                }
            }
        }

        /// <summary>
        /// 重构Grid
        /// </summary>
        /// <param name="index"></param>
        public void RefrashTaoZhuangItemGrid()
        {
            CalContentChildView();
            //CalStartEndIndex();
            RefrashGrid(true);
            foreach (var item in items)
            {
                if (item.Index >= 0)
                {
                    CallAction(item);
                }
            }
            //Vector3 vec = new Vector3(_mRTF.transform.localPosition.x, _mRTF.transform.localPosition.y + items[0].itemData.transform.localPosition.y,
            //   _mRTF.transform.localPosition.z);

            //_mRTF.transform.localPosition = vec;
        }


        /// <summary>
        /// 插入一个item 默认插入到末尾
        /// </summary>
        /// <param name="_itemCount">item 数量</param>
        /// <param name="index">插入到指定下标<</param>
        public void InsertItem(int index = -1)
        {
            //如果不传下标 或 下标不合法 则 添加item到末尾
            if (index < 0 || index > this.mItemCount) index = this.mItemCount;

            this.mItemCount++;

            CalContentChildView();
            CalStartEndIndex();
            RefrashGrid(false);

            //刷新下标大于或等于index 的item
            foreach (var item in items)
            {
                if (item.Index >= index)
                {
                    CallAction(item);
                }
            }
        }

        /// <summary>
        /// 删除指定下标item
        /// </summary>
        /// <param name="index"></param>
        public void DeleteItem(int index)
        {
            if (mItemCount <= 0) return;
            mItemCount--;
            CalContentChildView();
            CalStartEndIndex();
            RefrashGrid(false);
            //刷新下标大于或等于index 的item
            foreach (var item in items)
            {
                if (item.Index >= index)
                {
                    CallAction(item);
                }
            }
        }

        /// <summary>
        /// 删除所有
        /// </summary>
        public void DeleteALL()
        {
            mItemCount = 0;
            CalContentChildView();
            CalStartEndIndex();
            RefrashGrid(false);
        }

        public void ClickLastOrDownItemShow(int index, int fl)
        {
            if (index <= 1 || ((mItemCount - 2) <= index && index <= mItemCount))
            {
                return;
            }

            int numItem = FixedColumnCount / 2;
            if (FixedColumnCount % 2 != 0)
            {
                if (startIndex <= index && index <= (startIndex + numItem - 1))
                {
                    mRTF.transform.localPosition += new Vector3(0, -fl, 0);
                }
                else if ((startIndex + numItem + 2) <= index && index <= (endIndex))
                {
                    mRTF.transform.localPosition += new Vector3(0, fl, 0);
                }
            }
            else
            {
                if (startIndex <= index && index <= (startIndex + numItem - 1))
                {
                    mRTF.transform.localPosition += new Vector3(0, -fl, 0);
                }
                else if ((startIndex + numItem + 1) < index && index <= (endIndex))
                {
                    mRTF.transform.localPosition += new Vector3(0, fl, 0);
                }
            }
        }

        public bool isOpenWuJin = true;

        /// <summary>
        /// 跳转到指定下标
        /// </summary>
        /// <param name="index"></param>
        public void ToIndex(int index)
        {
            Vector2 mMaskSize = scrollViewRectTransform.rect.size;

            int line = (int)mMaskSize.x / GetItemSizeX();

            int row = (int)mMaskSize.y / GetItemSizeY();
            //isTaoZhuan=true;
            Vector2 v2 = Vector2.zero;
            if (scrollType == ScrollType.Vertical)
            {
                int curRow = index / line;

                int maxRow = Mathf.CeilToInt(1f * mItemCount / line) - row;

                int toRow = Mathf.Min(curRow, maxRow);
                toRow = Mathf.Max(toRow, 0);

                v2.y = toRow * GetItemSizeY();
            }
            else
            {
                int curline = index / row;

                int maxLine = Mathf.CeilToInt(1f * mItemCount / row) - line;

                int toLine = Mathf.Min(curline, maxLine);
                toLine = Mathf.Max(toLine, 0);

                v2.x = -1 * toLine * GetItemSizeX();
            }

            StartCoroutine(ToPosition(v2));
        }

        bool isTaoBao = false;

        IEnumerator ToPosition(Vector2 v2)
        {
            Vector3 toV3 = new Vector3(v2.x, v2.y, 0);
            while ((mRTF.localPosition - toV3).sqrMagnitude > 0.1369f)
            {
                var position = mRTF.localPosition;
                mRTF.localPosition = Vector3.Lerp(position, toV3, Time.deltaTime * 10);
                yield return null;
            }

            mRTF.localPosition = toV3;
        }
        

        /// <summary>
        /// 计算 行 列
        /// </summary>
        void ColContentRowLine()
        {
            //content自适应后尺寸
            Vector2 mMaskSize = scrollViewRectTransform.rect.size;
            //lineMaxCount = (int)mMaskSize.x / GetItemSizeX() + (scrollType == ScrollType.Vertical ? 0 : 1);

            //rowMaxCount = (int)mMaskSize.y / GetItemSizeY() + (scrollType == ScrollType.Vertical ? 1 : 0);

            if (scrollType == ScrollType.Horizontal)
            {
                lineMaxCount = Mathf.CeilToInt(mMaskSize.x / GetItemSizeX()) + 1;
                rowMaxCount = (int)mMaskSize.y / GetItemSizeY();
            }
            else
            {
                lineMaxCount = (int)mMaskSize.x / GetItemSizeX();
                rowMaxCount = Mathf.CeilToInt(mMaskSize.y / GetItemSizeY()) + 1;
                ;
            }
        }


        /// <summary>
        /// 重算 content 子对象
        /// </summary>
        void CalContentChildView()
        {
            //计算需要渲染的item数量
            itemRendererCount = Mathf.Min(Mathf.CeilToInt(rowMaxCount * lineMaxCount), mItemCount);

            //设置content尺寸
            Vector2 sizeDelta = scrollType == ScrollType.Vertical
                    ? new Vector2(lineMaxCount * GetItemSizeX(),
                        Mathf.CeilToInt(1f * mItemCount / lineMaxCount) * GetItemSizeY())
                    : new Vector2(Mathf.CeilToInt(1f * mItemCount / rowMaxCount) * GetItemSizeX(),
                        rowMaxCount * GetItemSizeY())
                ;


            mRTF.sizeDelta = sizeDelta
                //+ offset
                ;

            //补齐item
            if (itemRendererCount > itemInstanceCount)
            {
                for (int i = itemInstanceCount; i < itemRendererCount; i++)
                {
                    GameObject itemChild = Instantiate(itemGameObject, this.transform);
                    itemChild.SetActive(true);
                    var rtf = itemChild.transform as RectTransform;

                    rtf.localPosition = new Vector3(-1 * GetItemSizeX(), -1 * GetItemSizeY(), 0);

                    rtf.sizeDelta = cellSize;
                    UISerializableKeyObject keyObjectData = itemChild.GetComponent<UISerializableKeyObject>();
                    var item = new GridItem() { Index = -1, itemData = keyObjectData };
                    items.Add(item);
                    //CallAction(item);
                }

                itemInstanceCount = items.Count;
            }
        }


        /// <summary>
        /// 要求刷新item数据
        /// </summary>
        /// <param name="item"></param>
        void CallAction(GridItem item)
        {
            if (DebugLog) UnityEngine.Debug.Log(string.Format(" OnItemShow index = {0}", item.Index));
            OnItemShow?.Invoke(item);
        }

        /// <summary>
        /// 获得一个闲置的item
        /// </summary>
        /// <returns></returns>
        GridItem GetUnoccupiedItem()
        {
            foreach (var item in items)
            {
                if (item.Index == -1)
                {
                    return item;
                }
            }

            return null;
        }
    }

    public class GridItem
    {
        private int _index;

        public int Index
        {
            set
            {
                if (itemData != null && itemData.transform)
                {
                    itemData.transform.localScale = value < 0 ? Vector3.zero : Vector3.one;
                }

                _index = value;
            }

            get { return _index; }
        }

        public object data;

        public UISerializableKeyObject itemData;
    }
}