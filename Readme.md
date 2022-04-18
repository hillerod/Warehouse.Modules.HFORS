# Warehouse module: Example

This module is both a 'hello world' example and a start install, you can use for building a new module.

The module is build with [Bygdrift Warehouse](https://github.com/Bygdrift/Warehouse), that enables one to attach multiple modules within the same azure environment, that can collects and wash data from all kinds of services, in a cheap data lake and database.
By saving data to a MS SQL database, it is:
- easy to fetch data with Power BI, Excel and other systems
- easy to control who has access to what - actually, it can be controlled with AD so you don't have to handle credentials
- It's cheap

## Installation

All modules can be installed and facilitated with ARM templates (Azure Resource Management): [Use ARM templates to setup and maintain this module](https://github.com/Bygdrift/Warehouse.Modules.Example/blob/master/Deploy).

## Database content

| TABLE_NAME      | COLUMN_NAME     | DATA_TYPE |
| :-------------- | :-------------- | :-------- |
| Data            | Id              | int       |
| Data            | Text            | varchar   |
| Data            | Date            | datetime  |
| Data            | DataFromSetting | varchar   |
| Data            | DataFromSecret  | varchar   |
| DataAccumulated | Id              | int       |
| DataAccumulated | Text            | varchar   |
| DataAccumulated | Date            | datetime  |
| DataAccumulated | DataFromSetting | varchar   |
| DataAccumulated | DataFromSecret  | varchar   |

## Data lake content

In the data lake container with this modules name, there are three main folders `Drawings`, `Raw` and `Refined`.

 The folder structure:

+ Raw
    - {yyyy the year}
        - {MM the month}
            - {dd the month}
                - Data.txt
+ Refined
    - {yyyy the year}
        - {MM the month}
            - {dd the month}
                - Data.csv

# Updates

## 0.3.3

In 0.3.2, all user settings should have a prefix of 'Setting--'. That has been removed, so when upgrading from 0.3.2, then go this module's Configuration and find `Setting--DataFromSetting` and change it to `DataFromSetting`.

# License

[MIT License](https://github.com/Bygdrift/Warehouse.Modules.Example/blob/master/License.md)