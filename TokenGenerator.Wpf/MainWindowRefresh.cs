using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Linq;
using System.Collections.ObjectModel;

namespace TokenGenerator.Wpf
{
    public partial class MainWindow : Window
    {
        private void UpdateData()
        {
            foreach (var path in paths)
            {
                File.WriteAllText(path + "config.json", JsonConvert.SerializeObject(environmentData));
            }
        }

        bool cmbBoxCodeTrigger = false;

        private void LoadViewData()
        {
            var selected = EnvironmentConfig.Text;
            if (EnvironmentConfig.SelectedIndex < 0)
            {
                ObservableCollection<string> list = (ObservableCollection<string>)EnvironmentConfig.ItemsSource;
                cmbBoxCodeTrigger = true;
                EnvironmentConfig.SelectedIndex = list.IndexOf(selected);
                cmbBoxCodeTrigger = false;
            }

            CryptographicKey.Text = environmentData[EnvironmentConfig.SelectedIndex].CryptoKey;
            ValidIssuer.Text = environmentData[EnvironmentConfig.SelectedIndex].ValidIssuer;
            ValidAudience.Text = environmentData[EnvironmentConfig.SelectedIndex].ValidAudience;
            ExpiresInSeconds.Text = environmentData[EnvironmentConfig.SelectedIndex].ExpirationInSeconds.ToString();
            NotBefore.Text = environmentData[EnvironmentConfig.SelectedIndex].NotBeforeInSeconds.ToString();
            Claims.Text = JsonConvert.SerializeObject(environmentData[EnvironmentConfig.SelectedIndex].StringClaims);
        }

        private EnvironmentData GetDataModel(int selectedIndex)
        {
            EnvironmentData newEnvironmentData = new EnvironmentData();
            newEnvironmentData.DisplayName = EnvironmentConfig.Text;
            newEnvironmentData.Id = selectedIndex;
            newEnvironmentData.CryptoKey = CryptographicKey.Text;
            newEnvironmentData.ValidIssuer = ValidIssuer.Text;
            newEnvironmentData.ValidAudience = ValidAudience.Text;
            newEnvironmentData.ExpirationInSeconds = Int32.Parse(ExpiresInSeconds.Text);
            newEnvironmentData.NotBeforeInSeconds = Int32.Parse(NotBefore.Text);
            newEnvironmentData.StringClaims = JsonConvert.DeserializeObject<List<Claims>>(Claims.Text);
            return newEnvironmentData;
        }
    }
}
