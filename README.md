# 🏠 Umbrella Dashboard

A modern smart home dashboard application with photo slideshow capabilities, built with React and FastAPI.

![Dashboard Preview](https://via.placeholder.com/800x400/1f2937/ffffff?text=Smart+Home+Dashboard+Preview)

## 🌟 Features

### Smart Home Dashboard

- **Room-based Configuration**: Dynamic layouts loaded from backend API
- **Widget System**: Extensible widget architecture (Time, Slideshow, and more)
- **Responsive Grid Layout**: Configurable widget positioning
- **Dark/Light Mode Toggle**: Modern UI with theme switching

### Photo Slideshow

- **Auto-triggered Slideshow**: Activates after configurable idle time
- **Navigation Controls**: Click zones and keyboard shortcuts
- **Time/Date Overlay**: Optional time and date display during slideshow
- **Subfolder Support**: Recursive photo loading from subdirectories
- **Multiple Formats**: Support for JPG, PNG, WebP, GIF, BMP, TIFF

### User Experience

- **Idle Detection**: Smart user activity detection
- **CORS Support**: Seamless frontend-backend integration
- **Error Handling**: Comprehensive error management
- **Performance Optimized**: Fast loading and smooth transitions

## 🏗️ Architecture

This application consists of two main components:

### Frontend (React + TypeScript)

- **Framework**: React 19.1.0 with TypeScript
- **UI Library**: Tailwind CSS + Radix UI
- **State Management**: React hooks and context
- **Build Tool**: Create React App

### Backend (FastAPI + Python)

- **Framework**: FastAPI with Python 3.8+
- **File Serving**: Secure photo file serving
- **Configuration**: JSON-based room configurations
- **API Documentation**: Auto-generated OpenAPI docs

## 🚀 Quick Start

### Prerequisites

- **Node.js** 16.0 or higher
- **Python** 3.8 or higher
- **npm** or **yarn**

### 1. Clone the Repository

```bash
git clone https://github.com/anmalkov/umbrella.git
cd umbrella/dashboard
```

### 2. Backend Setup

```bash
# Navigate to backend directory
cd backend

# Create virtual environment
python -m venv venv

# Activate virtual environment
# Windows:
venv\Scripts\activate
# macOS/Linux:
source venv/bin/activate

# Install dependencies
pip install -r requirements.txt

# Start the backend server
python main.py
```

The backend will be available at `http://localhost:8081`

### 3. Frontend Setup

```bash
# Navigate to frontend directory (in a new terminal)
cd frontend

# Install dependencies
npm install

# Create environment file
echo "REACT_APP_API_URL=http://localhost:8081" > .env

# Start the frontend server
npm start
```

The frontend will be available at `http://localhost:3000`

## 📁 Project Structure

```text
umbrella/dashboard/
├── backend/                    # FastAPI backend
│   ├── config/                # Room configurations
│   │   └── rooms/             # Room-specific config files
│   │       └── kitchen.json   # Example room config
│   ├── photos/                # Photo storage
│   │   └── kitchen/           # Room-specific photos
│   ├── tests/                 # Backend tests
│   ├── main.py                # FastAPI application
│   ├── requirements.txt       # Python dependencies
│   └── api-test.http         # HTTP test requests
├── frontend/                  # React frontend
│   ├── public/               # Static assets
│   ├── src/
│   │   ├── components/       # UI components
│   │   ├── hooks/            # Custom React hooks
│   │   ├── layout/           # Layout components
│   │   ├── slideshow/        # Slideshow functionality
│   │   ├── utils/            # Utility functions
│   │   └── widgets/          # Widget components
│   ├── package.json          # Frontend dependencies
│   └── tailwind.config.js    # Tailwind configuration
└── README.md                 # This file
```

## 🎛️ Configuration

### Room Configuration

Create room configurations in `backend/config/rooms/{room_id}.json`:

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

### Photo Organization

Place photos in `backend/photos/{room_id}/`:

```text
backend/photos/
├── kitchen/
│   ├── 001.jpg
│   ├── 002.jpg
│   └── subfolder/
│       ├── 003.jpg
│       └── 004.jpg
└── living-room/
    ├── photo1.jpg
    └── photo2.jpg
```

## 🔧 API Endpoints

### Core Endpoints

- `GET /` - API information
- `GET /api/health` - Health check
- `GET /debug/info` - Debug information (development only)

### Room Configuration in API

- `GET /api/config/{room_id}` - Get room configuration

### Photo Management

- `GET /api/photos/list/{folder}` - List photos in folder
- `GET /api/photos/next` - Get next photo in sequence
- `GET /api/photos/previous` - Get previous photo in sequence
- `GET /api/photos/file/{folder}/{filename}` - Serve photo file

## 🎮 Usage

### Dashboard Navigation

1. **Room Selection**: Use the navigation panel to switch between rooms
2. **Widget Interaction**: Widgets display real-time information
3. **Theme Toggle**: Switch between dark and light modes

### Slideshow Controls

- **Auto-activation**: Slideshow starts after idle timeout
- **Navigation**: Click left/right sides to navigate photos
- **Exit**: Click center or press ESC to exit slideshow
- **Keyboard**: ESC key exits slideshow

### Idle Detection

- **Configurable timeout**: Set per room (default: 10 seconds)
- **Activity tracking**: Keyboard, scroll, touch events
- **Smart detection**: Excludes mouse movement to prevent accidental activation

## 🧪 Testing

### Backend Tests

```bash
cd backend
python -m pytest tests/ -v
```

### Frontend Tests

```bash
cd frontend
npm test
```

### API Testing

Use the provided HTTP test files:

- `backend/api-test.http` - Core API endpoints
- `backend/test-slideshow.http` - Slideshow functionality

## 🚀 Deployment

### Docker Deployment

This project includes automated Docker image building and publishing to Docker Hub via GitHub Actions.

#### Using Pre-built Docker Image

```bash
# Pull and run the latest image
docker run -p 80:80 anmalkov/umbrella:latest

# Or run a specific version
docker run -p 80:80 anmalkov/umbrella:v1.0.0
```

#### Automated Publishing

The project automatically builds and publishes Docker images when you create a new tag:

```bash
# Create and push a tag to trigger Docker build
git tag v1.0.0
git push origin v1.0.0
```

This will:
- Build a multi-architecture Docker image (AMD64 and ARM64)
- Push to Docker Hub as `anmalkov/umbrella`
- Tag with version numbers (e.g., `v1.0.0`, `1.0`, `1`)

#### Setting up Docker Hub Publishing

To enable automatic Docker publishing, add these secrets to your GitHub repository:

1. Go to your GitHub repository → Settings → Secrets and variables → Actions
2. Add the following repository secrets:
   - `DOCKER_USERNAME`: Your Docker Hub username
   - `DOCKER_PASSWORD`: Your Docker Hub password or access token

### Manual Docker Build

```bash
# Build the image locally
docker build -t umbrella-dashboard .

# Run the container
docker run -p 80:80 umbrella-dashboard
```

### Production Build

**Backend:**

```bash
cd backend
pip install -r requirements.txt
python main.py
```

**Frontend:**

```bash
cd frontend
npm run build
```

### Environment Variables

**Backend:**

```env
DEBUG=false
LOG_LEVEL=INFO
ENVIRONMENT=production
```

**Frontend:**

```env
REACT_APP_API_URL=https://your-api-domain.com
```

### Deployment Options

- **Docker Hub**: Automated container deployment with `anmalkov/umbrella`
- **Docker**: Manual containerized deployment
- **Cloud Services**: AWS, Azure, Google Cloud
- **Static Hosting**: Netlify, Vercel for frontend
- **VPS**: Traditional server deployment

## 🛠️ Development

### Backend Development

- **FastAPI**: Modern Python web framework
- **Auto-reload**: Development server with hot reload
- **API Documentation**: Available at `/docs` and `/redoc`
- **Logging**: Configurable logging levels

### Frontend Development

- **React**: Component-based architecture
- **TypeScript**: Type safety and better DX
- **Tailwind CSS**: Utility-first styling
- **Hot Reload**: Instant updates during development

### Code Quality

- **ESLint**: JavaScript/TypeScript linting
- **Prettier**: Code formatting
- **Type Checking**: Full TypeScript support
- **Testing**: Comprehensive test coverage

## 📚 Technology Stack

### Frontend

- **React 19.1.0** - UI framework
- **TypeScript** - Type safety
- **Tailwind CSS** - Styling
- **Radix UI** - Accessible components
- **Lucide React** - Icon library

### Backend

- **FastAPI** - Web framework
- **Python 3.8+** - Programming language
- **Uvicorn** - ASGI server
- **python-dotenv** - Environment variables
- **pytest** - Testing framework

### Development Tools

- **VS Code** - IDE configuration included
- **GitHub Copilot** - AI assistance
- **Create React App** - Frontend tooling
- **HTTP Test Files** - API testing

## 🤝 Contributing

1. **Fork the repository**
2. **Create a feature branch**: `git checkout -b feature/amazing-feature`
3. **Make your changes**: Implement your feature or fix
4. **Run tests**: Ensure all tests pass
5. **Commit changes**: `git commit -m 'Add amazing feature'`
6. **Push to branch**: `git push origin feature/amazing-feature`
7. **Open a Pull Request**: Describe your changes

### Development Guidelines

- Follow TypeScript best practices
- Write tests for new features
- Use conventional commit messages
- Update documentation as needed

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **FastAPI** - For the excellent Python web framework
- **React Team** - For the amazing frontend library
- **Tailwind CSS** - For the utility-first CSS framework
- **Radix UI** - For accessible UI components

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/anmalkov/umbrella/issues)
- **Discussions**: [GitHub Discussions](https://github.com/anmalkov/umbrella/discussions)
- **Documentation**: See `backend/README.md` and `frontend/README.md`

## 🔮 Roadmap

- [ ] **Docker Support**: Container deployment
- [ ] **Weather Widget**: Weather information display
- [ ] **Calendar Widget**: Calendar and events
- [ ] **Voice Control**: Voice command support
- [ ] **Mobile App**: React Native mobile application
- [ ] **Database Integration**: Persistent data storage

---

**Made with ❤️ by [anmalkov](https://github.com/anmalkov)**
