/// <summary>
/// @file   ViCommands.cs
///	@brief  ViGET项目中所用到的相关命令。
/// @author	DothanTech 胡殿兴
/// 
/// Copyright(C) 2011~2018, DothanTech. All rights reserved.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DothanTech.ViGET.ViCommand
{
    public class ViCommands
    {
        //----------------- Project相关命令----------------//
        public static RoutedCommand NewProject = new RoutedCommand();
        public static RoutedCommand OpenProject = new RoutedCommand();
        public static RoutedCommand AddNewProject = new RoutedCommand();
        public static RoutedCommand AddExistingProject = new RoutedCommand();
        public static RoutedCommand CloseSolution = new RoutedCommand();
        public static RoutedCommand Exit = new RoutedCommand();
        public static RoutedCommand OpenLocalFolder = new RoutedCommand();

        // ------------------ProjectItem相关命令--------------//
        public static RoutedUICommand AddNewItem = new RoutedUICommand();
        public static RoutedUICommand AddExistingItem = new RoutedUICommand();

        // ----------------- CPU 相关命令 -----------------//
        public static RoutedUICommand IsActive = ViCommands.CreateUICommand("IsActive");

        //-------------------File-------------------//
        //public static CompositeCommand NewFile = new CompositeCommand();
        //public static CompositeCommand OpenFile = new CompositeCommand();
        //public static CompositeCommand Close = new CompositeCommand();
        //public static CompositeCommand Save = new CompositeCommand();
        //public static CompositeCommand SaveAll = new CompositeCommand();
        public static RoutedUICommand Link = ViCommands.CreateUICommand("Link");
        public static RoutedUICommand Unlink = ViCommands.CreateUICommand("Unlink");

        //------------------Comman------------------//;
        public static RoutedUICommand Rename = new RoutedUICommand();

        //----------- Show View----------------//
        //public static RoutedUICommand ShowSolutionExplorer = ViCommands.CreateUICommand("ShowSolutionExplorer");
        //public static RoutedUICommand ShowStartPage = ViCommands.CreateUICommand("ShowStartPage");
        //public static RoutedUICommand ShowErrorList = ViCommands.CreateUICommand("ShowErrorList");
        //public static RoutedUICommand ShowOutput = ViCommands.CreateUICommand("ShowOutput");
        //public static RoutedUICommand ShowPOUs = ViCommands.CreateUICommand("ShowPOUs");
        //public static RoutedUICommand ShowProperties = ViCommands.CreateUICommand("ShowProperties");
        //------------Build------------------//
        public static RoutedUICommand Build = CreateUICommand("Build", null, Key.F5, ModifierKeys.None, "F5");        // Build CPU
        public static RoutedUICommand Rebuild = CreateUICommand("Rebuild");
        public static RoutedUICommand Clean = CreateUICommand("Clean");

        //public static RoutedUICommand BuildSolution = new RoutedUICommand();
        //public static RoutedUICommand RebuildSolution = new RoutedUICommand();
        //public static RoutedUICommand CleanSolution = new RoutedUICommand();

        //public static RoutedUICommand BuildActiveProject = new RoutedUICommand();
        //public static RoutedUICommand RebuildActiveProject = new RoutedUICommand();
        //public static RoutedUICommand CleanActiveProject = new RoutedUICommand();

        //public static RoutedUICommand BatchBuild = new RoutedUICommand();
        //public static RoutedUICommand BuildStop = new RoutedUICommand();
        //----------------Git--------------------//
        public static RoutedUICommand GitCreate = CreateUICommand("Create Repository");
        public static RoutedUICommand GitDiff = CreateUICommand("Diff");
        public static RoutedUICommand GitCommit = CreateUICommand("Commit");
        public static RoutedUICommand GitShowLog = CreateUICommand("Show Log");
        public static RoutedUICommand GitPull = CreateUICommand("Pull");
        public static RoutedUICommand GitPush = CreateUICommand("Push");

        public static RoutedUICommand CreateUICommand(String name, Type type = null, Key key = Key.None, ModifierKeys modify = ModifierKeys.None, String display = "")
        {
            RoutedUICommand routedUICommand = new RoutedUICommand(name, name, (type == null ? typeof(ViCommands) : type));
            if (key != Key.None)
            {
                KeyGesture gesture = new KeyGesture(key, modify, display);
                routedUICommand.InputGestures.Add(gesture);
            }
            return routedUICommand;
        }
    }
}
