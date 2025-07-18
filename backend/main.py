"""
FastAPI backend application for the umbrella dashboard.
"""

import os
import sys
import logging
import json
from pathlib import Path
from fastapi import FastAPI, Request, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import JSONResponse
from dotenv import load_dotenv

# Load environment variables from .env file
load_dotenv()

def get_logging_level():
    """Get logging level based on environment variable."""
    level = os.getenv("LOG_LEVEL", "INFO").upper()
    if level not in ["DEBUG", "INFO", "WARNING", "ERROR", "CRITICAL"]:
        raise ValueError(f"Invalid LOG_LEVEL: {level}")
    if level == "DEBUG":
        return logging.DEBUG
    elif level == "INFO":
        return logging.INFO
    elif level == "WARNING":
        return logging.WARNING
    elif level == "ERROR":
        return logging.ERROR
    elif level == "CRITICAL":
        return logging.CRITICAL
    return logging.INFO

# Set up logging for debugging
logging.basicConfig(
    level=get_logging_level(),
    format="%(asctime)s - %(name)s - %(levelname)s - %(message)s"
)
logger = logging.getLogger(__name__)

# Create FastAPI app instance
app = FastAPI(
    title="Umbrella Dashboard API",
    description="Backend API for the umbrella dashboard application",
    version="1.0.0",
    debug=os.getenv("DEBUG", "false").lower() == "true"
)

# Configure CORS middleware for development
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Allow all origins for development
    allow_credentials=True,
    allow_methods=["*"],  # Allow all methods
    allow_headers=["*"],  # Allow all headers
)

@app.middleware("http")
async def log_requests(request: Request, call_next):
    """Log all HTTP requests for debugging."""
    logger.info(f"Request: {request.method} {request.url}")
    response = await call_next(request)
    logger.info(f"Response: {response.status_code}")
    return response

@app.get("/api/health")
async def health_check():
    """
    Health check endpoint to verify the API is running.
    
    Returns:
        dict: Status information
    """
    logger.debug("Health check endpoint called")
    return {"status": "ok"}

@app.get("/")
async def root():
    """
    Root endpoint with basic API information.
    
    Returns:
        dict: Welcome message and API info
    """
    logger.debug("Root endpoint called")
    return {
        "message": "Welcome to Umbrella Dashboard API",
        "version": "1.0.0",
        "health_check": "/api/health"
    }

@app.get("/debug/info")
async def debug_info():
    """
    Debug information endpoint (only available in debug mode).
    
    Returns:
        dict: Debug information
    """
    if not app.debug:
        return JSONResponse(
            status_code=404,
            content={"detail": "Debug endpoint not available in production mode"}
        )
    
    logger.debug("Debug info endpoint called")
    return {
        "debug_mode": app.debug,
        "environment": os.environ.get("ENVIRONMENT", "development"),
        "python_path": sys.path,
        "working_directory": os.getcwd()
    }

@app.get("/api/config/{room_id}")
async def get_room_config(room_id: str):
    """
    Get the configuration for a specific room.
    
    Args:
        room_id: The ID of the room to get configuration for
        
    Returns:
        dict: Room configuration including widgets and layout
        
    Raises:
        HTTPException: If room configuration is not found
    """
    logger.debug(f"Getting config for room: {room_id}")
    
    # Construct the path to the room config file
    config_path = Path(__file__).parent / "config" / "rooms" / f"{room_id}.json"
    
    if not config_path.exists():
        logger.warning(f"Room config not found: {config_path}")
        raise HTTPException(
            status_code=404, 
            detail=f"Room configuration not found for room_id: {room_id}"
        )
    
    try:
        with open(config_path, 'r', encoding='utf-8') as file:
            config = json.load(file)
        
        logger.info(f"Successfully loaded config for room: {room_id}")
        return config
    except json.JSONDecodeError as e:
        logger.error(f"Invalid JSON in config file {config_path}: {e}")
        raise HTTPException(
            status_code=500,
            detail=f"Invalid JSON configuration for room: {room_id}"
        )
    except Exception as e:
        logger.error(f"Error reading config file {config_path}: {e}")
        raise HTTPException(
            status_code=500,
            detail=f"Error reading configuration for room: {room_id}"
        )

if __name__ == "__main__":
    import uvicorn
    
    # Enable debug mode if DEBUG environment variable is set
    debug_mode = os.getenv("DEBUG", "false").lower() == "true"
    
    logger.info(f"Starting FastAPI application in {'debug' if debug_mode else 'production'} mode")
    
    uvicorn.run(
        "main:app", 
        host="0.0.0.0", 
        port=8081,
        reload=debug_mode,  # Enable auto-reload in debug mode
        log_level="debug" if debug_mode else "info"
    )
