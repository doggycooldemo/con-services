using System;

namespace Repositories.DBModels
{
    public enum ServiceTypeEnum
    {
        Essentials = 1,
        ManualMaintenanceLog = 2,
        CATHealth = 3,
        StandardHealth = 4,
        CATUtilization = 5,
        StandardUtilization = 6,
        CATMAINT = 7,
        VLMAINT = 8,
        RealTimeDigitalSwitchAlerts = 9,
        oneMinuteUpdateRateUpgrade = 10,
        ConnectedSiteGateway = 11,
        LoadCycleMonitoring = 12,
        ThreeDProjectMonitoring = 13,
        VisionLinkRFID = 14,
        Manual3DProjectMonitoring = 15,
        VehicleConnect = 16,
        UnifiedFleet = 17,
        AdvancedProductivity = 18,
        Landfill = 19,
        rojectMonitoring = 20,
        OperatorIdManageOperators = 21,
    }

    public enum ServiceTypeFamilyEnum
    {
        Asset = 1,
        Customer = 2,
        Project = 3,
    }
}