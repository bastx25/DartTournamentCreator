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
    <>
      <h1>{group.name}</h1>

      {matches.map((match, index) => {
        <DashboardMatch key={match.id} match={match} index={index} />;
      })}
    </>
  );
}
