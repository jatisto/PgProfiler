using System.Text;
using System.Text.RegularExpressions;
using PgProfile.Service.Interface;
using PgProfile.Service.Model;

namespace PgProfile.Service.FactoryMethod.ParsingSingleFile
{
    public class ParsingDebugFile : IParsingFactory<PgLogFileModel>
    {
        public List<PgLogFileModel> RunParsing(string? pathToFolder, DateTime? selectedDate, string? fileName = null, bool isDateFilter = false) =>
            ParsingSingleFileToContent(pathToFolder, selectedDate, fileName, isDateFilter);

        private List<PgLogFileModel> ParsingSingleFileToContent (string? pathToFolder, DateTime? selectedDate, string? fileName, bool isDateFilter = false)
        {
            var finResult = new List<PgLogFileModel>();

            if (pathToFolder == null || fileName == null) return new List<PgLogFileModel>();
            var input = CustomReadAllLines(Path.Combine(pathToFolder, fileName));

            const string pattern =
                @"^(?'dateTimeLog' \d{4}.\d{2}.\d{2}.\d{2}.\d{2}.\d{2}.\d{3})\s(?'zone' \+\d{2})\s(?'codeLog' \[\d+\])\s(?'typeOperator' \w+):\s{2}(?'content' .+?)(?= ^\d{4} | \Z)";

            const RegexOptions options =
                RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace;

            var matches = Regex.Matches(input!, pattern, options);

            foreach (Match match in matches)
            {
                var groups = match.Groups;

                var data = new PgLogFileModel()
                {
                    DateTimeLog = DateTime.Parse(groups["dateTimeLog"].Value),
                    Zone = groups["zone"].Value,
                    CodeLog = groups["codeLog"].Value,
                    TypeOperator = groups["typeOperator"].Value,
                    Content = groups["content"].Value,
                };

                finResult.Add(data);
            }

            if (isDateFilter)
            {
                finResult = finResult.Where(x => x.DateTimeLog.Date == selectedDate!.Value.Date).ToList();
            }
            return new List<PgLogFileModel>(finResult.OrderByDescending(x => x.DateTimeLog).Distinct());
        }


        /// <summary>
        /// Кастомный ReadAllLines
        /// </summary>
        /// <param name="path">Путь</param>
        private static string? CustomReadAllLines (string path)
        {
            using var csv = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var bytes = new byte[csv.Length];
            var numBytesToRead = (int) csv.Length;
            var numBytesRead = 0;
            while (numBytesToRead > 0)
            {
                var n = csv.Read(bytes, numBytesRead, numBytesToRead);
                if (n == 0)
                    break;
                numBytesRead += n;
                numBytesToRead -= n;
            }
            
            var strMessage = Encoding.Default.GetString(bytes.Where(x => x != 0).ToArray());
            return strMessage;
        }
    }
}