<!--  ABOUT  THE  PROJECT  -->

##  About  The  Project

Transporter is an open-source data transferring tool developed by Trendyol with love ![:orange_heart:](https://a.slack-edge.com/production-standard-emoji-assets/13.0/apple-medium/1f9e1.png).

#### The Why?

We developed this tool because we needed to archive our high throughput tables. In other words; we need to move our table data from one database to another. Some of our tables have a daily throughput of more than 1 million. We use these tables constantly. To achieve high performance, we need to archive the data we do not need actively. Some data is stale after a month, some after 6 months. There are a lot of tools out there that do just that, but they are not very flexible or extensible. So we developed our own tool, which has proven to be very useful, and we decided to share it with the community.



#### The How?

Transporter has two important concepts; Source and Target. As inferred by names our goal is to move data from Source to Target. Each transporting operation corresponds to a json object. Using Quartz from each json object a cron job is produced.


![Transporter](images/Transporter.png)
​

There are two jobs that run in parallel; Polling Job and Transfer Job. 

**Polling Job Steps**

1. Select IDs from source table according to given conditions
2. Write selected IDs to interim table

**Transfer Job Steps**

1. Select IDs from the interim table.
2. Select data from the source table that corresponds to selected IDs.
3. Write data to the target. 
4. Delete transferred data from the source table.
5. Delete transferred data from the interim respectively. 

In database processes, data safety is our first concern. Since we delete data only after it is written to target, we make sure that even if Transporter fails in any of the transferring steps, data would still be protected.​

###  Features

-  Moves  data  from  source  database  to  target  database

-  Performs  transporting  on  given  time  intervals
 
-   Dockerizability: Can work in a container.
    
-   Extensibility: Adding support for a new database type is easy as it requires writing only one adapter.
    
-   Configurability: The user can specify many settings such as the “where” clause, cron job interval, excluded columns, and batch quantity. 
    
-   Scalability: The project can arrange the pod number according to throughput since it has docker support.
    
-   Interoperability: The project can transfer data mutually between different database types

 |              | Couchbase   |  MSSQL     |
| :---         |    :----:   |     :----: |
| **Couchbase**| **✓**       | **✓**      |
| **MSSQL**    | **✓**       |  **✓**     |

-   Schedulability: Can work at the given interval.



### Configs

Explanation of some main properties of config properties. Please note that the config properties are case sensitive. Example can be found in “examples/configs” folder. 

- Name: Must be unique. 
- Type: Specifies database type. You can give "Couchbase" or "mssql" for now. 
- Cron: Specifies the interval that transporter will work on. For example: "0/20 * * ? * *"
- Condition: Condition that specifies which data is to be transferred
- KeyProperty: When transferring data from Mssql to Couchbase you can select the key of Couchbase that corresponds to any MSSQL column, not just Id property. If you are transferring data from Couchbase to Couchbase you must use "id" like in the examples.
  
  
### Noteworthy Mentions 

For Couchbase: When generating a key we use id and name of data source. Example: {id}_{dataSourceName}
For mssql: Unique Index must be created for id and data source name on interim table. 


<!--  GETTING  STARTED  -->

## Getting Started

To use Transporter, there are config examples in "examples/configs" folder. Transporter is a dockerized project, so you can deploy it to a Kubernetes cluster if you want. 
There are two string config keys that should be given to Transporter, "PollingJobSettings" and "TransferJobSettings" as strings. You can use online sites like [JSON Online Converter](https://tools.knowledgewalls.com/jsontostring) to convert your JSON file to string.

​

## Roadmap

See the [open issues]([https://github.com/github_username/repo_name/issues](https://github.com/github_username/repo_name/issues)) for a list of proposed features (and known issues).

​

 <!--  CONTRIBUTING  -->

## Contributing

Currently project is not open to contributions.
​

 <!--  LICENSE  -->

## License

Distributed under the MIT License. See [`LICENSE`](https://choosealicense.com/licenses/mit/) for more information.

​

 <!--  CONTACT  -->

## Contact

Fatiha Beqirovski Polattimur - [Github](https://github.com/FatihaBeqirovski) - [fatiha.beqirovski@trendyol.com](mailto:fatiha.beqirovski@trendyol.com)


Mehmet Fırat Kömürcü - [Github](https://github.com/MehmetFiratKomurcu) - [firat.komurcu@trendyol.com](mailto:firat.komurcu@trendyol.com)
