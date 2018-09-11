using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KeyHolder.Model;
using KeyHolder.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows;

namespace KeyHolder.ViewModel
{
    public class CreateDbFileViewModel : ViewModelBase
    {
        private string fileName;
        private string location;
        private string filePassword;

        public CreateDbFileViewModel()
        {
            CreateCommand = new RelayCommand(CreateFile);
        }

        public RelayCommand CreateCommand { get; set; }

        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                RaisePropertyChanged();
            }
        }

        public string Location
        {
            get { return location; }
            set
            {
                location = value;
                RaisePropertyChanged();
            }
        }

        public string FilePassword
        {
            get { return this.filePassword; }
            set
            {
                this.filePassword = value;
                this.RaisePropertyChanged();
            }
        }

        // https://daedtech.com/directory-browser-dialog/
        // https://stackoverflow.com/questions/740837/how-to-create-a-password-protected-file-in-c-sharp
        // https://www.codeproject.com/Questions/552029/EncryptplusandplusDecryptplustheplusfilesplusinplu
        private void CreateFile()
        {
            var file = this.Location + "\\" + this.FileName + ".kh";
            TechOperations.CurrentWorkingFile = file;
            SecureHelper.CreateFileDb(new FileInfo(file));

            this.CachePathInConfig(file);
            this.CreateFilePassword();

            MessageBox.Show("Your file has been created!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

            this.FileName = string.Empty;
            this.Location = string.Empty;
            this.FilePassword = string.Empty;
        }

        private void CreateFilePassword()
        {
            TechOperations.CheckIfSettingsExists();

            string readJson = File.ReadAllText(TechOperations.CurrentWorkingFile);
            var decrypted = SecureHelper.DecryptString(readJson);

            var desJson = JsonConvert.DeserializeObject<List<PasswordModel>>(decrypted);

            if (desJson == null)
            {
                desJson = new List<PasswordModel>();
            }

            var pass = new PasswordModel { Username = this.FileName, Password = this.FilePassword };
            desJson.Add(pass);

            var json = JsonConvert.SerializeObject(desJson);
            using (var writer = new StreamWriter(TechOperations.SettingsFileLocation, true))
            {
                var text = SecureHelper.EncryptString(json);
                writer.Write(text);
            }
        }

        private void CachePathInConfig(string path)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove("LastFilePath");
            config.AppSettings.Settings.Add("LastFilePath", path);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
