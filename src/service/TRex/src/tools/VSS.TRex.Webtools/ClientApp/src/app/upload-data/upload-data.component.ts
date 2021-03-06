import { Component, EventEmitter } from '@angular/core';
import { UploadEvent, UploadFile, FileSystemDirectoryEntry, FileSystemFileEntry } from 'ngx-file-drop';
import { _ } from 'underscore'

import { UploadDataService } from './upload-data.service'
import { ITagFileRequest } from "./tagfile-request"


@Component({
  selector: 'upload-data',
  providers: [UploadDataService],
  templateUrl: 'upload-data.component.html'
})
export class UploadDataComponent {

  public files: UploadFile[] = [];
  public selectedProject: any;
  public existingFlowObject: any;
  public totalTagFilesSize: number;
  public singleFileSize: number = 0;
  public failedFileUpload: any = [];
  public selectedFilesLength: any;
  public fileUploadedSuccessfully: number = 0;
  public tagFileUpload: boolean = false;
  public displayInvalidFileTypeError: boolean = false;
  public uploadFile: any;
  public selectedFiles: any;
  public projectUid: string;
  public orgUid: string;
  public treatAsJohnDoe: boolean;
  private fileCount: number = 0;
  public isFinishEnabled: boolean = false;


  constructor(private uploadDataService: UploadDataService) { }
  
  public uploadTagFiles() {
    this.tagFileUpload = true;
    this.selectedFilesLength = this.selectedFiles.length;
    this.totalTagFilesSize = 0;
    _.map(this.selectedFiles, (tagFile: any) => {
      this.totalTagFilesSize += tagFile.size;
    });
    this.fileCount = 0;
    this.failedFileUpload = [];
    this.isFinishEnabled = false;
    this.processTagFile();
  }

  public invalidFileType(files, invalidfiles) {
    if (invalidfiles.length > 0) {
      this.displayInvalidFileTypeError = true;
    }
  }

  public getBase64(file) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result.slice(reader.result.indexOf(',') + 1));
      reader.onerror = error => reject(error);
    });
  }

  public processTagFile() {
    this.getBase64(this.selectedFiles[this.fileCount]).then((data) => {
      this.uploadTagFile(data as string);
    }, (error) => {
      console.log("File Reading Error" +  error);
      this.continueFileUpload();
    });
  }

  private continueFileUpload() {
    this.fileCount++;
    if (this.fileCount < this.selectedFilesLength) {
      this.processTagFile();
    }
    else {
      this.isFinishEnabled = true;
    }
  }

  private uploadTagFile(data: string) {
    const tagToUpload: ITagFileRequest = {
      fileName: this.selectedFiles[this.fileCount].name,
      data: data,
      ProjectUid: this.projectUid,
      OrgId: this.orgUid,
      TreatAsJohnDoe: this.treatAsJohnDoe
    } as ITagFileRequest;


    this.uploadDataService.addTagfile(tagToUpload).subscribe( () =>
      this.continueFileUpload()
    );
  }

  public cancelUpload() {
    if (this.uploadFile) {
      this.uploadFile.abort();
    }
    this.tagFileUpload = false;
  }

  public dropped(event: UploadEvent) {
    this.files = event.files;
    this.selectedFiles = [];
    for (const droppedFile of event.files) {

      // Is it a file?
      if (droppedFile.fileEntry.isFile) {
        const fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
        fileEntry.file((file: File) => {

          // Here is the file log it to console for fun
          console.log(droppedFile.relativePath, file);
          this.selectedFiles.push(file);

        });
      } else {
        // It was a directory (empty directories are added, otherwise only files)
        const fileEntry = droppedFile.fileEntry as FileSystemDirectoryEntry;
        console.log(droppedFile.relativePath, fileEntry);
      }
    }
  }

  public fileOver(event) {
    console.log(event);
  }

  public fileLeave(event) {
    console.log(event);
  }

  public clearProjectUid() {
    this.projectUid = "";
  }

  public clearOrgUid() {
    this.orgUid = "";
  }
}
