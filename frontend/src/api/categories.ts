
import { apiClient } from './client'

import type {
    VehicleCategory,
    CategoryConfigurationRequest,
} from '../types/category'

export async function getCategories(): Promise<VehicleCategory[]> {
    const response = await apiClient.get<VehicleCategory[]>(
        '/categories',
    )

    return response.data
}

export async function updateCategoryConfiguration(
    request: CategoryConfigurationRequest,
): Promise<VehicleCategory[]> {
    const response = await apiClient.put<VehicleCategory[]>(
        '/categories',
        request,
    )

    return response.data
}
