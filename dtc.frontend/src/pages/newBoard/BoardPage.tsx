import Header from "../../components/Header";

import { BoardList } from "./BoardList";

import { BoardDetails } from "./BoardDetails";
import { DeleteBoardModal } from "./DeleteBoardModal";
import { UpdateBoardModal } from "./UpdateBoardModal";
import { CreateBoardModal } from "./CreateBoardModal";
import { useBoards } from "../../hooks/useBoards";
import { useEffect, useState } from "react";
import type { LocationDto } from "../../dtos/location/LocationDto";
import { getLocations } from "../../services/locationService";

export function BoardPage() {
  const {
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
  } = useBoards();

  const [locations, setLocations] = useState<LocationDto[]>([]);

  function getBoardLocation(locationId: number): string {
    const location = locations.find((location) => location.id === locationId);
    console.log(locationId);
    return location?.name ?? "fail";
  }

  useEffect(() => {
    const loadLocations = async () => {
      const locations = await getLocations();

      setLocations(locations);
    };

    loadLocations();
  }, []);

  return (
    <>
      <Header />

      <main className="min-h-screen bg-gray-100 px-4 py-8 sm:px-6 lg:px-8">
        {loading && (
          <div className="flex min-h-75 items-center justify-center">
            <p className="text-sm text-gray-400">Boards werden geladen...</p>
          </div>
        )}

        {error && (
          <div className="mx-auto max-w-7xl rounded-lg border border-red-500/20 bg-red-500/10 p-4">
            <p className="text-sm text-red-400">{error}</p>
          </div>
        )}

        {!loading && !error && (
          <div className="mx-auto grid max-w-7xl grid-cols-1 gap-6 lg:grid-cols-[1fr_350px]">
            <BoardList
              boards={boards}
              selectedBoard={selectedBoard}
              onSelectBoard={setSelectedBoard}
              onDeleteBoard={setBoardToDelete}
              onUpdateBoard={setBoardToUpdate}
              onAddBoard={setAddBoard}
              getBoardLocation={getBoardLocation}
            />

            <BoardDetails
              board={selectedBoard}
              getBoardLocation={getBoardLocation}
            />
          </div>
        )}
      </main>

      {boardToDelete && (
        <DeleteBoardModal
          board={boardToDelete}
          deleting={deleting}
          onCancel={() => setBoardToDelete(null)}
          onConfirm={handleDelete}
        />
      )}

      {boardToUpdate && (
        <UpdateBoardModal
          board={boardToUpdate}
          updating={updating}
          onCancel={() => setBoardToUpdate(null)}
          onConfirm={handleUpdate}
          locations={locations}
        />
      )}

      {addBoard && (
        <CreateBoardModal
          adding={adding}
          onCancel={() => {
            setAddBoard(false);
          }}
          onConfirm={handleAdd}
          locations={locations}
        />
      )}
    </>
  );
}
