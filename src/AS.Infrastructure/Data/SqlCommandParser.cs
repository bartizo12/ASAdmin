using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AS.Infrastructure.Data
{
    /// <summary>
    /// Helper class to parse SqlCommands from sql script files.
    /// Works for both MySql scripts and MsSql scripts. Other database scripts can be added
    /// </summary>
    internal sealed class SqlCommandParser
    {
        private string _scriptPath;
        private string _eoc; //End of command

        internal SqlCommandParser(string scriptPath, string endOfCommand)
        {
            this._scriptPath = scriptPath;
            this._eoc = endOfCommand;
        }

        internal string[] ParseFromFile(bool throwExceptionIfNonExists)
        {
            if (!File.Exists(_scriptPath))
            {
                if (throwExceptionIfNonExists)
                    throw new FileNotFoundException(string.Format("Specified file doesn't exist - {0}", _scriptPath));

                return new string[0];
            }
            var statements = new List<string>();

            using (var reader = new StreamReader(File.OpenRead(_scriptPath)))
            {
                string statement;
                while ((statement = ReadNextStatementFromStream(reader, _eoc)) != null)
                {
                    statements.Add(statement);
                }
            }

            return statements.ToArray();
        }

        private string ReadNextStatementFromStream(StreamReader reader, string eocStr)
        {
            var sb = new StringBuilder();

            while (true)
            {
                var lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();

                    return null;
                }

                if (lineOfText.TrimEnd().ToUpper() == eocStr)
                    break;

                sb.Append(lineOfText + Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}