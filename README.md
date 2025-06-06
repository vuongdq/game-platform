# Game Platform

A web application for managing and playing games, built with ASP.NET Core and React.

## Features

- User authentication (login/register)
- JWT-based authentication
- Responsive UI with Material-UI
- Docker containerization
- SQL Server database

## Prerequisites

- Docker and Docker Compose
- .NET SDK 7.0 (for local development)
- Node.js and npm (for local development)

## Getting Started

1. Clone the repository:
```bash
git clone <repository-url>
cd game-platform
```

2. Start the application using Docker Compose:
```bash
docker-compose up --build
```

The application will be available at:
- Frontend: http://localhost:3000
- Backend API: http://localhost:5000
- SQL Server: localhost,1433

## Development

### Backend (ASP.NET Core)

1. Navigate to the backend directory:
```bash
cd backend
```

2. Install dependencies and run:
```bash
dotnet restore
dotnet run
```

### Frontend (React)

1. Navigate to the frontend directory:
```bash
cd frontend
```

2. Install dependencies and run:
```bash
npm install
npm start
```

## Environment Variables

### Backend
- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `ConnectionStrings__DefaultConnection`: Database connection string
- `Jwt__Key`: JWT secret key
- `Jwt__Issuer`: JWT issuer
- `Jwt__Audience`: JWT audience

### Frontend
- No environment variables required for basic setup

## License

MIT #   g a m e - p l a t f o r m  
 