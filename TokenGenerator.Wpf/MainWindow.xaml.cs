using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Windows;
using System.Windows.Controls;

namespace TokenGenerator.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private JwtService _jwtService { get; set; }
        public ObservableCollection<string> comboBoxList { get; private set; }
        private List<string> paths { get; set; }
        List<EnvironmentData> environmentData { get; set; }


        public MainWindow()
        {

            InitializeComponent();
            comboBoxList = new ObservableCollection<string>();
            _jwtService = new JwtService();

            paths = new List<string>();
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            paths.Add(path);

            if (path.Contains("bin"))
            {
                paths.Add(path.Replace("bin\\Debug\\netcoreapp3.1\\", ""));
            }

            using (StreamReader r = new StreamReader("config.json"))
            {
                string json = r.ReadToEnd();
                environmentData =  JsonConvert.DeserializeObject<List<EnvironmentData>>(json);
            }

            foreach (var data in environmentData)
            {
                comboBoxList.Add(data.DisplayName);
            }
             
            EnvironmentConfig.ItemsSource = comboBoxList;
            EnvironmentConfig.SelectedIndex = 0;

            SecurityKeyBtn.Visibility = Visibility.Hidden;
            CreateSecureTokenBtn.Visibility = Visibility.Hidden;
            ValidateSecureTokenBtn.Visibility = Visibility.Hidden;
            SecurityKey.Visibility = Visibility.Hidden;
            SecurityKeyLbl.Visibility = Visibility.Hidden;

            LoadViewData();
        }

        
    }
}
