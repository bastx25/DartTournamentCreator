import type { BoardDto } from "../../dtos/board/BoardDto";
import { BoardRow } from "./BoardRow";

interface BoardListProps {
  boards: BoardDto[];
  selectedBoard: BoardDto | null;
  onSelectBoard: (board: BoardDto) => void;
  onDeleteBoard: (board: BoardDto) => void;
  onUpdateBoard: (board: BoardDto) => void;
  onAddBoard: (addBoard: boolean) => void;
  getBoardLocation: (locationid: number) => string;
}

export function BoardList({
  boards,
  selectedBoard,
  onSelectBoard,
  onDeleteBoard,
  onUpdateBoard,
  onAddBoard,
  getBoardLocation,
}: BoardListProps) {
  return (
    <section className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
      <div className="flex items-center justify-between border-b border-gray-200 px-4 py-4">
        <div>
          <h1 className="text-xl font-semibold tracking-tight text-gray-900">
            Boards
          </h1>

          <p className="mt-1 text-sm text-gray-500">
            {boards.length} Boards verfügbar
          </p>
        </div>

        <button
          type="button"
          onClick={(e) => {
            e.stopPropagation();
            onAddBoard(true);
          }}
          className="flex h-9 w-9 items-center justify-center rounded-lg bg-green-600 text-xl font-semibold text-white shadow-sm transition hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2"
          title="Boards hinzufügen"
        >
          +
        </button>
      </div>

      <div className="grid grid-cols-[1fr_580px_80px] items-center border-b border-gray-200 bg-gray-50 px-4 py-2.5 text-xs font-medium uppercase tracking-wide text-gray-500">
        <span>Board</span>
        <span>Address</span>
        <span className="text-right">Actions</span>
      </div>

      <div className="divide-y divide-gray-100">
        {boards.map((board) => (
          <BoardRow
            key={board.id}
            board={board}
            selected={selectedBoard?.id === board.id}
            onSelect={onSelectBoard}
            onDelete={onDeleteBoard}
            onUpdate={onUpdateBoard}
            getBoardLocation={getBoardLocation}
          />
        ))}
      </div>
    </section>
  );
}
