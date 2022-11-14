import React from 'react';
// images
import ClearDay from '../images/weather/clear_day.png'
import ClearNight from '../images/weather/clear_night.png'
import MostlyClearDay from '../images/weather/mostly_clear_day.png'
import MostlyClearNight from '../images/weather/mostly_clear_night.png'
import PartlyCloudyDay from '../images/weather/partly_cloudy_day.png'
import PartlyCloudyNight from '../images/weather/partly_cloudy_night.png'
import MostlyCloudyDay from '../images/weather/mostly_cloudy_day.png'
import MostlyCloudyNight from '../images/weather/mostly_cloudy_night.png'
import Cloudy from '../images/weather/cloudy.png'
import ScatteredShowersDay from '../images/weather/scattered_showers_day.png'
import ScatteredShowersNight from '../images/weather/scattered_showers_night.png'
import ScatteredTstormsDay from '../images/weather/isolated_scattered_tstorms_day.png'
import ScatteredTstormsNight from '../images/weather/isolated_scattered_tstorms_night.png'
import Drizzle from '../images/weather/drizzle.png'
import ShowersRain from '../images/weather/showers_rain.png'
import HeavyRain from '../images/weather/heavy_rain.png'
import StrongTstorms from '../images/weather/strong_tstorms.png'
import Flurries from '../images/weather/flurries.png'
import SnowShowers from '../images/weather/snow_showers.png'
import RainSnow from '../images/weather/wintry_mix_rain_snow.png'
import Blizzard from '../images/weather/blizzard.png'
import Fog from '../images/weather/haze_fog_dust_smoke.png'


const WeatherIcon = ({ conditionCode, isNight, className }) => {

    let icon = '';
    switch (conditionCode) {
        case 1: // clear
            console.log("clear");
            isNight ? icon = ClearNight : icon = ClearDay;
            break;
        case 2: // few-clouds
            isNight ? icon = MostlyClearNight : icon = MostlyClearDay;
            break;
        case 3: // broken-clouds
            isNight ? icon = PartlyCloudyNight : icon = PartlyCloudyDay;
            break;
        case 4: // mostly-cloudy
            isNight ? icon = MostlyCloudyNight : icon = MostlyCloudyDay;
            break;
        case 5: // cloudy
            icon = Cloudy;
            break;
        case 10: // scattered-showers
            isNight ? icon = ScatteredShowersNight : icon = ScatteredShowersDay;
            break;
        case 11: // scattered-tstorms
            isNight ? icon = ScatteredTstormsNight : icon = ScatteredTstormsDay;
            break;
        case 12: // light-rain
            icon = Drizzle;
            break;
        case 13: // moderate-rain
            icon = ShowersRain;
            break;
        case 14: // heavy-rain
            icon = HeavyRain;
            break;
        case 15: // tstorms
            icon = StrongTstorms;
            break;
        case 20: // light-snow
            icon = Flurries;
            break;
        case 21: // heavy-snow
            icon = SnowShowers;
            break;
        case 22: // rain-snow
            icon = RainSnow;
            break;
        case 23: // blizzard
            icon = Blizzard;
            break;
        case 30: // fog
            icon = Fog;
            break;
        default:
            isNight ? icon = ClearNight : icon = ClearDay;
            break;
    }

    return (
        <img src={icon} className={className} />
    );
}

export default WeatherIcon;