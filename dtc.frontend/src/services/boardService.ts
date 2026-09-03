import axios from "axios";
import type { BoardDto } from "../dtos/board/BoardDto";
import type { CreateBoardDto } from "../dtos/board/CreateBoardDto";

export async function getBoards(locationId: number): Promise<BoardDto[]> {
  const response = await axios.get<BoardDto[]>(
    `/api/boards/location/${locationId}`,
  );

  return response.data;
}


export async function createBoard(data: CreateBoardDto): Promise<BoardDto> {
  const response = await axios.post<BoardDto>("/api/boards", data, {
    headers: { "Content-Type": "application/json" },
  });
  return response.data;
}
