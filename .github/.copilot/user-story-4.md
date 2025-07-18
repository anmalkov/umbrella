# 🧩 User Story #4 – Fullscreen Slideshow Widget (Idle-Activated)

## 🧠 Goal

Implement a special widget that triggers a fullscreen slideshow after a period of inactivity. It should read photos from a local folder on the backend and serve them one by one via API. This widget must be unique per room config and hidden during normal dashboard operation.

---

## ✅ Acceptance Criteria

### 🧰 Backend (FastAPI)

- Create a folder-based image API:
  - **`GET /api/photos/next`**
  - **`GET /api/photos/previous`**
  - Parameters:
    - `folder`: string (e.g., `kitchen`, in this case the full path would be `/photos/kitchen`)
    - `current`: string (optional, current filename)
- When `current` is empty or not provided:
  - Return the first file in alphabetical order.
- If `next` is called on the last file in the folder, return the **first** one.
- If `previous` is called on the first file in the folder, return the **last** one.
- Files must be collected **recursively**:
  - If the provided folder contains subfolders, all valid image files from all subdirectories must be included in the list.
  - The file list should behave as if there are **no subfolders**—all valid image files are part of a single sorted list.
- Files are served from the local folder under `/photos`.
- Validate that the folder exists and is safe to access.
- Only image file types should be served (`.jpg`, `.jpeg`, `.png`, `.webp`, etc.).

---

### 🖥 Frontend (React + Tailwind + shadcn/ui)

- Do **not render the slideshow widget** during normal dashboard use.
- Use `idle-js` or a custom JavaScript idle timer to detect inactivity.
- When idle for `inactivityDelay` seconds, switch to **fullscreen slideshow** mode.
- Use `react-slideshow-image` or another lightweight slideshow library to display images using the different slideshow effects.
- Fetch photos using `/api/photos/next` and `/api/photos/previous` endpoints.
- Display each photo for `interval` seconds, as defined in config.
- Exit slideshow on user activity (e.g., mouse move, click, key press).

---

### 🔐 Config Requirements

- This widget must appear only once in a room config.
- If more than one is found, log a warning and ignore all but the first.
- Sample config format:

  ```json
  {
    "type": "slideshow",
    "folder": "kitchen",
    "interval": 10,
    "inactivityDelay": 60
  }
  ```

---

### 🖼️ Slideshow Widget Behavior

- The widget acts more like a **behavioral mode** than a visible dashboard component.
- It activates only after defined `inactivityDelay`.
- Automatically cycles through images using `interval`.
- Fetches next/previous image filenames via API.
- Fullscreen overlay should gracefully fade in and out on activation/deactivation.

---

### 📂 Suggested Structure

```
/dashboard
├── backend/
│       ├── main.py
│       └── photos/
│           ├── kitchen/
│           │   ├── 001.jpg
│           │   ├── subfolder/
│           │   │   ├── 002.jpg
│           │   │   └── 003.jpg
├── frontend/
│   └── src/
│       ├── slideshow/SlideshowOverlay.tsx
│       ├── hooks/useIdle.ts
│       └── utils/photoApi.ts
```
