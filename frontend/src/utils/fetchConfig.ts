const API_BASE_URL = process.env.REACT_APP_API_URL || '';

export interface WidgetPosition {
  x: number;
  y: number;
  w: number;
  h: number;
}

export interface TimeWidgetConfig {
  type: 'time';
  title: string;
  showTime: boolean;
  timezone: string;
  timeFormat: string;
  showDate: boolean;
  dateFormat: string;
  position: WidgetPosition;
}

export interface SlideshowWidgetConfig {
  type: 'slideshow';
  folder: string;
  interval: number;
  inactivityDelay: number;
  showTime?: boolean;
  timeFormat?: string;
  showDate?: boolean;
  dateFormat?: string;
}

export type WidgetConfig = TimeWidgetConfig | SlideshowWidgetConfig;

export interface RoomConfig {
  roomId: string;
  widgets: WidgetConfig[];
}

export const fetchRoomConfig = async (roomId: string): Promise<RoomConfig> => {
  const response = await fetch(`${API_BASE_URL}/api/config/${roomId}`);
  
  if (!response.ok) {
    throw new Error(`Failed to fetch room config: ${response.status} ${response.statusText}`);
  }
  
  return response.json();
};
