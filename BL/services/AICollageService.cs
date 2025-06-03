using BL.InterfaceServices;
using DL.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BL.services
{
    public class AICollageService : IAICollageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly ILogger<AICollageService> _logger;

        public AICollageService(HttpClient httpClient, IConfiguration configuration, ILogger<AICollageService> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Groq_API_KEY"];
            _model = configuration["Groq:Model"] ?? "llama-3.1-8b-instant";
            _logger = logger;
        }

        public async Task<CollageDesignResponse> GenerateCollageDesignAsync(string userPrompt)
        {
            // שיפור משמעותי של ה-prompt
            var systemPrompt = @"
אתה מעצב קולאז'ים מקצועי שמחזיר רק JSON תקין. אל תחזיר שום טקסט מסביב ל-JSON.

המשתמש יתאר איך הוא רוצה שהקולאז' ייראה, ואתה תחזיר הגדרות בפורמט JSON בלבד:

{
  ""layout"": ""grid"",
  ""style"": ""modern"",
  ""imageCount"": 2,
  ""spacing"": 10,
  ""borderRadius"": 8,
  ""addText"": false,
  ""backgroundType"": ""hearts"",
  ""explanation"": ""הסבר קצר""
}

חוקים חשובים:
1. אם המשתמש מבקש מספר תמונות מסוים (למשל ""2 תמונות""), תן בדיוק את המספר הזה ב-imageCount.
2. אם המשתמש מבקש רקע מסוים, השתמש בערך המתאים ב-backgroundType:
   - ""לבבות"" -> backgroundType: ""hearts""
   - ""כוכבים"" -> backgroundType: ""stars""
   - ""נקודות"" -> backgroundType: ""dots""
   - ""גרדיאנט"" -> backgroundType: ""gradient""
   - אחרת -> backgroundType: ""solid""

דוגמאות:
1. אם המשתמש כותב: ""צור קולאז' עם 2 תמונות ורקע של לבבות""
   תחזיר: {""layout"":""grid"",""style"":""modern"",""imageCount"":2,""spacing"":10,""borderRadius"":8,""addText"":false,""backgroundType"":""hearts"",""explanation"":""קולאז' עם 2 תמונות ורקע לבבות""}

2. אם המשתמש כותב: ""3 תמונות עם כוכבים""
   תחזיר: {""layout"":""grid"",""style"":""modern"",""imageCount"":3,""spacing"":10,""borderRadius"":8,""addText"":false,""backgroundType"":""stars"",""explanation"":""קולאז' עם 3 תמונות ורקע כוכבים""}

3. אם המשתמש כותב: ""קולאז' עם נקודות""
   תחזיר: {""layout"":""grid"",""style"":""modern"",""imageCount"":4,""spacing"":10,""borderRadius"":8,""addText"":false,""backgroundType"":""dots"",""explanation"":""קולאז' עם רקע נקודות""}

חשוב מאוד: החזר רק את ה-JSON, ללא טקסט נוסף לפני או אחרי!";

            try
            {
                // בדיקה מקדימה - אם יש מילות מפתח ברורות, נשתמש בהן ישירות
                var fallbackResponse = CreateFallbackResponse(userPrompt);

                // אם זיהינו בקשה פשוטה, נשתמש בה ישירות
                if (fallbackResponse != null)
                {
                    _logger.LogInformation($"Using direct fallback for prompt: {userPrompt}");
                    _logger.LogInformation($"Fallback response: {JsonSerializer.Serialize(fallbackResponse)}");
                    return fallbackResponse;
                }

                var requestBody = new
                {
                    model = _model,
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userPrompt }
                    },
                    temperature = 0.0, // הורדה ל-0 לקבלת תוצאות עקביות
                    max_tokens = 200
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Groq Response Status: {response.StatusCode}");
                _logger.LogInformation($"Groq Response Content: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Groq API Error: {responseContent}");
                    return fallbackResponse ?? CreateDefaultResponse();
                }

                var groqResponse = JsonSerializer.Deserialize<GroqResponse>(responseContent);
                var aiContent = groqResponse?.choices?[0]?.message?.content;

                if (string.IsNullOrEmpty(aiContent))
                {
                    _logger.LogWarning("Empty response from Groq");
                    return fallbackResponse ?? CreateDefaultResponse();
                }

                _logger.LogInformation($"AI Content: {aiContent}");

                // נסה לחלץ JSON מהתשובה
                var jsonMatch = Regex.Match(aiContent, @"\{.+\}", RegexOptions.Singleline);

                if (jsonMatch.Success)
                {
                    var jsonPart = jsonMatch.Value;
                    _logger.LogInformation($"Extracted JSON: {jsonPart}");

                    try
                    {
                        var result = JsonSerializer.Deserialize<CollageDesignResponse>(jsonPart, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        // וידוא שהתוצאה תקינה
                        if (result != null)
                        {
                            _logger.LogInformation($"Parsed Result - ImageCount: {result.ImageCount}, BackgroundType: {result.BackgroundType}");

                            // וידוא שיש לפחות 2 תמונות
                            if (result.ImageCount < 2)
                                result.ImageCount = 2;

                            return result;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error parsing JSON: {ex.Message}");
                    }
                }

                _logger.LogWarning("Failed to extract valid JSON from AI response");
                return fallbackResponse ?? CreateDefaultResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GenerateCollageDesignAsync: {ex.Message}");
                return CreateFallbackResponse(userPrompt) ?? CreateDefaultResponse();
            }
        }

        // מערכת גיבוי משופרת שמזהה בקשות נפוצות
        private CollageDesignResponse CreateFallbackResponse(string userPrompt)
        {
            // זיהוי פשוט של הבקשה
            int? imageCount = null;
            string backgroundType = null;

            // המרה לאותיות קטנות לזיהוי טוב יותר
            string prompt = userPrompt.ToLower();

            // חיפוש מספר תמונות
            var numberMatch = Regex.Match(prompt, @"(\d+)\s*תמונות");
            if (numberMatch.Success && int.TryParse(numberMatch.Groups[1].Value, out int count))
            {
                imageCount = count;
            }
            else if (prompt.Contains("שתי תמונות") || prompt.Contains("2 תמונות"))
            {
                imageCount = 2;
            }
            else if (prompt.Contains("כל התמונות"))
            {
                // אם המשתמש מבקש את כל התמונות, נשתמש במספר גבוה
                // בפועל זה יוגבל למספר התמונות הזמינות בקוד הקליינט
                imageCount = 12;
            }

            // חיפוש סוג רקע - שיפור הזיהוי
            if (prompt.Contains("לבבות") || prompt.Contains("לב"))
                backgroundType = "hearts";
            else if (prompt.Contains("כוכבים") || prompt.Contains("כוכב"))
                backgroundType = "stars";
            else if (prompt.Contains("נקודות") || prompt.Contains("נקודה"))
                backgroundType = "dots";
            else if (prompt.Contains("גרדיאנט") || prompt.Contains("מדורג"))
                backgroundType = "gradient";

            // בדיקה נוספת לנקודות - זיהוי מדויק יותר
            if (prompt.Contains("רקע של נקודות") || prompt.Contains("רקע נקודות") ||
                prompt.Contains("עם נקודות") || prompt.Contains("נקודות ברקע"))
            {
                backgroundType = "dots";
                _logger.LogInformation("Detected dots background pattern in prompt");
            }

            // אם זיהינו לפחות אחד מהפרמטרים, נחזיר תשובה
            if (imageCount.HasValue || backgroundType != null)
            {
                var response = new CollageDesignResponse
                {
                    Layout = "grid",
                    Style = "modern",
                    ImageCount = imageCount ?? 4,
                    Spacing = 10,
                    BorderRadius = 8,
                    AddText = false,
                    BackgroundType = backgroundType ?? "solid",
                    Explanation = $"יצרתי קולאז' עם {imageCount ?? 4} תמונות ורקע {GetBackgroundTypeHebrew(backgroundType ?? "solid")}"
                };

                _logger.LogInformation($"Fallback created response with imageCount: {response.ImageCount}, backgroundType: {response.BackgroundType}");
                return response;
            }

            return null;
        }

        // המרת סוג רקע לעברית להסבר
        private string GetBackgroundTypeHebrew(string backgroundType)
        {
            switch (backgroundType)
            {
                case "hearts": return "לבבות";
                case "stars": return "כוכבים";
                case "dots": return "נקודות";
                case "gradient": return "גרדיאנט";
                default: return "רגיל";
            }
        }

        private CollageDesignResponse CreateDefaultResponse()
        {
            return new CollageDesignResponse
            {
                Layout = "grid",
                Style = "modern",
                ImageCount = 4,
                Spacing = 10,
                BorderRadius = 8,
                AddText = false,
                BackgroundType = "solid",
                Explanation = "יצרתי קולאז' בסיסי"
            };
        }

        private class GroqResponse
        {
            public Choice[] choices { get; set; }
        }

        private class Choice
        {
            public Message message { get; set; }
        }

        private class Message
        {
            public string content { get; set; }
        }
    }
}
