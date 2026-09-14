import type { BoardDto } from "../../dtos/board/BoardDto";

interface BoardDetailsProps {
  board: BoardDto | null;
  getBoardLocation: (boardId: number) => string;
}

export function BoardDetails({ board, getBoardLocation }: BoardDetailsProps) {
  return (
    <aside className="h-fit rounded-xl border border-gray-200 bg-white p-5 shadow-sm lg:sticky lg:top-6">
      {board ? (
        <>
          <div className="mb-5 border-b border-gray-200 pb-5">
            <p className="mb-1 text-xs font-medium uppercase tracking-wider text-gray-400">
              Board Details
            </p>

            <h2 className="text-xl font-semibold text-gray-900">
              Board {board.label}
            </h2>
          </div>

          <div className="space-y-4">
            <Detail label="Nummer" value={board.number.toString() ?? ""} />
            <Detail label="Label" value={board.label ?? ""} />
            <Detail
              label="Addresse"
              value={getBoardLocation(board.locationId)}
            />
            <Detail label="Ist Aktiv" value={board.isActive ? "Ja" : "Nein"} />
          </div>
        </>
      ) : (
        <p className="text-sm text-gray-500">Wähle eine Board aus.</p>
      )}
    </aside>
  );
}

function Detail({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <span className="block text-xs font-medium uppercase tracking-wide text-gray-400">
        {label}
      </span>

      <strong className="mt-1 block text-sm font-medium text-gray-900">
        {value}
      </strong>
    </div>
  );
}
