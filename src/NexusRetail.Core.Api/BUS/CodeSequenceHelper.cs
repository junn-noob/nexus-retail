using System.Text.RegularExpressions;

namespace NexusRetail.Core.Api.BUS;

/// <summary>
/// Bộ sinh mã định danh tuần tự dùng chung cho các nghiệp vụ (SPxx, KHxx, HDBxx, HDNxx, NHxx).
/// </summary>
internal static class CodeSequenceHelper
{
    public static string GenerateNextCode(IEnumerable<string> existingCodes, string prefix)
    {
        int maxNumber = 0;
        string pattern = $@"^{Regex.Escape(prefix)}(\d+)$";

        foreach (var code in existingCodes)
        {
            if (string.IsNullOrWhiteSpace(code)) continue;
            var match = Regex.Match(code.Trim(), pattern, RegexOptions.IgnoreCase);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int num) && num < 10000 && num > maxNumber)
            {
                maxNumber = num;
            }
        }

        return $"{prefix}{maxNumber + 1:D2}";
    }
}

