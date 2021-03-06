{{/* vim: set filetype=mustache: */}}
{{/*
Expand the name of the chart.
*/}}
{{- define "tagfileauthservice.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}


{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "tagfileauthservice.fullname" -}}
{{- if .Values.fullnameOverride -}}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- $name := default .Chart.Name .Values.nameOverride -}}
{{- if contains $name .Release.Name -}}
{{- .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" -}}
{{- end -}}
{{- end -}}
{{- end -}}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "tagfileauthservice.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
This isn't working as expected and is unused TODO come back and investigate further
*/}}
{{- define "ingressservice.name" -}}
{{- $releaseName := .Release.Name -}}
{{- $environment := .Values.environment -}}
{{- $image := .Values.image.tag -}}
{{- printf "%s-%s-%s" $releaseName $environment $image | lower | replace "_" "-" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Create name for webapi component.
*/}}
{{- define "component.webapi" -}}
{{- $name := default .Chart.Name .Values.nameOverride -}}
{{- printf "%s-%s-%s-%s" "webapi" $name .Values.environment .Values.image.tag | lower | replace "_" "-" | trunc 63 | trimSuffix "-" -}}
{{- end -}}