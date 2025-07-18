## 🧩 User Story #2 – Frontend Hello World (React + Tailwind + shadcn/ui)

### 🧠 Goal  
Create a frontend React app scaffolded with best practices, including Tailwind and shadcn/ui, and implement a collapsible dashboard panel on the right side of the screen.

### ✅ Acceptance Criteria

- Use `create-react-app` with the TypeScript template using react and TypeScript community best practices.
- Install and configure Tailwind CSS with dark mode support (`dark` class strategy).
- Integrate `@shadcn/ui` library.
- Implement a right-side dashboard panel that:
  - Can be collapsed to display only page icons.
  - Can be expanded to display both page icons and titles.
  - Uses `shadcn/ui` components and Tailwind utility classes.
- Display icons and text for at least 4 pages:
  - Kitchen
  - Bedroom
  - Logs
  - Settings
- Include a toggle button to collapse/expand the panel.
- Apply dark mode styles and responsive layout.

### 📂 Suggested Structure

```
/dashboard
└── frontend/
    └── src/
        ├── components/NavigationPanel.tsx
        ├── App.tsx
        └── index.tsx
```
