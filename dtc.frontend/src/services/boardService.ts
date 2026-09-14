import axios from "axios";
import type { BoardDto } from "../dtos/board/BoardDto";
import type { CreateBoardDto } from "../dtos/board/CreateBoardDto";
import type { UpdateBoardDto } from "../dtos/board/UpdateBoardDto";

export async function getBoards(): Promise<BoardDto[]> {
  const response = await axios.get<BoardDto[]>(`/api/boards`);

  return response.data;
}

export async function getBoard(boardId: number): Promise<BoardDto | null> {
  const response = await axios.get<BoardDto>(`/api/boards/${boardId}`);

  return response.data;
}

export async function createBoard(data: CreateBoardDto): Promise<BoardDto> {
  const response = await axios.post<BoardDto>("/api/boards", data, {
    headers: { "Content-Type": "application/json" },
  });
  return response.data;
}

export async function deleteBoard(id: number): Promise<string> {
  const response = await axios.delete(`/api/boards/${id}`);

  return response.data;
}

export async function updateBoard(
  id: number,
  data: UpdateBoardDto,
): Promise<BoardDto> {
  const response = await axios.put<BoardDto>(`/api/boards/${id}`, data, {
    headers: {
      "Content-Type": "application/json",
    },
  });

  return response.data;
}
