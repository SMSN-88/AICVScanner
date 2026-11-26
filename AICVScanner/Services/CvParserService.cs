using Microsoft.AspNetCore.Components.Forms;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace AICVScanner.Services;

public class CvParserService
{
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public async Task<string> ExtractTextAsync(IBrowserFile file)
    {
        var extension = Path.GetExtension(file.Name).ToLowerInvariant();

        if (extension != ".pdf" && extension != ".txt")
            throw new NotSupportedException(
                $"File type {extension} is not supported. Please upload a PDF or TXT file.");

        // Copy the upload into memory using an async read
        using var memoryStream = new MemoryStream();
        await using (var uploadStream = file.OpenReadStream(maxAllowedSize: MaxFileSize))
        {
            await uploadStream.CopyToAsync(memoryStream);
        }
        memoryStream.Position = 0;

        return extension == ".pdf"
            ? ExtractTextFromPdf(memoryStream)
            : ExtractTextFromTxt(memoryStream);
    }

    private static string ExtractTextFromPdf(Stream pdfStream)
    {
        using var document = PdfDocument.Open(pdfStream);
        var text = new System.Text.StringBuilder();

        foreach (Page page in document.GetPages())
            text.AppendLine(page.Text);

        return text.ToString().Trim();
    }

    private static string ExtractTextFromTxt(Stream txtStream)
    {
        using var reader = new StreamReader(txtStream);
        return reader.ReadToEnd().Trim();
    }
}