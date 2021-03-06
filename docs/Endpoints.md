# HTTP Endpoints Description

## Tabular Model Processing

azure function name | HTTP Operation | endpoints | path parameters | body | description |
 ------------------ | -------------- | --------- | --------------- | ---- | ----------- | 
| ProcessModel | GET | `<host>://api/ProcessTabularModel/{databaseName}` | *databaseName*: Analysis service database| none | Process the full database model. Processes all the tables in the specified database.|
| ProcessTable | GET |`<host>://api/ProcessTabularModel/{databaseName}/{tableList}`| *databaseName*: Analysis service database hosting the table.  *tableList*: Single Table or list of comma seperated table names to process | none | Process the table(s) hosted in the specified analysis services dataset. Processes all the existing partitions in the specified table.|
| ProcessPartition | GET |`<host>://api/ProcessTabularModel/{databaseName}/tables/{tableName}/partitions/{partitionName}`| *databaseName*: Analysis service database hosting the table.   *tableName*: Table containing the partition.    *partitionName*: Name of the partition to process|none|Process the specified partition in the specified table and database.|
| ProcessModelAsync | GET | `<host>://api/ProcessTabularModel/{databaseName}/async` | *databaseName*: Analysis service database| none | Queues the request to process the specified database.|
| ProcessTableAsync | GET |`<host>://api/ProcessTabularModel/{databaseName}/{tableList}/async`| *databaseName*: Analysis service database hosting the table.  *tableList*: Single Table or list of comma seperated table names to process  | none | Queues a request to process the specified table(s) in the specified database. |
| ProcessPartitionAsync | GET |`<host>://api/ProcessTabularModel/{databaseName}/tables/{tableName}/partitions/{partitionName}/async`|*databaseName*: Analysis service database hosting the table.  *tableName*: Table containing the partition.  *partitionName*: Name of the partition to process|none|Queues a request to process the specified partition in the specified table and database.|


## Partition Creation

azure function name | HTTP Operation | endpoints | path parameters | body | description |
 ------------------ | -------------- | --------- | --------------- | ---- | ----------- | 
| CreateNewPartitions | POST |`<host>://api/ProcessTabularModel/{databaseName}/tables/{tableName}/partitions/new`|*databaseName*: Analysis service database hosting the table.  *tableName*: Table containing the partition.|*JSON Payload*   `[{"TableName": "name of the table where partition is to be created - informational only",    "PartitionName" : "name of the partition - required",   "SourceQuery" : "Query to extract data from the source for the partition - required"  }  ]`| Creates the partitions specified in the JSON payload in the specified table and database in URL path.    Note: The table name in JSON payload is for informational purpose only. The partitions specified in the JSON payload are created in the table specified in the path.|
| TableRepartitionByDate | POST |`<host>://api/ProcessTabularModel/{databaseName}/tables/{tableName}/repartition/{count}/bydate/{date?}`| *databaseName*: Analysis service database hosting the table.  *tableName*: Table containing the partition  *count*: Number of monthly partitions  *date*: Date of the last partition in yyyy-mm-dd format. (optional). Defaults to current date if not specified. | none | Creates the monthly partitions specified working back from the date specified in the specified table and database. Existing partitions are not impacted.|

## Merge Partitions

azure function name | HTTP Operation | endpoints | path parameters | body | description |
 ------------------ | -------------- | --------- | --------------- | ---- | ----------- | 
|MergePartitions|PUT|`<host>://api/ProcessTabularModel/{databaseName}/tables/{tableName}/merge`|*databaseName*: Analysis service database hosting the table  *tableName*: Table containing the partition | `{ "TableName" : "name of the table where partitions exist - informational only", "TargetPartition":  "TableName": "name of the table where new partition is created -informational only",        "PartitionName":"name of the partition - required",   "SourceQuery": "Query to extract data from the source for the partition - required" },   "SourcePartitionNames": ["Partion name 1 to be erged", "Partion name 2 to be merged"]  }` | Merges the specified partitions from list of partitions to the specified partition. The partition is created, if it does not exist.|

## See Also
 - [Application Design](./AppDesign.md)
