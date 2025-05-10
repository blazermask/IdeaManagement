# Idea Management System

A console application for managing ideas with MySQL database storage and secure Windows credential management.

## Features

- 🔐 Secure database credential management using Windows Credential Manager
- 🔄 Automatic login with saved credentials
- 📝 CRUD operations for ideas
- 🔢 Automatic ID management with lowest available ID assignment
- 🔄 ID reordering functionality
- 🎯 Single executable deployment

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- MySQL Server 8.0 or higher
- Windows OS (for Credential Manager functionality)

## Installation

1. Clone the repository
```bash
git clone https://github.com/yourusername/idea-management.git
cd idea-management
```

2. Build the project
```bash
dotnet publish src/IdeaManagement/IdeaManagement.csproj --configuration Release --runtime win-x64 --self-contained true --output "dist" -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=embedded
```

The executable will be created in the `dist` folder.

## Usage

### First Run
1. Run `IdeaManagement.exe` from the dist folder
2. Enter your database credentials:
   - Server
   - Port
   - Database name
   - Username
   - Password
3. Choose whether to save credentials securely

### Main Features

#### Quick Idea Creation Mode (Default)
- Enter idea content directly
- Type 'EDIT' to access main menu
- Type 'EXIT' to close program

#### Main Menu Options
1. Create new idea
2. View all ideas
3. Update idea
4. Delete idea
5. Reorder all IDs
6. Remove all ideas
7. Logout
8. Exit

## Common Operations

### Creating an Idea
```
Enter your idea: My new idea
Idea created with ID: 1
```

### Updating an Idea
```
Enter idea ID to update: 1
Current idea details:
ID: 1
Content: My new idea
Created: [timestamp]
Modified: [timestamp]

Options:
(C)ancel - Cancel update
(E)dit - Edit content
(R)eidentify - Change idea ID
```

### Reordering IDs
```
Reorder IDs Operation
This will reorganize all idea IDs sequentially.
Example:
Current IDs:  1, 2, 4, 7
New IDs:      1, 2, 3, 4
```

## Security Features

- Secure credential storage using Windows Credential Manager
- Hidden password input
- Encrypted credential storage
- Secure credential removal option

## Error Handling

The application handles:
- Database connection failures
- Invalid input
- Duplicate IDs
- Missing records
- Credential management errors

## Best Practices

1. Always use the logout option instead of directly closing
2. Regularly backup your database
3. Keep track of your database credentials
4. Test new features in a test database first

## Project Structure

```
IdeaManagement/
├── src/
│   └── IdeaManagement/
│       ├── Models/
│       │   ├── DatabaseCredentials.cs
│       │   └── Idea.cs
│       ├── Data/
│       │   └── IdeaDbContext.cs
│       ├── Repositories/
│       │   ├── IIdeaRepository.cs
│       │   └── IdeaRepository.cs
│       ├── Services/
│       │   └── DatabaseCredentialManager.cs
│       ├── GlobalUsings.cs
│       ├── Program.cs
│       └── IdeaManagement.csproj
└── tests/
    └── IdeaManagement.Tests/
        ├── IdeaRepositoryTests.cs
        └── TestHelper.cs
```

## Development

### Building for Development
```bash
dotnet build
```

### Running Tests
```bash
dotnet test
```

### Building Release Version
```bash
# Clean previous builds
rmdir /s /q "dist"

# Create distribution
dotnet publish src/IdeaManagement/IdeaManagement.csproj --configuration Release --runtime win-x64 --self-contained true --output "dist" -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=embedded
```

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Acknowledgments

- Entity Framework Core team
- MySQL team
- .NET community

---
For issues and feature requests, please [open an issue](https://github.com/blazermask/IdeaManagement/issues)
