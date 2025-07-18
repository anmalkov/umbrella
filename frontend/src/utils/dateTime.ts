export const formatDate = (date: Date, format: string, timezone = 'UTC'): string => {
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