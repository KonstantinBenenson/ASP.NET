import React from "react";
import { DogBreedProp } from "./DogBreedsProvider";

export const DogBreedCard: React.FC<DogBreedProp> = (prop) => {
  const { attributes } = prop;
  return (
    <div style={{ marginTop: "10px", border: "1px solid lightgreen" }}>
      <span style={{ fontSize: "1.1rem", fontWeight: "bolder" }}>
        {attributes.name}
      </span>
      <div style={{ padding: "0px 10px", textAlign: "left" }}>
        <p>{attributes.description}</p>
      </div>
    </div>
  );
};

/* 
 {
      "id": "68f47c5a-5115-47cd-9849-e45d3c378f12",
      "type": "breed",
      "attributes": {
        "name": "Caucasian Shepherd Dog",
        "description": "The Caucasian Shepherd Dog is a large and powerful breed of dog from the Caucasus Mountains region. These dogs are large in size, with a thick double coat to protect them from the cold. They have a regal bearing, with a proud and confident demeanor. They are highly intelligent and loyal, making them excellent guard dogs. They are courageous and alert, with an instinct to protect their family and property. They are highly trainable, but require firm and consistent training.",
        "life": {
          "max": 20,
          "min": 15
        },
        "male_weight": {
          "max": 90,
          "min": 50
        },
        "female_weight": {
          "max": 70,
          "min": 45
        },
        "hypoallergenic": false
      },
      "relationships": {
        "group": {
          "data": {
            "id": "8000793f-a1ae-4ec4-8d55-ef83f1f644e5",
            "type": "group"
          }
        }
      }
    },

    */
