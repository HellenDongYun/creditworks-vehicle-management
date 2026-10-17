
import {
    Bike,
    Car,
    CarFront,
    CircleHelp,
    Gauge,
    Package,
    Truck,
    type LucideIcon,
} from 'lucide-react'

interface CategoryIconProps {
    name: string
    size?: number
}

const iconMap: Record<string, LucideIcon> = {
    car: Car,
    'car-front': CarFront,
    carfront: CarFront,
    bike: Bike,
    truck: Truck,
    gauge: Gauge,
    package: Package,
}

export default function CategoryIcon({
                                         name,
                                         size = 16,
                                     }: CategoryIconProps) {
    const normalizedName = name.trim().toLowerCase()
    const IconComponent = iconMap[normalizedName] ?? CircleHelp

    return (
        <IconComponent
            size={size}
            aria-label={`${name} category icon`}
        />
    )
}
