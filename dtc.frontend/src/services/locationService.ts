import axios from "axios";
import type { CreateLocationDto } from "../dtos/location/CreateLocationDto";
import type { LocationDto } from "../dtos/location/LocationDto";

export async function createLocation(
  data: CreateLocationDto,
): Promise<LocationDto> {
  const response = await axios.post<LocationDto>("/api/locations", data, {
    headers: {
      "Content-Type": "application/json",
    },
  });

  return response.data;
}
