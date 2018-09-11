using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KeyHolder.Model;
using KeyHolder.Utils;
using KeyHolder.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace KeyHolder.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<PasswordModel> passCollection;
        private string selectedListItem;
        private PasswordModel selectedGrid;
        private string selectedFileLocation;

        public MainViewModel()
        {
            this.NewDatabaseCommand = new RelayCommand(this.NewDatabase);
            this.OpenFileCommand = new RelayCommand(this.OpenFile);
            this.CreatePasswordCommand = new RelayCommand(this.CreatePassword);
            this.CopyPasswordCommand = new RelayCommand(this.CopyPassword);
            this.CopyUsernameCommand = new RelayCommand(this.CopyUsername);
            this.CopyAddressCommand = new RelayCommand(this.CopyAddress);
            this.DeleteCommand = new RelayCommand(this.Delete);


            this.PassCollection = new ObservableCollection<PasswordModel>();
        }

        public RelayCommand NewDatabaseCommand { get; set; }
        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand CreatePasswordCommand { get; set; }
        public RelayCommand CopyPasswordCommand { get; set; }
        public RelayCommand CopyUsernameCommand { get; set; }
        public RelayCommand CopyAddressCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }

        public ObservableCollection<PasswordModel> PassCollection
        {
            get { return passCollection; }
            set
            {
                passCollection = value;
                this.RaisePropertyChanged();
            }
        }

        public string SelectedListItem
        {
            get { return this.selectedListItem; }
            set
            {
                this.selectedListItem = value;
                this.RaisePropertyChanged();
            }
        }

        public PasswordModel SelectedGrid
        {
            get { return this.selectedGrid; }
            set
            {
                this.selectedGrid = value;
                this.RaisePropertyChanged();
            }
        }

        public string SelectedFileLocation
        {
            get
            {
                TechOperations.CurrentWorkingFile = this.selectedFileLocation;
                return this.selectedFileLocation;
            }
            set
            {
                this.selectedFileLocation = value;
                TechOperations.CurrentWorkingFile = this.selectedFileLocation;
                this.RaisePropertyChanged();
            }
        }

        private void NewDatabase()
        {
            var win = new CreateDbFileWindow();
            win.Show();
        }

        private void OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    var win = new FilePasswordWindow();
                    win.ShowDialog();
                    var state = this.AuthorizeFile(ofd.FileName);

                    if (state != true)
                    {
                        MessageBox.Show("Wrong password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var json = File.ReadAllText(ofd.FileName);
                    this.SelectedFileLocation = ofd.FileName;
                    if (string.IsNullOrEmpty(json))
                    {
                        MessageBox.Show("Your database is empty!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    json = SecureHelper.DecryptString(json);

                    var passMask = JsonConvert.DeserializeObject<ObservableCollection<PasswordModel>>(json);
                    foreach (var item in passMask)
                    {
                        item.Password = "**********";
                    }

                    this.PassCollection = passMask;
                }
                catch (Exception ex)
                {

                    MessageBox.Show($"Error - {ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CreatePassword()
        {
            var win = new CreatePasswordWindow();
            win.Show();
        }

        private void CopyPassword()
        {
            var pass = TechOperations.GetPassword(SelectedGrid.Name);
            Clipboard.SetText(pass);
        }

        private void CopyUsername()
        {
            Clipboard.SetText(this.SelectedGrid.Username);
        }

        private void CopyAddress()
        {
            Clipboard.SetText(this.SelectedGrid.Address);
        }

        private void Delete()
        {
            TechOperations.DeleteEntry(SelectedGrid.Name);
            MessageBox.Show("Your entry has been deleted!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool AuthorizeFile(string file)
        {
            var text = File.ReadAllText(TechOperations.SettingsFileLocation);
            var decr = SecureHelper.DecryptString(text);
            var json = JsonConvert.DeserializeObject<List<PasswordModel>>(decr);
            var fileName = Path.GetFileNameWithoutExtension(file);

            var item = json.Where(x => x.Username.Equals(fileName)).FirstOrDefault();

            if (item.Username.Equals(fileName) && item.Password.Equals(TechOperations.FilePass))
            {
                return true;
            }

            return false;
        }
    }
}