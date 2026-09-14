import Header from "../../components/Header";

import { LocationList } from "./LocationList";

import { LocationDetails } from "./LocationDetails";
import { DeleteLocationModal } from "./DeleteLocationModal";
import { UpdateLocationModal } from "./UpdateLocationModal";
import { CreateLocationModal } from "./CreateLocationModal";
import { useLocations } from "../../hooks/useLocations";

export function LocationPage() {
  const {
    locations,
    selectedLocation,
    setSelectedLocation,
    loading,
    error,
    locationToDelete,
    setLocationToDelete,
    deleting,
    handleDelete,
    locationToUpdate,
    setLocationToUpdate,
    updating,
    handleUpdate,

    addLocation,
    setAddLocation,
    adding,
    handleAdd,
  } = useLocations();

  return (
    <>
      <Header />

      <main className="min-h-screen bg-gray-100 px-4 py-8 sm:px-6 lg:px-8">
        {loading && (
          <div className="flex min-h-75 items-center justify-center">
            <p className="text-sm text-gray-400">Locations werden geladen...</p>
          </div>
        )}

        {error && (
          <div className="mx-auto max-w-7xl rounded-lg border border-red-500/20 bg-red-500/10 p-4">
            <p className="text-sm text-red-400">{error}</p>
          </div>
        )}

        {!loading && !error && (
          <div className="mx-auto grid max-w-7xl grid-cols-1 gap-6 lg:grid-cols-[1fr_350px]">
            <LocationList
              locations={locations}
              selectedLocation={selectedLocation}
              onSelectLocation={setSelectedLocation}
              onDeleteLocation={setLocationToDelete}
              onUpdateLocation={setLocationToUpdate}
              onAddLocation={setAddLocation}
            />

            <LocationDetails location={selectedLocation} />
          </div>
        )}
      </main>

      {locationToDelete && (
        <DeleteLocationModal
          location={locationToDelete}
          deleting={deleting}
          onCancel={() => setLocationToDelete(null)}
          onConfirm={handleDelete}
        />
      )}

      {locationToUpdate && (
        <UpdateLocationModal
          location={locationToUpdate}
          updating={updating}
          onCancel={() => setLocationToUpdate(null)}
          onConfirm={handleUpdate}
        />
      )}

      {addLocation && (
        <CreateLocationModal
          adding={adding}
          onCancel={() => {
            setAddLocation(false);
          }}
          onConfirm={handleAdd}
        />
      )}
    </>
  );
}
