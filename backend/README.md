# Umbrella Dashboard Backend

A FastAPI backend application for the umbrella dashboard.

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
   # From the backend directory
   uvicorn main:app --reload --host 0.0.0.0 --port 8080
   ```

   Or run directly with Python:

   ```bash
   python main.py
   ```

## API Endpoints

- `GET /` - Root endpoint with API information
- `GET /api/health` - Health check endpoint

## Development

The application runs on `http://localhost:8080` by default.

- API documentation is available at: `http://localhost:8080/docs`
- Alternative API documentation: `http://localhost:8080/redoc`

## Project Structure

```text
backend/
├── main.py             # Main FastAPI application
├── requirements.txt    # Python dependencies
├── pyproject.toml      # Project configuration
├── README.md           # This file
├── run.py              # Startup script
├── tests/              # Test files
│   ├── __init__.py
│   └── test_main.py
└── venv/               # Virtual environment
```
