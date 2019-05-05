﻿namespace VSS.TRex.Types
{
    /// <summary>
    /// A relatively generic collection of error responses that application service request processing layers
    /// may return. Cloned from Current Gen. Refactor as appropriate.
    /// </summary>
    public enum RequestErrorStatus
    {
        OK,
        Unknown,
        Exception,
        UnsupportedCSDFileType,
        CouldNotConvertCSDFile,
        CToolBoxFailedToComplete,
        FailedToWriteCSDStream,
        FailedOnRequestProfile,
        NoSuchDataModel,
        UnsupportedDisplayType,
        FailedOnRequestColourGraduatedProfile,
        FailedToConvertClientWGSCoords,
        FailedToRequestSubgridExistenceMap,
        InvalidCoordinateRange,
        FailedToRequestDatamodelStatistics,
        FailedOnRequestCoordinateSystemProjectionFile,
        EmptyCoordinateSystem,
        AbortedDueToPipelineTimeout,
        UnsupportedFilterAttribute,
        ServiceStopped,
        RequestScheduleLoadTooHigh,
        RequestScheduleFailure,
        RequestScheduleTimeout,
        RequestHasBeenCancelled,
        FailedToObtainCoordinateSystemInterlock,
        FailedOnRequestCoordinateSystemHorizontalAdjustmentFile,
        NoConnectionToServer,
        InvalidResponseCode,
        NoResultReturned,
        FailedToNotifyCSChange,
        FailedToCreateDCToIRecordConverter,
        FailedToGetCSSettings,
        DCToIRecIncompleteCS,
        DCToIRecFailedCreateCSIB,
        DCToIRecFailedToGetGeoidInfo,
        DCToIRecFailedToGetZoneParams,
        DCToIRecFailedToCreateConstGeoid,
        DCToIRecFailedToCreateDatumGrid,
        DCToIRecFailedToCreateEllipsoid,
        DCToIRecFailedToCreateGridGeoid,
        DCToIRecFailedToCreateMolodenskyDatum,
        DCToIRecFailedToCreateMultiRegressionDatum,
        DCToIRecFailedToCreateSevenParamsDatum,
        DCToIRecFailedToCreateWGS84Datum,
        DCToIRecFailedToCreateZoneGroup,
        DCToIRecFailedToCreateZoneBasedSite,
        DCToIRecFailedToCreateAZIParamsObject,
        DCToIRecFailedToCreateCSIBObject,
        DCToIRecFailedToOpenCalibrationReader,
        DCToIRecFailedToSetZoneParams,
        DCToIRecFailedToReadCSIB,
        DCToIRecFailedToReadInCSIB,
        DCToIRecFailedToReadZoneBasedSite,
        DCToIRecFailedToReadZone,
        DCToIRecFailedToWriteDatum,
        DCToIRecFailedToWriteGeoid,
        DCToIRecFailedToWriteCSIB,
        DCToIRecFailedToSetZoneInfo,
        DCToIRecInfiniteAdjustmentSlopeValue,
        DCToIRecInvalidEllipsoid,
        DCToIRecDatumFailedToLoad,
        DCToIRecFailedToLoadCSIB,
        DCToIRecNotWGS84Ellipsoid,
        DCToIRecNotWGS84EllipsoidSameAsProj,
        DCToIRecScaleOnlyProj,
        DCToIRecUnknownCSType,
        DCToIRecUnknownDatumModel,
        DCToIRecUnknownGeoidModel,
        DCToIRecUnknownProjType,
        DCToIRecUnsupportedDatum,
        DCToIRecUnsupportedGeoid,
        DCToIRecUnsupportedZoneOrientation,
        FailedToRequestFileFromTCC,
        FailedToReadLineworkBoundaryFile,
        NoBoundariesInLineworkFile,
        FailedToPerformCoordinateConversion,
        NoProductionDataFound,
        InvalidPlanExtents,
        NoDesignProvided,
        ExportInvalidCSIB,
        ExportCoordConversionError,
        ExportNoDataFound,
        ExportExceededRowLimit,
        ExportUnableToLoadFileToS3,
        InvalidArgument,
        FailedToConfigureInternalPipeline,
        DesignImportUnableToRetrieveFromS3,
        DesignImportUnableToCreateDesign,
        DesignImportUnableToUpdateDesign,
        DesignImportUnableToDeleteDesign,
        FailedToPrepareFilter,
        FailedToGetCCAMinimumPassesValue
  }
}
