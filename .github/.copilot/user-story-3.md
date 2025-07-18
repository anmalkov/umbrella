## 🧩 User Story #3 – `/api/config/{room_id}` Endpoint & TimeWidget Component

### 🧠 Goal

Fetch and render widgets dynamically on a dashboard using config from the backend. Support screen positioning for each widget to allow future drag-and-drop functionality.

### ✅ Acceptance Criteria

#### Backend

- Add GET /api/config/{room_id} endpoint.
- Load JSON config from src/config/rooms/{room_id}.json.
- Return the widget layout and configuration as JSON.
- write unit tests for the endpoint.

Example kitchen.json config should now include layout info:
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
          "dateFormat": "DD.MM.YYYY",
          "position": {
            "x": 0,
            "y": 0,
            "w": 2,
            "h": 1
          }
        }
      ]
    }
    ```

#### Frontend

- Fetch room config from the backend via /api/config/{room_id}.
- Support layout rendering using position data from each widget.
- Implement a basic grid system to position widgets:
  - x, y define the top-left corner of the widget
  - w, h define the width and height in grid units

- Render the TimeWidget with props:
  ```ts
  {
    "title": "Current Time",
    "showTime": true,
    "timezone": "UTC",
    "timeFormat": "HH:mm:ss",
    "showDate": true,
    "dateFormat": "DD.MM.YYYY",
  }
  ```
- Use `shadcn/ui` components (`Card`, etc.) and Tailwind for styling.
- Display current time (updated every second) and date conditionally.
- Dynamically fetch widget configuration from the backend endpoint.
- Render widgets from config using a grid layout.
- Write unit tests for the `TimeWidget` component.

### 📂 Suggested Structure
```
/dashboard
├── backend/
│       ├── main.py
│       └── config/
│           └── rooms/
│               └── kitchen.json
└── frontend/
    └── src/
        ├── widgets/TimeWidget.tsx
        ├── layout/GridDashboard.tsx
        ├── App.tsx
        └── utils/fetchConfig.ts
```
