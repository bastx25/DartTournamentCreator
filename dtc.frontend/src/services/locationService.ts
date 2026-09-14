import axios from "axios";
import type { CreateLocationDto } from "../dtos/location/CreateLocationDto";
import type { LocationDto } from "../dtos/location/LocationDto";
import type { UpdateLocationDto } from "../dtos/location/UpdateLocationDto";

export async function getLocations(): Promise<LocationDto[]> {
  const response = await axios.get<LocationDto[]>("/api/locations");

  return response.data;
}

export async function deleteLocation(id: number): Promise<string> {
  const response = await axios.delete(`/api/locations/${id}`);

  return response.data;
}

export async function updateLocation(
  id: number,
  data: UpdateLocationDto,
): Promise<LocationDto> {
  const response = await axios.put<LocationDto>(`/api/locations/${id}`, data, {
    headers: {
      "Content-Type": "application/json",
    },
  });

  return response.data;
}

export async function createLocation(
  data: CreateLocationDto,
): Promise<LocationDto> {
  const response = await axios.post<LocationDto>(`/api/locations`, data, {
    headers: {
      "Content-Type": "application/json",
    },
  });

  return response.data;
}
