# V1 Data Migration

At the beginning of 2024, Rodel Agent launched its V1 version, originally hosted at [Richasy/RichasyAssistant](https://github.com/Richasy/RichasyAssistant).

During that period, AI services were booming, and the industry had not yet established relatively uniform standards.

The initial design of Rodel Agent at that time had significant issues in its data structure due to inadequate consideration, making it difficult to adapt to the current AI services. Therefore, I restructured the entire application and migrated it to V2.

This resulted in the data from the old version not being compatible with the new version.

Rodel Agent V2 version includes a built-in migration tool to help V1 version users migrate to V2.

After downloading the new version, users of the old version can open the working directory created by the old version.

If the `_secret_.db` file is found in the directory, Rodel Agent will begin the migration process, primarily migrating the following content:

1. Key data
2. Chat history
3. Generated images

The following data will not be migrated due to functional reasons:

1. Local model configuration (replaced by Ollama)
2. Assistant (replaced by session presets and new version agents)
3. Translation records

---

Upon completion of the migration, Rodel Agent will restart the app. After restarting, Rodel Agent will package the V1 data content into `v1_backup.zip` and then transform the working directory into the V2 structure.