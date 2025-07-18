import { act, render, screen } from '@testing-library/react';
import TimeWidget from '../TimeWidget';

// Mock the timer to have predictable tests
jest.useFakeTimers();

describe('TimeWidget', () => {
  const defaultProps = {
    title: 'Test Time Widget',
    showTime: true,
    timezone: 'UTC',
    timeFormat: 'HH:mm:ss',
    showDate: true,
    dateFormat: 'DD.MM.YYYY',
  };

  beforeEach(() => {
    // Set a fixed date for consistent testing
    jest.setSystemTime(new Date('2023-07-15T14:30:45Z'));
  });

  afterEach(() => {
    jest.useRealTimers();
  });

  it('renders the widget title', () => {
    render(<TimeWidget {...defaultProps} />);
    expect(screen.getByText('Test Time Widget')).toBeInTheDocument();
  });

  it('displays time when showTime is true', () => {
    render(<TimeWidget {...defaultProps} />);
    
    // The time should be displayed in the format HH:mm:ss
    expect(screen.getByText(/14:30:45/)).toBeInTheDocument();
  });

  it('displays date when showDate is true', () => {
    render(<TimeWidget {...defaultProps} />);
    
    // The date should be displayed in the format DD.MM.YYYY
    expect(screen.getByText(/15.07.2023/)).toBeInTheDocument();
  });

  it('does not display time when showTime is false', () => {
    render(<TimeWidget {...defaultProps} showTime={false} />);
    
    // Time should not be displayed
    expect(screen.queryByText(/14:30:45/)).not.toBeInTheDocument();
  });

  it('does not display date when showDate is false', () => {
    render(<TimeWidget {...defaultProps} showDate={false} />);
    
    // Date should not be displayed
    expect(screen.queryByText(/15.07.2023/)).not.toBeInTheDocument();
  });

  it('updates time every second', () => {
    render(<TimeWidget {...defaultProps} />);
    
    // Initial time
    expect(screen.getByText(/14:30:45/)).toBeInTheDocument();
    
    // Advance time by 1 second
    act(() => {
      jest.advanceTimersByTime(1000);
    });
    
    // Time should update
    expect(screen.getByText(/14:30:46/)).toBeInTheDocument();
  });

  it('handles different time formats', () => {
    render(<TimeWidget {...defaultProps} timeFormat="HH:mm" />);
    
    // Should display only hours and minutes
    expect(screen.getByText(/14:30/)).toBeInTheDocument();
    expect(screen.queryByText(/14:30:45/)).not.toBeInTheDocument();
  });

  it('renders with custom timezone', () => {
    // Note: Testing timezone handling is complex due to browser differences
    // This test ensures the component renders without errors with different timezones
    render(<TimeWidget {...defaultProps} timezone="America/New_York" />);
    expect(screen.getByText('Test Time Widget')).toBeInTheDocument();
  });

  it('applies correct structure and accessibility', () => {
    render(<TimeWidget {...defaultProps} />);
    
    // Check that the widget has proper heading structure
    expect(screen.getByRole('heading', { level: 3 })).toBeInTheDocument();
    expect(screen.getByText('Test Time Widget')).toBeInTheDocument();
  });

  it('handles edge case where both showTime and showDate are false', () => {
    render(<TimeWidget {...defaultProps} showTime={false} showDate={false} />);
    
    // Only the title should be displayed
    expect(screen.getByText('Test Time Widget')).toBeInTheDocument();
    expect(screen.queryByText(/14:30:45/)).not.toBeInTheDocument();
    expect(screen.queryByText(/15.07.2023/)).not.toBeInTheDocument();
  });
});
