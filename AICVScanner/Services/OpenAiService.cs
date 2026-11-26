using System.Text;
using System.Text.Json;

namespace CVScanner.Web.Services;

public class ScanAnalysis
{
    public int MatchScore { get; set; }
    public int AtsScore { get; set; }
    public List<string> MissingSkills { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    public Dictionary<string, int> KeywordDensity { get; set; } = new();
    public string Summary { get; set; } = string.Empty;
}

public class OpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAiService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API key not configured.");
    }

    public async Task<ScanAnalysis> AnalyseCvAgainstJobAsync(string cvText, string jobDescription)
    {
        var promptTemplate = """
    You are an expert ATS (Applicant Tracking System) and CV analyst.

    Analyse the following CV against the job description provided. Return ONLY a valid JSON object with no additional text, no markdown, no code fences.

    CV:
    [[CV_TEXT]]

    Job Description:
    [[JOB_DESCRIPTION]]

    Return this exact JSON structure:
    {
      "matchScore": <integer 0-100>,
      "atsScore": <integer 0-100>,
      "missingSkills": [<list of skills/keywords in job description not found in CV>],
      "suggestions": [<list of specific actionable CV improvements>],
      "keywordDensity": {<important job keywords as keys, frequency in CV as integer values>},
      "summary": "<2-3 sentence overall assessment>"
    }
    """;

        var prompt = promptTemplate
            .Replace("[[CV_TEXT]]", cvText)
            .Replace("[[JOB_DESCRIPTION]]", jobDescription);
        var requestBody = new
        {
            model = "gpt-4o-mini",
            max_tokens = 1500,
            messages = new[]
            {
                new { role = "system", content = "You are a professional CV and ATS analyst. Always respond with valid JSON only." },
                new { role = "user", content = prompt }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);

        var messageContent = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "{}";

        // Strip any accidental markdown code fences
        messageContent = messageContent.Trim();
        if (messageContent.StartsWith("```"))
        {
            messageContent = messageContent.Split('\n').Skip(1).TakeWhile(l => !l.StartsWith("```")).Aggregate((a, b) => a + "\n" + b);
        }

        var analysis = JsonSerializer.Deserialize<ScanAnalysis>(messageContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new ScanAnalysis();

        return analysis;
    }
}