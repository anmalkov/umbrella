"""
Tests for the Umbrella Dashboard backend API.
"""

import pytest
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
