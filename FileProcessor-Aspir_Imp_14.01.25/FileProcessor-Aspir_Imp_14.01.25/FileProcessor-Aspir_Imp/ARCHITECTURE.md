# Commerce Connector POC - Architecture Diagram

## System Overview

```
┌─────────────────────────────────────────────────────────────────────────────────────────────────────────┐
│                              EXTERNAL SYSTEMS & CONSUMERS                                              │
└─────────────────────────────────────────────────────────────────────────────────────────────────────────┘
                                            ▲
                                            │
                    ┌───────────────────────┼───────────────────────┐
                    │                       │                       │
                    ▼                       ▼                       ▼
        ┌────────────────────┐  ┌────────────────────┐  ┌────────────────────┐
        │   CommerceConnector│  │  FileProcessingAPI │  │   RabbitMQ Message │
        │   (Web/UI Layer)   │  │    (API Layer)     │  │      Broker        │
        │                    │  │                    │  │                    │
        │ - Home Controller  │  │ - FileProcessing   │  │  - Message Queue   │
        │ - Jobs Controller  │  │   Controller       │  │  - Event Bus       │
        │ - Mapping         │  │ - Job Endpoints    │  │  - Async Messaging │
        │   Controller      │  │ - File Endpoints   │  │                    │
        │ - Template        │  │                    │  │                    │
        │   Controller      │  │                    │  │                    │
        └────────┬───────────┘  └────────┬───────────┘  └────────┬───────────┘
                 │                       │                       │
                 │ HTTP Requests         │ REST API             │ Message Consumer
                 │ (Port 5280)           │ (Port 7185)          │ (FileProcessingConsumer)
                 │                       │                       │
                 └───────────────────────┼───────────────────────┘
                                         │
                                         ▼
        ┌────────────────────────────────────────────────────────────────────────┐
        │                   BUSINESS LOGIC LAYER (BAL)                           │
        │                   Commerce_Connector_BAL                               │
        ├────────────────────────────────────────────────────────────────────────┤
        │                                                                         │
        │  ┌──────────────────────┐  ┌──────────────────────┐                  │
        │  │ FileProcessingAgent  │  │  MappingAgent        │                  │
        │  ├──────────────────────┤  ├──────────────────────┤                  │
        │  │ - ProcessFileData()  │  │ - GetMappings()      │                  │
        │  │ - GetJobListAsync()  │  │ - ValidateMappings() │                  │
        │  │ - TransformData()    │  │ - ApplyMappings()    │                  │
        │  └──────────────────────┘  └──────────────────────┘                  │
        │                                                                         │
        │  ┌──────────────────────┐  ┌──────────────────────┐                  │
        │  │  JobsAgent           │  │ TemplateAgent        │                  │
        │  ├──────────────────────┤  ├──────────────────────┤                  │
        │  │ - CreateJob()        │  │ - GetTemplates()     │                  │
        │  │ - GetJobDetails()    │  │ - ValidateTemplate() │                  │
        │  │ - UpdateJobStatus()  │  │ - ParseTemplate()    │                  │
        │  └──────────────────────┘  └──────────────────────┘                  │
        │                                                                         │
        │  All Agents are registered via Dependency Injection                  │
        │  All Agents have corresponding IAgent Interfaces                     │
        │                                                                         │
        └────────────────────────┬───────────────────────────────────────────────┘
                                 │
                                 ▼
        ┌────────────────────────────────────────────────────────────────────────┐
        │            FACTORY & PROCESSOR LAYER (Specialized Processing)         │
        │                  Commerce_Connector_Factory                            │
        ├────────────────────────────────────────────────────────────────────────┤
        │                                                                         │
        │  ┌─────────────────────────────────────────────────────────────────┐  │
        │  │              FileProcessorFactory (Factory Pattern)            │  │
        │  │  - CreateFileProcessor(fileType, locationType, modelType)      │  │
        │  │  - CreateFileProcessorGeneric<T>(fileType)                     │  │
        │  └────────────────────┬────────────────────────────────────────────┘  │
        │                       │                                                │
        │       ┌───────────────┼───────────────┬───────────────┐               │
        │       │               │               │               │               │
        │       ▼               ▼               ▼               ▼               │
        │  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────────────────┐ │
        │  │   CSV    │  │   JSON   │  │   XML    │  │ File Location        │ │
        │  │Processor │  │Processor │  │Processor │  │ Handler Factory      │ │
        │  │          │  │          │  │          │  │                      │ │
        │  │Generic<T>│  │Generic<T>│  │Generic<T>│  │ - SFTP Handler       │ │
        │  │Transform │  │Transform │  │Transform │  │ - Local FS Handler   │ │
        │  │ & Parse  │  │ & Parse  │  │ & Parse  │  │ - Cloud Handler      │ │
        │  │          │  │          │  │          │  │ - API Handler        │ │
        │  └──────────┘  └──────────┘  └──────────┘  └──────────────────────┘ │
        │                                                                         │
        │  ┌──────────────────────────────────────────────────────────────────┐ │
        │  │  Expression Evaluator (TransformTableData)                      │ │
        │  │  - Evaluates conditional expressions                            │ │
        │  │  - Applies transformations to data                              │ │
        │  └──────────────────────────────────────────────────────────────────┘ │
        │                                                                         │
        │  All processors work with generic types from Znode.Engine.Api.Models │
        │  Supports CSV, JSON, XML file formats                                │
        │  Extensible location handlers (SFTP, Local, Cloud, API)              │
        │                                                                         │
        └────────────────────────┬───────────────────────────────────────────────┘
                                 │
                                 ▼
        ┌────────────────────────────────────────────────────────────────────────┐
        │                   DATA ACCESS LAYER (DAL)                             │
        │                  Commerce_Connector_Data                              │
        ├────────────────────────────────────────────────────────────────────────┤
        │                                                                         │
        │  ┌──────────────────────────────────────────────────────────────────┐  │
        │  │        CommerceConnectorServices (IService Pattern)             │  │
        │  │                                                                  │  │
        │  │  ┌────────────────────┐  ┌────────────────────┐               │  │
        │  │  │ FileDetailsService │  │  JobService        │               │  │
        │  │  ├────────────────────┤  ├────────────────────┤               │  │
        │  │  │ - GetFileDetail()  │  │ - CreateJob()      │               │  │
        │  │  │ - SaveFileDetail() │  │ - GetJobDetails()  │               │  │
        │  │  │ - UpdateFileDetail│  │ - UpdateJobStatus()│               │  │
        │  │  │ - DeleteFileDetail│  │ - GetAllJobs()     │               │  │
        │  │  └────────────────────┘  └────────────────────┘               │  │
        │  │                                                                  │  │
        │  │  IService Interfaces define contracts for all services         │  │
        │  │  Services use Entity Framework Core for database operations    │  │
        │  └──────────────────────────────────────────────────────────────────┘  │
        │                                                                         │
        │  ┌──────────────────────────────────────────────────────────────────┐  │
        │  │              Entity Framework Core - DbContext                  │  │
        │  │              FileProcessing_Entities (DbContext)                │  │
        │  │                                                                  │  │
        │  │  Data Models/Entities:                                         │  │
        │  │  - FileDetail (Job ID, File Path, Type, Location, Mappings)   │  │
        │  │  - LocationConfiguration (Connection strings, auth details)    │  │
        │  │  - PropertyMapping (Field mappings between source & target)    │  │
        │  │  - TemplateDetail (Job templates and configurations)           │  │
        │  │                                                                  │  │
        │  └──────────────────────────────────────────────────────────────────┘  │
        │                                                                         │
        └────────────────────────┬───────────────────────────────────────────────┘
                                 │
                                 ▼
        ┌────────────────────────────────────────────────────────────────────────┐
        │                         DATABASE LAYER                                │
        │              SQL Server - FileProcessing_Database                     │
        │  ┌──────────────────────────────────────────────────────────────────┐  │
        │  │ Tables:                                                          │  │
        │  │ - FileDetails (Job configurations and file metadata)             │  │
        │  │ - LocationConfigurations (Source/Target location configs)        │  │
        │  │ - PropertyMappings (Field mappings and transformations)          │  │
        │  │ - TemplateDetails (Job templates and workflow definitions)       │  │
        │  │ - Job Status/History (Tracking and audit logs)                   │  │
        │  └──────────────────────────────────────────────────────────────────┘  │
        └────────────────────────────────────────────────────────────────────────┘
```

---

## Component Architecture Details

### Layer 1: Presentation/API Layer
**Components**: CommerceConnector, FileProcessingAPI, RabbitMQ

- **CommerceConnector** (Web Application)
  - MVC-based UI application
  - Controllers: Home, Jobs, Mapping, Template, FileProcessing
  - Hosts .NET Aspire service defaults

- **FileProcessingAPI** (REST API)
  - REST endpoints for file processing operations
  - Handles HTTP requests from external clients
  - Integrates with message queue (RabbitMQ)

- **RabbitMQ** (Message Broker)
  - Asynchronous message queue for background jobs
  - Decouples producers from consumers
  - Enables scalable processing

---

### Layer 2: Business Logic Layer (Commerce_Connector_BAL)

**Design Pattern**: Agent Pattern with Dependency Injection

| Agent | Responsibility |
|-------|-----------------|
| **FileProcessingAgent** | Orchestrates file processing workflow, coordinates with API |
| **MappingAgent** | Manages field mappings and transformation rules |
| **JobsAgent** | Manages job lifecycle (create, update, status tracking) |
| **TemplateAgent** | Handles template management and validation |

**Key Features**:
- Each Agent has corresponding Interface (IAgent)
- Loose coupling through dependency injection
- Direct HTTP communication with FileProcessingAPI
- Handles transformation logic and data orchestration

---

### Layer 3: Factory & Processing Layer (Commerce_Connector_Factory)

**Design Patterns**: Factory Pattern, Strategy Pattern, Generic Programming

#### File Processor Factory
```
Supports Multiple File Formats:
├── CSV Files
│   └── CsvFileProcessor<T>
├── JSON Files
│   └── JsonFileProcessor<T>
└── XML Files
    └── XmlFileProcessor<T>

Generic Type T resolves at runtime:
└── Dynamically loads from Znode.Engine.Api.Models
    └── Supports any data model (Product, Customer, Order, etc.)
```

#### File Location Handler Factory
```
Supports Multiple Location Types:
├── SFTP Handler
│   └── Remote file server access
├── Local FileSystem Handler
│   └── Local file operations
├── Cloud Handler
│   └── Cloud storage integration
└── API Handler
    └── REST API-based file retrieval
```

#### Expression Evaluator
- Evaluates conditional logic for data transformations
- Applies business rules during processing
- Supports dynamic expression evaluation

---

### Layer 4: Data Access Layer (Commerce_Connector_Data)

**Pattern**: Repository Pattern with Service Layer

#### Services (IService Pattern)
- **FileDetailsService**: CRUD operations on file configurations
- **JobService**: Job lifecycle management

#### Entity Framework Core
- **DbContext**: `FileProcessing_Entities`
- ORM for database operations
- SQL Server connectivity

#### Data Models
| Model | Purpose |
|-------|---------|
| **FileDetail** | Stores job configuration, file path, type, location, mappings |
| **LocationConfiguration** | Connection strings and credentials for file locations |
| **PropertyMapping** | Field-level mapping rules between source and target systems |
| **TemplateDetail** | Reusable job templates and workflow configurations |

---

### Layer 5: Infrastructure

#### Database
- **SQL Server** (FileProcessing_Database)
- Server: ACL-228\SQL2022
- Stores all configuration and execution data

#### Job Scheduler
- **Hangfire**: Background job scheduling
- CRON expression support for recurring jobs
- Hangfire Dashboard for monitoring

#### Configuration
- **appsettings.json**: Application configuration
- **appsettings.Development.json**: Environment-specific settings
- Database connection strings
- RabbitMQ connection strings

---

## Data Flow Diagram

### File Processing Workflow

```
┌─────────────────┐
│  User triggers  │
│ file processing │
└────────┬────────┘
         │
         ▼
┌──────────────────────────────────┐
│ FileProcessingController         │
│ POST /api/FileProcessing/Process │
└────────┬─────────────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│  MassTransit Message Bus         │
│  (RabbitMQ Endpoint)             │
└────────┬─────────────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│ FileProcessingConsumer           │
│ (Async Message Handler)          │
└────────┬─────────────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│ FileProcessingAgent              │
│ (Orchestrates workflow)          │
└────────┬─────────────────────────┘
         │
         ├─────────────────────────────────────┐
         │                                     │
         ▼                                     ▼
┌────────────────────┐         ┌──────────────────────────┐
│ FileProcessorFactory│         │ FileLocationHandlerFactory
│ - Select processor  │         │ - Create location handler
│   based on filetype │         │   based on location type
└────────┬───────────┘         └──────────┬────────────────┘
         │                                 │
         ▼                                 ▼
┌────────────────────┐         ┌──────────────────────────┐
│ FileProcessor<T>   │         │ IFileLocationHandler     │
│ (CSV/JSON/XML)     │         │ (SFTP/Local/Cloud/API)   │
└────────┬───────────┘         └──────────┬────────────────┘
         │                                 │
         │◄────────────────────────────────┤
         │   Retrieve file & location info │
         │                                 │
         ▼
┌──────────────────────────────────┐
│ MappingAgent                     │
│ - Get mappings from DB           │
│ - Validate mappings              │
└────────┬─────────────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│ Parse & Transform Data           │
│ - ExpressionEvaluator            │
│ - Apply field mappings           │
│ - Execute business rules         │
└────────┬─────────────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│ FileDetailsService               │
│ - Save processed data            │
│ - Update file status             │
└────────┬─────────────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│ Database (SQL Server)            │
│ - Store processed records        │
│ - Update job status              │
└──────────────────────────────────┘
```

---

## Job Scheduling Workflow

```
┌──────────────────────────────────┐
│ User creates recurring job       │
│ POST /CreateRecurringJob         │
│ (with CRON expression)           │
└────────┬─────────────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│ FileProcessingController         │
│ - Fetch most recent file detail  │
│ - Validate CRON expression       │
└────────┬─────────────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│ Hangfire Background Job Queue    │
│ - Schedule recurring job         │
│ - Store in SQL Server            │
└────────┬─────────────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│ Hangfire Server                  │
│ - Executes job on CRON schedule  │
│ - Triggers file processing       │
│ - Monitors job execution         │
└────────┬─────────────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│ Hangfire Dashboard               │
│ - Monitor job execution          │
│ - View execution history         │
│ - Access failure logs            │
└──────────────────────────────────┘
```

---

## Key Functionalities & Features

### 1. **Multi-Format File Processing**
   - ✅ CSV file processing with custom parsing
   - ✅ JSON file processing with structure validation
   - ✅ XML file processing with schema support
   - ✅ Extensible to other formats

### 2. **Flexible Location Handling**
   - ✅ Local file system access
   - ✅ SFTP remote server support
   - ✅ Cloud storage integration
   - ✅ REST API-based file retrieval
   - ✅ Configurable connection strings per location

### 3. **Data Transformation & Mapping**
   - ✅ Field-level property mapping
   - ✅ Conditional logic evaluation via Expression Evaluator
   - ✅ Business rule application during transformation
   - ✅ Support for complex data models (via Znode.Engine.Api.Models)

### 4. **Job Management**
   - ✅ Create and manage processing jobs
   - ✅ Track job status and progress
   - ✅ Job history and audit logging
   - ✅ Configurable job templates

### 5. **Background Job Processing**
   - ✅ Asynchronous file processing via message queue
   - ✅ Recurring job scheduling with CRON expressions
   - ✅ Hangfire-based job orchestration
   - ✅ Job failure handling and retry logic

### 6. **Configuration Management**
   - ✅ Template-based job configuration
   - ✅ Reusable mapping templates
   - ✅ Location configuration management
   - ✅ Environment-specific settings

### 7. **Monitoring & Visibility**
   - ✅ Hangfire Dashboard for job monitoring
   - ✅ Job execution history and logs
   - ✅ Real-time job status tracking
   - ✅ Error handling and reporting

### 8. **API Integration**
   - ✅ RESTful API endpoints
   - ✅ CORS support for cross-origin requests
   - ✅ Message-based async communication (RabbitMQ)
   - ✅ MassTransit for event-driven architecture

### 9. **Extensibility**
   - ✅ Generic file processor implementation
   - ✅ Factory pattern for pluggable components
   - ✅ Interface-based architecture for easy mocking and testing
   - ✅ Custom location handler support

### 10. **Database Management**
   - ✅ Entity Framework Core ORM
   - ✅ SQL Server database backend
   - ✅ Configured connection pooling
   - ✅ Data persistence and retrieval

---

## Technology Stack

| Layer | Technology |
|-------|------------|
| **Frontend** | ASP.NET Core MVC, HTML/CSS, JavaScript |
| **Web Server** | Kestrel (ASP.NET Core) |
| **APIs** | ASP.NET Core REST API, MassTransit |
| **Message Queue** | RabbitMQ |
| **Business Logic** | C# (.NET 6+) |
| **Data Access** | Entity Framework Core |
| **Database** | SQL Server 2022 |
| **Job Scheduling** | Hangfire |
| **Dependency Injection** | Built-in .NET DI Container |
| **API Documentation** | Swagger/OpenAPI |
| **Mapping** | AutoMapper |
| **Logging** | Built-in .NET Logging |
| **Hosting** | .NET Aspire |

---

## Dependencies & Project References

```
CommerceConnector
├── Commerce_Connector_BAL (Business Logic Agents)
├── Commerce_Connector_Factory (File Processors & Factories)
├── Commerce_Connector_ViewModel (View Models)
└── TransformTableData (Expression Evaluation)

FileProcessingAPI
├── Commerce_Connector_Data (Data Access & Services)
├── Commerce_Connector_Factory (File Processing)
├── Commerce_Connector_ViewModel (Data Contracts)
├── MassTransit (Message Bus)
├── Hangfire (Job Scheduling)
├── RabbitMQ (Message Broker)
├── Entity Framework Core (ORM)
└── AutoMapper (Object Mapping)

Commerce_Connector_BAL
├── Commerce_Connector_Factory
├── Commerce_Connector_ViewModel
└── System.Net.Http (HTTP Client)

Commerce_Connector_Factory
├── TransformTableData (Expression Evaluator)
└── Znode.Engine.Api.Models (External Data Models)

Commerce_Connector_Data
├── Entity Framework Core
└── SQL Server Driver
```

---

## Deployment Architecture

```
┌────────────────────────────────────────────────────────────┐
│                       IIS Server                           │
├────────────────────────────────────────────────────────────┤
│                                                            │
│  ┌─────────────────────┐  ┌────────────────────────┐     │
│  │  CommerceConnector  │  │  FileProcessingAPI     │     │
│  │  Application Pool   │  │  Application Pool      │     │
│  │  (Port 5280)        │  │  (Port 7185)           │     │
│  │  - MVC App          │  │  - REST API            │     │
│  │  - UI Controllers   │  │  - API Controllers     │     │
│  │  - BAL Agents       │  │  - Message Consumers   │     │
│  │  - View Models      │  │  - Job Scheduler       │     │
│  └─────────────────────┘  └────────────────────────┘     │
│                                                            │
└────────────┬─────────────────────────────┬────────────────┘
             │                             │
             └──────────┬──────────────────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
   ┌─────────┐    ┌──────────┐   ┌─────────────┐
   │RabbitMQ │    │SQL Server│   │ Hangfire    │
   │Service  │    │Database  │   │ Dashboard   │
   │         │    │          │   │             │
   │Message  │    │File      │   │Job Monitor  │
   │Broker   │    │Processing│   │             │
   │         │    │Database  │   │             │
   └─────────┘    └──────────┘   └─────────────┘
```

---

## Security Considerations

1. **Configuration Security**
   - Database credentials in secure configuration
   - Location handler credentials encrypted in database

2. **API Security**
   - CORS policies configured
   - HTTPS/TLS for data transmission
   - Authentication/Authorization ready for implementation

3. **Data Protection**
   - SQL Server encryption capable
   - Entity Framework Core prevents SQL injection
   - Input validation in controllers

---

## Extensibility Points

1. **Add New File Format**
   - Create new `IFileProcessor<T>` implementation
   - Register in `FileProcessorFactory`

2. **Add New Location Type**
   - Create new `IFileLocationHandler` implementation
   - Register in `FileLocationHandlerFactory`

3. **Add New Transformation Logic**
   - Extend `ExpressionEvaluator`
   - Add new mapping rules in `PropertyMapping`

4. **Add New Job Type**
   - Create new Agent implementing `IAgent`
   - Register in dependency injection
   - Add corresponding Controller

---

## Summary

This is a **multi-tier, event-driven file processing platform** with:
- **Enterprise architecture** using design patterns (Factory, Strategy, Repository)
- **Async/message-driven processing** for scalability
- **Generic, extensible components** supporting multiple file formats and locations
- **Job scheduling and monitoring** capabilities
- **Clean separation of concerns** across layers
- **REST API + Web UI** for user interaction
- **Production-ready** with error handling, logging, and monitoring

