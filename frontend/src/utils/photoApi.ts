/**
 * API utilities for photo slideshow functionality
 */

export interface PhotoInfo {
  filename: string;
  url: string;
  total_images: number;
}

export interface PhotoListResponse {
  folder: string;
  photos: Array<{
    filename: string;
    url: string;
  }>;
  total_images: number;
}

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:8081';

/**
 * Get the next photo in the slideshow sequence
 */
export async function getNextPhoto(folder: string, current?: string): Promise<PhotoInfo> {
  const params = new URLSearchParams({ folder });
  if (current) {
    params.append('current', current);
  }
  
  const response = await fetch(`${API_BASE_URL}/api/photos/next?${params}`);
  
  if (!response.ok) {
    throw new Error(`Failed to get next photo: ${response.statusText}`);
  }
  
  return response.json();
}

/**
 * Get the previous photo in the slideshow sequence
 */
export async function getPreviousPhoto(folder: string, current?: string): Promise<PhotoInfo> {
  const params = new URLSearchParams({ folder });
  if (current) {
    params.append('current', current);
  }
  
  const response = await fetch(`${API_BASE_URL}/api/photos/previous?${params}`);
  
  if (!response.ok) {
    throw new Error(`Failed to get previous photo: ${response.statusText}`);
  }
  
  return response.json();
}

/**
 * List all photos in a folder
 */
export async function listPhotos(folder: string): Promise<PhotoListResponse> {
  const response = await fetch(`${API_BASE_URL}/api/photos/list/${folder}`);
  
  if (!response.ok) {
    throw new Error(`Failed to list photos: ${response.statusText}`);
  }
  
  return response.json();
}

/**
 * Get the full URL for a photo file
 */
export function getPhotoUrl(folder: string, filename: string): string {
  return `${API_BASE_URL}/api/photos/file/${folder}/${filename}`;
}
