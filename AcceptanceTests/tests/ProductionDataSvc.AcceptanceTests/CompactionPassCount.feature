﻿Feature: CompactionPassCount
  I should be able to request Pass Count compaction data

######################################################## Pass Count Summary #####################################################
Scenario Outline: Compaction Get Passcount Summary - No Design Filter
	Given the Compaction service URI "/api/v2/compaction/passcounts/summary" for operation "PassCountSummary"
  And the result file "CompactionGetPassCountDataResponse.json"
	And projectUid "<ProjectUID>"
	When I request result
  Then the result should match the "<ResultName>" from the repository
	Examples: 
	| RequetsName     | ProjectUID                           | ResultName                |
	|                 | ff91dd40-1569-4765-a2bc-014321f76ace | NoDesignFilter_Summary    |
  | ProjectSettings | 3335311a-f0e2-4dbe-8acd-f21135bafee4 | NoDesignFilter_Summary_PS |

######################################################## Pass Count Details #####################################################
Scenario Outline: Compaction Get Passcount Details - No Design Filter
	Given the Compaction service URI "/api/v2/compaction/passcounts/details" for operation "PassCountDetails"
  And the result file "CompactionGetPassCountDataResponse.json"
	And projectUid "<ProjectUID>"
	When I request result
  Then the result should match the "<ResultName>" from the repository
	Examples: 
	| RequetsName     | ProjectUID                           | ResultName                |
	|                 | ff91dd40-1569-4765-a2bc-014321f76ace | NoDesignFilter_Details    |
  | ProjectSettings | 3335311a-f0e2-4dbe-8acd-f21135bafee4 | NoDesignFilter_Details_PS |
