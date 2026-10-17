
import { apiClient } from './client'
import type {
    Vehicle,
    VehicleSortBy,
    SortDirection,
    CreateVehicleRequest,
} from '../types/vehicle'

export async function getVehicles(
    sortBy: VehicleSortBy = 'ownerName',
    direction: SortDirection = 'asc',
): Promise<Vehicle[]> {
    const response = await apiClient.get<Vehicle[]>(
        '/vehicles',
        {
            params: { sortBy, direction },
        },
    )

    return response.data
}
export async function createVehicle(
    request: CreateVehicleRequest,
): Promise<Vehicle> {
    const response = await apiClient.post<Vehicle>('/vehicles', request)
    return response.data
}


