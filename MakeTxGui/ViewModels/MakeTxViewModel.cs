using MakeTxGui.Helpers;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace MakeTxGui.ViewModels
{
    public class MakeTxViewModel : BindableBase
    {
        #region Field
        private Dispatcher _dispatcher;
        private string _sourcePath;
        private string _targetPath;
        private bool _isTargetDifferent = false;
        private bool _isIncludeSubDirectory = true;
        private bool _isEnabled = true;
        private bool _toStop = true;
        private double _progress = 0;

        private ObservableCollection<string> _allFiles = new ObservableCollection<string>();
        private ObservableCollection<string> _selectedFiles = new ObservableCollection<string>();
        private ObservableCollection<string> _allFilesSelected = new ObservableCollection<string>();
        private ObservableCollection<string> _selectedFilesSelected = new ObservableCollection<string>();

        private int _totalFileCount;
        private int _processedFileCount;
        private string[] _filterExtensions = { ".bmp", ".jpg", ".jpeg", ".tif", ".tiff", ".png", ".ico", ".icon", ".gif", ".tga", ".exr" };

        private MakeTxHelper _makeTxHelper;
        #endregion

        #region Property
        public string SourcePath
        {
            get => _sourcePath;
            set
            {
                if (SetProperty(ref _sourcePath, value))
                {
                    OnSourcePathChanged();
                }
            }
        }
        public string TargetPath { get => _targetPath; set => SetProperty(ref _targetPath, value); }
        public bool IsTargetDifferent
        {
            get => _isTargetDifferent;
            set
            {
                SetProperty(ref _isTargetDifferent, value);
                if (!_isTargetDifferent)
                {
                    TargetPath = SourcePath;
                }
            }
        }
        public bool IsIncludeSubDirectory
        {
            get => _isIncludeSubDirectory;
            set
            {
                if (SetProperty(ref _isIncludeSubDirectory, value))
                {
                    OnIncludeSubDirectoryChanged();
                }
            }
        }

        public ObservableCollection<string> AllFiles => _allFiles;
        public ObservableCollection<string> SelectedFiles => _selectedFiles;
        public ObservableCollection<string> AllFilesSelected => _allFilesSelected;
        public ObservableCollection<string> SelectedFilesSelected => _selectedFilesSelected;

        public int TotalFileCount { get => _totalFileCount; set => SetProperty(ref _totalFileCount, value); }
        public int ProcessedFileCount
        {
            get => _processedFileCount;
            set
            {
                if (SetProperty(ref _processedFileCount, value))
                {
                    if (_totalFileCount != 0)
                    {
                        Progress = _processedFileCount * 100.00 / _totalFileCount;
                    }
                    else
                    {
                        Progress = 100;
                    }
                }
            }
        }
        public double Progress { get => _progress; set => SetProperty(ref _progress, value); }
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (SetProperty(ref _isEnabled, value))
                {
                    RaisePropertyChanged(nameof(IsProcessing));
                }
            }
        }
        public bool IsProcessing => !IsEnabled;
        #endregion

        #region Command
        public ICommand BrowseSourcePathCommand => new DelegateCommand(OnBrowseSourcePath);
        public ICommand BrowseTargetPathCommand => new DelegateCommand(OnBrowseTargetPath);
        public ICommand SelectAllCommand => new DelegateCommand(OnSelectAll);
        public ICommand UnselectAllCommand => new DelegateCommand(OnUnselectAll);
        public ICommand SelectCommand => new DelegateCommand(OnSelect);
        public ICommand UnselectCommand => new DelegateCommand(OnUnselect);
        public ICommand ProcessCommand => new DelegateCommand(OnProcess);

        private void OnBrowseSourcePath()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog()
            {
                ShowNewFolderButton = false,
                SelectedPath = SourcePath,
                Description = "Please select source folder",
            };

            DialogResult result = folderDialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                if (folderDialog.SelectedPath != null && folderDialog.SelectedPath.Length > 0)
                {
                    SourcePath = folderDialog.SelectedPath;
                }
            }
        }

        private void OnBrowseTargetPath()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog()
            {
                ShowNewFolderButton = true,
                SelectedPath = SourcePath,
                Description = "Please select target folder",
            };

            DialogResult result = folderDialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                if (folderDialog.SelectedPath != null && folderDialog.SelectedPath.Length > 0)
                {
                    TargetPath = folderDialog.SelectedPath;
                }
            }
        }

        private void OnSelectAll()
        {
            SelectedFiles.AddRange(AllFiles.Where(f => !SelectedFiles.Contains(f))).OrderBy(f => f);
        }

        private void OnUnselectAll()
        {
            SelectedFiles.Clear();
        }

        private void OnSelect()
        {
            SelectedFiles.AddRange(AllFilesSelected.Where(f => !SelectedFiles.Contains(f))).OrderBy(f => f);
        }

        private void OnUnselect()
        {
            List<string> filesToRemove = SelectedFilesSelected.ToList();
            foreach (string f in filesToRemove)
            {
                SelectedFiles.Remove(f);
            }
        }

        private void OnProcess()
        {
            if (!IsProcessing)
            {
                Task task = new Task(ProcessFiles);
                task.Start();
            }
            else
            {
                _toStop = true;
            }
        }

        #endregion

        public MakeTxViewModel()
        {
            SourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            _makeTxHelper = new MakeTxHelper();
        }

        private void OnSourcePathChanged()
        {
            if (!IsTargetDifferent)
            {
                TargetPath = SourcePath;
            }

            GetFileList();
        }

        private void OnIncludeSubDirectoryChanged()
        {
            GetFileList();
        }

        private void GetFileList()
        {
            AllFiles.Clear();
            SelectedFiles.Clear();
            DirectoryInfo di = new DirectoryInfo(_sourcePath);
            if (di.Exists)
            {
                FileInfo[] files = di.GetFiles("*.*", _isIncludeSubDirectory ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                IEnumerable<string> filteredFiles = from FileInfo file in files
                                                    where _filterExtensions.Contains(file.Extension.ToLower())
                                                    select file.FullName.Substring(_sourcePath.Length + 1);

                AllFiles.AddRange(filteredFiles);
            }
            else
            {

            }
        }

        private void ProcessFiles()
        {
            _toStop = false;
            IsEnabled = false;
            TotalFileCount = SelectedFiles.Count;
            ProcessedFileCount = 0;
            foreach (string f in SelectedFiles)
            {
                string sourceFullFilename = Path.Combine(SourcePath, f);
                string targetFullFilename = Path.Combine(TargetPath, f.Remove(f.LastIndexOf(".")) + ".tx");
                List<string> stdout, errout;
                int result = _makeTxHelper.CallMakeTx(out stdout, out errout, sourceFullFilename, targetFullFilename);
                ProcessedFileCount++;
                if (_toStop)
                {
                    break;
                }
            }
            IsEnabled = true;
        }
    }
}
