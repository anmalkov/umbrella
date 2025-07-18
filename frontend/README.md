# Umbrella Dashboard Frontend

A React-based smart home dashboard application with photo slideshow capabilities and configurable widget layouts.

## Features

### 🏠 Smart Home Dashboard
- **Room-based Configuration**: Dynamic room layouts loaded from backend API
- **Widget System**: Extensible widget architecture (Time widgets, Slideshow widgets)
- **Grid Layout**: Responsive grid system with configurable widget positioning
- **Dark/Light Mode**: Toggle between dark and light themes

### 📸 Photo Slideshow
- **Auto-triggered Slideshow**: Activates after configurable idle time
- **Navigation Controls**: Click zones for previous/next photo navigation
- **Keyboard Support**: ESC key to exit slideshow
- **Time/Date Overlay**: Optional time and date display during slideshow
- **Subfolder Support**: Recursive photo loading from subdirectories
- **Multiple Formats**: Support for JPG, PNG, WebP, GIF, BMP, TIFF

### 🎛️ User Interface
- **Responsive Design**: Works on desktop and mobile devices
- **Navigation Panel**: Room selection and navigation
- **Modern UI**: Built with Tailwind CSS and Radix UI components
- **Idle Detection**: Smart user activity detection for slideshow activation

## Technology Stack

- **React 19.1.0** - Frontend framework
- **TypeScript** - Type safety and better development experience
- **Tailwind CSS** - Utility-first CSS framework
- **Radix UI** - Accessible UI components
- **Lucide React** - Icon library
- **React Testing Library** - Testing utilities

## Getting Started

### Prerequisites

- Node.js 16.0 or higher
- npm or yarn package manager
- Backend API server running (see backend README)

### Installation

1. **Clone the repository and navigate to frontend directory:**
   ```bash
   cd frontend
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Configure environment variables:**
   Create a `.env` file in the frontend directory:
   ```env
   REACT_APP_API_URL=http://localhost:8081
   ```

4. **Start the development server:**
   ```bash
   npm start
   ```

   The application will open at [http://localhost:3000](http://localhost:3000).

## Available Scripts

### `npm start`

Runs the app in development mode at [http://localhost:3000](http://localhost:3000).

- Hot reloading enabled for rapid development
- Lint errors displayed in console
- Automatic browser refresh on changes

### `npm test`

Launches the test runner in interactive watch mode.

- Runs all test files matching `*.test.tsx` or `*.test.ts`
- Includes component tests and utility function tests
- Coverage reporting available

### `npm run build`

Builds the app for production to the `build` folder.

- React code optimized for production
- Minified and hashed filenames
- Ready for deployment to static hosting

### `npm run eject`

**⚠️ Note: This is a one-way operation. Once you `eject`, you can't go back!**

Exposes all configuration files and dependencies for full customization.

## Configuration

### Environment Variables

- `REACT_APP_API_URL` - Backend API base URL (default: `http://localhost:8081`)

### Room Configuration

Rooms are configured via the backend API. Each room can contain:

- **Time Widgets**: Display current time and date
- **Slideshow Widgets**: Photo slideshow with configurable settings

Example room configuration structure:
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
      "position": { "x": 0, "y": 0, "w": 2, "h": 1 }
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

## Application Structure

```
frontend/
├── public/                 # Static assets
│   ├── index.html         # Main HTML template
│   ├── favicon.ico        # App icon
│   └── manifest.json      # PWA configuration
├── src/
│   ├── components/        # Reusable UI components
│   │   ├── NavigationPanel.tsx
│   │   └── ui/           # Base UI components
│   │       ├── button.tsx
│   │       └── card.tsx
│   ├── hooks/            # Custom React hooks
│   │   └── useIdle.ts    # Idle detection hook
│   ├── layout/           # Layout components
│   │   └── GridDashboard.tsx
│   ├── slideshow/        # Slideshow functionality
│   │   └── SlideshowOverlay.tsx
│   ├── utils/            # Utility functions
│   │   ├── fetchConfig.ts # API configuration fetching
│   │   ├── photoApi.ts   # Photo API utilities
│   │   └── dateTime.ts   # Date/time formatting
│   ├── widgets/          # Widget components
│   │   ├── TimeWidget.tsx
│   │   └── __tests__/    # Widget tests
│   ├── App.tsx           # Main application component
│   ├── index.tsx         # Application entry point
│   └── index.css         # Global styles
├── package.json          # Project dependencies
├── tailwind.config.js    # Tailwind CSS configuration
├── tsconfig.json         # TypeScript configuration
└── postcss.config.js     # PostCSS configuration
```

## Key Components

### GridDashboard
- Manages room configuration loading
- Handles widget positioning and rendering
- Integrates slideshow functionality
- Manages idle state detection

### SlideshowOverlay
- Full-screen photo slideshow
- Navigation controls and keyboard shortcuts
- Time/date overlay display
- Error handling and loading states

### TimeWidget
- Configurable time and date display
- Multiple timezone support
- Customizable formatting options

### NavigationPanel
- Room selection interface
- Responsive design
- Integration with main dashboard

## Slideshow Controls

### Navigation
- **Left side click**: Previous photo
- **Right side click**: Next photo
- **Center click**: Exit slideshow
- **ESC key**: Exit slideshow

### Auto-advance
- Configurable interval (default: 10 seconds)
- Pauses on user interaction
- Resumes after interaction timeout

### Idle Detection
- **Configurable timeout**: Set per room (default: 10 seconds)
- **Activity events**: Keyboard, scroll, touch (excludes mouse movement)
- **Visibility API**: Handles tab switching and window focus

## API Integration

The frontend communicates with the backend API for:

### Room Configuration
- `GET /api/config/{roomId}` - Load room widget configuration

### Photo Management
- `GET /api/photos/list/{folder}` - List available photos
- `GET /api/photos/next` - Get next photo in sequence
- `GET /api/photos/previous` - Get previous photo in sequence
- `GET /api/photos/file/{folder}/{filename}` - Serve photo files

## Development

### Project Structure
- **Component-based architecture**: Modular, reusable components
- **TypeScript**: Full type safety and IntelliSense support
- **Custom hooks**: Reusable logic (idle detection, API calls)
- **Utility functions**: Centralized helper functions

### Testing
- **Jest**: Testing framework
- **React Testing Library**: Component testing utilities
- **Test coverage**: Component and utility function tests

### Styling
- **Tailwind CSS**: Utility-first styling approach
- **CSS Custom Properties**: Theme variables for dark/light mode
- **Responsive design**: Mobile-first approach

## Deployment

### Production Build
```bash
npm run build
```

### Static Hosting
The built application can be deployed to:
- **Netlify**: Drag and drop deployment
- **Vercel**: Git-based deployment
- **AWS S3**: Static website hosting
- **GitHub Pages**: Free hosting for public repositories

### Environment Configuration
Set the production API URL:
```env
REACT_APP_API_URL=https://your-api-domain.com
```

## Browser Support

- **Chrome**: Latest 2 versions
- **Firefox**: Latest 2 versions
- **Safari**: Latest 2 versions
- **Edge**: Latest 2 versions

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/new-feature`
3. Commit changes: `git commit -am 'Add new feature'`
4. Push to the branch: `git push origin feature/new-feature`
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the backend README for details.

## Related Documentation

- [Backend API Documentation](../backend/README.md)
- [Create React App Documentation](https://facebook.github.io/create-react-app/docs/getting-started)
- [React Documentation](https://reactjs.org/)
- [Tailwind CSS Documentation](https://tailwindcss.com/docs)
