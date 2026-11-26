using AICVScanner.Data;
using AICVScanner.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using CVScanner.Web.Services;

namespace AICVScanner.Services;

    public class ScanService
    {
        private readonly AppDbContext _db;
        private readonly CvParserService _parser;
        private readonly OpenAiService _openAi;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ScanService(AppDbContext db, CvParserService parser, OpenAiService openAi, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _parser = parser;
            _openAi = openAi;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ScanResult> RunScanAsync(string userId, IBrowserFile file, string jobDescription)
        {
            var extractText = await _parser.ExtractTextAsync(file);
            var cvDoc = new CvDocument
            {
                UserId = userId,
                FileName = file.Name,
                ExtractedText = extractText
            };
            _db.CvDocuments.Add(cvDoc);
            await _db.SaveChangesAsync();

            var analysis = await _openAi.AnalyseCvAgainstJobAsync(extractText, jobDescription);
            var scanResult = new ScanResult
            {
                UserId = userId,
                CvDocumentId = cvDoc.Id,
                JobDescription = jobDescription,
                MatchScore = analysis.MatchScore,
                AtsScore = analysis.AtsScore,
                MissingSkills = JsonSerializer.Serialize(analysis.MissingSkills),
                Suggestions = JsonSerializer.Serialize(analysis.Suggestions),
                KeywordAnalysis = JsonSerializer.Serialize(analysis.KeywordDensity),
            };
            _db.ScanResults.Add(scanResult);
            await _db.SaveChangesAsync();
            return scanResult;
        }

        public async Task<List<ScanResult>> GetUserScanAsync(string userId)
        {
            return await _db.ScanResults
                .Where(s => s.UserId == userId)
                .Include(s => s.CvDocument)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ScanResult>> GetUserScansAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                return new List<ScanResult>();
            }

            var appUser = await _userManager.GetUserAsync(user);
            if (appUser == null)
            {
                return new List<ScanResult>();
            }

            return await _db.ScanResults
                .Include(s => s.CvDocument)
                .Where(s => s.UserId == appUser.Id)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }
    }

