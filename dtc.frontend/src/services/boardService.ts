import axios from "axios";
import type { BoardDto } from "../dtos/board/BoardDto";

export async function getBoards(): Promise<BoardDto[]> {
  const response = await axios.get<BoardDto[]>("/api/boards/location/1");

  return response.data;
}
