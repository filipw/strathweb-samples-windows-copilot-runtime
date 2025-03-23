using Microsoft.Windows.AI.ContentModeration;
using Microsoft.Windows.AI.Generative;
using System;
using System.Threading.Tasks;

namespace App2.Services
{
    public class MedicalClassificationService
    {
        private readonly string _classificationPrompt = @"You are a medical classification engine for health conditions. Classify the prompt into into one of the following possible treatment options: doctor_required (serious condition), pharmacist_required (light condition) or rest_required (general tiredness). If you cannot classify the prompt, output unknown. 
Only respond with the single word classification. Do not produce any additional output. Do not use quotation marks in the response.

# Examples:
User: I did not sleep well. Assistant: rest_required
User: I might have broken an arm. Assistant: doctor_required

# Task
User: ";

        public async Task<string> ClassifyMedicalConditionAsync(string condition)
        {
            try
            {
                if (App.LanguageModel == null)
                {
                    return "AI model not initialized";
                }

                var prompt = $"{_classificationPrompt}{condition} Assistant: ";

                var result = await App.LanguageModel.GenerateResponseAsync(prompt);
                return result.Response.Trim();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
