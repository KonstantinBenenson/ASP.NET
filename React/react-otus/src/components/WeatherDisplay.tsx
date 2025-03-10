import WeatherForecast from "../interfaces/WeatherForecast";

const WeatherDisplay = ({ data }: WeatherDisplayProps) => {
  return (
    <div>
      <h2>Weather Forecasts</h2>
      <table
        border={1}
        cellPadding={5}
        style={{ width: "100%", borderCollapse: "collapse" }}
      >
        <thead>
          <tr>
            <th>Date</th>
            <th>Celcius</th>
            <th>Fahrenheit</th>
            <th>Summary</th>
          </tr>
        </thead>
        <tbody>
          {data.map((forecasts, idx) => (
            <tr key={idx}>
              <td>{forecasts.date}</td>
              <td>{forecasts.temperatureC}</td>
              <td>{forecasts.temperatureF}</td>
              <td>{forecasts.summary}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default WeatherDisplay;

export interface WeatherDisplayProps {
  data: WeatherForecast[];
}
