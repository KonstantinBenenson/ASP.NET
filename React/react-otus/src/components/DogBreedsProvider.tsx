import axios from "axios";
import { useState } from "react";
import { DogBreedCard } from "./DogBreedCard";
import { ErrorPage } from "./ErrorPage";

export default function DogBreedsProvider() {
  const getUrl = import.meta.env.VITE_API_URL;
  const [dogBreeds, setDogBreeds] = useState<DogBreedProp[] | null>(null);
  const [isLoading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const getDogBreeds = async () => {
    setLoading(true);

    try {
      const response = await axios.get(`${getUrl}/breeds`);
      setDogBreeds(response.data.data);
    } catch (e) {
      setError(`An error occured while fetching dog breeds: ${e}`);
    } finally {
      setLoading(false);
    }
  };

  if (error) {
    return <ErrorPage error={error} />;
  }

  return (
    <div>
      <button onClick={getDogBreeds}>GET DOG BREEDS!</button>
      <div className="weather-style">
        {isLoading ? (
          <p style={{ fontSize: "16px" }}>Loading dog breeds...</p>
        ) : (
          dogBreeds?.map((dogBreed) => <DogBreedCard {...dogBreed} />)
        )}
      </div>
    </div>
  );
}

export interface DogBreedProp {
  id: String;
  type: String;
  attributes: {
    name: string;
    description: string;
    life: {
      max: number;
      min: number;
    };
    male_weight: {
      max: number;
      min: number;
    };
    female_weight: {
      max: number;
      min: number;
    };
    hypoallergenic: boolean;
  };
  relationships: {
    group: {
      data: {
        id: string;
        type: string;
      };
    };
  };
}
