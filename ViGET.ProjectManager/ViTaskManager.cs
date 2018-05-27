using DothanTech.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DothanTech.ViGET.Manager
{
    public class ViTaskManager : ViObject
    {
        public ViObservableCollection<ViTaskInfo> Tasks
        {
            get
            {
                if (this._tasks == null)
                    this._tasks = new ViObservableCollection<ViTaskInfo>();

                return this._tasks;
            }
        }
        private ViObservableCollection<ViTaskInfo> _tasks;

        public ViTaskInfo GetTaskInfo(String taskName)
        {
            if (String.IsNullOrEmpty(taskName))
                return null;

            foreach (var item in this.Tasks)
            {
                if (taskName.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                    return item;
            }

            return null;
        }

        public ViTaskInfo CreateTaskInfo(String type, String name, int priority)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(type))
                return null;

            ViTaskInfo task = this.GetTaskInfo(name);
            if (task != null)
                return task;

            task = ViTaskInfo.CreateTaskInfo(type, name, priority);
            if (task == null)
                return null;

            this.Tasks.SortedAdd(task);
            return task;
        }

        ///// <summary>
        ///// 暂时无用
        ///// 更新 Task 执行的优先级（先后顺序）；
        ///// </summary>
        //public void UpdateTaskPriority()
        //{
        //    int index = 1;
        //    foreach (var item in this.CyclicTasks)
        //    {
        //        item.Priority = index++;
        //    }

        //    index = 1;
        //    foreach (var item in this.InterruptTasks)
        //    {
        //        item.Priority = index++;
        //    }
        //}

        public IEnumerable<ViTaskInfo> CyclicTasks
        {
            get { return this.Tasks.Where(item => item is CyclicTaskInfo); }
        }

        public IEnumerable<ViTaskInfo> InterruptTasks
        {
            get { return this.Tasks.Where(item => item is InterruptTaskInfo); }
        }

        private ViTaskInfo GetDefaultCyclicTask()
        {
            ViTaskInfo task = this.CyclicTasks.FirstOrDefault();
            if (task != null)
                return task;

            task = ViTaskInfo.CreateTaskInfo(CyclicTaskInfo.TASK_TYPE, "T1", 1);
            this.Tasks.SortedAdd(task);
            return task;
        }

        private ViTaskInfo GetDefaultInterruptTask()
        {
            ViTaskInfo task = this.InterruptTasks.FirstOrDefault();
            if (task != null)
                return task;

            task = ViTaskInfo.CreateTaskInfo(InterruptTaskInfo.TASK_TYPE, "T1", 1);
            this.Tasks.SortedAdd(task);
            return task;
        }

        public bool HasLinked(String fileName)
        {
            TaskFile target = null;
            foreach (var item in this.Tasks)
            {
                target = item.GetTaskFile(fileName);
                if (target != null)
                    break;
            }

            return target != null;
        }

        public void LinkTask(String fileName)
        {
            // 1、县查找下当前文件是否已经存在；
            if (this.HasLinked(fileName))
                return;

            ViTaskInfo task = this.GetDefaultCyclicTask();
            task.AddTaskFile(fileName);
            task.UpdatePriority();
        }

        public void RenameTaskFile(String oldFile, String newFile)
        {
            foreach (var item in this.Tasks)
            {
                item.RenameTaskFile(oldFile, newFile);
            }
        }

        /// <summary>
        /// 遍历所有的Task，如果Link过给定的文件，则全部进行Unlink操作；
        /// </summary>
        /// <param name="fileName"></param>
        public void UnlinkTask(String fileName)
        {
            foreach (var item in this.Tasks)
            {
                if (item.RemoveTaskFile(fileName))
                    item.UpdatePriority();
            }
        }

    }
}
