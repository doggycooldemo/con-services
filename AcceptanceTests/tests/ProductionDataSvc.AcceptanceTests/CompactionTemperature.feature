﻿Feature: CompactionTemperature
I should be able to request compaction temperature data

######################################################## Temperature Summary ####################################################
Scenario Outline: Compaction Get Temperature Summary - No Design Filter
Given the Compaction service URI "/api/v2/temperature/summary" for operation "TemperatureSummary"
And the result file "CompactionGetTemperatureDataResponse.json"
And projectUid "<ProjectUID>"
When I request result
Then the result should match the "<ResultName>" from the repository
Examples: 
| RequestName     | ProjectUID                           | ResultName                |
|                 | ff91dd40-1569-4765-a2bc-014321f76ace | NoDesignFilter_Summary    |
| ProjectSettings | 3335311a-f0e2-4dbe-8acd-f21135bafee4 | NoDesignFilter_Summary_PS |

Scenario Outline: Compaction Get Temperature Summary
Given the Compaction service URI "/api/v2/temperature/summary" for operation "TemperatureSummary"
And the result file "CompactionGetTemperatureDataResponse.json"
And projectUid "<ProjectUID>"
And filterUid "<FilterUID>"
When I request result
Then the result should match the "<ResultName>" from the repository
Examples: 
| RequestName       | ProjectUID                           | FilterUID                            | ResultName                |
| DesignOutside     | 7925f179-013d-4aaf-aff4-7b9833bb06d6 | 1cf81668-1739-42d5-b068-ea025588796a | DesignOutside_Summary     |
| DesignIntersects  | 7925f179-013d-4aaf-aff4-7b9833bb06d6 | 3d9086f2-3c04-4d92-9141-5134932b1523 | DesignIntersects_Summary  |
| FilterArea        | ff91dd40-1569-4765-a2bc-014321f76ace | a37f3008-65e5-44a8-b406-9a078ec62ece | BoundaryTempFilter        |
| AlignmentFilter   | ff91dd40-1569-4765-a2bc-014321f76ace | 2811c7c3-d270-4d63-97e2-fc3340bf6c7a | AlignmentFilter_Summary   |
| TemperatureFilter | ff91dd40-1569-4765-a2bc-014321f76ace | 1980fc8b-c892-4f9f-b673-bc09827bf2b5 | TemperatureFilter_Summary |
| PassCountFilter   | ff91dd40-1569-4765-a2bc-014321f76ace | c5590172-a1bb-440a-bc7d-6c35ecc75724 | PassCountFilter_Summary   |

