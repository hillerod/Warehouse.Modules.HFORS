# Warehouse module: HFORS

This module connects to 'Hillerød Forsyning' through a FTPS connection and downloads all csv-files, containing information about heat consumption.
The csv from the FTP is not a correct CSV, but in a format called `EK109`, invented by KMD. So the module first have to convert the file into correct CSV.
All the data, then gets merged into the database in the Warehouse. It also gets saved into the datalake in the raw format and refined.

## Installation

All modules can be installed and facilitated with ARM templates (Azure Resource Management): [Use ARM templates to setup and maintain this module](https://github.com/hillerod/Warehouse.Modules.HFORS/blob/master/Deploy).

## Database content

| TABLE_NAME      | COLUMN_NAME                               | DATA_TYPE      |
| :-------------- | :---------------------------------------- | :------------- |
| Artskode        | Beskrivelse                               | varchar        |
| Artskode        | Kode                                      | int            |
| Artskode        | Målepunktsart                             | varchar        |
| Energiartskode  | Beskrivelse                               | varchar        |
| Energiartskode  | Energiart                                 | varchar        |
| Energiartskode  | Kode                                      | int            |
| Enhedskode      | Beskrivelse                               | varchar        |
| Enhedskode      | Enhed                                     | varchar        |
| Enhedskode      | Enhedskode                                | varchar        |
| FilerImporteret | AflæstMax                                 | datetimeoffset |
| FilerImporteret | AflæstMin                                 | datetimeoffset |
| FilerImporteret | Fil                                       | varchar        |
| FilerImporteret | FilGemt                                   | datetimeoffset |
| FilerImporteret | Oprettelsesdato                           | datetimeoffset |
| Forbrug         | Aflæst                                    | datetimeoffset |
| Forbrug         | Energi_Enhed                              | varchar        |
| Forbrug         | Energi_Type                               | int            |
| Forbrug         | Energi_Værdi                              | float          |
| Forbrug         | Energiartskode                            | varchar        |
| Forbrug         | Fremløb_Enhed                             | varchar        |
| Forbrug         | Fremløb_Type                              | int            |
| Forbrug         | Fremløb_Værdi                             | float          |
| Forbrug         | GældendeFra                               | varchar        |
| Forbrug         | Id                                        | varchar        |
| Forbrug         | Installation                              | varchar        |
| Forbrug         | Målernummer                               | int            |
| Forbrug         | Note                                      | varchar        |
| Forbrug         | Returløb_Enhed                            | varchar        |
| Forbrug         | Returløb_Type                             | int            |
| Forbrug         | Returløb_Værdi                            | float          |
| Forbrug         | Timer_Enhed                               | varchar        |
| Forbrug         | Timer_Type                                | int            |
| Forbrug         | Timer_Værdi                               | float          |
| Forbrug         | Volumen_Enhed                             | varchar        |
| Forbrug         | Volumen_Type                              | int            |
| Forbrug         | Volumen_Værdi                             | float          |
| ForbrugPrDag    | Energi_Værdi                              | float          |
| ForbrugPrDag    | Fra                                       | datetimeoffset |
| ForbrugPrDag    | Fremløb_Værdi                             | float          |
| ForbrugPrDag    | Id                                        | varchar        |
| ForbrugPrDag    | Målernummer                               | int            |
| ForbrugPrDag    | Returløb_Værdi                            | float          |
| ForbrugPrDag    | Til                                       | datetimeoffset |
| ForbrugPrDag    | Volumen_Værdi                             | float          |
| ForbrugPrMåned  | Energi_Værdi                              | float          |
| ForbrugPrMåned  | Fra                                       | datetimeoffset |
| ForbrugPrMåned  | Fremløb_Værdi                             | float          |
| ForbrugPrMåned  | Id                                        | varchar        |
| ForbrugPrMåned  | Målernummer                               | int            |
| ForbrugPrMåned  | Returløb_Værdi                            | float          |
| ForbrugPrMåned  | Til                                       | datetimeoffset |
| ForbrugPrMåned  | Volumen_Værdi                             | float          |
| ForbrugPrTime   | Energi_Værdi                              | float          |
| ForbrugPrTime   | Fra                                       | datetimeoffset |
| ForbrugPrTime   | Fremløb_Værdi                             | float          |
| ForbrugPrTime   | Id                                        | varchar        |
| ForbrugPrTime   | Målernummer                               | int            |
| ForbrugPrTime   | Returløb_Værdi                            | float          |
| ForbrugPrTime   | Til                                       | datetimeoffset |
| ForbrugPrTime   | Volumen_Værdi                             | float          |
| Målere          | aftagernummer                             | bigint         |
| Målere          | device.devicekey                          | varchar        |
| Målere          | device.installDate                        | varchar        |
| Målere          | device.location.city                      | varchar        |
| Målere          | device.location.consumerContact.contactId | int            |
| Målere          | device.location.floor                     | varchar        |
| Målere          | device.location.lat                       | decimal        |
| Målere          | device.location.lng                       | decimal        |
| Målere          | device.location.postcode                  | int            |
| Målere          | device.location.room                      | varchar        |
| Målere          | device.location.street                    | varchar        |
| Målere          | device.location.streetnumber              | int            |
| Målere          | device.location.streetnumberadd           | varchar        |
| Målere          | device.manufacturer                       | varchar        |
| Målere          | device.serialNo                           | int            |
| Målere          | device.version                            | int            |
| Målere          | Indlæst                                   | datetime       |
| Målere          | Qn                                        | int            |
| Typekode        | Beskrivelse                               | varchar        |
| Typekode        | Kode                                      | int            |
| Typekode        | Type                                      | varchar        |



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
                - Forbrug.csv
                - ForbrugPrDag.csv
                - ForbrugPrMåned.csv
                - ForbrugPrTime.csv

# License

[MIT License](https://github.com/Bygdrift/Warehouse.Modules.Example/blob/master/License.md)