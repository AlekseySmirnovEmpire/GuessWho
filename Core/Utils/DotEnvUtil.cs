using System.Text.RegularExpressions;

namespace Core.Utils;

public static class DotEnvUtil
{
    public static void Load(string pathString)
    {
        if (!File.Exists(pathString)) return;
        
        File.ReadAllLines(pathString)
            .Select(l => new Regex("""^([\w_]+)[\s]?=[\s"]?([^"]*)["]?$""")
                .Matches(l)
                .Select(m => m.Groups.Cast<Group>().Select(e => e.Value).Skip(1))
                .First()
                .ToList())
            .Where(e => e.Count == 2)
            .ToList()
            .ForEach(e => Environment.SetEnvironmentVariable(e.First(), e.Last()));
    }
}