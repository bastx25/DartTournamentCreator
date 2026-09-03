import { useEffect, useState } from "react";
import type { BoardDto } from "../dtos/board/BoardDto";
import { getBoards } from "../services/boardService";

export function useBoards() {
  const [boards, setBoards] = useState<BoardDto[]>([]);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchBoards() {
      try {
        setLoading(true);
        setError(null);

        const boards = await getBoards();
        setBoards(boards);
      } catch (error) {
        console.error(error);
        setError("Die Boards konnten nicht geladen werden.");
      } finally {
        setLoading(false);
      }
    }

    fetchBoards();
  }, []);

  return {
    boards,
    loading,
    error,
  };
}
