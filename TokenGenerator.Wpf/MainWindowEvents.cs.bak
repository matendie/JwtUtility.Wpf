using System;
using System.Linq;
using System.Security.Claims;
using System.Windows;
using System.Windows.Controls;

namespace TokenGenerator.Wpf
{
    public partial class MainWindow : Window
    {
        private void ValidateIssuerChk_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
            {

        }

        private void ValidateExpirationChk_Checked(object sender, RoutedEventArgs e)
                {

                }

        private void UseSecureTokenChk_Checked(object sender, RoutedEventArgs e)
        {
            SecurityKeyBtn.Visibility = Visibility.Visible;
            CreateSecureTokenBtn.Visibility = Visibility.Visible;
            ValidateSecureTokenBtn.Visibility = Visibility.Visible;
            SecurityKey.Visibility = Visibility.Visible;
            SecurityKeyLbl.Visibility = Visibility.Visible;
            CreateTokenBtn.Visibility = Visibility.Hidden;
            ValidateTokenBtn.Visibility = Visibility.Hidden;
        }

        private void UseSecureTokenChk_Unchecked(object sender, RoutedEventArgs e)
        {
            SecurityKeyBtn.Visibility = Visibility.Hidden;
            CreateSecureTokenBtn.Visibility = Visibility.Hidden;
            ValidateSecureTokenBtn.Visibility = Visibility.Hidden;
            SecurityKey.Visibility = Visibility.Hidden;
            SecurityKeyLbl.Visibility = Visibility.Hidden;

            CreateTokenBtn.Visibility = Visibility.Visible;
            ValidateTokenBtn.Visibility = Visibility.Visible;
        }
 
        private void CreateKeyBtn_Click(object sender, RoutedEventArgs e)
        {
            CryptographicKey.Text = CipherService.GenerateSecureKey(256 * 2);
            }

        private void SecurityKeyBtn_Click(object sender, RoutedEventArgs e)
        {
            SecurityKey.Text = CipherService.GenerateSecureKey(512 * 2);
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
            catch (Exception exc)
            {
                ErrorLabel.Content = "Can't create given token..." + exc.Message;
            }

        }

        private void ValidateTokenBtn_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";
            TokenClaims.Text = "";
            try
            {
                if (TokenResult.Text != null || TokenResult.Text != "")
                {
                    TokenClaims.Text = _jwtService.GetClaims(_jwtService.ValidateToken(TokenResult.Text, ValidAudience.Text, ValidIssuer.Text, CryptographicKey.Text,
                                                        ValidateAudienceChk.IsChecked, ValidateIssuerChk.IsChecked, ValidateExpirationChk.IsChecked) as ClaimsPrincipal);
                }
            }
            catch (Exception exc)
            {
                ErrorLabel.Content = "Can't validate given token... " + exc.Message;
            }

        }

        private void CreateSecureTokenBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ErrorLabel.Content = "";
                var result = _jwtService.GenerateJSONSecureWebToken(CryptographicKey.Text, SecurityKey.Text, Claims.Text, ValidAudience.Text, ValidIssuer.Text, NotBefore.Text, ExpiresInSeconds.Text);
                TokenResult.Text = result;
            }
            catch (Exception exc)
            {                
                ErrorLabel.Content = "Can't create given token..." + exc.Message;
            }
        }

        private void ValidateSecureTokenBtn_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";
            TokenClaims.Text = "";
            try
            {
                if (TokenResult.Text != null || TokenResult.Text != "")
                {
                    TokenClaims.Text = _jwtService.GetClaims(_jwtService.ValidateSecureToken(TokenResult.Text, ValidAudience.Text, ValidIssuer.Text, CryptographicKey.Text, SecurityKey.Text,
                                                        ValidateAudienceChk.IsChecked, ValidateIssuerChk.IsChecked, ValidateExpirationChk.IsChecked) as ClaimsPrincipal);
                }
            }
            catch (Exception exc)
            {
                ErrorLabel.Content = "Can't validate given token... " + exc.Message;
            }
        }
    }
}
