import { Moon, Sun } from 'lucide-react';
import { useState } from 'react';
import { NavigationPanel } from './components/NavigationPanel';
import { Button } from './components/ui/button';
import GridDashboard from './layout/GridDashboard';

function App() {
  const [isDark, setIsDark] = useState(false);
  const [currentRoom, setCurrentRoom] = useState('kitchen');
  const [isSlideshowActive, setIsSlideshowActive] = useState(false);

  const toggleDarkMode = () => {
    setIsDark(!isDark);
    document.documentElement.classList.toggle('dark', !isDark);
  };

  return (
    <div className="min-h-screen bg-background">
      {/* Main content area */}
      <div className={`min-h-screen ${!isSlideshowActive ? 'pl-16' : ''}`}>
        {/* Top bar */}
        <div className="flex items-center justify-between p-4 md:p-6 border-b border-border">
          <h1 className="text-2xl md:text-3xl font-bold text-foreground">
            Smart Home Dashboard
          </h1>
          <Button
            variant="outline"
            size="icon"
            onClick={toggleDarkMode}
            className="ml-4"
            aria-label={isDark ? "Switch to light mode" : "Switch to dark mode"}
          >
            {isDark ? (
              <Sun className="w-4 h-4" />
            ) : (
              <Moon className="w-4 h-4" />
            )}
          </Button>
        </div>

        {/* Content area */}
        <div className="p-4 md:p-6">
          <GridDashboard roomId={currentRoom} onSlideshowStateChange={setIsSlideshowActive} />
        </div>
      </div>

      {/* Navigation Panel */}
      {!isSlideshowActive && <NavigationPanel />}
    </div>
  );
}

export default App;
