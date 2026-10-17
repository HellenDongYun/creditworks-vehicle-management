
export interface VehicleCategory {
    id: number
    name: string
    icon: string
    minWeight: number
    maxWeight: number | null
}

export interface CategoryItemRequest {
    id: number | null
    name: string
    icon: string
    minWeight: number
    maxWeight: number | null
}

export interface CategoryConfigurationRequest {
    categories: CategoryItemRequest[]
}
