
import { useEffect, useState } from 'react'
import {
  ArrowDown,
  ArrowUp,
  ArrowUpDown,
  CarFront,
  Plus,
  RefreshCw,
  Tags,
} from 'lucide-react'

import { getVehicles } from './api/vehicles'
import AddVehicleForm from './components/AddVehicleForm'
import CategoryManagement from './components/CategoryManagement'
import CategoryIcon from './components/CategoryIcon'
import type {
  Vehicle,
  VehicleSortBy,
  SortDirection,
} from './types/vehicle'

import './App.css'

type ActivePage = 'vehicles' | 'categories'

interface SortHeaderProps {
  label: string
  field: VehicleSortBy
  sortBy: VehicleSortBy
  direction: SortDirection
  onSort: (field: VehicleSortBy) => void
}

function SortHeader({
                      label,
                      field,
                      sortBy,
                      direction,
                      onSort,
                    }: SortHeaderProps) {
  const active = sortBy === field

  return (
      <button
          type="button"
          className={`sort-button ${active ? 'active' : ''}`}
          onClick={() => onSort(field)}
      >
        {label}

        {active ? (
            direction === 'asc' ? (
                <ArrowUp size={15} />
            ) : (
                <ArrowDown size={15} />
            )
        ) : (
            <ArrowUpDown size={15} />
        )}
      </button>
  )
}

function App() {
  const [activePage, setActivePage] =
      useState<ActivePage>('vehicles')

  const [vehicles, setVehicles] = useState<Vehicle[]>([])
  const [sortBy, setSortBy] =
      useState<VehicleSortBy>('ownerName')
  const [direction, setDirection] =
      useState<SortDirection>('asc')

  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [refreshKey, setRefreshKey] = useState(0)
  const [showAddVehicle, setShowAddVehicle] = useState(false)

  useEffect(() => {
    if (activePage !== 'vehicles') {
      return
    }

    let cancelled = false

    async function loadVehicles() {
      setLoading(true)
      setError('')

      try {
        const data = await getVehicles(sortBy, direction)

        if (!cancelled) {
          setVehicles(data)
        }
      } catch (err) {
        console.error(err)

        if (!cancelled) {
          setError('Failed to load vehicles. Please try again.')
        }
      } finally {
        if (!cancelled) {
          setLoading(false)
        }
      }
    }

    void loadVehicles()

    return () => {
      cancelled = true
    }
  }, [activePage, sortBy, direction, refreshKey])

  function handleSort(field: VehicleSortBy) {
    if (sortBy === field) {
      setDirection(current =>
          current === 'asc' ? 'desc' : 'asc',
      )
    } else {
      setSortBy(field)
      setDirection('asc')
    }
  }

  function handleVehicleCreated() {
    setShowAddVehicle(false)
    setRefreshKey(value => value + 1)
  }

  return (
      <div className="app-shell">
        <header className="topbar">
          <div className="brand">
            <div className="brand-icon">
              <CarFront size={22} />
            </div>

            <div>
              <strong>CreditWorks</strong>
              <span>Vehicle Management</span>
            </div>
          </div>

          <nav className="app-navigation">
            <button
                type="button"
                className={`nav-button ${
                    activePage === 'vehicles' ? 'active' : ''
                }`}
                onClick={() => setActivePage('vehicles')}
            >
              <CarFront size={17} />
              Vehicles
            </button>

            <button
                type="button"
                className={`nav-button ${
                    activePage === 'categories' ? 'active' : ''
                }`}
                onClick={() => setActivePage('categories')}
            >
              <Tags size={17} />
              Categories
            </button>
          </nav>
        </header>

        <main className="main-content">
          {activePage === 'vehicles' ? (
              <>
                <div className="page-heading">
                  <div>
                    <h1>Vehicle Management</h1>
                    <p>View and manage registered vehicles.</p>
                  </div>

                  <div className="heading-actions">
                    <button
                        className="refresh-button"
                        type="button"
                        onClick={() =>
                            setRefreshKey(value => value + 1)
                        }
                        disabled={loading}
                    >
                      <RefreshCw size={16} />
                      Refresh
                    </button>

                    <button
                        className="primary-button"
                        type="button"
                        onClick={() => setShowAddVehicle(true)}
                    >
                      <Plus size={16} />
                      Add Vehicle
                    </button>
                  </div>
                </div>

                <section className="vehicles-card">
                  <div className="card-heading">
                    <div>
                      <h2>Vehicles</h2>
                      <p>
                        {loading
                            ? 'Loading vehicles...'
                            : `${vehicles.length} registered vehicles`}
                      </p>
                    </div>
                  </div>

                  {error && (
                      <div className="error-message" role="alert">
                        {error}
                        <button
                            type="button"
                            onClick={() =>
                                setRefreshKey(value => value + 1)
                            }
                        >
                          Retry
                        </button>
                      </div>
                  )}

                  <div className="table-container">
                    <table className="vehicles-table">
                      <thead>
                      <tr>
                        <th>
                          <SortHeader
                              label="Owner Name"
                              field="ownerName"
                              sortBy={sortBy}
                              direction={direction}
                              onSort={handleSort}
                          />
                        </th>

                        <th>
                          <SortHeader
                              label="Manufacturer"
                              field="manufacturer"
                              sortBy={sortBy}
                              direction={direction}
                              onSort={handleSort}
                          />
                        </th>

                        <th>
                          <SortHeader
                              label="Year"
                              field="year"
                              sortBy={sortBy}
                              direction={direction}
                              onSort={handleSort}
                          />
                        </th>

                        <th>
                          <SortHeader
                              label="Weight"
                              field="weight"
                              sortBy={sortBy}
                              direction={direction}
                              onSort={handleSort}
                          />
                        </th>

                        <th>Category</th>
                      </tr>
                      </thead>

                      <tbody>
                      {loading ? (
                          <tr>
                            <td colSpan={5} className="table-status">
                              Loading vehicles...
                            </td>
                          </tr>
                      ) : error ? (
                          <tr>
                            <td colSpan={5} className="table-status">
                              Unable to display vehicles.
                            </td>
                          </tr>
                      ) : vehicles.length === 0 ? (
                          <tr>
                            <td colSpan={5} className="table-status">
                              No vehicles found.
                            </td>
                          </tr>
                      ) : (
                          vehicles.map(vehicle => (
                              <tr key={vehicle.id}>
                                <td className="owner-cell">
                                  {vehicle.ownerName}
                                </td>
                                <td>{vehicle.manufacturerName}</td>
                                <td>{vehicle.yearOfManufacture}</td>
                                <td>{vehicle.weight.toFixed(2)} kg</td>
                                <td>
                            <span className="category-pill">
                               <CategoryIcon name={vehicle.categoryIcon} size={15} />
                                {vehicle.categoryName}
                             </span>
                                </td>
                              </tr>
                          ))
                      )}
                      </tbody>
                    </table>
                  </div>
                </section>
              </>
          ) : (
              <>
                <div className="page-heading">
                  <div>
                    <h1>Category Management</h1>
                    <p>
                      Manage vehicle categories and weight ranges.
                    </p>
                  </div>
                </div>

                <CategoryManagement />
              </>
          )}
        </main>

        {showAddVehicle && (
            <AddVehicleForm
                onClose={() => setShowAddVehicle(false)}
                onCreated={handleVehicleCreated}
            />
        )}
      </div>
  )
}

export default App
