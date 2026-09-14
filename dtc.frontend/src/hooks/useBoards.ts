import { useEffect, useState } from "react";
import {
  deleteBoard,
  getBoards,
  updateBoard,
  createBoard,
} from "../services/boardService";
import type { BoardDto } from "../dtos/board/BoardDto";
import type { CreateBoardDto } from "../dtos/board/CreateBoardDto";

export function useBoards() {
  const [boards, setBoards] = useState<BoardDto[]>([]);
  const [selectedBoard, setSelectedBoard] = useState<BoardDto | null>(null);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [boardToDelete, setBoardToDelete] = useState<BoardDto | null>(null);
  const [deleting, setDeleting] = useState(false);

  const [boardToUpdate, setBoardToUpdate] = useState<BoardDto | null>(null);
  const [updating, setUpdating] = useState(false);

  const [addBoard, setAddBoard] = useState(false);
  const [adding, setAdding] = useState(false);

  useEffect(() => {
    async function fetchBoards() {
      try {
        setLoading(true);
        setError(null);

        const boards = await getBoards();

        setBoards(boards);

        if (boards.length > 0) {
          setSelectedBoard(boards[0]);
        }
      } catch (error) {
        console.error(error);
        setError("Die Boards konnten nicht geladen werden.");
      } finally {
        setLoading(false);
      }
    }

    fetchBoards();
  }, []);

  const handleDelete = async () => {
    if (!boardToDelete) return;

    try {
      setDeleting(true);

      await deleteBoard(boardToDelete.id);

      setBoards((currentBoards) =>
        currentBoards.filter((board) => board.id !== boardToDelete.id),
      );

      if (selectedBoard?.id === boardToDelete.id) {
        setSelectedBoard(null);
      }

      setBoardToDelete(null);
    } catch (error) {
      console.error("Fehler beim Löschen des Boards:", error);
      setError("Die Boards konnte nicht gelöscht werden. ");
    } finally {
      setDeleting(false);
    }
  };

  const handleUpdate = async (board: BoardDto) => {
    try {
      setUpdating(true);
      setError(null);

      const updatedBoard = await updateBoard(board.id, {
        locationId: board.locationId,
        number: board.number,
        label: board.label,
        isActive: board.isActive,
      });

      setBoards((currentBoards) =>
        currentBoards.map((currentBoard) =>
          currentBoard.id === updatedBoard.id ? updatedBoard : currentBoard,
        ),
      );

      setSelectedBoard((currentSelectedBoard) =>
        currentSelectedBoard?.id === updatedBoard.id
          ? updatedBoard
          : currentSelectedBoard,
      );

      setBoardToUpdate(null);
    } catch (error) {
      console.error("Fehler beim Aktualisieren des Boards:", error);
      setError("Die Boards konnten nicht aktualisiert werden.");
    } finally {
      setUpdating(false);
    }
  };

  const handleAdd = async (board: CreateBoardDto) => {
    try {
      setAdding(true);
      setError(null);

      const newBoard = await createBoard({
        locationId: board.locationId,
        number: board.number,
        label: board.label,
        isActive: board.isActive,
      });

      setBoards((currentBoards) => [...currentBoards, newBoard]);

      setSelectedBoard(newBoard);
    } catch (error) {
      console.error("Fehler beim Aktualisieren des Boards:", error);
      setError("Die Boards konnte nicht hinzugefügt werden.");
    } finally {
      setAddBoard(false);
      setAdding(false);
    }
  };

  return {
    boards,
    selectedBoard,
    setSelectedBoard,

    loading,
    error,

    boardToDelete,
    setBoardToDelete,

    deleting,
    handleDelete,

    boardToUpdate,
    setBoardToUpdate,

    updating,
    handleUpdate,

    addBoard,
    setAddBoard,
    adding,
    handleAdd,
  };
}
