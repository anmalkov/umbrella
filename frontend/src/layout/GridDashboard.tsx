import React, { useEffect, useState } from 'react';
import { useIdle } from '../hooks/useIdle';
import { SlideshowOverlay } from '../slideshow/SlideshowOverlay';
import { fetchRoomConfig, RoomConfig, SlideshowWidgetConfig } from '../utils/fetchConfig';
import TimeWidget from '../widgets/TimeWidget';

interface GridDashboardProps {
  roomId: string;
  onSlideshowStateChange: (isActive: boolean) => void;
}

const GridDashboard: React.FC<GridDashboardProps> = ({ roomId, onSlideshowStateChange }) => {
  const [config, setConfig] = useState<RoomConfig | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [slideshowConfig, setSlideshowConfig] = useState<SlideshowWidgetConfig | null>(null);
  const [isSlideshowActive, setIsSlideshowActive] = useState(false);

  // Setup idle detection for slideshow (excluding mouse events)
  // When slideshow is active, disable event listening to prevent slideshow navigation from triggering idle reset
  const { isIdle, reset } = useIdle({
    timeout: slideshowConfig ? slideshowConfig.inactivityDelay * 1000 : 60000,
    initialState: false,
    events: ['keypress', 'scroll', 'touchstart', 'click', 'mousedown', 'mousemove'],
    disabled: isSlideshowActive, // Disable idle detection when slideshow is active
  });

  // Track slideshow state changes
  useEffect(() => {
    setIsSlideshowActive(isIdle);
  }, [isIdle]);

  // Notify parent about slideshow state changes
  useEffect(() => {
    onSlideshowStateChange(isIdle);
  }, [isIdle, onSlideshowStateChange]);

  useEffect(() => {
    const loadConfig = async () => {
      try {
        setLoading(true);
        setError(null);
        const roomConfig = await fetchRoomConfig(roomId);
        setConfig(roomConfig);
        
        // Find slideshow widget (should be unique)
        const slideshowWidgets = roomConfig.widgets.filter(w => w.type === 'slideshow') as SlideshowWidgetConfig[];
        if (slideshowWidgets.length > 1) {
          console.warn('Multiple slideshow widgets found, using the first one');
        }
        setSlideshowConfig(slideshowWidgets[0] || null);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load room configuration');
      } finally {
        setLoading(false);
      }
    };

    loadConfig();
  }, [roomId]);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-lg text-muted-foreground">Loading room configuration...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-lg text-destructive">Error: {error}</div>
      </div>
    );
  }

  if (!config) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-lg text-muted-foreground">No configuration found</div>
      </div>
    );
  }

  // Filter out slideshow widgets from grid rendering
  const gridWidgets = config.widgets.filter(w => w.type !== 'slideshow');

  // Calculate grid dimensions based on widgets (excluding slideshow)
  const maxX = gridWidgets.length > 0 ? Math.max(...gridWidgets.map(w => {
    if (w.type === 'time') {
      return w.position.x + w.position.w;
    }
    return 0;
  })) : 8;
  
  const maxY = gridWidgets.length > 0 ? Math.max(...gridWidgets.map(w => {
    if (w.type === 'time') {
      return w.position.y + w.position.h;
    }
    return 0;
  })) : 8;

  return (
    <div className="p-4">
      <h2 className="text-2xl font-bold mb-4 capitalize">{config.roomId} Dashboard</h2>
      <div 
        className="grid gap-4"
        style={{
          gridTemplateColumns: `repeat(${Math.max(maxX, 8)}, 1fr)`,
          gridTemplateRows: `repeat(${Math.max(maxY, 8)}, 150px)`,
        }}
      >
        {gridWidgets.map((widget, index) => {
          if (widget.type === 'time') {
            const gridArea = `${widget.position.y + 1} / ${widget.position.x + 1} / ${widget.position.y + widget.position.h + 1} / ${widget.position.x + widget.position.w + 1}`;
            
            return (
              <div
                key={index}
                style={{ gridArea }}
                className="min-h-0"
              >
                <TimeWidget
                  title={widget.title}
                  showTime={widget.showTime}
                  timezone={widget.timezone}
                  timeFormat={widget.timeFormat}
                  showDate={widget.showDate}
                  dateFormat={widget.dateFormat}
                />
              </div>
            );
          }
          
          return (
            <div
              key={index}
              className="bg-card border border-border rounded-lg p-4 flex items-center justify-center"
            >
              <div className="text-muted-foreground">
                Unknown widget type: {widget.type}
              </div>
            </div>
          );
        })}
      </div>
      
      {/* Slideshow overlay */}
      {slideshowConfig && (
        <SlideshowOverlay
          folder={slideshowConfig.folder}
          interval={slideshowConfig.interval}
          isActive={isIdle}
          onExit={reset}
          showTime={slideshowConfig.showTime}
          timeFormat={slideshowConfig.timeFormat}
          showDate={slideshowConfig.showDate}
          dateFormat={slideshowConfig.dateFormat}
        />
      )}
    </div>
  );
};

export default GridDashboard;
