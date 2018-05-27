using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Windows;
using DothanTech.Helpers;

namespace DothanTech.ViGET.Manager
{
    public abstract class ViTaskInfo : ViNamedObject<ViCPUInfo>
    {
        public ViTaskInfo(String name, int priority)
        {
            this.Name = name;
            this.Priority = priority;
        }

        public new String Type
        {
            get { return base.Type as String; }
            set { base.Type = value; }
        }

        public int Priority { get; set; }

        #region TaskFiles

        public ViObservableCollection<TaskFile> TaskFiles
        {
            get
            {
                if (this._taskFiles == null)
                    this._taskFiles = new ViObservableCollection<TaskFile>();

                return this._taskFiles;
            }
        }
        private ViObservableCollection<TaskFile> _taskFiles;

        // ------------------------- 增、删、查、改 -------------------//

        #region Task 的增删查改操作

        public void AddTaskFile(String fileName, int order = 0)
        {
            if (String.IsNullOrEmpty(fileName))
                return;

            fileName = Path.GetFileNameWithoutExtension(fileName);
            TaskFile file = new TaskFile(fileName, order);
            file.SetParent(this);
            this.TaskFiles.SortedAdd(file);
        }

        public bool RemoveTaskFile(String fileName)
        {
            TaskFile target = this.GetTaskFile(fileName);
            if (target == null)
                return false;

            return this.TaskFiles.Remove(target);
        }

        public TaskFile GetTaskFile(String fileName)
        {
            if (String.IsNullOrEmpty(fileName))
                return null;

            fileName = Path.GetFileNameWithoutExtension(fileName);
            foreach (var item in this.TaskFiles)
            {
                if (item.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    return item;
            }

            return null;
        }

        public bool RenameTaskFile(String oldFileName, String newFileName)
        {
            if (String.IsNullOrEmpty(oldFileName) || String.IsNullOrEmpty(newFileName))
                return false;
            TaskFile target = this.GetTaskFile(oldFileName);
            if (target == null)
                return false;

            newFileName = Path.GetFileNameWithoutExtension(newFileName);
            target.Name = newFileName;
            return true;
        }

        /// <summary>
        /// 更新Task中文件的执行顺序；
        /// </summary>
        public void UpdatePriority()
        {
            int index = 1;
            foreach (var item in this.TaskFiles)
            {
                item.ExectionOrder = index++;
            }
        }

        #endregion

        #endregion

        public override int CompareTo(ViNamedObject other)
        {
            // 类型相同比较优先级；
            if (this.GetType().Equals(other.GetType()))
                return this.Priority - (other as ViTaskInfo).Priority;
            // 类型不同，则Cycle类型的Task排到前边；
            if (this is CyclicTaskInfo)
                return -1;

            return 1;
        }

        /// <summary>
        /// 根据给定的类型，创建目标任务实例；
        /// </summary>
        public static ViTaskInfo CreateTaskInfo(String type, String name, int priority)
        {
            if (CyclicTaskInfo.TASK_TYPE.Equals(type, StringComparison.OrdinalIgnoreCase))
                return new CyclicTaskInfo(name, priority);
            else if (InterruptTaskInfo.TASK_TYPE.Equals(type, StringComparison.OrdinalIgnoreCase))
                return new InterruptTaskInfo(name, priority);

            return null;
        }

        /// <summary>
        /// 从XMLElement中读取相关有效数据；
        /// </summary>
        public bool LoadElement(XmlElement element)
        {
            if (element == null || !Constants.TAG.Task.Equals(element.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            foreach (XmlNode fileElement in element.ChildNodes)
            {
                if (Constants.TAG.File.Equals(fileElement.Name, StringComparison.OrdinalIgnoreCase))
                {
                    String fileName = (fileElement as XmlElement).GetAttribute(Constants.Attribute.FileName);
                    String order = (fileElement as XmlElement).GetAttribute(Constants.Attribute.ExectionOrder);
                    int exectionOrder = Int32.Parse(order);
                    this.TaskFiles.SortedAdd(new TaskFile(Path.GetFileNameWithoutExtension(fileName), exectionOrder));
                }
            }

            return true;
        }

        /// <summary>
        /// 将相关的配置信息保存到XMLElement中；
        /// </summary>
        public bool SaveElement(XmlElement parentElement, XmlDocument doc)
        {
            XmlElement element = doc.CreateElement(Constants.TAG.Task);
            parentElement.AppendChild(element);
            element.SetAttribute(Constants.Attribute.Name, this.Name);
            element.SetAttribute(Constants.Attribute.Type, this.Type);
            element.SetAttribute(Constants.Attribute.Priority, this.Priority.ToString());
            element.SetAttribute(Constants.Attribute.FileNumber, this.TaskFiles.Count.ToString());

            foreach (var item in this.TaskFiles)
            {
                XmlElement fileElement = doc.CreateElement(Constants.TAG.File);
                element.AppendChild(fileElement);
                fileElement.SetAttribute(Constants.Attribute.ExectionOrder, item.ExectionOrder.ToString());
                fileElement.SetAttribute(Constants.Attribute.FileName, item.Name);
            }

            return true;
        }
    }

    public class CyclicTaskInfo : ViTaskInfo
    {
        public const String TASK_TYPE = "cyclic";

        public CyclicTaskInfo(String name, int priority)
            : base(name, priority)
        {
            this.Type = TASK_TYPE;
        }
    }

    public class InterruptTaskInfo : ViTaskInfo
    {
        public const String TASK_TYPE = "interrupt";

        public InterruptTaskInfo(String name, int priority)
            : base(name, priority)
        {
            this.Type = TASK_TYPE;
        }
    }

    public class TaskFile : ViNamedObject<ViTaskInfo>
    {
        public TaskFile(String name)
            : base(name)
        {
        }

        public TaskFile(String name, int order)
            : this(name)
        {
            this.ExectionOrder = order;
        }

        #region 基本属性

        /// <summary>
        /// 当前文件的执行顺序；
        /// </summary>
        public int ExectionOrder { get; set; }

        #endregion

        public override int CompareTo(ViNamedObject other)
        {
            TaskFile target = other as TaskFile;
            // 如果执行顺序小于等于0，则说明该Task是随机添加上去的，在排序的时候需要放到后面；
            if (this.ExectionOrder > 0 && target.ExectionOrder > 0)
                return this.ExectionOrder - target.ExectionOrder;

            return this.ExectionOrder > 0 ? -1 : 1;
        }
    }
}
