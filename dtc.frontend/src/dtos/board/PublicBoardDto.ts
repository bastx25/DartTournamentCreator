export interface PublicBoardDto {
  id: number;
  label?: string | null;
  locationId: number;
  locationName?: string | null;
  isActive: boolean;
}
