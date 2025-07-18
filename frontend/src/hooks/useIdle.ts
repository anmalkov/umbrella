import { useCallback, useEffect, useState } from 'react';

export interface UseIdleOptions {
  timeout: number; // timeout in milliseconds
  events?: string[]; // events to track for activity
  initialState?: boolean; // initial idle state
}

const DEFAULT_EVENTS = [
  'mousedown',
  'mousemove',
  'keypress',
  'scroll',
  'touchstart',
  'click',
];

/**
 * Custom hook to detect user idle state
 */
export function useIdle({
  timeout,
  events = DEFAULT_EVENTS,
  initialState = false,
}: UseIdleOptions) {
  const [isIdle, setIsIdle] = useState(initialState);

  const reset = useCallback(() => {
    setIsIdle(false);
  }, []);

  useEffect(() => {
    let timeoutId: NodeJS.Timeout;

    const handleActivity = () => {
      setIsIdle(false);
      clearTimeout(timeoutId);
      timeoutId = setTimeout(() => setIsIdle(true), timeout);
    };

    const handleVisibilityChange = () => {
      if (document.hidden) {
        clearTimeout(timeoutId);
      } else {
        handleActivity();
      }
    };

    // Set initial timeout
    timeoutId = setTimeout(() => setIsIdle(true), timeout);

    // Add event listeners
    events.forEach(event => {
      document.addEventListener(event, handleActivity, true);
    });

    // Listen for visibility changes
    document.addEventListener('visibilitychange', handleVisibilityChange);

    // Cleanup
    return () => {
      clearTimeout(timeoutId);
      events.forEach(event => {
        document.removeEventListener(event, handleActivity, true);
      });
      document.removeEventListener('visibilitychange', handleVisibilityChange);
    };
  }, [timeout, events]);

  return { isIdle, reset };
}
