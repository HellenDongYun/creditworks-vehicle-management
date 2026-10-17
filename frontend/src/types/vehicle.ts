
export interface Vehicle {
    id: number
    ownerName: string
    manufacturerId: number
    manufacturerName: string
    yearOfManufacture: number
    weight: number
    categoryId: number
    categoryName: string
    categoryIcon: string
}

export type VehicleSortBy =
    | 'ownerName'
    | 'manufacturer'
    | 'year'
    | 'weight'

export type SortDirection = 'asc' | 'desc'

export interface CreateVehicleRequest {
    ownerName: string
    manufacturerId: number
    yearOfManufacture: number
    weight: number
}
