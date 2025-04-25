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

                // variant 1: defaults
                //var result = await App.LanguageModel.GenerateResponseAsync(prompt);

                // variant 2: more control
                var contentFilterOptions = new ContentFilterOptions();
                contentFilterOptions.PromptMaxAllowedSeverityLevel.SelfHarm = SeverityLevel.Low;
                contentFilterOptions.PromptMaxAllowedSeverityLevel.Violent = SeverityLevel.Low;

                var languageModelOptions = new LanguageModelOptions();
                languageModelOptions.Temperature = 0.0f;
                languageModelOptions.TopP = 0.1f;
                languageModelOptions.TopK = 1;
                languageModelOptions.ContentFilterOptions = contentFilterOptions;

                var result = await App.LanguageModel.GenerateResponseAsync(prompt, languageModelOptions);
                
                switch (result.Status)
                {
                    case LanguageModelResponseStatus.Complete:
                        return result.Text.Trim();
                    case LanguageModelResponseStatus.ResponseBlockedByContentModeration:
                        return "Error: Response blocked by content moderation";
                    case LanguageModelResponseStatus.PromptBlockedByContentModeration:
                        return "Error: Prompt Blocked by content moderation";
                    case LanguageModelResponseStatus.BlockedByPolicy:
                        return "Error: Blocked by runtime";
                    case LanguageModelResponseStatus.PromptLargerThanContext:
                        return "Error: Prompt larger than context";
                    default:
                        return "Error: Unknown status";
                }
            }
            catch (Exception ex)
            {
                return $"Unknown Error: {ex}";
            }
        }
    }
}
