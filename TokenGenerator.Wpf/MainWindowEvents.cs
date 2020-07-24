using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace TokenGenerator.Wpf
{
    public partial class MainWindow : Window
    {
        private void CreateKeyBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                var stringChars = new char[64];
                var random = new Random();
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = (char)chars[random.Next(chars.Length)];
                }

                var finalString = new String(stringChars);
                CryptographicKey.Text = Convert.ToBase64String(Encoding.ASCII.GetBytes(finalString));
            }
        }

        private void EnvironmentConfig_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ErrorLabel.Content = "";
            if (EnvironmentConfig.SelectedIndex > -1 && !cmbBoxCodeTrigger)
            {
                LoadViewData();
            }
        }

        private void SaveEnvBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // create new record
                if (!environmentData.Any(o => o.DisplayName == EnvironmentConfig.Text))
                {
                    var selectedIndex = environmentData.Max(x => x.Id) + 1;

                    EnvironmentData newEnvironmentData = GetDataModel(selectedIndex);

                    environmentData.Add(newEnvironmentData);
                    comboBoxList.Add(newEnvironmentData.DisplayName);
                    UpdateData();
                }
                // update existing record
                else
                {
                    MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Override Confirmation", System.Windows.MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        var selectedIndex = EnvironmentConfig.SelectedIndex;

                        EnvironmentData newEnvironmentData = GetDataModel(selectedIndex);

                        var updateObject = environmentData.FirstOrDefault(o => o.DisplayName == EnvironmentConfig.Text);
                        environmentData.Remove(updateObject);
                        environmentData.Insert(EnvironmentConfig.SelectedIndex, newEnvironmentData);

                        UpdateData();
                    }
                }
            }
            catch (Exception exc)
            {
                ErrorLabel.Content = "Can't save given data... " + exc.Message;
            }

            LoadViewData();
        }

        

        private void DeleteEnvBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    string selected = EnvironmentConfig.SelectedItem.ToString();
                    EnvironmentConfig.SelectedIndex = 0;

                    var data = environmentData.FirstOrDefault(o => o.DisplayName == selected);

                    environmentData.Remove(data);
                    comboBoxList.Remove(selected);

                    UpdateData();
                }
            }
            catch (Exception exc)
            {
                ErrorLabel.Content = "Can't delete given data... " + exc.Message;
            }

        }



        private void ClearTokenBtn_Click(object sender, RoutedEventArgs e)
        {
            TokenClaims.Text = "";
            TokenResult.Text = "";
        }

        private void CreateTokenBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ErrorLabel.Content = "";
                var result = _jwtService.GenerateJSONWebToken(CryptographicKey.Text, Claims.Text, ValidAudience.Text, ValidIssuer.Text, NotBefore.Text, ExpiresInSeconds.Text);
                TokenResult.Text = result;
            }
            catch (Exception)
            {
                ErrorLabel.Content = "Can't create given token...";
            }

        }

        private void ValidateTokenBtn_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";
            try
            {
                if (TokenResult.Text != null || TokenResult.Text != "")
                {
                    TokenClaims.Text = _jwtService.GetClaims(_jwtService.ValidateToken(TokenResult.Text, ValidAudience.Text, ValidIssuer.Text, CryptographicKey.Text) as ClaimsPrincipal);
                }
            }
            catch (Exception exc)
            {
                ErrorLabel.Content = "Can't validate given token... " + exc.Message;
            }

        }
    }
}
