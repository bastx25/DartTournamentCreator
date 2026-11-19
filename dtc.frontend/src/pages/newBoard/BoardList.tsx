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
            Dartboards
          </h1>

          <p className="mt-1 text-sm text-gray-500">
            {boards.length} {boards.length === 1 ? "Board" : "Boards"} verfügbar
          </p>
        </div>

        <button
          type="button"
          onClick={(e) => {
            e.stopPropagation();
            onAddBoard(true);
          }}
          className="flex h-9 items-center gap-1.5 rounded-lg bg-blue-600 px-3 text-sm font-semibold text-white shadow-sm transition hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
          title="Board hinzufügen"
        >
          <span>+</span>
          <span>Board hinzufügen</span>
        </button>
      </div>

      <div className="grid grid-cols-[1.5fr_1.5fr_100px_140px] items-center border-b border-gray-200 bg-gray-50 px-4 py-2.5 text-xs font-medium uppercase tracking-wide text-gray-500 portrait:max-sm:grid-cols-[1fr_80px]">
        <span>Board / Bezeichnung</span>
        <span className="portrait:max-sm:hidden">Austragungsort</span>
        <span className="portrait:max-sm:hidden">Status</span>
        <span className="text-right">Aktionen</span>
      </div>

      <div className="divide-y divide-gray-100">
        {boards.length === 0 ? (
          <div className="p-8 text-center text-sm text-gray-500">
            Noch keine Dartboards vorhanden. Klicke auf „+ Board hinzufügen“, um dein erstes Board anzulegen.
          </div>
        ) : (
          boards.map((board) => (
            <BoardRow
              key={board.id}
              board={board}
              selected={selectedBoard?.id === board.id}
              onSelect={onSelectBoard}
              onDelete={onDeleteBoard}
              onUpdate={onUpdateBoard}
              getBoardLocation={getBoardLocation}
            />
          ))
        )}
      </div>
    </section>
  );
}
