# Umbrella Dashboard Backend

A FastAPI backend application for the umbrella dashboard with photo slideshow capabilities and room configuration management.

## Requirements

- Python 3.8 or higher
- pip

## Setup

1. **Create and activate a virtual environment:**

   ```bash
   # Create virtual environment
   python -m venv venv
   
   # Activate virtual environment
   # On Windows:
   venv\Scripts\activate
   # On macOS/Linux:
   source venv/bin/activate
   ```

2. **Install dependencies:**

   ```bash
   pip install -r requirements.txt
   ```

3. **Run the application:**

   ```bash
   # Using uvicorn (recommended)
   uvicorn main:app --reload --host 0.0.0.0 --port 8080
   ```

   Or run directly with Python (runs on port 8081 by default):

   ```bash
   python main.py
   ```

## Environment Variables

The application supports the following environment variables:

- `DEBUG` - Enable debug mode (default: "false")
- `LOG_LEVEL` - Set logging level: DEBUG, INFO, WARNING, ERROR, CRITICAL (default: "INFO")
- `ENVIRONMENT` - Set environment name (default: "development")

Create a `.env` file in the backend directory to configure these variables:

```env
DEBUG=true
LOG_LEVEL=DEBUG
ENVIRONMENT=development
```

## API Endpoints

### Core Endpoints

- `GET /` - Root endpoint with API information
- `GET /api/health` - Health check endpoint
- `GET /debug/info` - Debug information (only available in debug mode)

### Room Configuration

- `GET /api/config/{room_id}` - Get room configuration (e.g., `/api/config/kitchen`)

### Photo Slideshow API

- `GET /api/photos/list/{folder}` - List all photos in a folder
- `GET /api/photos/next?folder={folder}&current={filename}` - Get next photo in slideshow
- `GET /api/photos/previous?folder={folder}&current={filename}` - Get previous photo in slideshow
- `GET /api/photos/file/{folder}/{filename}` - Serve photo file (supports subdirectories)

## Features

### Room Configuration Management

- JSON-based room configurations stored in `config/rooms/`
- Support for multiple widget types (time, slideshow)
- Flexible positioning and layout system

### Photo Slideshow System

- Recursive photo scanning in subdirectories
- Support for multiple image formats (JPG, PNG, WebP, GIF, BMP, TIFF)
- Navigation controls (next/previous)
- Secure file serving with path validation
- Automatic image listing and metadata

### Development Features

- Comprehensive logging with configurable levels
- CORS middleware for frontend integration
- Debug endpoints for development
- Request/response logging
- Environment-based configuration

## Development

The application runs on `http://localhost:8081` by default when using `python main.py`, or `http://localhost:8080` when using uvicorn directly.

- API documentation is available at: `http://localhost:8081/docs`
- Alternative API documentation: `http://localhost:8081/redoc`

### Testing

Run the test suite:

```bash
# From the backend directory
python -m pytest tests/ -v
```

### API Testing

Use the provided HTTP test files for manual API testing:

- `api-test.http` - Core API endpoints
- `test-slideshow.http` - Photo slideshow endpoints

## Project Structure

```text
backend/
├── main.py                 # Main FastAPI application
├── requirements.txt        # Python dependencies
├── pyproject.toml         # Project configuration
├── README.md              # This file
├── api-test.http          # HTTP test requests
├── test-slideshow.http    # Slideshow API test requests
├── config/                # Configuration files
│   └── rooms/             # Room configuration files
│       └── kitchen.json   # Kitchen room configuration
├── photos/                # Photo storage
│   └── kitchen/           # Kitchen photos
│       ├── 001.jpg        # Sample photos
│       └── subfolder/     # Subdirectory support
├── tests/                 # Test files
│   ├── __init__.py
│   └── test_main.py      # Main test suite
└── __pycache__/          # Python cache files
```

## Configuration

### Room Configuration Format

Room configurations are stored as JSON files in `config/rooms/{room_id}.json`:

```json
{
  "roomId": "kitchen",
  "widgets": [
    {
      "type": "time",
      "title": "Current Time",
      "showTime": true,
      "timezone": "UTC",
      "timeFormat": "HH:mm:ss",
      "showDate": true,
      "dateFormat": "dddd, MMMM DD",
      "position": {
        "x": 0,
        "y": 0,
        "w": 2,
        "h": 1
      }
    },
    {
      "type": "slideshow",
      "folder": "kitchen",
      "interval": 10,
      "inactivityDelay": 10,
      "showTime": true,
      "timeFormat": "HH:mm:ss",
      "showDate": true,
      "dateFormat": "dddd, MMMM DD"
    }
  ]
}
```

### Photo Organization

Photos are organized in the `photos/` directory:

- Each room can have its own subfolder (e.g., `photos/kitchen/`)
- Subdirectories are supported for better organization
- Supported formats: JPG, JPEG, PNG, WebP, GIF, BMP, TIFF

## Dependencies

- **FastAPI** - Modern web framework for building APIs
- **Uvicorn** - ASGI server for running the application
- **python-multipart** - For handling multipart form data
- **python-dotenv** - For loading environment variables from .env files

## Development Dependencies

- **pytest** - Testing framework
- **pytest-asyncio** - Async testing support
- **httpx** - HTTP client for testing
- **black** - Code formatter
- **flake8** - Linting
- **mypy** - Type checking
