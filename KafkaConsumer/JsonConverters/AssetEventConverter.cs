using System;
using Newtonsoft.Json.Linq;
using VSS.Project.Service.Utils.JsonConverters;
using VSS.VisionLink.Interfaces.Events.MasterData.Interfaces;
using VSS.VisionLink.Interfaces.Events.MasterData.Models;

public class AssetEventConverter : JsonCreationConverter<IAssetEvent>
{
  protected override IAssetEvent Create(Type objectType, JObject jObject)
  {
    if (jObject["CreateAssetEvent"] != null)
    {
      return jObject["CreateAssetEvent"].ToObject<CreateAssetEvent>();
    }
    if (jObject["UpdateAssetEvent"] != null)
    {
      return jObject["UpdateAssetEvent"].ToObject<UpdateAssetEvent>();
    }
    if (jObject["DeleteAssetEvent"] != null)
    {
      return jObject["DeleteAssetEvent"].ToObject<DeleteAssetEvent>();
    }
    return null;
  }
}