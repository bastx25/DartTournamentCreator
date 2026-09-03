import axios from "axios";
import type { BoardDto } from "../dtos/board/BoardDto";

export async function getBoards(locationId: number): Promise<BoardDto[]> {
  const response = await axios.get<BoardDto[]>(
    `/api/boards/location/${locationId}`,
  );

  return response.data;
}
