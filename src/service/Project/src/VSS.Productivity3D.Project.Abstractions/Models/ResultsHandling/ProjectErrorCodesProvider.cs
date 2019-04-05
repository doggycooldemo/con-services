﻿using VSS.MasterData.Models.ResultHandling.Abstractions;

namespace VSS.Productivity3D.Project.Abstractions.Models.ResultsHandling
{
  public class ProjectErrorCodesProvider : ContractExecutionStatesEnum
  {
    public ProjectErrorCodesProvider()
    {
      this.DynamicAddwithOffset("No access to the project for a customer or the project does not exist.", 1);
      this.DynamicAddwithOffset("Supplied CoordinateSystem filename is not valid. Exceeds the length limit of 256, is empty, or contains illegal characters.", 2);
      this.DynamicAddwithOffset("Unable to connect to the Project database repository.", 3);
      this.DynamicAddwithOffset("Missing ActionUTC.", 4);
      this.DynamicAddwithOffset("Missing ProjectUID.", 5);
      this.DynamicAddwithOffset("Project already exists.", 6);
      this.DynamicAddwithOffset("Project does not exist.", 7);
      this.DynamicAddwithOffset("Missing ProjectBoundary.", 8);
      this.DynamicAddwithOffset("Missing ProjectTimezone.", 9);
      this.DynamicAddwithOffset("Invalid ProjectTimezone.", 10);
      this.DynamicAddwithOffset("Missing ProjectName.", 11);
      this.DynamicAddwithOffset("ProjectName is longer than the 255 characters allowed.", 12);
      this.DynamicAddwithOffset("Description is longer than the 2000 characters allowed.", 13);
      this.DynamicAddwithOffset("Missing ProjectStartDate.", 14);
      this.DynamicAddwithOffset("Missing ProjectEndDate.", 15);
      this.DynamicAddwithOffset("Project start date must be earlier than end date.", 16);
      this.DynamicAddwithOffset("Project timezone cannot be updated.", 17);
      this.DynamicAddwithOffset("CustomerUid parameter differs to the requesting CustomerUid. Impersonation is not supported.", 18);
      this.DynamicAddwithOffset("Missing CustomerUID.", 19);
      this.DynamicAddwithOffset("Project already associated with a customer.", 20);
      this.DynamicAddwithOffset("Dissociating projects from customers is not supported.", 21);
      this.DynamicAddwithOffset("Missing GeofenceUID.", 22);
      this.DynamicAddwithOffset("Missing project boundary.", 23);
      this.DynamicAddwithOffset("Invalid project boundary as it should contain at least 3 points.", 24);
      this.DynamicAddwithOffset("Invalid project boundary.", 25);
      this.DynamicAddwithOffset("Filespace Id, filespace name, path and file name are all required.", 26);
      this.DynamicAddwithOffset("CreateImportedFileV4.The file was not imported successfully.", 27);
      this.DynamicAddwithOffset("CreateImportedFileV4. Supplied filename is not valid. Either exceeds the length limit of 256 is empty or contains illegal characters.", 28);
      this.DynamicAddwithOffset("CreateImportedFileV4. Supplied path is not valid. Either is empty or contains illegal characters.", 29);
      this.DynamicAddwithOffset("CreateImportedFileV4. ImportedFileType is an unrecognized type.", 30);
      this.DynamicAddwithOffset("CreateImportedFileV4. ImportedFileType is not supported at present.", 31);
      this.DynamicAddwithOffset("CreateImportedFileV4. ImportedFileType does not match the file extension.", 32);
      this.DynamicAddwithOffset("The fileCreatedUtc is over 30 years old or >2 days in the future.", 33);
      this.DynamicAddwithOffset("The fileUpdatedUtc is over 30 years old or >2 days in the future.", 34);
      this.DynamicAddwithOffset("The ImportedBy is not available from authentication.", 35);
      this.DynamicAddwithOffset("The SurveyedUtc is not available.", 36);
      this.DynamicAddwithOffset("There are no available subscriptions for the selected customer.", 37);
      this.DynamicAddwithOffset("Legacy CustomerID must be provided.", 38);
      this.DynamicAddwithOffset("Missing CreateProjectRequest.", 39);
      this.DynamicAddwithOffset("Missing UpdateProjectRequest.", 40);
      this.DynamicAddwithOffset("Unable to create/update CoordinateSystem in RaptorServices. returned: {0} {1}.", 41);
      this.DynamicAddwithOffset("LegacyProjectId has not been generated. {0}", 42);
      this.DynamicAddwithOffset("Project boundary overlaps another project, for this customer and time span.", 43);
      this.DynamicAddwithOffset("Missing legacyProjectId.", 44);
      this.DynamicAddwithOffset("Landfill is missing its CoordinateSystem.", 45);
      this.DynamicAddwithOffset("Invalid CoordinateSystem.", 46);
      this.DynamicAddwithOffset("Unable to validate CoordinateSystem in RaptorServices. returned: {0} {1}.", 47);
      this.DynamicAddwithOffset("Unable to obtain TCC fileSpaceId.", 48);
      this.DynamicAddwithOffset("CreateImportedFileV4. Unable to store Imported File event to database.", 49);
      this.DynamicAddwithOffset("LegacyImportedFileId has not been generated.", 50);
      this.DynamicAddwithOffset("DeleteImportedFileV4. Unable to set Imported File event to deleted.", 51);
      this.DynamicAddwithOffset("CreateImportedFileV4. Unable to store updated Imported File event to database.", 52);
      this.DynamicAddwithOffset("WriteFileToRepository: Unable to write file to TCC.", 53);
      this.DynamicAddwithOffset("Unable to delete fileDescriptor from TCC. TCC code {0} message {1}", 54);
      this.DynamicAddwithOffset("CreateImportedFileV4. The uploaded file is not accessible.", 55);
      this.DynamicAddwithOffset("DeleteImportedFileV4. The importedFileUid doesn't exist under this project.", 56);
      this.DynamicAddwithOffset("A problem occurred at the {0} endpoint. Exception: {1}", 57);
      this.DynamicAddwithOffset("CreateImportedFileV4. The file has already been created.", 58);
      this.DynamicAddwithOffset("GeofenceService CreateGeofence failed. No geofenceUid returned. {0}", 59);
      this.DynamicAddwithOffset("Application calling context supports only HttpGet endpoints.", 60);
      this.DynamicAddwithOffset("Unable to create project. {0}", 61);
      this.DynamicAddwithOffset("Unable to update project. {0}", 62);
      this.DynamicAddwithOffset("Unable to associate project with customer. {0}", 63);
      this.DynamicAddwithOffset("Unable to disassociate project from customer.", 64);
      this.DynamicAddwithOffset("Unable to store project-geofence associate in database. {0}", 65);
      this.DynamicAddwithOffset("Unable to delete project.", 66);
      this.DynamicAddwithOffset("FileImport AddFile in RaptorServices failed. Reason: {0} {1}.", 67);
      this.DynamicAddwithOffset("Invalid parameters.", 68);
      this.DynamicAddwithOffset("Unable to retrieve project settings from repository. Reason: {0} {1}.", 69);
      this.DynamicAddwithOffset("Unable to validate project settings with raptor. Reason: {0} {1}.", 70);
      this.DynamicAddwithOffset("Unable to update project settings with raptor. Reason: {0} {1}.", 71);
      this.DynamicAddwithOffset("Unable to create Kafka event. Reason: {0}.", 72);
      this.DynamicAddwithOffset("Invalid geofence Types.", 73);
      this.DynamicAddwithOffset("Landfill projects are not supported.", 74);
      this.DynamicAddwithOffset("CreateImportedFileV4. DxfUnitsType is an unrecognized type.", 75);
      this.DynamicAddwithOffset("CreateImportedFileV4. DxfUnitsType is not supported at present.", 76);
      this.DynamicAddwithOffset("Unsupported ProjectSettings type.", 77);
      this.DynamicAddwithOffset("GetCoordinateSystemFromFileRepo: Folder {0} doesn't exist.", 78);
      this.DynamicAddwithOffset("GetCoordinateSystemFromFileRepo: Exception reading file {0}.", 79);
      this.DynamicAddwithOffset("GetCoordinateSystemFromFileRepo: Returned file invalid {0}.", 80);
      this.DynamicAddwithOffset("CreateProjectV2: Missing CreateProjectRequest.", 81);
      this.DynamicAddwithOffset("CreateProjectV2: Missing BusinessCentreFile.", 82);
      this.DynamicAddwithOffset("CreateProjectV2: Invalid businessCentreFile path.", 83);
      this.DynamicAddwithOffset("CreateProjectV2: Invalid businessCentreFile fileSpaceId.", 84);
      this.DynamicAddwithOffset("UpdateProjectV4: Invalid ProjectType. Can ony be changed from Standard to Landfill/Civil.", 85);
      this.DynamicAddwithOffset("ValidateTCCAuthorization: Invalid Request: {0}.", 86);
      this.DynamicAddwithOffset("ValidateTCCAuthorization: Exception getting list of organizations from TCC: {0}.", 87);
      this.DynamicAddwithOffset("ValidateTCCAuthorization: Unable to locate orgShortName in TCC {0}.", 88);
      this.DynamicAddwithOffset("ValidateTCCAuthorization: This orgShortName is missing a filespaceId or orgId in TCC {0}.", 89);
      this.DynamicAddwithOffset("ValidateTCCAuthorization: Exception getting customerTccOrg from database: {0.}", 90);
      this.DynamicAddwithOffset("ValidateTCCAuthorization: Unable to match customer with tccOrg in database.", 91);
      this.DynamicAddwithOffset("CopyTccImportedFile: Unable to copy file {0}.", 92);
      this.DynamicAddwithOffset("CopyTccImportedFile: Unable to read file {0}.", 93);
      this.DynamicAddwithOffset("CopyTccImportedFile: Unable to obtain file properties {0}.", 94);
      this.DynamicAddwithOffset("UpsertImportedFileV2: Alignment type is missing detail parameter.", 95);
      this.DynamicAddwithOffset("UpdateGeofenceInGeofenceService: Unable to find the project-geofence association.", 96);
      this.DynamicAddwithOffset("UpdateGeofenceInGeofenceService: Unable to find the projects Geofence.", 97);
      this.DynamicAddwithOffset("UpdateGeofenceInGeofenceService: Unable to find the projects Geofence. Exception: {0}", 98);
      this.DynamicAddwithOffset("UpdateGeofenceInGeofenceService: Unable update the projects Geofence.", 99);
      this.DynamicAddwithOffset("UpdateGeofenceInGeofenceService: Unable update the projects Geofence. Exception: {0}", 100);
      this.DynamicAddwithOffset("UpdateGeofenceInGeofenceService: Unable to find the project-geofence association. Exception: {0}", 101);
      this.DynamicAddwithOffset("ProjectGeofenceAssociation: Only landfill project and site types are supported at present.", 102);
      this.DynamicAddwithOffset("ProjectGeofenceAssociation: Invalid GeofenceUid list.", 103);
      this.DynamicAddwithOffset("ProjectGeofenceAssociation: Unable to find GeofenceUid/s.", 104);
      this.DynamicAddwithOffset("ProjectGeofenceAssociation: Geofence is assigned to a different project.", 105);
      this.DynamicAddwithOffset("ProjectGeofenceAssociation: Error accessing database. Exception: {0}", 106);
      this.DynamicAddwithOffset("ProjectGeofenceAssociation: Already associated geofence is missing from request.", 107);
      this.DynamicAddwithOffset("FileImport DeleteFile in RaptorServices failed. Reason: {0} {1}.", 108);
      this.DynamicAddwithOffset("UpsertProject Not allowed duplicate, active projectnames: {0}.", 109);
      this.DynamicAddwithOffset("ProjectGeofenceAssociation: Duplicate GeofenceUids appear in the request body.", 110);
      this.DynamicAddwithOffset("Invalid project boundary points.Latitudes should be -90 through 90 and Longitude -180 through 180. Points around 0,0 are invalid", 111);
      this.DynamicAddwithOffset("Cannot delete a design, surveyed surface, or alignment file which is used in a filter.", 112);
      this.DynamicAddwithOffset("ImportedFile environment variable configuration will result in file being ignored.", 113);
      this.DynamicAddwithOffset("FileImport AddFile in TRex failed. Reason: {0} {1}.", 114);
      this.DynamicAddwithOffset("Unable to obtain DataOcean root folder.", 115);
      this.DynamicAddwithOffset("Unable to write file to DataOcean.", 116);
      this.DynamicAddwithOffset("Unable to delete file from DataOcean", 117);
      this.DynamicAddwithOffset("A reference surface must have a parent design surface and offset", 118);

    }
  }
}

