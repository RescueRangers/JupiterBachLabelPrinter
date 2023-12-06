# JupiterBachLabelPrinter
Print labels using raw ZPL sent over tcp/ip

Label Data is read from `CsvData` directory, name of the Csv file matters it is structured as follows

`MasterSetName` - `MasterSetNumber`

## Csv file data structure
Data in the Csv file is structured as bvelow:
`SetName`;`SetNumber`;`Material`;`ItemNumber`

This is done this way because of the possibility of the more than one set in a single nesting file.

The data is supposed to be orered sequentially as in the nesting file to easy in production.
