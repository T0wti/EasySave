namespace EasySave.Application.Utils;

public static class BackupIdParser
{
    public static IEnumerable<int> ParseIds(string arg)
    {
        // Remove any whitespace to ensure robust parsing
        arg = arg.Replace(" ", "");

        // Case 1: Handle a range of IDs (ex: "1-5")
        if (arg.Contains('-'))
        {
            var parts = arg.Split('-');

            int start = int.Parse(parts[0]);
            int end = int.Parse(parts[1]);

            if (start > end)
                throw new Exception("Invalid range");

            // Generate all integers between start and end
            return Enumerable.Range(start, end - start + 1);
        }

        // Case 2: Handle a semicolon-separated list (ex: "1;3;7")
        if (arg.Contains(';'))
        {
            return arg.Split(';')
                      .Select(int.Parse)
                      .Distinct();
        }
        // Case 3: Handle a single ID (ex: "1")
        return new[] { int.Parse(arg) };
    }
}


