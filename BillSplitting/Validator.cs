using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BillSplitting
{
    public static class Validator
    {
        private static readonly string _validNumericPattern = "[0-9.]";
        public static readonly string ValidEofCharacter = "0";
        public static bool ValidateDataFromInputFile(string[] values)
        {
            var val = values.Where(v => !Regex.IsMatch(v, _validNumericPattern));
            if (val.Any())
            {
                throw new InvalidDataException("The input file has non numeric characters.");
            }

            if (values[values.Length - 1] != ValidEofCharacter)
            {
                throw new InvalidDataException("The input file has no proper EOF expected.");
            }

            return true;
        }

        public static void VerifyInputParameter(string[] args)
        {
            if (args != null && args.Length != 0 && !string.IsNullOrEmpty(args[0]) &&
                !string.IsNullOrWhiteSpace(args[0])) return;
            throw new ArgumentNullException($"No argument has been passed.");
        }
    }
}
