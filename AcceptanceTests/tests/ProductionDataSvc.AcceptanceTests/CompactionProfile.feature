﻿Feature: CompactionProfile
I should be able to request Compaction Profile data.

Scenario: Compaction Get Slicer Empty Profile
Given the Compaction Profile service URI "/api/v2/profiles/productiondata/slicer"
And a projectUid "7925f179-013d-4aaf-aff4-7b9833bb06d6"
And a startLatDegrees "36.209310" and a startLonDegrees "-115.019584" and an endLatDegrees "36.209322" And an endLonDegrees "-115.019574"
And a cutfillDesignUid "dd64fe2e-6f27-4a78-82a3-0c0e8a5e84ff"
When I request a Compaction Profile 
Then the Compaction Profile should be
"""
  {
    "gridDistanceBetweenProfilePoints": 1.6069225472652788,
    "results": [
        {
            "type": "firstPass",
            "data": []
        },
        {
            "type": "highestPass",
            "data": []
        },
        {
            "type": "lastPass",
            "data": []
        },
        {
            "type": "lowestPass",
            "data": []
        },
        {
            "type": "lastComposite",
            "data": []
        },
        {
            "type": "cmvSummary",
            "data": []
        },
        {
            "type": "cmvDetail",
            "data": []
        },
        {
            "type": "cmvPercentChange",
            "data": []
        },
        {
            "type": "mdpSummary",
            "data": []
        },
        {
            "type": "temperatureSummary",
            "data": []
        },
        {
            "type": "speedSummary",
            "data": []
        },
        {
            "type": "passCountSummary",
            "data": []
        },
        {
            "type": "passCountDetail",
            "data": []
        },
		{
		  "type": "cutFill",
		  "data": []
		},
		{
		  "type": "summaryVolumes",
		  "data": []
		}
    ],
    "Code": 0,
    "Message": "success"
}
"""

Scenario: Compaction Get Slicer Profile
Given the Compaction Profile service URI "/api/v2/profiles/productiondata/slicer"
And a projectUid "7925f179-013d-4aaf-aff4-7b9833bb06d6"
And a startLatDegrees "36.207310" and a startLonDegrees "-115.019584" and an endLatDegrees "36.207322" And an endLonDegrees "-115.019574"
And a cutfillDesignUid "dd64fe2e-6f27-4a78-82a3-0c0e8a5e84ff"
When I request a Compaction Profile 
Then the Compaction Profile should be
"""
{
  "gridDistanceBetweenProfilePoints": 1.6069349839892924,
  "results": [
    {
      "type": "firstPass",
      "data": [
        {
          "cellType": 1,
          "x": 0.0,
          "y": 597.353,
          "value": 597.353,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.085205803116539316,
          "y": 597.3581,
          "value": 597.359,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.10001382450880753,
          "y": 597.359,
          "value": 597.359,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.11482184590107573,
          "y": 597.36084,
          "value": 597.386,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.31995441102981748,
          "y": 597.386,
          "value": 597.386,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.52508697615855926,
          "y": 597.3832,
          "value": 597.382,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.60891033073067113,
          "y": 597.382,
          "value": 597.382,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.692733685302783,
          "y": 597.3828,
          "value": 597.384,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.81404289585963441,
          "y": 597.384,
          "value": 597.384,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.93535210641648581,
          "y": 597.3836,
          "value": 597.383,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.117806836953072,
          "y": 597.383,
          "value": 597.383,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 1.300261567489658,
          "y": 597.383,
          "value": 597.383,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.3229394020819409,
          "y": 597.383,
          "value": 597.383,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 1.3456172366742238,
          "y": 597.382935,
          "value": 597.382,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.6069349839892926,
          "y": 597.382,
          "value": 597.382,
          "valueType": null,
          "y2": null,
          "value2": null
        }
      ]
    },
    {
      "type": "highestPass",
      "data": [
        {
          "cellType": 1,
          "x": 0.0,
          "y": 597.396,
          "value": 597.396,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.085205803116539316,
          "y": 597.388367,
          "value": 597.387,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.10001382450880753,
          "y": 597.387,
          "value": 597.387,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.11482184590107573,
          "y": 597.386963,
          "value": 597.386,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.31995441102981748,
          "y": 597.386,
          "value": 597.386,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.52508697615855926,
          "y": 597.3832,
          "value": 597.382,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.60891033073067113,
          "y": 597.382,
          "value": 597.382,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.692733685302783,
          "y": 597.3828,
          "value": 597.384,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.81404289585963441,
          "y": 597.384,
          "value": 597.384,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.93535210641648581,
          "y": 597.3836,
          "value": 597.383,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.117806836953072,
          "y": 597.383,
          "value": 597.383,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 1.300261567489658,
          "y": 597.383,
          "value": 597.383,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.3229394020819409,
          "y": 597.383,
          "value": 597.383,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 1.3456172366742238,
          "y": 597.382935,
          "value": 597.382,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.6069349839892926,
          "y": 597.382,
          "value": 597.382,
          "valueType": null,
          "y2": null,
          "value2": null
        }
      ]
    },
    {
      "type": "lastPass",
      "data": [
        {
          "cellType": 1,
          "x": 0.0,
          "y": 597.396,
          "value": 597.396,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.085205803116539316,
          "y": 597.388367,
          "value": 597.387,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.10001382450880753,
          "y": 597.387,
          "value": 597.387,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.11482184590107573,
          "y": 597.386963,
          "value": 597.386,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.31995441102981748,
          "y": 597.386,
          "value": 597.386,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.52508697615855926,
          "y": 597.3832,
          "value": 597.382,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.60891033073067113,
          "y": 597.382,
          "value": 597.382,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.692733685302783,
          "y": 597.3828,
          "value": 597.384,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.81404289585963441,
          "y": 597.384,
          "value": 597.384,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.93535210641648581,
          "y": 597.376,
          "value": 597.364,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.117806836953072,
          "y": 597.364,
          "value": 597.364,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 1.300261567489658,
          "y": 597.374634,
          "value": 597.376,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.3229394020819409,
          "y": 597.376,
          "value": 597.376,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 1.3456172366742238,
          "y": 597.375549,
          "value": 597.371,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.6069349839892926,
          "y": 597.371,
          "value": 597.371,
          "valueType": null,
          "y2": null,
          "value2": null
        }
      ]
    },
    {
      "type": "lowestPass",
      "data": [
        {
          "cellType": 1,
          "x": 0.0,
          "y": 597.353,
          "value": 597.353,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.085205803116539316,
          "y": 597.3581,
          "value": 597.359,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.10001382450880753,
          "y": 597.359,
          "value": 597.359,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.11482184590107573,
          "y": 597.36084,
          "value": 597.386,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.31995441102981748,
          "y": 597.386,
          "value": 597.386,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.52508697615855926,
          "y": 597.3832,
          "value": 597.382,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.60891033073067113,
          "y": 597.382,
          "value": 597.382,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.692733685302783,
          "y": 597.3828,
          "value": 597.384,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.81404289585963441,
          "y": 597.384,
          "value": 597.384,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.93535210641648581,
          "y": 597.376,
          "value": 597.364,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.117806836953072,
          "y": 597.364,
          "value": 597.364,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 1.300261567489658,
          "y": 597.374634,
          "value": 597.376,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.3229394020819409,
          "y": 597.376,
          "value": 597.376,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 1.3456172366742238,
          "y": 597.375549,
          "value": 597.371,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.6069349839892926,
          "y": 597.371,
          "value": 597.371,
          "valueType": null,
          "y2": null,
          "value2": null
        }
      ]
    },
    {
      "type": "lastComposite",
      "data": [
        {
          "cellType": 1,
          "x": 0.0,
          "y": 597.1041,
          "value": 597.1041,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.085205803116539316,
          "y": 597.1135,
          "value": 597.1152,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.10001382450880753,
          "y": 597.1152,
          "value": 597.1152,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.11482184590107573,
          "y": 597.115234,
          "value": 597.1158,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.31995441102981748,
          "y": 597.1158,
          "value": 597.1158,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.52508697615855926,
          "y": 597.11676,
          "value": 597.1172,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.60891033073067113,
          "y": 597.1172,
          "value": 597.1172,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.692733685302783,
          "y": 597.1214,
          "value": 597.127441,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.81404289585963441,
          "y": 597.127441,
          "value": 597.127441,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.93535210641648581,
          "y": 597.128,
          "value": 597.128845,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.117806836953072,
          "y": 597.128845,
          "value": 597.128845,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 1.300261567489658,
          "y": 597.138062,
          "value": 597.1392,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.3229394020819409,
          "y": 597.1392,
          "value": 597.1392,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 1.3456172366742238,
          "y": 597.139343,
          "value": 597.1405,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.6069349839892926,
          "y": 597.1405,
          "value": 597.1405,
          "valueType": null,
          "y2": null,
          "value2": null
        }
      ]
    },
    {
      "type": "cmvSummary",
      "data": []
    },
    {
      "type": "cmvDetail",
      "data": []
    },
    {
      "type": "cmvPercentChange",
      "data": []
    },
    {
      "type": "mdpSummary",
      "data": []
    },
    {
      "type": "temperatureSummary",
      "data": []
    },
    {
      "type": "speedSummary",
      "data": [
        {
          "cellType": 1,
          "x": 0.0,
          "y": 597.396,
          "value": 11.34,
          "valueType": 0,
          "y2": null,
          "value2": 21.744
        },
        {
          "cellType": 0,
          "x": 0.085205803116539316,
          "y": 597.388367,
          "value": 10.224,
          "valueType": 0,
          "y2": null,
          "value2": 11.844
        },
        {
          "cellType": 1,
          "x": 0.10001382450880753,
          "y": 597.387,
          "value": 10.224,
          "valueType": 0,
          "y2": null,
          "value2": 11.844
        },
        {
          "cellType": 0,
          "x": 0.11482184590107573,
          "y": 597.386963,
          "value": 11.34,
          "valueType": 0,
          "y2": null,
          "value2": 11.34
        },
        {
          "cellType": 1,
          "x": 0.31995441102981748,
          "y": 597.386,
          "value": 11.34,
          "valueType": 0,
          "y2": null,
          "value2": 11.34
        },
        {
          "cellType": 0,
          "x": 0.52508697615855926,
          "y": 597.3832,
          "value": 11.34,
          "valueType": 0,
          "y2": null,
          "value2": 11.34
        },
        {
          "cellType": 1,
          "x": 0.60891033073067113,
          "y": 597.382,
          "value": 11.34,
          "valueType": 0,
          "y2": null,
          "value2": 11.34
        },
        {
          "cellType": 0,
          "x": 0.692733685302783,
          "y": 597.3828,
          "value": 10.224,
          "valueType": 0,
          "y2": null,
          "value2": 10.224
        },
        {
          "cellType": 1,
          "x": 0.81404289585963441,
          "y": 597.384,
          "value": 10.224,
          "valueType": 0,
          "y2": null,
          "value2": 10.224
        },
        {
          "cellType": 0,
          "x": 0.93535210641648581,
          "y": 597.376,
          "value": 10.224,
          "valueType": 0,
          "y2": null,
          "value2": 12.204
        },
        {
          "cellType": 1,
          "x": 1.117806836953072,
          "y": 597.364,
          "value": 10.224,
          "valueType": 0,
          "y2": null,
          "value2": 12.204
        },
        {
          "cellType": 0,
          "x": 1.300261567489658,
          "y": 597.374634,
          "value": 10.224,
          "valueType": 0,
          "y2": null,
          "value2": 12.204
        },
        {
          "cellType": 1,
          "x": 1.3229394020819409,
          "y": 597.376,
          "value": 10.224,
          "valueType": 0,
          "y2": null,
          "value2": 12.204
        },
        {
          "cellType": 0,
          "x": 1.3456172366742238,
          "y": 597.375549,
          "value": 10.224,
          "valueType": 0,
          "y2": null,
          "value2": 12.204
        },
        {
          "cellType": 1,
          "x": 1.6069349839892926,
          "y": 597.371,
          "value": 10.224,
          "valueType": 0,
          "y2": null,
          "value2": 12.204
        }
      ]
    },
    {
      "type": "passCountSummary",
      "data": [
        {
          "cellType": 1,
          "x": 0.0,
          "y": 597.396,
          "value": 3.0,
          "valueType": 0,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.085205803116539316,
          "y": 597.388367,
          "value": 2.0,
          "valueType": 0,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.10001382450880753,
          "y": 597.387,
          "value": 2.0,
          "valueType": 0,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.11482184590107573,
          "y": 597.386963,
          "value": 1.0,
          "valueType": 0,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.31995441102981748,
          "y": 597.386,
          "value": 1.0,
          "valueType": 0,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.52508697615855926,
          "y": 597.3832,
          "value": 1.0,
          "valueType": 0,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.60891033073067113,
          "y": 597.382,
          "value": 1.0,
          "valueType": 0,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.692733685302783,
          "y": 597.3828,
          "value": 1.0,
          "valueType": 0,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.81404289585963441,
          "y": 597.384,
          "value": 1.0,
          "valueType": 0,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.93535210641648581,
          "y": 597.376,
          "value": 2.0,
          "valueType": 0,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.117806836953072,
          "y": 597.364,
          "value": 2.0,
          "valueType": 0,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 1.300261567489658,
          "y": 597.374634,
          "value": 2.0,
          "valueType": 0,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.3229394020819409,
          "y": 597.376,
          "value": 2.0,
          "valueType": 0,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 1.3456172366742238,
          "y": 597.375549,
          "value": 2.0,
          "valueType": 0,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.6069349839892926,
          "y": 597.371,
          "value": 2.0,
          "valueType": 0,
          "y2": null,
          "value2": null
        }
      ]
    },
    {
      "type": "passCountDetail",
      "data": [
        {
          "cellType": 1,
          "x": 0.0,
          "y": 597.396,
          "value": 3.0,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.085205803116539316,
          "y": 597.388367,
          "value": 2.0,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.10001382450880753,
          "y": 597.387,
          "value": 2.0,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.11482184590107573,
          "y": 597.386963,
          "value": 1.0,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.31995441102981748,
          "y": 597.386,
          "value": 1.0,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.52508697615855926,
          "y": 597.3832,
          "value": 1.0,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.60891033073067113,
          "y": 597.382,
          "value": 1.0,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.692733685302783,
          "y": 597.3828,
          "value": 1.0,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.81404289585963441,
          "y": 597.384,
          "value": 1.0,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.93535210641648581,
          "y": 597.376,
          "value": 2.0,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.117806836953072,
          "y": 597.364,
          "value": 2.0,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 1.300261567489658,
          "y": 597.374634,
          "value": 2.0,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.3229394020819409,
          "y": 597.376,
          "value": 2.0,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 1.3456172366742238,
          "y": 597.375549,
          "value": 2.0,
          "valueType": null,
          "y2": null,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.6069349839892926,
          "y": 597.371,
          "value": 2.0,
          "valueType": null,
          "y2": null,
          "value2": null
        }
      ]
    },
    {
      "type": "cutFill",
      "data": [
        {
          "cellType": 1,
          "x": 0.0,
          "y": 597.1041,
          "value": -0.3338623,
          "valueType": null,
          "y2": 597.4387,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.085205803116539316,
          "y": 597.1135,
          "value": -0.326843262,
          "valueType": null,
          "y2": 597.4384,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.10001382450880753,
          "y": 597.1152,
          "value": -0.326843262,
          "valueType": null,
          "y2": 597.438354,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.11482184590107573,
          "y": 597.115234,
          "value": -0.3222046,
          "valueType": null,
          "y2": 597.4383,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.31995441102981748,
          "y": 597.1158,
          "value": -0.3222046,
          "valueType": null,
          "y2": 597.4375,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.52508697615855926,
          "y": 597.11676,
          "value": -0.317810059,
          "valueType": null,
          "y2": 597.4367,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.60891033073067113,
          "y": 597.1172,
          "value": -0.317810059,
          "valueType": null,
          "y2": 597.43634,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.692733685302783,
          "y": 597.1214,
          "value": -0.310546875,
          "valueType": null,
          "y2": 597.436035,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 0.81404289585963441,
          "y": 597.127441,
          "value": -0.310546875,
          "valueType": null,
          "y2": 597.4356,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 0.93535210641648581,
          "y": 597.128,
          "value": -0.305175781,
          "valueType": null,
          "y2": 597.435364,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.117806836953072,
          "y": 597.128845,
          "value": -0.305175781,
          "valueType": null,
          "y2": 597.435059,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 1.300261567489658,
          "y": 597.138062,
          "value": -0.2998047,
          "valueType": null,
          "y2": 597.434753,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.3229394020819409,
          "y": 597.1392,
          "value": -0.2998047,
          "valueType": null,
          "y2": 597.434753,
          "value2": null
        },
        {
          "cellType": 0,
          "x": 1.3456172366742238,
          "y": 597.139343,
          "value": -0.294494629,
          "valueType": null,
          "y2": 597.4347,
          "value2": null
        },
        {
          "cellType": 1,
          "x": 1.6069349839892926,
          "y": 597.1405,
          "value": -0.294494629,
          "valueType": null,
          "y2": "NaN",
          "value2": null
        }
      ]
    },
    {
      "type": "summaryVolumes",
      "data": []
    }
  ],
  "Code": 0,
  "Message": "success"
}
"""
	
