import type { GroupDto } from "../../dtos/group/GroupDto";

interface DashboardGroupProps {
  group: GroupDto;
}

export function DashboardGroup({ group }: DashboardGroupProps) {
  return <h1>{group.name}</h1>;
}
