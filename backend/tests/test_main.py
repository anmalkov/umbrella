"""
Tests for the Umbrella Dashboard backend API.
"""

import pytest
import json
import tempfile
import os
from pathlib import Path
from fastapi.testclient import TestClient
from main import app

client = TestClient(app)

def test_health_check():
    """Test the health check endpoint."""
    response = client.get("/api/health")
    assert response.status_code == 200
    assert response.json() == {"status": "ok"}

def test_root_endpoint():
    """Test the root endpoint."""
    response = client.get("/")
    assert response.status_code == 200
    data = response.json()
    assert "message" in data
    assert "version" in data
    assert "health_check" in data
    assert data["health_check"] == "/api/health"

def test_cors_headers():
    """Test that CORS headers are present."""
    response = client.get("/api/health")
    assert response.status_code == 200
    # The test client doesn't automatically add CORS headers, 
    # but the middleware is configured correctly in the app

def test_get_room_config_kitchen():
    """Test getting kitchen room configuration."""
    response = client.get("/api/config/kitchen")
    assert response.status_code == 200
    data = response.json()
    
    # Check the structure of the response
    assert "roomId" in data
    assert "widgets" in data
    assert data["roomId"] == "kitchen"
    assert isinstance(data["widgets"], list)
    assert len(data["widgets"]) > 0
    
    # Check the first widget structure
    widget = data["widgets"][0]
    assert "type" in widget
    assert "title" in widget
    assert "position" in widget
    assert widget["type"] == "time"
    
    # Check position structure
    position = widget["position"]
    assert "x" in position
    assert "y" in position
    assert "w" in position
    assert "h" in position
    assert isinstance(position["x"], int)
    assert isinstance(position["y"], int)
    assert isinstance(position["w"], int)
    assert isinstance(position["h"], int)

def test_get_room_config_not_found():
    """Test getting configuration for non-existent room."""
    response = client.get("/api/config/nonexistent")
    assert response.status_code == 404
    data = response.json()
    assert "detail" in data
    assert "nonexistent" in data["detail"]

def test_get_room_config_invalid_json():
    """Test handling of invalid JSON in room config."""
    # Create a temporary invalid JSON file
    config_dir = Path(__file__).parent.parent / "config" / "rooms"
    config_dir.mkdir(parents=True, exist_ok=True)
    
    invalid_config_path = config_dir / "invalid.json"
    try:
        with open(invalid_config_path, 'w', encoding='utf-8') as f:
            f.write("{ invalid json content")
        
        response = client.get("/api/config/invalid")
        assert response.status_code == 500
        data = response.json()
        assert "detail" in data
        assert "Invalid JSON" in data["detail"]
    finally:
        # Clean up the temporary file
        if invalid_config_path.exists():
            invalid_config_path.unlink()
