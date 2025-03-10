import { useState } from "react";
import WeatherDisplay from "./components/WeatherDisplay";
import WeatherForecast from "./interfaces/WeatherForecast";
import axios from "axios";

const MainPage = () => {
  const [forecasts, setForecasts] = useState<WeatherForecast[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  const handleFetch = async () => {
    const ac = new AbortController();
    try {
      setIsLoading(true);
      const res = await axios.get("https://localhost:7122/WeatherForecast", {
        signal: ac.signal,
      });
      console.log("Fetch result", res);
      if (res && res.data) {
        setForecasts(res.data);
      } else {
        throw new Error("Could not fetch data");
      }
      console.log(forecasts);
    } catch (err) {
      if (axios.isCancel(err)) console.error("Request is canceled", err);
      else console.error("Error has occured", err);
    } finally {
      setIsLoading(false);
    }

    return () => {
      ac.abort();
    };
  };

  return (
    <div>
      {isLoading ? (
        <p>Waiting to load forecasts...</p>
      ) : (
        <div>
          {forecasts.length > 0 ? (
            <WeatherDisplay data={forecasts} />
          ) : (
            <p>Expected to load data...</p>
          )}
          <div style={{ display: "flex", flexDirection: "row", gap: 3 }}>
            <button
              type="button"
              onClick={handleFetch}
              style={{ marginTop: 10 }}
            >
              Press to get weather forecasts
            </button>
            <button
              type="button"
              onClick={() => setForecasts([])}
              style={{ marginTop: 10, backgroundColor: "black" }}
            >
              Reset
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default MainPage;
