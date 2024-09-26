// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.ExternalEntities.STR
{
    public abstract class STRWrapper : ExternalEntityDef
    {
        protected static readonly string eeKeyLetter = "STR";
        public string EEKey { get { return eeKeyLetter; } }

        protected Logger logger;
        public Logger Logger { get { return logger; } set { logger = value; } }

        public IList<string> ConfigurationKeys { get { return new List<string>(); } }

        public void Initialize(IDictionary<string, object> configuration)
        {
            ;
        }

        public abstract int charindex(string expressionToFind, string expressionToSearch, int startLocation);
        public abstract string concat(string expression1, string expression2);
        public abstract string left(string expression, int length);
        public abstract int len(string expression);
        public abstract string lower(string expression);
        public abstract string ltrim(string expression);
        public abstract string match(string pattern, string expression);
        public abstract string replace(string expression, string pattern, string replacement);
        public abstract string replicate(string expression, int round);
        public abstract string reverse(string expression);
        public abstract string right(string expression, int length);
        public abstract string rtrim(string expression);
        public abstract string substring(string expression, int start, int length);
        public abstract string trim(string expression);
        public abstract string upper(string expression);
        public abstract bool tobool(string expression);
        public abstract int tointeger(string expression);
        public abstract double toreal(string expression);
        public abstract DateTime todate(string expression);
        public abstract string frombool(bool value);
        public abstract string frominteger(int value);
        public abstract string fromreal(double value);
        public abstract string fromdate(DateTime value, string format);
        public abstract string newguid();

    }
}
