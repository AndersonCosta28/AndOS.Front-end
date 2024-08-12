# AndOS.Front-end

## Description

AndOS.Front-end is a Blazor WebAssembly project developed to act as the front-end of a web operating system. It supports multiple users and tasks, with file storage in the cloud through services like Azure Blob Storage, GCP Storage, and AWS S3.

## Main Features

- **Multi-user Support**: Support for multiple simultaneous users.
- **Multi-tasking**: Ability to execute several tasks at the same time.
- **Cloud Storage**: Use of cloud storage services to store files.
- **Access Authentication**: Verification of access permissions via API before allowing file downloads or uploads.
- **Extensibility**: Capability to build extendable programs that can be installed or uninstalled during runtime.
- **Binary Storage**: Storing binary program files in the browser's IndexedDB to leverage its large available space.

## Technologies

- .NET 8
- Blazor WebAssembly

### Prerequisites

* SDK .NET 8
* [AndOS.Back-end](https://github.com/AndersonCosta28/AndOS.Back-end)

### Steps to Run the Project

1. **Clone Necessary Repositories**: Clone the following repositories and place them at the same level as the main project.
    * [AndOS.Shared](https://github.com/AndersonCosta28/AndOS.Shared)
    * [AndOS.Core](https://github.com/AndersonCosta28/AndOS.Core)