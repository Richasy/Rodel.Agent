# FAQ

## I can't configure the service (the service setup page is blank)

When the app initializes, it attempts to copy the blank database file in the app to the working directory, but in a few rare cases, there may be an issue where the copy fails.

If the necessary database is not copied to the working directory, you will observe that the app is unable to configure various AI services, resulting in a blank page for the relevant service on the settings page.

In this case, you need to manually download the database file and copy it to the working directory.

The specific steps are as follows:

1. Download [database.zip](https://github.com/Richasy/Rodel.Agent/releases/download/empty-db/database.zip) to your local machine.
2. Extract the database file (with the `.db` extension) from the zip file to your working directory.
3. Restart the application.