import { useEffect, useState } from "react";
import type { GroupDto } from "../../dtos/group/GroupDto";
import type { MatchDto } from "../../dtos/match/MatchDto";
import { getMatchesByGroupId } from "../../services/groupService";
import { DashboardMatch } from "./DashboardMatch";

interface DashboardGroupProps {
  group: GroupDto;
}

export function DashboardGroup({ group }: DashboardGroupProps) {
  const [matches, setMatches] = useState<MatchDto[]>([]);
  useEffect(() => {
    const loadMatches = async () => {
      const response = await getMatchesByGroupId(group.id);
      setMatches(response);
    };

    loadMatches();
  }, [group]);

  return (
    <section className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
      <div className="border-b border-gray-200 px-6 py-5 sm:px-8">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <div className="flex flex-wrap items-center gap-2">
              <h2 className="text-lg font-semibold text-gray-900">
                {group.name}
              </h2>
            </div>
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
