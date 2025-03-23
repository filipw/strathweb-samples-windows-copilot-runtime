using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using App2.Services;
using Windows.UI;
using Microsoft.UI;

namespace App2
{
    /// <summary>
    /// Main window for the medical condition classifier application.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly MedicalClassificationService _classificationService;

        public MainWindow()
        {
            this.InitializeComponent();
            _classificationService = new MedicalClassificationService();
        }

        private async void ClassifyButton_Click(object sender, RoutedEventArgs e)
        {
            string condition = ConditionTextBox.Text.Trim();
            
            if (string.IsNullOrEmpty(condition))
            {
                ResultTextBlock.Text = "Please enter a medical condition to classify.";
                return;
            }

            try
            {
                LoadingIndicator.IsActive = true;
                ClassifyButton.IsEnabled = false;
                ResultTextBlock.Text = "Classifying...";

                string classification = await _classificationService.ClassifyMedicalConditionAsync(condition);
                
                ResultTextBlock.Text = classification;
                
                switch (classification.ToLower())
                {
                    case "doctor_required":
                        ResultTextBlock.Text = "Doctor Required - This appears to be a serious condition that requires medical attention.";
                        ResultTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                        break;
                    case "pharmacist_required":
                        ResultTextBlock.Text = "Pharmacist Required - This appears to be a mild condition that may be treated with over-the-counter medication.";
                        ResultTextBlock.Foreground = new SolidColorBrush(Colors.Orange);
                        break;
                    case "rest_required":
                        ResultTextBlock.Text = "Rest Required - This appears to be general tiredness or minor discomfort that may improve with rest.";
                        ResultTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                        break;
                    case "unknown":
                        ResultTextBlock.Text = "Unknown - Unable to classify the condition based on the provided information.";
                        ResultTextBlock.Foreground = new SolidColorBrush(Colors.Gray);
                        break;
                    default:
                        ResultTextBlock.Text = $"Unexpected response from the model: {ResultTextBlock.Text}";
                        ResultTextBlock.Foreground = new SolidColorBrush(Colors.White);
                        break;
                }
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Error: {ex.Message}";
            }
            finally
            {
                LoadingIndicator.IsActive = false;
                ClassifyButton.IsEnabled = true;
            }
        }
    }
}
