﻿using AutoMapper;
using VSS.MasterData.Models.Models;
using VSS.Productivity3D.Filter.Common.Models;
using VSS.VisionLink.Interfaces.Events.MasterData.Models;

namespace VSS.Productivity3D.Filter.Common.Utilities
{
  public class AutoMapperUtility
  {
    private static MapperConfiguration _automapperConfiguration;

    public static MapperConfiguration AutomapperConfiguration
    {
      get
      {
        if (_automapperConfiguration == null)
        {
          ConfigureAutomapper();
        }

        return _automapperConfiguration;
      }
    }

    private static IMapper _automapper;

    public static IMapper Automapper
    {
      get
      {
        if (_automapperConfiguration == null)
        {
          ConfigureAutomapper();
        }

        return _automapper;
      }
    }


    public static void ConfigureAutomapper()
    {
      _automapperConfiguration = new MapperConfiguration(
        //define mappings <source type, destination type>
        cfg =>
        {
          cfg.AllowNullCollections = true; // so that byte[] can be null
          cfg.CreateMap<MasterData.Repositories.DBModels.Filter, FilterDescriptor>();
          cfg.CreateMap<FilterRequestFull, CreateFilterEvent>()
            .ForMember(x => x.ActionUTC, opt => opt.Ignore())
            .ForMember(x => x.ReceivedUTC, opt => opt.Ignore())
#pragma warning disable CS0612 // Type or member is obsolete
            .ForMember(x => x.UserUID, opt => opt.Ignore());
#pragma warning restore CS0612 // Type or member is obsolete
          cfg.CreateMap<FilterRequest, CreateFilterEvent>()
            .ForMember(x => x.ActionUTC, opt => opt.Ignore())
            .ForMember(x => x.ReceivedUTC, opt => opt.Ignore())
#pragma warning disable CS0612 // Type or member is obsolete
            .ForMember(x => x.UserUID, opt => opt.Ignore())
#pragma warning restore CS0612 // Type or member is obsolete
            .ForMember(x => x.CustomerUID, opt => opt.Ignore())
            .ForMember(x => x.ProjectUID, opt => opt.Ignore())
            .ForMember(x => x.UserID, opt => opt.Ignore());
          cfg.CreateMap<FilterRequestFull, UpdateFilterEvent>()
            .ForMember(x => x.ActionUTC, opt => opt.Ignore())
            .ForMember(x => x.ReceivedUTC, opt => opt.Ignore())
            .ForMember(x => x.FilterJson, opt => opt.Ignore())
#pragma warning disable CS0612 // Type or member is obsolete
            .ForMember(x => x.UserUID, opt => opt.Ignore());
#pragma warning restore CS0612 // Type or member is obsolete
          cfg.CreateMap<FilterRequestFull, DeleteFilterEvent>()
            .ForMember(x => x.ActionUTC, opt => opt.Ignore())
            .ForMember(x => x.ReceivedUTC, opt => opt.Ignore())
#pragma warning disable CS0612 // Type or member is obsolete
            .ForMember(x => x.UserUID, opt => opt.Ignore());
#pragma warning restore CS0612 // Type or member is obsolete
          cfg.CreateMap<MasterData.Repositories.DBModels.Filter, DeleteFilterEvent>()
            .ForMember(x => x.ActionUTC, opt => opt.MapFrom(src => src.LastActionedUtc))
            .ForMember(x => x.ReceivedUTC, opt => opt.MapFrom(src => src.LastActionedUtc))
#pragma warning disable CS0612 // Type or member is obsolete
            .ForMember(x => x.UserUID, opt => opt.Ignore());
#pragma warning restore CS0612 // Type or member is obsolete
        }
      );

      _automapper = _automapperConfiguration.CreateMapper();
    }
  }
}