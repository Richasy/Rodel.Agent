# Working Directory and Data Synchronization

## Working Directory

The working directory is a fundamental concept of Rodel Agent.

When you run Rodel Agent for the first time, it will ask you to provide an empty folder to serve as the working directory.

The application will store the data generated during its operation in this working directory, including but not limited to:

- Key data
- Chat/drawing/TTS/translation sessions
- Plugins
- Presets and assistant configurations
- Generated images and audio files

Rodel Agent does not provide data encryption for storage. Users are responsible for ensuring their data security.

## Data Synchronization

Rodel Agent itself does not offer data synchronization features, but if you need to sync data across multiple devices, it is recommended to use OneDrive.

You can save the working directory in OneDrive. Since Rodel Agent manages data based on files, any modifications to the files will be synced to the cloud.

OneDrive provides a relatively reliable synchronization mechanism, allowing you to access data as needed.

This solution is also applicable to other cloud drives that can be mounted locally.