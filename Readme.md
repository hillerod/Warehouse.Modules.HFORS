# Warehouse module: HFORS

This module connects to 'Hillerød Forsyning' through a FTPS connection and downloads all csv-files, containing information about heat consumption.
The csv from the FTP is not a correct CSV, but in a format called `EK109`, invented by KMD. So the module first have to convert the file into correct CSV.
All the data, then gets merged into the database in the Warehouse. It also gets saved into the datalake in the raw format and refined.

## Installation

All modules can be installed and facilitated with ARM templates (Azure Resource Management): [Use ARM templates to setup and maintain this module](https://github.com/hillerod/Warehouse.Modules.HFORS/blob/master/Deploy).

## Database content

| TABLE_NAME     | COLUMN_NAME    | DATA_TYPE |
| :------------- | :------------- | :-------- |
| Forbrug        | Id             | varchar   |
| Forbrug        | Installation   | varchar   |
| Forbrug        | Målernummer    | int       |
| Forbrug        | Energiartskode | varchar   |
| Forbrug        | Aflæst         | datetime  |
| Forbrug        | GældendeFra    | varchar   |
| Forbrug        | Note           | varchar   |
| Forbrug        | Energi_Værdi   | float     |
| Forbrug        | Energi_Enhed   | varchar   |
| Forbrug        | Energi_Type    | int       |
| Forbrug        | Volumen_Værdi  | float     |
| Forbrug        | Volumen_Enhed  | varchar   |
| Forbrug        | Volumen_Type   | int       |
| Forbrug        | Timer_Værdi    | float     |
| Forbrug        | Timer_Enhed    | varchar   |
| Forbrug        | Timer_Type     | int       |
| Forbrug        | Fremløb_Værdi  | float     |
| Forbrug        | Fremløb_Enhed  | varchar   |
| Forbrug        | Fremløb_Type   | int       |
| Forbrug        | Returløb_Værdi | float     |
| Forbrug        | Returløb_Enhed | varchar   |
| Forbrug        | Returløb_Type  | int       |
| Typekode       | Kode           | int       |
| Typekode       | Type           | varchar   |
| Typekode       | Beskrivelse    | varchar   |
| Energiartskode | Kode           | int       |
| Energiartskode | Energiart      | varchar   |
| Energiartskode | Beskrivelse    | varchar   |
| Artskode       | Kode           | int       |
| Artskode       | Målepunktsart  | varchar   |
| Artskode       | Beskrivelse    | varchar   |
| Enhedskode     | Enhedskode     | varchar   |
| Enhedskode     | Enhed          | varchar   |
| Enhedskode     | Beskrivelse    | varchar   |
| Målere     | device.serialNo                           | int       |
| Målere     | Qn                                        | int       |
| Målere     | device.devicekey                          | varchar   |
| Målere     | device.version                            | int       |
| Målere     | device.manufacturer                       | varchar   |
| Målere     | device.location.street                    | varchar   |
| Målere     | device.location.streetnumber              | int       |
| Målere     | device.location.streetnumberadd           | varchar   |
| Målere     | device.location.floor                     | varchar   |
| Målere     | device.location.room                      | varchar   |
| Målere     | device.location.postcode                  | int       |
| Målere     | device.location.city                      | varchar   |
| Målere     | device.location.lng                       | decimal   |
| Målere     | device.location.lat                       | decimal   |
| Målere     | device.installDate                        | varchar   |
| Målere     | device.location.consumerContact.contactId | int       |
| Målere     | aftagernummer                             | bigint    |
| Målere     | Indlæst                                   | datetime  |


## Data lake content

In the data lake container with this modules name, there are two main folders `Raw` and `Refined`.

 The folder structure:

+ Raw
    - {yyyy the year}
        - {MM the month}
            - {dd the day}
                - Consumption data.csv
                - Customer data.csv
+ Refined
    - {yyyy the year}
        - {MM the month}
            - {dd the day}
                - Consumption data.csv

# License

[MIT License](https://github.com/Bygdrift/Warehouse.Modules.Example/blob/master/License.md)