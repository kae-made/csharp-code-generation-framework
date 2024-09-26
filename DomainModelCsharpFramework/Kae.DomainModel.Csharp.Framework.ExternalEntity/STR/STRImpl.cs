// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.ExternalEntities.STR
{
    public class STRImpl : STRWrapper
    {
        public override int charindex(string expressionToFind, string expressionToSearch, int startLocation)
        {
            return expressionToSearch.IndexOf(expressionToFind, startLocation);
        }

        public override string concat(string expression1, string expression2)
        {
            return expression1 + expression2;
        }

        public override string frombool(bool value)
        {
            return $"{value}";
        }

        public override string fromdate(DateTime value, string format)
        {
            return value.ToString(format);
        }

        public override string frominteger(int value)
        {
            return $"{value}";
        }

        public override string fromreal(double value)
        {
            return $"{value}";
        }

        public override string left(string expression, int length)
        {
            return expression.Substring(0,length);
        }

        public override int len(string expression)
        {
            return expression.Length;
        }

        public override string lower(string expression)
        {
            return expression.ToLower();
        }

        public override string ltrim(string expression)
        {
            return expression.TrimStart();
        }

        public override string newguid()
        {
            return Guid.NewGuid().ToString();
        }

        public override string match(string pattern, string expression)
        {
            var regex = new Regex(pattern);
            return regex.Match(expression).Value;
        }

        public override string replace(string expression, string pattern, string replacement)
        {
            var regex = new Regex(pattern);
            return regex.Replace(expression, replacement);
        }

        public override string replicate(string expression, int round)
        {
            string result = "";
            for(int i=0;i<round; i++)
            {
                result += expression;
            }
            return result;
        }

        public override string reverse(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                return "";
            }
            return expression.Reverse().ToString();
        }

        public override string right(string expression, int length)
        {
            if (expression.Length <= length)
            {
                return expression;
            }
            return expression.Substring(expression.Length - length);
        }

        public override string rtrim(string expression)
        {
            return expression.TrimEnd();
        }

        public override string substring(string expression, int start, int length)
        {
            return expression.Substring(start, length);
        }

        public override bool tobool(string expression)
        {
            return bool.Parse(expression.Trim());
        }

        public override DateTime todate(string expression)
        {
            return DateTime.Parse(expression.Trim());
        }

        public override int tointeger(string expression)
        {
            return int.Parse(expression.Trim());
        }

        public override double toreal(string expression)
        {
            return double.Parse(expression.Trim());
        }

        public override string trim(string expression)
        {
            return expression.Trim();
        }

        public override string upper(string expression)
        {
            return expression.ToUpper();
        }
    }
}
