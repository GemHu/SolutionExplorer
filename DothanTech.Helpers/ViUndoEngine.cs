/// <summary>
/// @file   ViUndoEngine.cs
///	@brief  ViGET 编辑器中注册依赖属性修改、支持 Undo/Redo 机制的引擎。
/// @author	DothanTech 刘伟宏
/// 
/// Copyright(C) 2011~2014, DothanTech. All rights reserved.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Windows.Input;
using System.Windows.Threading;

namespace DothanTech.Helpers
{
    /// <summary>
    /// ViGET 编辑器中注册依赖属性修改、支持 Undo/Redo 机制的引擎。
    /// </summary>
    public class ViUndoEngine
    {
        #region Life cycle

        /// <summary>
        /// 构建对象。
        /// </summary>
        public ViUndoEngine()
            : this(Dispatcher.CurrentDispatcher)
        {
        }

        /// <summary>
        /// 构建对象。
        /// </summary>
        public ViUndoEngine(DispatcherObject Dispatcher)
            : this(Dispatcher.Dispatcher)
        {
        }

        /// <summary>
        /// 构建对象。
        /// </summary>
        public ViUndoEngine(Dispatcher Dispatcher)
        {
            this.Dispatcher = Dispatcher;
        }

        #endregion

        #region Observe Change

        /// <summary>
        /// 观察对象的属性变化。如果 item 是 ViObservableCollection，则集合中的
        /// 所有元素（如果元素也是 ViObservableCollection，则还递归包含其子元素）
        /// 的属性变化也会被自动观察。
        /// </summary>
        /// <param name="item">对象</param>
        public void ObservePropertyChanged(IViPropertyChanged item)
        {
            item.PropertyChanged += item_PropertyChanged;
        }

        /// <summary>
        /// 观察集合的元素变化。
        /// </summary>
        /// <param name="collection">集合</param>
        /// @warning 集合元素的属性变化不会被观察。
        public void ObserveCollectionChanged(IViCollectionChanged collection)
        {
            collection.CollectionChanged += item_PropertyChanged;
        }

        void item_PropertyChanged(object sender, ViDataChangedEventArgs e)
        {
            if ((e.ChangeType != ViChangeType.None) &&
                (e.ChangeType & ViChangeType.NotCauseCache) == ViChangeType.None)
            {
                if (this.StartLogUndoInfo())
                {
                    this.AddLogUndoChange(e);
                }
            }
        }

        #endregion

        #region Observe Change Log Control

        /// <summary>
        /// 使能/禁止修改记录。被禁止后会自动异步使能。
        /// </summary>
        public void EnableLogUndoInfo(bool enable)
        {
            if (enable)
            {
                // 恢复修改记录
                this.logUndoStage = this.logUndoCurrChanges == null
                    ? LogUndoStage.NotStart : LogUndoStage.Logging;
            }
            else
            {
                if (this.logUndoStage != LogUndoStage.Disable)
                {
                    // 禁止修改记录
                    this.logUndoStage = LogUndoStage.Disable;

                    // 启动异步调用，以结束 Logging。需要使用最低的调用权限。
                    this.Dispatcher.BeginInvoke(new Action(() => this.StopLogUndoInfo()),
                        DispatcherPriority.ApplicationIdle);
                }
            }
        }

        /// <summary>
        /// 清空记录的 Undo/Redo 信息。
        /// </summary>
        public void ClearLogUndoInfo()
        {
            bool hasLog = (this.logUndoChangesChain.Count > 0);

            this.logUndoStage = LogUndoStage.NotStart;
            this.StopLogUndoInfo();

            this.logUndoChangesChain = new ArrayList();
            this.logUndoCurrSavedIndex = 0;
            this.logUndoCurrEditIndex = 0;

            // 只有原来有 LOG 内容时，才发出内容修改通知
            if (hasLog)
                this.NotifyContentIsChanged();
        }

        #endregion

        #region ContentIsChanged

        /// <summary>
        /// 当前编辑内容是否修改过？当文档被保存时，需要设置该属性为 false。
        /// </summary>
        public bool ContentIsChanged
        {
            get
            {
                if (this.logUndoCurrEditIndex == this.logUndoCurrSavedIndex)
                    return false;

                // 也许中间是些无关紧要、不导致文档变化的修改？
                int nStart = Math.Min(this.logUndoCurrEditIndex, this.logUndoCurrSavedIndex);
                int nEnd__ = Math.Max(this.logUndoCurrEditIndex, this.logUndoCurrSavedIndex);
                if (nStart < 0) nStart = 0;

                for (int i = nStart; i < nEnd__; ++i)
                {
                    LogUndoItem undoItem = this.logUndoChangesChain[i] as LogUndoItem;
                    if ((undoItem.ChangeType & ViChangeType.CauseDirty) != ViChangeType.None)
                        return true;
                }

                return false;
            }
            set
            {
                if (!value)
                {
                    if (this.logUndoCurrSavedIndex != this.logUndoCurrEditIndex)
                    {
                        this.logUndoCurrSavedIndex = this.logUndoCurrEditIndex;

                        // 内容或者修改状态发生变化
                        this.NotifyContentIsChanged();
                    }
                }
            }
        }

        /// <summary>
        /// ContentIsChanged 发生变化时通知事件的处理函数原型。
        /// </summary>
        /// <param name="sender">事件发出者</param>
        /// <param name="newState">新的 ContentIsChanged 状态</param>
        public delegate void ContentIsChangedHandler(object sender, bool oldState, bool newState);

        /// <summary>
        /// ContentIsChanged 发生变化时通知事件。
        /// </summary>
        public event ContentIsChangedHandler ContentIsChangedEvent;

        protected void NotifyContentIsChanged()
        {
            bool oldState = this.lastNotifiedChanged;
            bool newState = this.ContentIsChanged;
            lastNotifiedChanged = newState;
            if (ContentIsChangedEvent != null)
                ContentIsChangedEvent.Invoke(this, oldState, newState);
        }

        protected bool lastNotifiedChanged = false;

        #endregion

        #region Undo / Redo

        /// <summary>
        /// 是否可以撤销修改？
        /// </summary>
        public bool CanUndo
        {
            get
            {
                return this.logUndoCurrEditIndex > 0;
            }
        }

        /// <summary>
        /// 执行一步 Undo 操作。
        /// </summary>
        public uint Undo()
        {
            if (!this.CanUndo)
                return ViChangeType.None;

            // 显示沙漏鼠标
            OverrideBusyCursor obc = new OverrideBusyCursor(true);

            // 暂时禁止修改记录
            EnableLogUndoInfo(false);

            // 逆向循环所有修改
            --this.logUndoCurrEditIndex;
            //
            LogUndoItem undoItem = this.logUndoChangesChain[this.logUndoCurrEditIndex] as LogUndoItem;
            uint sumTypes = undoItem.ChangeType; ArrayList alChanges = undoItem.Changes;
            for (int i = alChanges.Count - 1; i >= 0; --i)
            {
                (alChanges[i] as ViDataChangedEventArgs).Undo();
            }
            //
            this.NotifyContentIsChanged();

            return sumTypes;
        }

        /// <summary>
        /// 是否可以重做修改？
        /// </summary>
        public bool CanRedo
        {
            get
            {
                return this.logUndoCurrEditIndex < this.logUndoChangesChain.Count;
            }
        }

        /// <summary>
        /// 执行一步 Redo 操作。
        /// </summary>
        public uint Redo()
        {
            if (!this.CanRedo)
                return ViChangeType.None;

            // 显示沙漏鼠标
            OverrideBusyCursor obc = new OverrideBusyCursor(true);

            // 暂时禁止修改记录
            EnableLogUndoInfo(false);

            // 逆向循环所有修改
            LogUndoItem undoItem = this.logUndoChangesChain[this.logUndoCurrEditIndex] as LogUndoItem;
            uint sumTypes = undoItem.ChangeType; ArrayList alChanges = undoItem.Changes;
            for (int i = 0; i < alChanges.Count; ++i)
            {
                (alChanges[i] as ViDataChangedEventArgs).Redo();
            }
            //
            ++this.logUndoCurrEditIndex;
            this.NotifyContentIsChanged();

            return sumTypes;
        }

        #endregion

        #region Undo Engine

        /// <summary>
        /// 记录 Undo/Redo 修改信息的状态
        /// </summary>
        protected enum LogUndoStage
        {
            Disable,        ///< 被禁止了
            NotStart,       ///< 尚未开始
            Logging,        ///< 记录过程中
        }

        /// <summary>
        /// 记录的修改信息列表。
        /// </summary>
        protected class LogUndoItem
        {
            public LogUndoItem(ArrayList changes)
            {
                this.Changes = changes;
                this.ChangeType = ViChangeType.None;
                foreach (ViDataChangedEventArgs change in changes)
                    this.ChangeType |= change.ChangeType;
            }

            public uint ChangeType { get; set; }
            public ArrayList Changes { get; set; }
        }

        /// <summary>
        /// 界面线程的 Dispatcher。
        /// </summary>
        protected Dispatcher Dispatcher = null;

        /// <summary>
        /// 当前记录修改的阶段。
        /// </summary>
        protected LogUndoStage logUndoStage = LogUndoStage.NotStart;
        /// <summary>
        /// 当前一步动作的所有修改，LogUndoItem。
        /// </summary>
        protected ArrayList logUndoCurrChanges = null;
        protected Dictionary<ViDataChangedEventArgs, byte> mapUndoCurrChanges = null;
        /// <summary>
        /// Undo/Redo 动作的最大步数。
        /// </summary>
        protected const int logUndoChainCapability = 30;
        /// <summary>
        /// 所有步数动作修改的数组，ArrayList。
        /// </summary>
        protected ArrayList logUndoChangesChain = new ArrayList();
        /// <summary>
        /// 当前保存内容对应的修改动作序号，0～Chain.Count / -1
        /// </summary>
        protected int logUndoCurrSavedIndex = 0;
        /// <summary>
        /// 当前 Undo/Redo 动作对应的修改动作序号。
        /// </summary>
        protected int logUndoCurrEditIndex = 0;

        /// <summary>
        /// 启动修改记录。启动之后会自动异步记录修改。
        /// </summary>
        /// <returns>是否处于记录修改状态？</returns>
        protected bool StartLogUndoInfo()
        {
            return this.StartLogUndoInfo(true);
        }

        /// <summary>
        /// 启动修改记录。启动之后会自动异步记录修改。
        /// </summary>
        /// <param name="autoCommit">是否自动提交修改？</param>
        /// <returns>是否处于记录修改状态？</returns>
        protected bool StartLogUndoInfo(bool autoCommit)
        {
            if (this.logUndoStage == LogUndoStage.NotStart)
            {
                // 准备空间，开始修改记录
                this.logUndoStage = LogUndoStage.Logging;
                this.logUndoCurrChanges = new ArrayList();
                this.mapUndoCurrChanges = new Dictionary<ViDataChangedEventArgs, byte>();

                // 启动异步调用，以结束 Logging
                if (autoCommit)
                {
                    this.Dispatcher.BeginInvoke(new Action(() => this.StopLogUndoInfo()),
                            DispatcherPriority.ApplicationIdle);
                }
            }

            return (this.logUndoStage == LogUndoStage.Logging);
        }

        /// <summary>
        /// 停止修改记录。
        /// </summary>
        protected void StopLogUndoInfo()
        {
            bool logChanged = false;

            if (this.logUndoStage == LogUndoStage.Logging &&
                this.logUndoCurrChanges.Count > 0)
            {
                Debug.Assert(this.logUndoCurrEditIndex >= 0 &&
                    this.logUndoCurrEditIndex <= this.logUndoChangesChain.Count);

                // 有修改，清除后面的修改记录
                if (this.logUndoCurrEditIndex < this.logUndoChangesChain.Count)
                {
                    this.logUndoChangesChain.RemoveRange(this.logUndoCurrEditIndex,
                        this.logUndoChangesChain.Count - this.logUndoCurrEditIndex);
                }
                // 如果文件保存的位置要被丢弃了，那只能将保存位置设置为无效值了
                if (this.logUndoCurrSavedIndex > this.logUndoCurrEditIndex)
                    this.logUndoCurrSavedIndex = -1;

                LogUndoItem undoItem = new LogUndoItem(this.logUndoCurrChanges);
                LogUndoItem lastItem = this.logUndoChangesChain.Count <= 0 ? null :
                    this.logUndoChangesChain[this.logUndoChangesChain.Count - 1] as LogUndoItem;

                if (lastItem != null &&
                    (undoItem.ChangeType & ViChangeType.CauseDirty) == ViChangeType.None)
                { // 当前的是无关紧要的修改，合并入前一个修改项目
                    lastItem.ChangeType |= undoItem.ChangeType;
                    lastItem.Changes.AddRange(undoItem.Changes);
                }
                else if (lastItem != null &&
                    this.logUndoCurrSavedIndex < this.logUndoCurrEditIndex &&
                    (lastItem.ChangeType & ViChangeType.CauseDirty) == ViChangeType.None)
                { // 如果前一个是无关紧要的修改，则合并入前一个修改项目（要考虑保存位置）
                    lastItem.ChangeType |= undoItem.ChangeType;
                    lastItem.Changes.AddRange(undoItem.Changes);
                }
                else
                {
                    // 将修改加入修改记录链表
                    this.logUndoChangesChain.Add(undoItem);
                    ++this.logUndoCurrEditIndex;

                    // 修改记录不能太多，防止占用太多内存
                    if (this.logUndoCurrEditIndex > logUndoChainCapability)
                    {
                        // 如果文件保存的位置要被丢弃了，那只能将保存位置设置为无效值了
                        if (this.logUndoCurrSavedIndex == 0)
                            this.logUndoCurrSavedIndex = -1;
                        // 删除最开始的一个修改记录
                        this.logUndoChangesChain.RemoveAt(0);
                        --this.logUndoCurrEditIndex;
                    }
                }

                // LOG 内容发生了变化
                logChanged = true;
            }

            // 恢复修改记录状态
            this.logUndoStage = LogUndoStage.NotStart;
            this.logUndoCurrChanges = null;
            this.mapUndoCurrChanges = null;

            // LOG 内容发生了变化时给出通知
            if (logChanged)
                this.NotifyContentIsChanged();

            // 刷新各工具的状态
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// 将修改动作加入当前修改列表。
        /// </summary>
        protected void AddLogUndoChange(ViDataChangedEventArgs change)
        {
            if (this.logUndoStage == LogUndoStage.Logging)
            {
                if (!this.mapUndoCurrChanges.ContainsKey(change))
                {
                    this.mapUndoCurrChanges[change] = 1;
                    this.logUndoCurrChanges.Add(change);
                }
            }
        }

        #endregion
    }
}
