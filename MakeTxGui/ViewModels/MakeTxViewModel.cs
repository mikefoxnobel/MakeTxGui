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
        private readonly Dispatcher _dispatcher;
        private string _sourcePath;
        private string _targetPath;
        private bool _isTargetDifferent = false;
        private bool _isIncludeSubDirectory = true;
        private bool _isEnabled = true;
        private bool _toStop = true;
        private double _progress = 0;

        private readonly ObservableCollection<string> _allFiles = new ObservableCollection<string>();
        private readonly ObservableCollection<string> _selectedFiles = new ObservableCollection<string>();
        private readonly ObservableCollection<string> _allFilesSelected = new ObservableCollection<string>();
        private readonly ObservableCollection<string> _selectedFilesSelected = new ObservableCollection<string>();

        private int _totalFileCount;
        private int _processedFileCount;
        private readonly string[] _filterExtensions = { ".bmp", ".jpg", ".jpeg", ".tif", ".tiff", ".png", ".ico", ".icon", ".gif", ".tga", ".exr" };

        private readonly MakeTxHelper _makeTxHelper;
        #endregion

        #region Property
        public string SourcePath
        {
            get => _sourcePath;
            set
            {
                if (SetProperty(ref _sourcePath, value))
                {
                    this.OnSourcePathChanged().ConfigureAwait(true);
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
                    this.OnIncludeSubDirectoryChanged().ConfigureAwait(true);
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

        private async void OnSelectAll()
        {
            await Task.Run(() =>
            {
                foreach (string filename in this.AllFiles.Where(f => !this.SelectedFiles.Contains(f)).OrderBy(f => f))
                {
                    this._dispatcher.Invoke(() => { this.SelectedFiles.Add(filename); });
                }
            });
        }

        private async void OnUnselectAll()
        {
            await Task.Run(() =>
            {
                this._dispatcher.Invoke(() => SelectedFiles.Clear());
            }).ConfigureAwait(true);
        }

        private async void OnSelect()
        {
            await Task.Run(() =>
            {
                this._dispatcher.Invoke(() =>
                {
                    foreach (string filename in this.AllFilesSelected.Where(f => !this.SelectedFiles.Contains(f)).OrderBy(f => f))
                    {
                        this._dispatcher.Invoke(() => this.SelectedFiles.Add(filename));
                    }
                });
            }).ConfigureAwait(true);
        }

        private async void OnUnselect()
        {
            await Task.Run(() =>
            {
                List<string> filesToRemove = SelectedFilesSelected.ToList();
                foreach (string f in filesToRemove)
                {
                    this._dispatcher.Invoke(() => this.SelectedFiles.Remove(f));
                }
            });
        }

        private async void OnProcess()
        {
            if (!IsProcessing)
            {
                await this.ProcessFiles();
            }
            else
            {
                _toStop = true;
            }
        }

        #endregion

        public MakeTxViewModel()
        {
            this._dispatcher = Dispatcher.CurrentDispatcher;
            SourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            _makeTxHelper = new MakeTxHelper();
        }

        private async Task OnSourcePathChanged()
        {
            if (!IsTargetDifferent)
            {
                TargetPath = SourcePath;
            }

            await this.RefreshFileList().ConfigureAwait(true);
        }

        private async Task OnIncludeSubDirectoryChanged()
        {
            await this.RefreshFileList().ConfigureAwait(true);
        }

        private async Task RefreshFileList()
        {
            await Task.Run(() =>
            {
                this._dispatcher.Invoke(() =>
                {
                    AllFiles.Clear();
                    SelectedFiles.Clear();
                });

                foreach (string filename in GetFileList())
                {
                    this._dispatcher.Invoke(() => { AllFiles.Add(filename); });
                }

            }).ConfigureAwait(true);
        }

        private IEnumerable<string> GetFileList()
        {
            DirectoryInfo di = new DirectoryInfo(_sourcePath);
            if (di.Exists)
            {
                IEnumerable<string> filteredFiles = di.EnumerateFiles("*.*", _isIncludeSubDirectory ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    .Where(file => _filterExtensions.Contains(file.Extension.ToLower()))
                    .Select(file => file.FullName.Substring(_sourcePath.Length + 1));

                foreach (string filename in filteredFiles)
                {
                    yield return filename;
                }
            }
            else
            {
                yield break;
            }
        }

        private async Task ProcessFiles()
        {
            await Task.Run(() =>
            {
                _toStop = false;
                this._dispatcher.Invoke(() =>
                {
                    this.IsEnabled = false;
                    this.TotalFileCount = this.SelectedFiles.Count;
                    this.ProcessedFileCount = 0;
                });

                Parallel.ForEach(this.SelectedFiles, (f, state) =>
                {
                    string sourceFullFilename = Path.Combine(SourcePath, f);
                    string targetFullFilename = Path.Combine(TargetPath, f.Remove(f.LastIndexOf(".")) + ".tx");
                    List<string> stdout, errout;
                    int result = _makeTxHelper.CallMakeTx(out stdout, out errout, sourceFullFilename, targetFullFilename);
                    this._dispatcher.Invoke(() => this.ProcessedFileCount++);
                    if (_toStop)
                    {
                        state.Break();
                    }
                });

                IsEnabled = true;
            });
        }
    }
}
