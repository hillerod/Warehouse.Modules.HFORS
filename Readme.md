# Warehouse module: HFORS

This module connects to 'Hillerød Forsyning' through a FTPS connection and downloads all csv-files, containing information about heat consumption. It is not saved in a normal CSV-format, but in a weird EK109-format from KMD, so the module, first have to convert the file into real CSV. All the data, then gets merged into a database in the Warehouse.

## Installation

All modules can be installed and facilitated with ARM templates (Azure Resource Management): [Use ARM templates to setup and maintain this module](https://github.com/Bygdrift/Warehouse.Modules.Example/blob/master/Deploy).

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

## Data lake content

In the data lake container with this modules name, there are two main folders `Raw` and `Refined`.

 The folder structure:

+ Raw
    - {yyyy the year}
        - {MM the month}
            - {dd the day}
                - TheNameOfTheCsvFile.csv
+ Refined
    - {yyyy the year}
        - {MM the month}
            - {dd the day}
                - TheNameOfTheCsvFile.csv

# License

[MIT License](https://github.com/Bygdrift/Warehouse.Modules.Example/blob/master/License.md)