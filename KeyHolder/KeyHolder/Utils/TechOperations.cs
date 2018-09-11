using KeyHolder.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace KeyHolder.Utils
{
    public static class TechOperations
    {
        public static string CurrentWorkingFile = string.Empty;
        public static string SettingsFileLocation = $@"C:\Users\{Environment.UserName}\AppData\Local\KeyHolder\settings.json";
        public static string FilePass = string.Empty;

        public static void DeleteEntry(string name)
        {
            string readJson = File.ReadAllText(CurrentWorkingFile);//(@"C:\Development\Playground\secure.kh");

            var cleaner = File.CreateText(CurrentWorkingFile);//(@"C:\Development\Playground\secure.kh");
            cleaner.Flush();
            cleaner.Close();

            var desJson = JsonConvert.DeserializeObject<List<PasswordModel>>(readJson);
            var removableEntry = desJson.Where(x => x.Name.Equals(name)).FirstOrDefault();

            desJson.Remove(removableEntry);

            var json = JsonConvert.SerializeObject(desJson);

            using (var writer = new StreamWriter(CurrentWorkingFile, true))//(@"C:\Development\Playground\secure.kh", true))
            {
                writer.Write(json);
            }
        }

        // this need to be used somehow, to update the collection between different windows
        public static ObservableCollection<PasswordModel> UpdateCollection()
        {
            var readJson = File.ReadAllText(CurrentWorkingFile);//(@"C:\Development\Playground\secure.kh");
            var desJson = JsonConvert.DeserializeObject<ObservableCollection<PasswordModel>>(readJson);

            if (desJson == null)
            {
                desJson = new ObservableCollection<PasswordModel>();
            }

            return desJson;
        }

        public static string GetPassword(string name)
        {
            var readJson = File.ReadAllText(CurrentWorkingFile);//(@"C:\Development\Playground\secure.kh");
            var desJson = JsonConvert.DeserializeObject<ObservableCollection<PasswordModel>>(readJson);

            var pass = desJson.Where(x => x.Name.Equals(name)).Select(x => x.Password).FirstOrDefault();
            return pass;
        }

        public static void CheckIfSettingsExists()
        {
            var path = Path.GetDirectoryName(SettingsFileLocation);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!File.Exists(SettingsFileLocation))
            {
                var fs = File.Create(SettingsFileLocation);
                fs.Dispose();
            }
        }
    }
}
