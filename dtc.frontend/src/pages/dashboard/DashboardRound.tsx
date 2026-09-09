import { useEffect, useState } from "react";
import type { RoundDto } from "../../dtos/rounds/RoundDto";
import { roundStatusLabel } from "../../enums/RoundStatus";
import { formatDate } from "../../utils/formatDate";
import { DashboardMatch } from "./DashboardMatch";
import { getMatchesByGroupId } from "../../services/groupService";
import type { MatchDto } from "../../dtos/match/MatchDto";

interface DashboardRoundProps {
  round: RoundDto;
}

export function DashboardRound({ round }: DashboardRoundProps) {
  const [matches, setMatches] = useState<MatchDto[]>([]);
  useEffect(() => {
    const loadMatches = async () => {
      const response = await getMatchesByGroupId(round.id);
      setMatches(response);
    };

    loadMatches();
  }, [round]);

  return (
    <section className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
      <div className="border-b border-gray-200 px-6 py-5 sm:px-8">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <div className="flex flex-wrap items-center gap-2">
              <h2 className="text-lg font-semibold text-gray-900">
                {round.name ?? `Runde ${round.sequence}`}
              </h2>
              <span className="rounded-full bg-gray-100 px-2.5 py-1 text-xs font-medium text-gray-600">
                {roundStatusLabel(round.status)}
              </span>
            </div>
            <p className="mt-1 text-sm text-gray-500">
              Geplanter Start: {formatDate(round.plannedStart)}
            </p>
          </div>
          <span className="text-sm font-medium text-gray-500">
            {matches?.length} {matches?.length === 1 ? "Match" : "Matches"}
          </span>
        </div>
      </div>

      <div className="divide-y divide-gray-100">
        {matches.map((match, index) => {
          return <DashboardMatch key={match.id} match={match} index={index} />;
        })}
      </div>
    </section>
  );
}
