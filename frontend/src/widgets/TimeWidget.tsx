import React, { useEffect, useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';

interface TimeWidgetProps {
  title: string;
  showTime: boolean;
  timezone: string;
  timeFormat: string;
  showDate: boolean;
  dateFormat: string;
}

const TimeWidget: React.FC<TimeWidgetProps> = ({
  title,
  showTime,
  timezone,
  timeFormat,
  showDate,
  dateFormat
}) => {
  const [currentTime, setCurrentTime] = useState<Date>(new Date());

  useEffect(() => {
    const timer = setInterval(() => {
      setCurrentTime(new Date());
    }, 1000);

    return () => clearInterval(timer);
  }, []);

  const formatTime = (date: Date): string => {
    // Convert timeFormat from moment.js style to Intl.DateTimeFormat style
    const options: Intl.DateTimeFormatOptions = {
      timeZone: timezone,
      hour12: false,
    };

    // Parse the timeFormat (HH:mm:ss)
    if (timeFormat.includes('HH')) {
      options.hour = '2-digit';
    }
    if (timeFormat.includes('mm')) {
      options.minute = '2-digit';
    }
    if (timeFormat.includes('ss')) {
      options.second = '2-digit';
    }

    return new Intl.DateTimeFormat('en-US', options).format(date);
  };

  const formatDate = (date: Date): string => {
    // Convert dateFormat from moment.js style to Intl.DateTimeFormat style
    const options: Intl.DateTimeFormatOptions = {
      timeZone: timezone,
    };

    // Parse the dateFormat (DD.MM.YYYY)
    if (dateFormat.includes('DD')) {
      options.day = '2-digit';
    }
    if (dateFormat.includes('MM')) {
      options.month = '2-digit';
    }
    if (dateFormat.includes('YYYY')) {
      options.year = 'numeric';
    }
    if (dateFormat.includes('dddd')) {
      options.weekday = 'long';
    }
    if (dateFormat.includes('MMMM')) {
      options.month = 'long';
    }

    const parts = new Intl.DateTimeFormat('en-US', options).formatToParts(date);
    const map: Record<string, string> = {};
    for (const part of parts) {
      if (part.type !== 'literal') {
        map[part.type] = part.value;
      }
    }

    // Replace format tokens with actual values
    return dateFormat
      .replace('dddd', map.weekday || '')
      .replace('MMMM', map.month || '')
      .replace('MM', map.month || '') // Optional: handle numeric month
      .replace('DD', map.day || '')
      .replace('D', String(Number(map.day || '1')))
      .replace('YYYY', map.year || '');
    };

  return (
    <Card className="w-full h-full">
      <CardHeader>
        <CardTitle className="text-lg">{title}</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="flex flex-col items-center space-y-2">
          {showTime && (
            <div className="text-3xl font-mono font-bold text-primary">
              {formatTime(currentTime)}
            </div>
          )}
          {showDate && (
            <div className="text-lg font-medium text-muted-foreground">
              {formatDate(currentTime)}
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  );
};

export default TimeWidget;
