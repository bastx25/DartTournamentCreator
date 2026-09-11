import { useEffect, useState } from "react";

import { braketStatusLabel } from "../../enums/BraketStatus";
import { formatDate } from "../../utils/formatDate";
import { DashboardMatch } from "./DashboardMatch";
import type { MatchDto } from "../../dtos/match/MatchDto";
import type { BraketDto } from "../../dtos/brakets/BraketDto";
import { getMatchesByBraketId } from "../../services/braketService";


interface DashboardBraketProps {
  braket: BraketDto;
}

export function DashboardBraket({ braket }: DashboardBraketProps) {
  const [matches, setMatches] = useState<MatchDto[]>([]);
  useEffect(() => {
    const loadMatches = async () => {
      const response = await getMatchesByBraketId(braket.id);
      setMatches(response);
    };

    loadMatches();
  }, [braket]);

  return (
    <section className="overflow-hidden braketed-xl border border-gray-200 bg-white shadow-sm">
      <div className="border-b border-gray-200 px-6 py-5 sm:px-8">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <div className="flex flex-wrap items-center gap-2">
              <h2 className="text-lg font-semibold text-gray-900">
                {braket.name ?? `Runde ${braket.sequence}`}
              </h2>
              <span className="braketed-full bg-gray-100 px-2.5 py-1 text-xs font-medium text-gray-600">
                {braketStatusLabel(braket.status)}
              </span>
            </div>
            <p className="mt-1 text-sm text-gray-500">
              Geplanter Start: {formatDate(braket.plannedStart)}
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
