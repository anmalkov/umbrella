import { Bed, ChefHat, ChevronLeft, ChevronRight, FileText, Settings } from 'lucide-react';
import React, { useState } from 'react';
import { cn } from '../lib/utils';
import { Button } from './ui/button';

interface NavigationItem {
  id: string;
  label: string;
  icon: React.ReactNode;
}

const navigationItems: NavigationItem[] = [
  {
    id: 'kitchen',
    label: 'Kitchen',
    icon: <ChefHat className="w-5 h-5" />,
  },
  {
    id: 'bedroom',
    label: 'Bedroom',
    icon: <Bed className="w-5 h-5" />,
  },
  {
    id: 'logs',
    label: 'Logs',
    icon: <FileText className="w-5 h-5" />,
  },
  {
    id: 'settings',
    label: 'Settings',
    icon: <Settings className="w-5 h-5" />,
  },
];

export const NavigationPanel: React.FC = () => {
  const [isCollapsed, setIsCollapsed] = useState(true);
  const [activeItem, setActiveItem] = useState('kitchen');

  const toggleCollapse = () => {
    setIsCollapsed(!isCollapsed);
  };

  return (
    <div className={cn(
      "fixed left-0 top-0 h-full bg-card border-l border-border transition-all duration-300 ease-in-out shadow-lg z-50",
      isCollapsed ? "w-14" : "w-64"
    )}>
      <div className="flex flex-col h-full">
        {/* Header with toggle button */}
        <div className="flex items-center justify-between p-4 border-b border-border">
          {!isCollapsed && (
            <h2 className="text-lg font-semibold text-foreground">Umbrella</h2>
          )}
          <Button
            variant="ghost"
            size="icon"
            onClick={toggleCollapse}
            className="ml-auto hover:bg-accent"
            aria-label={isCollapsed ? "Expand panel" : "Collapse panel"}
          >
            {isCollapsed ? (
              <ChevronRight className="w-4 h-4" />
            ) : (
              <ChevronLeft className="w-4 h-4" />
            )}
          </Button>
        </div>

        {/* Navigation items */}
        <div className="flex-1 p-2">
          <nav className="space-y-2">
            {navigationItems.map((item) => (
              <Button
                key={item.id}
                variant={activeItem === item.id ? "secondary" : "ghost"}
                className={cn(
                  "w-full justify-start transition-all duration-200 hover:bg-accent hover:text-accent-foreground",
                  isCollapsed ? "px-2" : "px-4",
                  activeItem === item.id && "bg-accent text-accent-foreground shadow-sm"
                )}
                onClick={() => setActiveItem(item.id)}
                aria-label={`Navigate to ${item.label}`}
              >
                <div className="flex items-center">
                  {item.icon}
                  {!isCollapsed && (
                    <span className="ml-3 text-sm font-medium">
                      {item.label}
                    </span>
                  )}
                </div>
              </Button>
            ))}
          </nav>
        </div>

        {/* Footer */}
        <div className="p-4 border-t border-border">
          {!isCollapsed && (
            <div className="text-xs text-muted-foreground text-center">
              Dashboard v1.0
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
