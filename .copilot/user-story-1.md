## 🧩 User Story #1 – Backend Hello World (FastAPI) Setup

### 🧠 Goal

Set up a basic FastAPI backend with CORS support and a health check endpoint.

### ✅ Acceptance Criteria

- Set up a FastAPI project using the latest Python version using python community best practices.
- Use `uvicorn` as the ASGI server.
- Create a virtual environment (venv) to install dependencies
- Create a `requirements.txt` or `pyproject.toml`.
- Enable CORS for development purposes (allow all origins for now).
- Implement a `GET /api/health` endpoint that returns `{"status": "ok"}`.
- Write unit tests for the health check endpoint using `pytest`.

### 📂 Suggested Structure

```
/dashboard
└── backend/
    └── src/
        └── main.py
```
