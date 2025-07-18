import React, { useCallback, useEffect, useState } from 'react';
import 'react-slideshow-image/dist/styles.css';
import { getNextPhoto, getPhotoUrl, getPreviousPhoto, PhotoInfo } from '../utils/photoApi';

// Helper function to format time
const formatTime = (date: Date, format: string): string => {
  const hours = date.getHours();
  const minutes = date.getMinutes();
  const seconds = date.getSeconds();
  
  return format
    .replace('HH', hours.toString().padStart(2, '0'))
    .replace('mm', minutes.toString().padStart(2, '0'))
    .replace('ss', seconds.toString().padStart(2, '0'));
};

// Helper function to format date
const formatDate = (date: Date, format: string): string => {
  const day = date.getDate();
  const month = date.getMonth() + 1;
  const year = date.getFullYear();
  const weekday = date.getDay();  // 0 (Sun) - 6 (Sat)

  const weekdays = [
    'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'
  ];
  const months = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December'
  ];
  
  return format
    .replace('dddd', weekdays[weekday])
    .replace('MMMM', months[month-1])
    .replace('DD', day.toString().padStart(2, '0'))
    .replace('D', day.toString())
    .replace('MM', month.toString().padStart(2, '0'))
    .replace('YYYY', year.toString());
};

export interface SlideshowOverlayProps {
  folder: string;
  interval: number; // interval in seconds
  isActive: boolean;
  onExit: () => void;
  showTime?: boolean;
  timeFormat?: string;
  showDate?: boolean;
  dateFormat?: string;
}

export const SlideshowOverlay: React.FC<SlideshowOverlayProps> = ({
  folder,
  interval,
  isActive,
  onExit,
  showTime = false,
  timeFormat = 'HH:mm:ss',
  showDate = false,
  dateFormat = 'DD.MM.YYYY',
}) => {
  const [currentPhoto, setCurrentPhoto] = useState<PhotoInfo | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [currentTime, setCurrentTime] = useState(new Date());

  const loadInitialPhoto = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);
      const photo = await getNextPhoto(folder);
      setCurrentPhoto(photo);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load photos');
      console.error('Error loading initial photo:', err);
    } finally {
      setIsLoading(false);
    }
  }, [folder]);

  // Load initial photo
  useEffect(() => {
    if (isActive && !currentPhoto) {
      loadInitialPhoto();
    }
  }, [isActive, currentPhoto, loadInitialPhoto]);

  // Update time every second
  useEffect(() => {
    if (!isActive || (!showTime && !showDate)) return;

    const timer = setInterval(() => {
      setCurrentTime(new Date());
    }, 1000);

    return () => clearInterval(timer);
  }, [isActive, showTime, showDate]);

  const loadNextPhoto = useCallback(async () => {
    if (!currentPhoto) return;

    try {
      const photo = await getNextPhoto(folder, currentPhoto.filename);
      setCurrentPhoto(photo);
    } catch (err) {
      console.error('Error loading next photo:', err);
    }
  }, [folder, currentPhoto]);

  const loadPreviousPhoto = useCallback(async () => {
    if (!currentPhoto) return;

    try {
      const photo = await getPreviousPhoto(folder, currentPhoto.filename);
      setCurrentPhoto(photo);
    } catch (err) {
      console.error('Error loading previous photo:', err);
    }
  }, [folder, currentPhoto]);

  // Handle keyboard navigation (only ESC key)
  useEffect(() => {
    if (!isActive) return;

    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Escape') {
        onExit();
      }
    };

    document.addEventListener('keydown', handleKeyDown);
    return () => document.removeEventListener('keydown', handleKeyDown);
  }, [isActive, onExit]);

  // Handle click zones for navigation
  const handleSlideClick = useCallback((e: React.MouseEvent) => {
    console.log('handleSlideClick called!'); // Debug: confirm handler is called
    
    const rect = e.currentTarget.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const width = rect.width;
    
    // Left zone (50px from left edge)
    if (x <= 50) {
      loadPreviousPhoto();
    }
    // Right zone (50px from right edge)
    else if (x >= width - 50) {
      loadNextPhoto();
    }
    // Center zone - exit slideshow
    else {
      onExit();
    }
  }, [loadPreviousPhoto, loadNextPhoto, onExit]);

  // Auto-advance slideshow
  useEffect(() => {
    if (!isActive || !currentPhoto || isLoading) return;

    const timer = setInterval(loadNextPhoto, interval * 1000);
    return () => clearInterval(timer);
  }, [isActive, currentPhoto, isLoading, interval, loadNextPhoto]);

  if (!isActive) return null;

  return (
    <div
      className={`fixed inset-0 z-50 bg-black transition-opacity duration-500 ${
        isActive ? 'opacity-100' : 'opacity-0'
      }`}
    >
      <div className="absolute inset-0 flex items-center justify-center">
        {isLoading && (
          <div className="text-white text-xl">Loading slideshow...</div>
        )}
        
        {error && (
          <div className="text-red-500 text-xl text-center p-4">
            <div>Error: {error}</div>
            <div className="text-sm mt-2">Click to exit</div>
          </div>
        )}
        
        {currentPhoto && !isLoading && !error && (
          <div className="w-full h-full relative">
            <img
              src={getPhotoUrl(folder, currentPhoto.filename)}
              alt={currentPhoto.filename}
              className="w-full h-full object-contain"
              onError={(e) => {
                console.error('Error loading image:', currentPhoto.filename);
                setError(`Failed to load image: ${currentPhoto.filename}`);
              }}
            />
            
            {/* Single click zone for all navigation */}
            <div 
              className="absolute top-0 left-0 w-full h-full cursor-pointer"
              onClick={handleSlideClick}
            >
              {/* Visual indicators for navigation zones */}
              <div className="absolute top-0 left-0 w-[50px] h-full flex items-center justify-center hover:bg-white hover:bg-opacity-10 transition-colors">
                <div className="text-white opacity-50 hover:opacity-100 transition-opacity text-2xl">
                  ‹
                </div>
              </div>
              
              <div className="absolute top-0 right-0 w-[50px] h-full flex items-center justify-center hover:bg-white hover:bg-opacity-10 transition-colors">
                <div className="text-white opacity-50 hover:opacity-100 transition-opacity text-2xl">
                  ›
                </div>
              </div>
            </div>
            
            {/* Photo info overlay */}
            <div className="absolute bottom-4 right-4 text-white bg-black bg-opacity-50 px-3 py-2 rounded pointer-events-none z-20">
              <div className="text-sm text-right">
                {currentPhoto.filename}
              </div>
              <div className="text-xs text-gray-300 text-right">
                Photo {currentPhoto.total_images > 0 ? '1' : '0'} of {currentPhoto.total_images}
              </div>
            </div>
                       
            {/* Time/Date overlay */}
            {(showTime || showDate) && (
              <div className="absolute top-4 left-4 text-white bg-black bg-opacity-50 px-3 py-2 rounded pointer-events-none z-20">
                {showTime && (
                  <div className="text-lg font-mono">
                    {formatTime(currentTime, timeFormat)}
                  </div>
                )}
                {showDate && (
                  <div className="text-sm text-gray-300">
                    {formatDate(currentTime, dateFormat)}
                  </div>
                )}
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};
