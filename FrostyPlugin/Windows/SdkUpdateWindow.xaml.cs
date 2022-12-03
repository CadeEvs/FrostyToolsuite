using Frosty.Controls;
using Frosty.Core.Sdk;
using Frosty.Core.IO;
using FrostySdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace Frosty.Core.Windows
{
    public enum SdkUpdateTaskState
    {
        Inactive,
        Active,
        CompletedSuccessful,
        CompletedFail
    }

    public class SdkUpdateTask : INotifyPropertyChanged
    {
        public delegate bool TaskDelegate(SdkUpdateTask task, object state);

        public string DisplayName { get => displayName; set { displayName = value; NotifyPropertyChanged(); } }
        public SdkUpdateTaskState State { get => state; set { state = value; NotifyPropertyChanged(); } }
        public string StatusMessage { get => statusMessage; set { statusMessage = value; NotifyPropertyChanged(); } }
        public string FailMessage { get => failMessage; set { failMessage = value; NotifyPropertyChanged(); } }
        public TaskDelegate Task { get; set; }

        private string displayName;
        private SdkUpdateTaskState state;
        private string statusMessage;
        private string failMessage;

        public SdkUpdateTask()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SdkUpdateState
    {
        public Process Process;
        public long TypeInfoOffset;
        public ClassesSdkCreator Creator;
    }

    /// <summary>
    /// Interaction logic for SdkUpdateWindow.xaml
    /// </summary>
    public partial class SdkUpdateWindow : FrostyDockableWindow
    {
        public string ProfileName => ProfilesLibrary.DisplayName;
        private SdkUpdateTask failedTask = null;

        public SdkUpdateWindow(Window owner)
        {
            InitializeComponent();
            Owner = owner;
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            pageOne.Visibility = Visibility.Collapsed;
            pageTwo.Visibility = Visibility.Visible;

            // tasks to execute async
            List<SdkUpdateTask> tasks = new List<SdkUpdateTask>()
            {
                new SdkUpdateTask() { DisplayName = "Waiting for process to become active", Task = OnDetectRunningProcess },
                new SdkUpdateTask() { DisplayName = "Scanning for type info offset", Task = OnFindTypeInfoOffset },
                new SdkUpdateTask() { DisplayName = "Dumping types from memory", Task = OnGatherTypesFromMemory },
                new SdkUpdateTask() { DisplayName = "Cross referencing assets", Task = OnCrossReferenceAssets },
                new SdkUpdateTask() { DisplayName = "Creating SDK", Task = OnCreateSdk }
            };
            tasksListBox.ItemsSource = tasks;

            SdkUpdateState state = new SdkUpdateState();

            foreach (var task in tasks)
            {
                // set current task to active and execute
                task.State = SdkUpdateTaskState.Active;
                await Task.Run(() => { task.Task(task, state); });

                if (task.State == SdkUpdateTaskState.CompletedFail)
                {
                    failedTask = task;
                    break;
                }
            }

            successMessage.Visibility = (failedTask == null) ? Visibility.Visible : Visibility.Collapsed;
            failMessage.Text = (failedTask != null) ? failedTask.FailMessage : "";
            finishButton.IsEnabled = true;
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Waits for the game process to become active before completing
        /// </summary>
        private bool OnDetectRunningProcess(SdkUpdateTask task, object state)
        {
            Process foundProcess = null;
            SdkUpdateState updateState = state as SdkUpdateState;

            while (true)
            {
                foreach (var process in Process.GetProcesses())
                {
                    try
                    {
                        string processFilename = process.MainModule?.ModuleName;
                        if (string.IsNullOrEmpty(processFilename))
                            continue;

                        FileInfo fi = new FileInfo(processFilename);
                        if (fi.Name.IndexOf(ProfilesLibrary.ProfileName, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            foundProcess = process;
                            break;
                        }
                    }
                    catch (Exception)
                    {
                        if (process.ProcessName.IndexOf(ProfilesLibrary.ProfileName, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            task.FailMessage = string.Format("Unable to access the specified process");
                            task.StatusMessage = process.ProcessName;
                            task.State = SdkUpdateTaskState.CompletedFail;
                            return false;
                        }
                    }
                }

                if (foundProcess != null)
                {
                    while (foundProcess.MainWindowHandle == IntPtr.Zero)
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));

                    updateState.Process = foundProcess;
                    task.StatusMessage = foundProcess.ProcessName;
                    break;
                }
            }

            task.State = SdkUpdateTaskState.CompletedSuccessful;
            return true;
        }

        /// <summary>
        /// Locates the offset to the first type info object in the active games memory
        /// </summary>
        private bool OnFindTypeInfoOffset(SdkUpdateTask task, object state)
        {
            SdkUpdateState updateState = state as SdkUpdateState;
            long startAddress = updateState.Process.MainModule.BaseAddress.ToInt64();

            MemoryReader reader = new MemoryReader(updateState.Process, startAddress);
            if (reader == null)
            {
                task.State = SdkUpdateTaskState.CompletedFail;
                return false;
            }

            string[] patterns = new string[]
            {
                "488B05???????? 48894108 ?? 488D05???????? 483905???????????? 488B05???????? 488905????????",
                "488b05???????? 48894108 48890d???????? 48???? C3",
                "488b05???????? 48894108 48890d???????? C3",
                "488b05???????? 48894108 48890d????????",
                "488b05???????? 488905???????? 488d05???????? 488905???????? E9",
                "488b05???????? 4885C074 ???????? 488b40", // not really a pattern but seems to work for bf2042?
                "48391D???????? ????488b4310"
            };

            // TODO: manually adding offsets for new games, need to find the pattern
            switch (ProfilesLibrary.DataVersion)
            {
                case (int)ProfileVersion.Madden22:
                    updateState.TypeInfoOffset = 0x146987A18;
                    break;
                //case (int)ProfileVersion.Battlefield2042:
                //    updateState.TypeInfoOffset = 0x14677EA30;
                //    break;
                case (int)ProfileVersion.Madden23:
                    updateState.TypeInfoOffset = 0x146BA7088;
                    break;
                default:
                    {
                        IList<long> offsets = null;
                        foreach (var pattern in patterns)
                        {
                            reader.Position = startAddress;
                            offsets = reader.scan(pattern);
                            if (offsets.Count != 0)
                                break;
                        }

                        if (offsets.Count == 0)
                        {
                            task.State = SdkUpdateTaskState.CompletedFail;
                            task.FailMessage = "Unable to find the first type info offset";
                            return false;
                        }

                        reader.Position = offsets[0] + 3;
                        int newValue = reader.ReadInt();
                        reader.Position = offsets[0] + 3 + newValue + 4;
                        updateState.TypeInfoOffset = reader.ReadLong();
                        break;
                    }
            }
            
            task.State = SdkUpdateTaskState.CompletedSuccessful;
            task.StatusMessage = string.Format("0x{0}", updateState.TypeInfoOffset.ToString("X8"));
            return true;
        }

        /// <summary>
        /// Gathers all type info objects from the active games memory
        /// </summary>
        private bool OnGatherTypesFromMemory(SdkUpdateTask task, object state)
        {
            SdkUpdateState updateState = state as SdkUpdateState;

            updateState.Creator = new ClassesSdkCreator(updateState);
            bool retCode = updateState.Creator.GatherTypeInfos(task);

            task.State = retCode ? SdkUpdateTaskState.CompletedSuccessful : SdkUpdateTaskState.CompletedFail;
            return retCode;
        }

        /// <summary>
        /// Uses the games assets to determine the final set of classes for the SDK
        /// </summary>
        private bool OnCrossReferenceAssets(SdkUpdateTask task, object state)
        {
            SdkUpdateState updateState = state as SdkUpdateState;
            bool retCode = updateState.Creator.CrossReferenceAssets(task);

            task.State = (retCode) ? SdkUpdateTaskState.CompletedSuccessful : SdkUpdateTaskState.CompletedFail;
            return retCode;
        }

        /// <summary>
        /// Produces the final Profile SDK DLL
        /// </summary>
        private bool OnCreateSdk(SdkUpdateTask task, object state)
        {
            SdkUpdateState updateState = state as SdkUpdateState;
            bool retCode = updateState.Creator.CreateSDK();

            task.State = (retCode) ? SdkUpdateTaskState.CompletedSuccessful : SdkUpdateTaskState.CompletedFail;
            return retCode;
        }
    }
}
