Here's a comprehensive README.md for the project:


# Idea Management System

A console application for managing ideas with persistent storage in MySQL database and secure credential management.

## Features

- Secure database credential management using Windows Credential Manager
- Automatic login with saved credentials
- CRUD operations for ideas
- ID management with automatic lowest ID assignment
- ID reordering functionality
- Complete test suite with in-memory and real database testing options

## Prerequisites

- .NET 8.0 SDK
- MySQL Server 8.0 or higher
- Windows OS (for Credential Manager functionality)

## Project Structure


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
        ├── TestHelper.cs
        └── IdeaManagement.Tests.csproj


## Installation

1. Clone the repository
2. Navigate to the project directory
3. Restore dependencies:
```bash
dotnet restore


Database Setup
1. Create a MySQL database
2. Ensure your MySQL user has the following permissions:
- CREATE, ALTER, DROP (for table management)
- SELECT, INSERT, UPDATE, DELETE (for data operations)


Running the Application

cd src/IdeaManagement
dotnet run


First Run
1. You'll be prompted for database credentials:
- Server
- Port
- Database name
- Username
- Password
2. Option to save credentials securely
3. Application will create necessary database tables


Main Features
1. **Idea Creation Mode** (Default)
- Enter idea content directly
- Type 'EDIT' to access main menu
- Type 'EXIT' to close program

2. **Main Menu Options**
- Create new idea
- View all ideas
- Update idea
- Delete idea
- Reorder all IDs
- Remove all ideas
- Logout
- Exit


Special Features
1. **ID Management**
- Automatic lowest available ID assignment
- Manual ID assignment option
- ID reordering functionality
- ID change capability

2. **Update Options**
- Edit content
- Change ID
- Cancel operation

3. **Credential Management**
- Secure credential storage
- Automatic login
- Option to remove saved credentials


Testing

Running Tests with In-Memory Database

cd tests/IdeaManagement.Tests
dotnet test


Running Tests with Real Database

dotnet test -- --connection="server=localhost;port=3306;database=test_db;user=test_user;password=test_pass"


Technical Details

Dependencies
• Microsoft.EntityFrameworkCore (8.0.3)
• Microsoft.EntityFrameworkCore.Design (8.0.3)
• Microsoft.EntityFrameworkCore.Tools (8.0.3)
• Pomelo.EntityFrameworkCore.MySql (8.0.0)


Test Dependencies
• Microsoft.EntityFrameworkCore.InMemory (8.0.3)
• Microsoft.NET.Test.Sdk (17.8.0)
• xunit (2.7.0)
• Moq (4.20.70)


Security Features
1. **Credential Storage**
- Uses Windows Credential Manager
- Encrypted storage
- Secure credential removal

2. **Database Security**
- Parameterized queries
- Connection string protection
- Error handling for security issues


Common Operations

Creating an Idea

Enter your idea: My new idea
Idea created with ID: 1


Updating an Idea

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


Reordering IDs

Reorder IDs Operation
This will reorganize all idea IDs sequentially based on their current order.
For example:
Current IDs:  1, 2, 4, 7
New IDs:      1, 2, 3, 4

Are you sure you want to reorder all IDs? (y/n):


Error Handling
• Database connection failures
• Invalid input validation
• Duplicate ID handling
• Missing record handling
• Credential management errors


Best Practices
1. Always use the logout option instead of directly closing the application
2. Regularly backup your database
3. Keep track of your database credentials
4. Test new features in a test database first


Contributing
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request
