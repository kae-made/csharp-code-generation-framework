// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Azure;
using Azure.Core;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Kae.DomainModel.Csharp.Framework;
using Kae.DomainModel.Csharp.Framework.Adaptor.ExternalStorage;
using Kae.Utility.Logging;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.Json;

namespace Kae.DomainModel.Csharp.Framework.Adaptor.ExternalStorage.AzureDigitalTwins
{
    public abstract class AzureDigitalTwinsAdaptor : IExternalStorageAdaptor
    {
        protected DigitalTwinsClient adtClient = null;
        protected IExternalStorageAdaptable instanceRepository;
        protected Logger logger;
        protected string adtInstanceUrl;
        protected TokenCredential credential;


        public AzureDigitalTwinsAdaptor(string adtInstanceUrl, TokenCredential credential, IExternalStorageAdaptable repository, Logger logger)
        {
            this.instanceRepository = repository;
            this.logger = logger;
            this.adtInstanceUrl = adtInstanceUrl;
            this.credential = credential;
        }

        public async void Initialize()
        {
            //var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            //string adtInstanceUrl = configuration.GetConnectionString("ADT");
            //var credential = new DefaultAzureCredential();
            adtClient = new DigitalTwinsClient(new Uri(adtInstanceUrl), credential);

            instanceRepository.AddCInstanceChangedStateNotifyHandler(CInstanceChangedStateNotified);
            instanceRepository.AddCLinkChangedStateNotifyHandler(CLinkChangedStateNotified);
            instanceRepository.AddCEventChangedStateNotifyHandler(CEventChangedStateNotified);

#if false
            string query = "SELECT * FROM DIGITALTWINS WHERE IS_OF_MODEL('dtmi:com:kae_made:ADTTestModel:TE;1')";
            try{
                var queryResult = adtClient.QueryAsync<BasicDigitalTwin>(query);
                await foreach (var twin in queryResult)
                {
                    var id = twin.Id;
                }
            }
            catch(Exception ex)
            {
                //
            }
#endif
        }

        public bool DoseEventComeFromExternal()
        {
            return true;
        }

        protected async void CLinkChangedStateNotified(CLinkChangedState changedState)
        {
            logger.LogInfo("Relationship Updated");

            var relDef = GetDTDLRelationshipDef(changedState.Target.Source.DomainName, changedState.Target.RelationshipID);
            string sourceTwinId = changedState.Target.Source.GetIdForExternalStorage();
            string destinationTwinId = changedState.Target.Destination.GetIdForExternalStorage();
            var tgRelId = $"{sourceTwinId}-{relDef.Id}-{destinationTwinId}";
            try
            {
                if (changedState.OP == ChangedState.Operation.Create)
                {
                    var twinRelationship = new BasicRelationship() { Id = tgRelId, Name = relDef.Name, SourceId = sourceTwinId, TargetId = destinationTwinId };
                    await adtClient.CreateOrReplaceRelationshipAsync<BasicRelationship>(sourceTwinId, tgRelId, twinRelationship);
                }
                else if (changedState.OP == ChangedState.Operation.Delete)
                {
                    await adtClient.DeleteRelationshipAsync(sourceTwinId, tgRelId);
                }
                else
                {
                    logger.LogWarning($"Relationship Update - ?'{changedState.OP.ToString()}'");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        protected async void CInstanceChangedStateNotified(CInstanceChagedState changedState)
        {
            logger.LogInfo("Class Properties Updated");
            string twinId = changedState.Target.GetIdForExternalStorage();
            try
            {
                if (changedState.OP == ChangedState.Operation.Create)
                {
                    var twin = new BasicDigitalTwin()
                    {
                        Id = twinId,
                        Metadata = { ModelId = GetDTDLTwinModelId(changedState.Target.DomainName, changedState.Target.ClassName) },
                        Contents = changedState.ChangedProperties
                    };
                    await adtClient.CreateOrReplaceDigitalTwinAsync<BasicDigitalTwin>(twinId, twin);
                }
                else if (changedState.OP == ChangedState.Operation.Delete)
                {
                    await adtClient.DeleteDigitalTwinAsync(twinId);
                }
                else if (changedState.OP == ChangedState.Operation.Update)
                {
                    var existedTwin = await adtClient.GetDigitalTwinAsync<BasicDigitalTwin>(twinId);
                    var updateTwinData = new JsonPatchDocument();
                    foreach (var ckey in changedState.ChangedProperties.Keys)
                    {
                        var changedProperty = changedState.ChangedProperties[ckey];
                        if (changedProperty is TimerImpl)
                        {
                            changedProperty = ((TimerImpl)changedProperty).TimerIdOnService;
                        }
                        if (existedTwin.Value.Contents.ContainsKey(ckey))
                        {
                            updateTwinData.AppendReplace($"/{ckey}", changedProperty);
                        }
                        else
                        {
                            updateTwinData.AppendAdd($"/{ckey}", changedProperty);
                        }
                    }
                    await adtClient.UpdateDigitalTwinAsync(twinId, updateTwinData);
                }
                else
                {
                    logger.LogWarning($"Operation='{changedState.OP.ToString()}'");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        protected async void CEventChangedStateNotified(CEventChangedState changedState)
        {
            try
            {
                string msgId = Guid.NewGuid().ToString();
                var eventContent = new Dictionary<string, object>()
                {
                    { changedState.Event.EventName , changedState.Event.GetSupplementalData() }
                };
                string payload = Newtonsoft.Json.JsonConvert.SerializeObject(eventContent);
                await adtClient.PublishTelemetryAsync(changedState.Target.GetIdForExternalStorage(), msgId, payload);
                logger.LogInfo($"Published Telemetry:messageId={msgId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        public async Task<IEnumerable<DomainClassDef>> CheckInstanceStatus(string domainName, string classKeyLetter, IEnumerable<DomainClassDef> existingInstances, Func<string> query, Func<DomainClassDef> create, string cardinarity)
        {
            var resultSet = new List<DomainClassDef>();
            resultSet.AddRange(existingInstances);
            string queryStatement = $"SELECT * FROM DIGITALTWINS WHERE IS_OF_MODEL('{GetDTDLTwinModelId(domainName, classKeyLetter)}')";
            string queryCondition = query();
            if (!string.IsNullOrEmpty(queryCondition))
            {
                queryStatement += $" AND {queryCondition}";
            }
            if (existingInstances.Count() > 0)
            {
                string notList = "";
                foreach (var instance in existingInstances)
                {
                    if (!string.IsNullOrEmpty(notList))
                    {
                        notList += ",";
                    }
                    notList += $"'{instance.GetIdForExternalStorage()}'";
                }
                queryStatement = $"{queryStatement} AND NOT ( $dtId IN [{notList}] )";
            }
            AsyncPageable<BasicDigitalTwin> queryResult = adtClient.QueryAsync<BasicDigitalTwin>(queryStatement);
            try
            {
                await foreach (var result in queryResult)
                {
                    //lock (instanceRepository)
                    //{
                    var targetInstance = create();
                    var properties = new Dictionary<string, object>();
                    foreach (var ck in result.Contents.Keys)
                    {
                        properties.Add(ck, ResolveContent((JsonElement)result.Contents[ck]));
                    }
                    targetInstance.Restore(properties);
                    resultSet.Add(targetInstance);
                    if (cardinarity != "many")
                    {
                        break;
                    }
                    //}
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
            return resultSet;
        }

        public async Task<IEnumerable<DomainClassDef>> CheckTraverseStatus(string domainName, DomainClassDef spInstance, string epClassKeyLetter, string relationshipName, IEnumerable<DomainClassDef> existingInstances, Func<DomainClassDef> create, string cardinarity)
        {
            var resultSet = new List<DomainClassDef>();
            resultSet.AddRange(existingInstances);
            var relDef = GetDTDLRelationshipDef(domainName, relationshipName);
            string twinKeyword = "";
            string queryStatement = "";
            if (spInstance.ClassName == relDef.FormClassKeyLetter)
            {
                twinKeyword = "target";
                queryStatement = $"SELECT {twinKeyword} FROM DIGITALTWINS source JOIN {twinKeyword} RELATED source.{relDef.Name} WHERE source.$dtId = '{spInstance.GetIdForExternalStorage()}'";
            }
            else
            {
                twinKeyword = "source";
                queryStatement = $"SELECT {twinKeyword} FROM DIGITALTWINS {twinKeyword} JOIN target RELATED {twinKeyword}.{relDef.Name} WHERE target.$dtId = '{spInstance.GetIdForExternalStorage()}'";
            }
            if (existingInstances.Count() > 0)
            {
                string notList = "";
                foreach (var instance in existingInstances)
                {
                    if (!string.IsNullOrEmpty(notList))
                    {
                        notList += ",";
                    }
                    notList += $"'{instance.GetIdForExternalStorage()}'";
                }
                queryStatement = $"{queryStatement} AND NOT ( {twinKeyword}.$dtId IN [{notList}] )";
            }

            try
            {
                AsyncPageable<BasicDigitalTwin> queryResult = adtClient.QueryAsync<BasicDigitalTwin>(queryStatement);
                int index = 0;
                await foreach (var twin in queryResult)
                {
                    Debug.WriteLine($"Index={index++}");
                    var content = (JsonElement)twin.Contents[twinKeyword];
                    string dtId = null;
                    var propperties = ResolveClassProperties(content, out dtId);
                    var instance = create();
                    instance.Restore(propperties);
                    resultSet.Add(instance);
                    if (cardinarity != "many")
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            return resultSet;
        }

        public abstract string GetDTDLTwinModelId(string domainName, string classKeyLetter);
        public abstract DTDLRelationshipDef GetDTDLRelationshipDef(string domainName, string relationshipName);

        public async void ClassPropertiesUpdater(string classKeyLetter, string operation, DomainClassDef instance, string identities, IDictionary<string, object> properties)
        {
            logger.LogInfo("Class Properties Updated");
            string twinId = instance.GetIdForExternalStorage();
            try
            {
                if (operation == "Create")
                {
                    var twin = new BasicDigitalTwin()
                    {
                        Id = twinId,
                        Metadata = { ModelId = GetDTDLTwinModelId(instance.DomainName, instance.ClassName) },
                        Contents = properties
                    };
                    await adtClient.CreateOrReplaceDigitalTwinAsync<BasicDigitalTwin>(twinId, twin);
                }
                else if (operation == "Delete")
                {
                    await adtClient.DeleteDigitalTwinAsync(twinId);
                }
                else if (operation == "Update")
                {
                    var existedTwin = await adtClient.GetDigitalTwinAsync<BasicDigitalTwin>(twinId);
                    var updateTwinData = new JsonPatchDocument();
                    foreach (var ckey in properties.Keys)
                    {
                        if (existedTwin.Value.Contents.ContainsKey(ckey))
                        {
                            updateTwinData.AppendReplace($"/{ckey}", properties[ckey]);
                        }
                        else
                        {
                            updateTwinData.AppendAdd($"/{ckey}", properties[ckey]);
                        }
                    }
                    await adtClient.UpdateDigitalTwinAsync(twinId, updateTwinData);
                }
                else
                {
                    logger.LogWarning($"Operation='{operation}'");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        public async void RelationshipUpdater(string relationshipId, string operation, string sourceClassKeyLetter, DomainClassDef sourceInstance, string sourceIdentities, string destinationClassKeyLetter, DomainClassDef destinationInstance, string destinationIdenities)
        {
            logger.LogInfo("Relationship Updated");

            var relDef = GetDTDLRelationshipDef(sourceInstance.DomainName, relationshipId);
            string sourceTwinId = sourceInstance.GetIdForExternalStorage();
            string destinationTwinId = destinationInstance.GetIdForExternalStorage();
            var tgRelId = $"{sourceTwinId}-{relDef.Id}-{destinationTwinId}";
            try
            {
                if (operation == "Create")
                {
                    var twinRelationship = new BasicRelationship() { Id = tgRelId, Name = relDef.Name, SourceId = sourceTwinId, TargetId = destinationTwinId };
                    await adtClient.CreateOrReplaceRelationshipAsync<BasicRelationship>(sourceTwinId, tgRelId, twinRelationship);
                }
                else if (operation == "Delete")
                {
                    await adtClient.DeleteRelationshipAsync(sourceTwinId, tgRelId);
                }
                else
                {
                    logger.LogWarning($"Relationship Update - ?'{operation}'");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        public async void EventUpdater(string classKeyLetter, DomainClassDef instance, string eventLabel, IDictionary<string, object> supplimentalData)
        {
            logger.LogInfo("Called Event Updater)");
        }

        protected static object ResolveContent(JsonElement jcontent)
        {
            object resolvedValue = null;
            switch (jcontent.ValueKind)
            {
                case JsonValueKind.Array:
                    var array = new List<object>();
                    foreach (var item in jcontent.EnumerateArray())
                    {
                        array.Add(ResolveContent(item));
                    }
                    resolvedValue = array;
                    break;
                case JsonValueKind.Null:
                    break;
                case JsonValueKind.Number:
                    string rawValue = jcontent.GetRawText();
                    double dtmp;
                    Int64 i64tmp;
                    Int32 i32tmp;
                    if (jcontent.TryGetInt32(out i32tmp))
                    {
                        resolvedValue = i32tmp;
                    }
                    else
                    {
                        if (jcontent.TryGetInt64(out i64tmp))
                        {
                            resolvedValue = i64tmp;
                        }
                        else
                        {
                            if (jcontent.TryGetDouble(out dtmp))
                            {
                                resolvedValue = dtmp;
                            }
                        }
                    }
                    break;
                case JsonValueKind.Object:
                    var fields = new Dictionary<string, object>();
                    foreach (var field in jcontent.EnumerateObject())
                    {
                        fields.Add(field.Name, ResolveContent(field.Value));
                    }
                    resolvedValue = fields;
                    break;
                case JsonValueKind.String:
                    resolvedValue = jcontent.GetString();
                    break;
                case JsonValueKind.True:
                    resolvedValue = true;
                    break;
                case JsonValueKind.False:
                    resolvedValue = false;
                    break;
                case JsonValueKind.Undefined:
                default:
                    break;
            }

            return resolvedValue;
        }

        protected static IDictionary<string, object> ResolveClassProperties(JsonElement content, out string dtId)
        {
            IDictionary<string, object> properties = new Dictionary<string, object>();
            try
            {
                dtId = content.GetProperty("$dtId").GetString();
                var jsonObject = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content.GetRawText());
                ResolveClassProperty(jsonObject, properties);
            }
            catch (Exception ex)
            {
                dtId = ex.Message;
            }
            return properties;
        }

        protected static void ResolveClassProperty(JObject property, IDictionary<string, object> parent)
        {
            foreach (var p in property)
            {
                string pk = p.Key;
                if (pk.StartsWith("$"))
                {
                    continue;
                }
                if (p.Value.Type == JTokenType.Object)
                {
                    var children = new Dictionary<string, object>();
                    parent.Add(pk, children);

                    ResolveClassProperty((JObject)p.Value, children);
                }
                else
                {
                    object value = null;
                    switch (p.Value.Type)
                    {
                        case JTokenType.String:
                            value = (string)p.Value;
                            break;
                        case JTokenType.Integer:
                            value = (int)p.Value;
                            break;
                        case JTokenType.Date:
                            value = (DateTime)p.Value;
                            break;
                        case JTokenType.Float:
                            value = (float)p.Value;
                            break;
                        case JTokenType.Guid:
                            value = (Guid)p.Value;
                            break;
                        case JTokenType.Boolean:
                            value = (bool)p.Value;
                            break;
                        case JTokenType.Bytes:
                            value = (byte[])p.Value;
                            break;
                        case JTokenType.TimeSpan:
                            value = (TimeSpan)p.Value;
                            break;
                        case JTokenType.Uri:
                            value = (Uri)p.Value;
                            break;
                        case JTokenType.Null:
                        default:
                            value = null;
                            break;
                    }
                    parent.Add(pk, value);
                }
            }
        }

        public void ClearCache(string domainName)
        {
            instanceRepository.ClearAllInstances(domainName);
            logger.LogInfo("Cache cleared");
        }

    }
}