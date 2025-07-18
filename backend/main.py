"""
FastAPI backend application for the umbrella dashboard.
"""

import os
import sys
import logging
import json
from pathlib import Path
from typing import Optional
from fastapi import FastAPI, Request, HTTPException, Query
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import JSONResponse, FileResponse
from fastapi.staticfiles import StaticFiles
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

# Mount React build folder
app.mount("/static", StaticFiles(directory="frontend/build/static"), name="static")

@app.get("/")
def serve_root():
    return FileResponse("frontend/build/index.html")

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

@app.get("/api")
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

# Photo API endpoints for slideshow functionality
def get_image_files(folder_path: Path) -> list[str]:
    """
    Get all image files from a folder and its subfolders recursively.
    
    Args:
        folder_path: Path to the folder to scan
        
    Returns:
        list: Sorted list of image filenames (relative to folder_path)
    """
    valid_extensions = {'.jpg', '.jpeg', '.png', '.webp', '.gif', '.bmp', '.tiff'}
    image_files = []
    
    if not folder_path.exists():
        return image_files
    
    for file_path in folder_path.rglob('*'):
        if file_path.is_file() and file_path.suffix.lower() in valid_extensions:
            # Get relative path from folder_path
            relative_path = file_path.relative_to(folder_path)
            image_files.append(str(relative_path))
    
    return sorted(image_files)

def validate_folder_path(folder: str) -> Path:
    """
    Validate and construct the full path to a photo folder.
    
    Args:
        folder: Folder name (e.g., 'kitchen')
        
    Returns:
        Path: Full path to the folder
        
    Raises:
        HTTPException: If folder is invalid or doesn't exist
    """
    # Sanitize folder name to prevent directory traversal
    if not folder or '..' in folder or '/' in folder or '\\' in folder:
        raise HTTPException(
            status_code=400,
            detail="Invalid folder name"
        )
    
    photos_dir = Path(__file__).parent / "photos"
    folder_path = photos_dir / folder
    
    if not folder_path.exists():
        raise HTTPException(
            status_code=404,
            detail=f"Photo folder not found: {folder}"
        )
    
    if not folder_path.is_dir():
        raise HTTPException(
            status_code=400,
            detail=f"Path is not a directory: {folder}"
        )
    
    return folder_path

@app.get("/api/photos/next")
async def get_next_photo(folder: str = Query(..., description="Photo folder name"), 
                        current: Optional[str] = Query(None, description="Current photo filename")):
    """
    Get the next photo in the slideshow sequence.
    
    Args:
        folder: Name of the photo folder
        current: Current photo filename (optional)
        
    Returns:
        dict: Next photo information
    """
    logger.debug(f"Getting next photo for folder: {folder}, current: {current}")
    
    folder_path = validate_folder_path(folder)
    image_files = get_image_files(folder_path)
    
    if not image_files:
        raise HTTPException(
            status_code=404,
            detail=f"No images found in folder: {folder}"
        )
    
    if not current:
        # Return first image if no current image specified
        next_image = image_files[0]
    else:
        try:
            current_index = image_files.index(current)
            # Get next image, wrap around to first if at end
            next_index = (current_index + 1) % len(image_files)
            next_image = image_files[next_index]
        except ValueError:
            # Current image not found, return first image
            next_image = image_files[0]
    
    logger.info(f"Next photo: {next_image}")
    return {
        "filename": next_image,
        "url": f"/api/photos/file/{folder}/{next_image}",
        "total_images": len(image_files)
    }

@app.get("/api/photos/previous")
async def get_previous_photo(folder: str = Query(..., description="Photo folder name"), 
                           current: Optional[str] = Query(None, description="Current photo filename")):
    """
    Get the previous photo in the slideshow sequence.
    
    Args:
        folder: Name of the photo folder
        current: Current photo filename (optional)
        
    Returns:
        dict: Previous photo information
    """
    logger.debug(f"Getting previous photo for folder: {folder}, current: {current}")
    
    folder_path = validate_folder_path(folder)
    image_files = get_image_files(folder_path)
    
    if not image_files:
        raise HTTPException(
            status_code=404,
            detail=f"No images found in folder: {folder}"
        )
    
    if not current:
        # Return last image if no current image specified
        previous_image = image_files[-1]
    else:
        try:
            current_index = image_files.index(current)
            # Get previous image, wrap around to last if at beginning
            previous_index = (current_index - 1) % len(image_files)
            previous_image = image_files[previous_index]
        except ValueError:
            # Current image not found, return last image
            previous_image = image_files[-1]
    
    logger.info(f"Previous photo: {previous_image}")
    return {
        "filename": previous_image,
        "url": f"/api/photos/file/{folder}/{previous_image}",
        "total_images": len(image_files)
    }

@app.get("/api/photos/file/{folder}/{filename:path}")
async def get_photo_file(folder: str, filename: str):
    """
    Serve a specific photo file.
    
    Args:
        folder: Name of the photo folder
        filename: Photo filename (can include subdirectory path)
        
    Returns:
        FileResponse: The photo file
    """
    logger.debug(f"Serving photo file: {folder}/{filename}")
    
    folder_path = validate_folder_path(folder)
    file_path = folder_path / filename
    
    if not file_path.exists():
        raise HTTPException(
            status_code=404,
            detail=f"Photo file not found: {filename}"
        )
    
    if not file_path.is_file():
        raise HTTPException(
            status_code=400,
            detail=f"Path is not a file: {filename}"
        )
    
    # Security check: ensure file is within the folder
    try:
        file_path.resolve().relative_to(folder_path.resolve())
    except ValueError:
        raise HTTPException(
            status_code=403,
            detail="Access denied"
        )
    
    return FileResponse(file_path)

@app.get("/api/photos/list/{folder}")
async def list_photos(folder: str):
    """
    List all photos in a folder.
    
    Args:
        folder: Name of the photo folder
        
    Returns:
        dict: List of photo information
    """
    logger.debug(f"Listing photos in folder: {folder}")
    
    folder_path = validate_folder_path(folder)
    image_files = get_image_files(folder_path)
    
    photos = []
    for filename in image_files:
        photos.append({
            "filename": filename,
            "url": f"/api/photos/file/{folder}/{filename}"
        })
    
    return {
        "folder": folder,
        "photos": photos,
        "total_images": len(photos)
    }

# Catch-all route for React SPA (must be last!)
@app.get("/{full_path:path}")
def serve_spa(full_path: str):
    """
    Serve React SPA for any unmatched routes.
    This must be the last route defined to avoid overriding API endpoints.
    """
    return FileResponse("frontend/build/index.html")

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
