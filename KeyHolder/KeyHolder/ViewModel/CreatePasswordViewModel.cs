using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KeyHolder.Model;
using KeyHolder.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Web.Security;
using System.Windows;

namespace KeyHolder.ViewModel
{
    public class CreatePasswordViewModel : ViewModelBase
    {
        private string name;
        private string description;
        private string username;
        private string password;
        private string address;

        public CreatePasswordViewModel()
        {
            this.SaveCommand = new RelayCommand(this.SavePassword);
            this.GenerateCommand = new RelayCommand(this.GeneratePassword);
        }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand GenerateCommand { get; set; }

        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                this.RaisePropertyChanged();
            }
        }

        public string Description
        {
            get { return this.description; }
            set
            {
                this.description = value;
                this.RaisePropertyChanged();
            }
        }

        public string Username
        {
            get { return this.username; }
            set
            {
                this.username = value;
                this.RaisePropertyChanged();
            }
        }

        public string Password
        {
            get { return this.password; }
            set
            {
                this.password = value;
                this.RaisePropertyChanged();
            }
        }

        public string Address
        {
            get { return this.address; }
            set
            {
                this.address = value;
                this.RaisePropertyChanged();
            }
        }

        private void SavePassword()
        {
            var pass = new PasswordModel
            {
                Name = this.Name,
                Description = this.Description,
                Username = this.Username,
                Password = this.Password,
                Address = this.Address
            };

            string readJson = File.ReadAllText(TechOperations.CurrentWorkingFile);//(@"C:\Development\Playground\secure.kh");
            var decrypted = SecureHelper.DecryptString(readJson);

            var cleaner = File.CreateText(TechOperations.CurrentWorkingFile);//(@"C:\Development\Playground\secure.kh");
            cleaner.Flush();
            cleaner.Close();

            var desJson = JsonConvert.DeserializeObject<List<PasswordModel>>(decrypted);

            if (desJson == null)
            {
                desJson = new List<PasswordModel>();
            }

            desJson.Add(pass);

            var json = JsonConvert.SerializeObject(desJson);

            using (var writer = new StreamWriter(TechOperations.CurrentWorkingFile, true))//(@"C:\Development\Playground\secure.kh", true))
            {
                var text = SecureHelper.EncryptString(json);
                writer.Write(text);
            }

            MessageBox.Show("Your password has been created!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

            this.Name = string.Empty;
            this.Description = string.Empty;
            this.Username = string.Empty;
            this.Password = string.Empty;
            this.Address = string.Empty;
        }

        private void GeneratePassword()
        {
            this.Password = Membership.GeneratePassword(12, 3);
        }
    }
}
