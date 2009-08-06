using System.Collections.Generic;
using System.Text;

namespace ExportDatabaseObjects
{
    public class FileSet
    {
        public void Include(string pattern)
        {
            if (!includes.Contains(pattern)) includes.Add(pattern);
            matcher = null;
        }
        public void Exclude(string pattern)
        {
            if (!excludes.Contains(pattern)) excludes.Add(pattern);
            matcher = null;
        }

        public FileSetMatcher GetMatcher()
        {
            if (matcher == null) matcher = new FileSetMatcher(this);
            return matcher;
        }

        public class FileSetMatcher
        {
            System.Text.RegularExpressions.Regex regexInclude;
            System.Text.RegularExpressions.Regex regexExclude;
            internal FileSetMatcher(FileSet fileSet)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string pattern in fileSet.includes)
                {
                    if (sb.Length > 0) sb.Append("|");
                    sb.Append("(" + pattern + ")");
                }
                if (sb.Length > 0) regexInclude = new System.Text.RegularExpressions.Regex(sb.ToString(), System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);
                sb.Remove(0, sb.Length);
                foreach (string pattern in fileSet.excludes)
                {
                    if (sb.Length > 0) sb.Append("|");
                    sb.Append("(" + pattern + ")");
                }
                if (sb.Length > 0) regexExclude = new System.Text.RegularExpressions.Regex(sb.ToString(), System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);
            }
            public bool IsMatch(string name)
            {
                return (IsIncluded(name) && !IsExcluded(name));
            }
            bool IsIncluded(string name)
            {
                return (regexInclude == null || regexInclude.IsMatch(name));
            }
            bool IsExcluded(string name)
            {
                return (regexExclude != null && regexExclude.IsMatch(name));
            }
        }

        List<string> includes = new List<string>();
        List<string> excludes = new List<string>();
        FileSetMatcher matcher = null;
    }
}