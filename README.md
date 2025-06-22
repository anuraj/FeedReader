# FeedReader

## Description

FeedReader is an ASP.NET Core web application that allows users to manage and monitor RSS/Atom feeds. The application provides functionality to add RSS feed URLs, validate them, and automatically fetch feed items on a scheduled basis. Built with .NET 9, Entity Framework Core, and Coravel for background job scheduling, it offers a clean web interface for feed management with breadcrumb navigation and health checks.

## Project Structure

```
/FeedReader/
├── src/
│   ├── Controllers/
│   │   └── HomeController.cs
│   ├── Data/
│   │   └── FeedReaderWebDbContext.cs
│   ├── Models/
│   │   ├── FeedEntity.cs
│   │   └── FeedItemEntity.cs
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Services/
│   │   ├── FeedFetcher.cs
│   │   └── FeedsFetcher.cs
│   ├── ViewModels/
│   │   └── CreateFeedRequest.cs
│   ├── appsettings.json
│   ├── FeedReader.Web.csproj
│   └── Program.cs
├── LICENSE
└── README.md
```

## Key Classes and Interfaces

### Models
- **FeedEntity**: Represents a feed with properties like URL, title, description, and timestamps
- **FeedItemEntity**: Represents individual feed items with content, links, categories, and metadata

### Services
- **FeedFetcher**: Background service implementing `IInvocable` and `IInvocableWithPayload<FeedEntity>` for fetching individual feeds
- **FeedsFetcher**: Bulk feed fetching service that updates all feeds older than one hour

### Controllers
- **HomeController**: Main controller handling feed CRUD operations, validation, and dashboard views

### ViewModels
- **CreateFeedRequest**: Request model with validation attributes for creating new feeds

### Data
- **FeedReaderWebDbContext**: Entity Framework DbContext for database operations

## Usage

### Prerequisites
- .NET 9 SDK
- SQL Server (configured via connection string)

### Setup
1. Clone the repository
2. Configure the database connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "FeedReaderDbConnection": "your-connection-string-here"
     }
   }
   ```
3. Run Entity Framework migrations to create the database schema
4. Build and run the application:
   ```bash
   dotnet run --project src/FeedReader.Web.csproj
   ```

### Features
- **Add Feeds**: Submit RSS/Atom feed URLs with real-time validation
- **Dashboard**: View all registered feeds with pagination
- **Feed Details**: Browse individual feed items with full content
- **Automatic Updates**: Background jobs fetch new feed items hourly
- **Health Checks**: Monitor application and database health at `/health`
- **Responsive UI**: Clean interface with breadcrumb navigation

### API Endpoints
- `GET /` - Home page with feed creation form
- `POST /home/create` - Create new feed
- `GET /home/dashboard` - View all feeds (paginated)
- `GET /home/details/{id}` - View specific feed details
- `GET /home/validate-feed-url` - AJAX endpoint for feed URL validation
- `GET /health` - Health check endpoint

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.