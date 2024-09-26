// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.Adaptor
{
    public abstract class DomainModelAdaptor
    {
        protected Logger logger;

        public DomainModelAdaptor(Logger logger)
        {
            this.logger = logger;
        }

        protected Dictionary<string, Dictionary<string, ParamSpec>> domainOpsParamSpecs;
        protected Dictionary<string, ClassSpec> classSpecs;
        protected static string domainModelName;

        public Dictionary<string, Dictionary<string, ParamSpec>> DomainOperationSpecs { get { return domainOpsParamSpecs; } }
        public Dictionary<string, ClassSpec> ClassSpecs { get { return classSpecs; } }
        public string DomainModelName { get { return domainModelName; } }

        public abstract IDictionary<string, IList<string>> ConfigurationKeys();
        public abstract void Initialize(IDictionary<string, IDictionary<string, object>> config);

        public string GetDataTypeName(int n)
        {
            return ((ParamSpec.DataType)Enum.ToObject(typeof(ParamSpec.DataType), n)).ToString();
        }

        public abstract string InvokeDomainOperation(string name, RequestingParameters parameters);
        public abstract string InvokeDomainClassOperation(string classKeyLett, string name,  RequestingParameters parameters);
        public abstract string SendEvent(string classKeyLett, string eventLabel, RequestingParameters parameters);
        public abstract string UpdateClassProperties(string classKeyLett, RequestingParameters parameters);
        public abstract string GetInstances(string classKeyLett);
        public abstract string GetInstance(string classKeyLett, IDictionary<string, string> identities);
        // relName ::= R relNumb [ 'txtPhrs' ]
        public abstract string GetLinkedInstances(string classKeyLett, IDictionary<string, string> identities, string relName);

        public abstract string GetDomainOperationsSpec();
        public abstract string GetClassesSpec();

        public abstract string GetDomainModelSpec();

        public abstract void LoadDomainInstances(string instances);
        public abstract string SaveDomainInstances();

        public abstract void RegisterUpdateHandler(ClassPropertiesUpdateHandler classPropertiesUpdateHandler, RelationshipUpdateHandler relationshipUpdateHandler);

        protected bool CheckProperties(IDictionary<string, PropSpec> propSpecs, RequestingParameters propUpdateParams, RequestingParameters invSpec)
        {
            bool valid = CheckIdentity(propSpecs, propUpdateParams, invSpec);
            if (valid)
            {
                foreach (var propKey in propUpdateParams.Parameters.Keys)
                {
                    if (propSpecs.ContainsKey(propKey))
                    {
                        var propSpec = propSpecs[propKey];
                        if (propSpec.Writable)
                        {
                            object pValue = null;
                            if (propUpdateParams.Parameters[propKey] is JsonElement)
                            {
                                var jeP = (JsonElement)propUpdateParams.Parameters[propKey];
                                pValue = XFromJsonToObj(ref valid, propSpec.DataType, jeP);
                            }
                            else
                            {
                                pValue = propUpdateParams.Parameters[propKey];
                            }
                            if (pValue != null && valid)
                            {
                                invSpec.Parameters.Add(propKey, pValue);
                            }
                        }
                    }
                    else
                    {
                        valid = false;
                        break;
                    }
                }
            }
            return valid;
        }

        protected bool CheckIdentity(IDictionary<string,PropSpec> propSpecs, RequestingParameters opInvocation, RequestingParameters invSpec)
        {
            bool valid = true;
            var idPropSpecs = new List< PropSpec>();
            foreach(var pk in propSpecs.Keys)
            {
                var p = propSpecs[pk];
                if (p.Identity == 1)
                {
                    idPropSpecs.Add(p);
                }
            }
            int paramCount = idPropSpecs.Count;
            foreach (var opIdKey in opInvocation.Identities.Keys)
            {
                var idCandidates = idPropSpecs.Where(i => (i.Name == opIdKey));
                valid = idCandidates.Count() > 0;
                if (valid)
                {
                    invSpec.Identities.Add(opIdKey, opInvocation.Identities[opIdKey]);
                    paramCount--;
                }
                else
                {
                    break;
                }
            }
            if (paramCount != 0)
            {
                valid = false;
            }

            return valid;
        }
        protected bool CheckParameters(IDictionary<string, ParamSpec> paramSpecs, RequestingParameters opInvocation, RequestingParameters invSpec)
        {
            bool valid = true;
            int paramSpecNo = paramSpecs.Count;
            foreach (var pk in opInvocation.Parameters.Keys)
            {
                valid = paramSpecs.ContainsKey(pk);
                if (valid)
                {
                    var paramSpec = paramSpecs[pk];
                    object pValue = null;
                    if (opInvocation.Parameters[pk] is JsonElement)
                    {
                        var jeP = (JsonElement)opInvocation.Parameters[pk];
                        pValue = XFromJsonToObj(ref valid, paramSpec.TypeKind, jeP);
                    }
                    else
                    {
                        pValue = opInvocation.Parameters[pk];
                    }
                    if (pValue != null)
                    {
                        invSpec.Parameters.Add(pk, pValue);
                    }
                }
                if (valid)
                {
                    paramSpecNo--;
                }
                else
                {
                    break;
                }
            }
            if (paramSpecNo > 0)
            {
                valid = false;
            }

            return valid;
        }

        public object XFromJsonToObj(ref bool valid, ParamSpec.DataType typeKind, JsonElement jeP)
        {
            object pValue = null;
            switch (jeP.ValueKind)
            {
                case JsonValueKind.String:
                    if (typeKind == ParamSpec.DataType.String)
                    {
                        pValue = jeP.GetString();
                    }
                    else
                    {
                        valid = false;
                    }
                    break;
                case JsonValueKind.Number:
                    int v32;
                    long v64;
                    double d;
                    if (jeP.TryGetInt32(out v32))
                    {
                        if (typeKind == ParamSpec.DataType.Integer)
                        {
                            pValue = v32;
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                    else if (jeP.TryGetInt64(out v64))
                    {
                        if (typeKind == ParamSpec.DataType.Integer)
                        {
                            pValue = v64;
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                    else if (jeP.TryGetDouble(out d))
                    {
                        if (typeKind == ParamSpec.DataType.Real)
                        {
                            pValue = d;
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                    break;
                case JsonValueKind.True:
                    if (typeKind == ParamSpec.DataType.Boolean)
                    {
                        pValue = true;
                    }
                    else
                    {
                        valid = false;
                    }
                    break;
                case JsonValueKind.False:
                    if (typeKind == ParamSpec.DataType.Boolean)
                    {
                        pValue = false;
                    }
                    else
                    {
                        valid = false;
                    }
                    break;
#if false
                case JsonValueKind.Array:
                    if (paramSpec.IsArray)
                    {
                        var arrayValue = new List<string>();
                        foreach (var pa in jeP.EnumerateArray())
                        {
                            if (pa.ValueKind == JsonValueKind.String)
                            {
                                arrayValue.Add(pa.GetString());
                            }
                        }
                        pValue = arrayValue.ToArray();
                    }
                    else
                    {
                        valid = false;
                    }
                    break;
#endif
                default:
                    break;
            }

            return pValue;
        }

        public static object XFromStringToObj(string data, ParamSpec.DataType dataType, out bool valid)
        {
            object result;
            valid = true;

            switch (dataType)
            {
                case ParamSpec.DataType.Boolean:
                    result = Convert.ToBoolean(data);
                    break;
                case ParamSpec.DataType.Integer:
                    result = Convert.ToInt32(data);
                    break;
                case ParamSpec.DataType.Real:
                    result = Convert.ToDouble(data);
                    break;
                case ParamSpec.DataType.String:
                    result = data;
                    break;
                case ParamSpec.DataType.DateTime:
                    result = Convert.ToDateTime(data);
                    break;
                default:
                    result = data;
                    valid = false;
                    break;
            }

            return result;
        }
    }
}
