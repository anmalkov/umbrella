
export const fetchPhoto = async () => {
    const response = await fetch('api/photos', { cache: "no-cache" });
    const blob = await response.blob();
    const objectUrl = URL.createObjectURL(blob);
    return objectUrl;
}