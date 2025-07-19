import { useCallback, useEffect, useState } from 'react';

export interface UseIdleOptions {
  timeout: number; // timeout in milliseconds
  events?: string[]; // events to track for activity
  initialState?: boolean; // initial idle state
  disabled?: boolean; // when true, disables event listening (but keeps timeout running)
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
  disabled = false,
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

    // Only add event listeners if not disabled
    if (!disabled) {
      // Add event listeners
      events.forEach(event => {
        document.addEventListener(event, handleActivity, true);
      });

      // Listen for visibility changes
      document.addEventListener('visibilitychange', handleVisibilityChange);
    }

    // Cleanup
    return () => {
      clearTimeout(timeoutId);
      if (!disabled) {
        events.forEach(event => {
          document.removeEventListener(event, handleActivity, true);
        });
        document.removeEventListener('visibilitychange', handleVisibilityChange);
      }
    };
  }, [timeout, events, disabled]);

  return { isIdle, reset };
}
