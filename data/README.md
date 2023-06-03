# Folder: data_format_version_* (required)
- "*" can be anything. An unsigned integer or any kind of string as a more meaningful name (e.g. data_format_version_1, data_format_version_Short-AssetId).
- a new module version may require a new data format. Everytime this happens, a new data_format_version_* folder has to be created. A new data format does not mean that updated data is available but that the way the data is structured has changed.
- The module version determines which data_format_version_* folder the module has to check to get its data. The module can ignore other (older, newer) data_format_version_* folders.
- DONT remove old data_format_version_* folders. They are still necessary too keep old module versions running.

# File: data_format_version_*/data_content_version.txt (required)
- contains an unsigned integer
- this file allows the module to check if new data is available. This way the module can cache the data locally to avoid the need to download the data every time the module is started. If the number in this file is bigger than the number the module has stored locally, it can trigger a download of the new data to replace the local outdated data.

# File: data_format_version_*/deprecated.txt (required when data is removed)
- when this file exists:
  - the module has to inform the user that a module version update is required. This may require a Blish HUD version update too, because newer module versions may rely on a newer Blish HUD version.
  - optionally this file can contain a message that the module can display to the user for this purpose.
  - the module must assume that no data is available in the data_format_version_* folder anymore.
  - it is now safe to remove any other folders and files from the data_format_version_* folder. This includes the data_content_version.txt file. This can be useful to free up  space when there was a lot of data.